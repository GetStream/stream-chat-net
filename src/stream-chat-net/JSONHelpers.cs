using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        internal static GenericData FromJObject<T>(T obj, JObject json)
        {
#if NETSTANDARD1_6
            PropertyInfo[] properties = typeof(T).GetTypeInfo().GetProperties();
#else
            PropertyInfo[] properties = typeof(T).GetProperties();
#endif
            Dictionary<string, PropertyInfo> objProps = new Dictionary<string, PropertyInfo>();
            GenericData extra = new GenericData();

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
}
