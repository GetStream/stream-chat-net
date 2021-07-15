using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace StreamChat
{
    public class CustomDataBase
    {
        protected GenericData _data = new GenericData();

        public CustomDataBase() { }

        public T GetData<T>(string name)
        {
            return this._data.GetData<T>(name);
        }

        public void SetData<T>(string name, T data)
        {
            this._data.SetData<T>(name, data);
        }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            this._data.AddToJObject(root);
            return root;
        }
    }

    internal class JsonHelpers
    {
        private delegate GenericData Builder<T>(T target, IEnumerable<JProperty> jsonProps);

        private static readonly Dictionary<Type, object> CachedBuilders = new Dictionary<Type, object>();

        internal static GenericData FromJObject<T>(T obj, JObject json)
        {
            var jProps = json.Properties();

            if (CachedBuilders.TryGetValue(typeof(T), out var builder))
                return (builder as Builder<T>)(obj, jProps);

            var properties = typeof(T).GetPropertiesPortable();

            var newBuilder = ConstructBuilder<T>(properties);

            CachedBuilders[typeof(T)] = newBuilder;

            return newBuilder(obj, jProps);
        }

        internal static void RegisterType<T>()
        {
            if (CachedBuilders.ContainsKey(typeof(T)))
                return;

            var properties = typeof(T).GetPropertiesPortable();

            var newBuilder = ConstructBuilder<T>(properties);

            CachedBuilders[typeof(T)] = newBuilder;
        }

        private static Builder<T> ConstructBuilder<T>(PropertyInfo[] properties)
        {
            var mappedProperties = GetObjectPropertyMap(properties);

            var extraDataExp = Expression.Variable(typeof(GenericData), "extra");
            var targetExp = Expression.Parameter(typeof(T), "target");
            var jsonPropertiesExp = Expression.Parameter(typeof(IEnumerable<JProperty>), "jProps");
            var jsonPropertyType = typeof(JProperty);

            var forEachExp = ReflectionHelper.CreateForEach(
                    jsonPropertiesExp,
                    currentExp =>
                    {
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
                    });

            var finalExpression = Expression.Lambda<Builder<T>>(
                Expression.Block(
                    new[] { extraDataExp },
                    Expression.Assign(extraDataExp, Expression.New(typeof(GenericData))),
                    forEachExp,
                    extraDataExp),
                targetExp,
                jsonPropertiesExp
                );

            return finalExpression.Compile();
        }

        private static Dictionary<string, PropertyInfo> GetObjectPropertyMap(PropertyInfo[] properties)
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
                    else if (attribute is JsonIgnoreAttribute ignoreAttr)
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

        internal static GenericData FromJObjectOld<T>(T obj, JObject json)
        {
            var properties = typeof(T).GetPropertiesPortable();

            var objProps = new Dictionary<string, PropertyInfo>();
            var extra = new GenericData();

            foreach (var prop in properties)
            {
                bool ignore = false;
                string propName = prop.Name;
                var attrs = prop.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    JsonIgnoreAttribute ignoreAttr = attr as JsonIgnoreAttribute;
                    if (ignoreAttr != null)
                    {
                        ignore = true;
                        break;
                    }
                    JsonPropertyAttribute result = attr as JsonPropertyAttribute;
                    if (result != null)
                    {
                        propName = result.PropertyName;
                        break;
                    }
                }
                if (!ignore && !objProps.ContainsKey(propName))
                    objProps.Add(propName, prop);
            }
            var jsonProps = json.Properties();
            foreach (var jsonProp in jsonProps)
            {
                PropertyInfo objProp;
                if (objProps.TryGetValue(jsonProp.Name, out objProp))
                    objProp.SetValue(obj, jsonProp.Value.ToObject(objProp.PropertyType));
                else
                    extra.SetData(jsonProp.Name, jsonProp.Value);
            }
            return extra;
        }
    }

    internal static class ReflectionHelper
    {
        public static PropertyInfo GetPropertyPortable(this Type type, string propName)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetProperty(propName);
        }

        public static PropertyInfo[] GetPropertiesPortable(this Type type)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetProperties();
        }

        public static MethodInfo GetMethodPortable(this Type type, string methodName)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetMethod(methodName);
        }

        public static MethodInfo GetMethodPortable(this Type type, string methodName, params Type[] types)
        {
            types = types ?? (new Type[0]);

            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetMethod(methodName, types);
        }

        public static Type[] GetInterfacesPortable(this Type type)
        {
            return type
#if NETSTANDARD1_6
                .GetTypeInfo()
#endif
                .GetInterfaces();
        }

        public static Expression CreateForEach(ParameterExpression enumerableExp, Func<ParameterExpression, Expression> bodyGenerator)
        {
            var breakLabel = Expression.Label("ForeachBreak");
            var getEnumeratorMethod = enumerableExp.Type.GetMethodPortable(nameof(IEnumerable.GetEnumerator));
            var enumeratorType = getEnumeratorMethod.ReturnType;
            var enumeratorExp = Expression.Parameter(enumeratorType, "enumerator");
            var moveNextMethod = typeof(IEnumerator).GetMethodPortable(nameof(IEnumerator.MoveNext));
            var currentProperty = enumeratorType.GetPropertyPortable(nameof(IEnumerator.Current));
            var currentExpression = Expression.Parameter(currentProperty.PropertyType, "current");

            Expression loopBlock = Expression.Loop(
                        Expression.IfThenElse(
                            Expression.Call(enumeratorExp, moveNextMethod),
                            Expression.Block(
                                new[] { currentExpression },
                                Expression.Assign(
                                    currentExpression,
                                    Expression.Property(enumeratorExp, currentProperty)),
                                bodyGenerator(currentExpression)
                                ),
                            Expression.Break(breakLabel)));

            var diposableType = typeof(IDisposable);

            if (enumeratorType.GetInterfacesPortable().Contains(diposableType))
            {
                var disposeMethod = diposableType.GetMethodPortable(nameof(IDisposable.Dispose));

                loopBlock = Expression.TryFinally(loopBlock, Expression.Call(Expression.Convert(enumerableExp, diposableType), disposeMethod));
            }

            var res = Expression.Block(
                    new[]
                    {
                        enumeratorExp
                    },
                    Expression.Assign(
                        enumeratorExp,
                        Expression.Call(enumerableExp, getEnumeratorMethod)),
                    loopBlock,
                    Expression.Label(breakLabel)
                );

            return res;
        }
    }
}
