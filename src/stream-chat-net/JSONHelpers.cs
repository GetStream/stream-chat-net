using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StreamChat
{
    internal static class JsonHelpers
    {
        private delegate GenericData Builder<T>(T target, IEnumerable<JProperty> jsonProps);

        private static readonly ConcurrentDictionary<Type, object> CachedBuilders = new ConcurrentDictionary<Type, object>();

        internal static GenericData FromJObject<T>(T obj, JObject json)
            where T : class
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            if (json is null)
                throw new ArgumentNullException(nameof(json));

            var jProps = json.Properties();
            
            var builder = (Builder<T>)CachedBuilders.GetOrAdd(typeof(T), _ => ConstructBuilder<T>());
            
            return builder(obj, jProps);
        }

        internal static void RegisterType<T>()
            where T : class => 
            CachedBuilders.GetOrAdd(typeof(T), _ => ConstructBuilder<T>());

        private static Builder<T> ConstructBuilder<T>()
        {
            var properties = typeof(T)
                .GetPropertiesPortable()
                .Where(p => p.SetMethod != null);

            var mappedProperties = GetObjectPropertyMap(properties);

            var extraDataExp = Expression.Variable(typeof(GenericData), "extra");
            var targetExp = Expression.Parameter(typeof(T), "target");
            var jsonPropertiesExp = Expression.Parameter(typeof(IEnumerable<JProperty>), "jProps");

            var forEachExp = ReflectionHelper.CreateForEach(
                    jsonPropertiesExp,
                    current => CreateForEachBody(current, mappedProperties, extraDataExp, targetExp));

            var initExtraDataExp = Expression.Assign(extraDataExp, Expression.New(typeof(GenericData)));

            var finalExpression = Expression.Lambda<Builder<T>>(
                Expression.Block(
                    new[] { extraDataExp },
                    initExtraDataExp,
                    forEachExp,
                    extraDataExp),
                targetExp,
                jsonPropertiesExp
                );

            return finalExpression.Compile();
        }

        private static Expression CreateForEachBody(
            Expression currentExp, 
            Dictionary<string, PropertyInfo> mappedProperties, 
            Expression extraDataExp, 
            Expression targetExp)
        {
            var jsonPropertyType = typeof(JProperty);
            var currentValueExp = Expression.Property(currentExp, jsonPropertyType.GetPropertyPortable(nameof(JProperty.Value)));
            var currentNameExp = Expression.Property(currentExp, jsonPropertyType.GetPropertyPortable(nameof(JProperty.Name)));

            var defaultCase = Expression.Call(
                    extraDataExp,
                    typeof(GenericData).GetMethodPortable(nameof(GenericData.SetData)).MakeGenericMethod(typeof(JToken)),
                    currentNameExp,
                    currentValueExp);

            var cases = mappedProperties
                    .Select(pair =>
                    {
                        var currentValueToObject = Expression.Call(
                            currentValueExp,
                            jsonPropertyType
                                .GetMethodPortable(nameof(JProperty.ToObject), Type.EmptyTypes)
                                .MakeGenericMethod(pair.Value.PropertyType));

                        return Expression.SwitchCase(
                            Expression.Assign(Expression.Property(targetExp, pair.Value), currentValueToObject),
                            Expression.Constant(pair.Key));

                    })
                    .ToArray();

            var result = Expression.Switch(typeof(void), currentNameExp, defaultCase, null, cases);

            return result;
        }

        private static Dictionary<string, PropertyInfo> GetObjectPropertyMap(IEnumerable<PropertyInfo> properties)
        {
            var mappedProperties = new Dictionary<string, PropertyInfo>();

            foreach (var property in properties)
            {
                var ignore = false;
                var propertyName = property.Name;
                var attributes = property.GetCustomAttributes(true);

                foreach (var attribute in attributes)
                {
                    if (attribute is JsonPropertyAttribute propertyAttribute)
                    {
                        propertyName = propertyAttribute.PropertyName;
                        break;
                    }
                    
                    if (attribute is JsonIgnoreAttribute ignoreAttr)
                    {
                        ignore = true;
                        break;
                    }
                }

                if (ignore || mappedProperties.ContainsKey(propertyName))
                    continue;

                mappedProperties.Add(propertyName, property);
            }

            return mappedProperties;
        }
    }
}
