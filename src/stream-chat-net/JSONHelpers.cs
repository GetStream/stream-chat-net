using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StreamChat
{
    public sealed class JsonPropAttribute : Attribute
    {
        public string Tag { get; set; }
        public bool IgnoreNull { get; set; } = true;

        public JsonPropAttribute() { }
    }

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

        public JObject ToJObject()
        {
            var root = new JObject();
            this.AddToJObject(ref root);
            this._data.AddToJObject(ref root);
            return root;
        }

        private void AddToJObject(ref JObject root)
        {
            PropertyInfo[] properties = this.GetType().GetProperties();
            foreach (var prop in properties)
            {
                string propName = prop.Name;
                bool ignoreNull = true;
                foreach (var attr in prop.GetCustomAttributes(true))
                {
                    JsonPropAttribute result = attr as JsonPropAttribute;
                    if (result != null)
                    {
                        propName = result.Tag;
                        ignoreNull = result.IgnoreNull;
                        break;
                    }
                }
                var propVal = prop.GetValue(this);
                if (propVal != null || (propVal == null && !ignoreNull))
                {
                    root.Add(new JProperty(propName, propVal));
                }
            }
        }
    }

    internal class JsonHelpers
    {
        internal static void AddToJObject<T>(T obj, ref JObject root)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                string propName = prop.Name;
                bool ignoreNull = true;
                foreach (var attr in prop.GetCustomAttributes(true))
                {
                    JsonPropAttribute result = attr as JsonPropAttribute;
                    if (result != null)
                    {
                        propName = result.Tag;
                        ignoreNull = result.IgnoreNull;
                        break;
                    }
                }
                var propVal = prop.GetValue(obj);
                if (propVal != null || (propVal == null && !ignoreNull))
                {
                    root.Add(new JProperty(propName, propVal));
                }
            }
        }

        internal static GenericData FromJObject<T>(ref T obj, JObject json)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
            Dictionary<string, PropertyInfo> objProps = new Dictionary<string, PropertyInfo>();
            GenericData extra = new GenericData();

            foreach (var prop in properties)
            {
                string propName = prop.Name;
                foreach (var attr in prop.GetCustomAttributes(true))
                {
                    JsonPropAttribute result = attr as JsonPropAttribute;
                    if (result != null)
                    {
                        propName = result.Tag;
                        break;
                    }
                }
                objProps.Add(propName, prop);
            }
            foreach (var jsonProp in json.Properties())
            {
                PropertyInfo objProp;
                if (objProps.TryGetValue(jsonProp.Name, out objProp))
                {
                    objProp.SetValue(obj, jsonProp.Value.ToObject(objProp.PropertyType));
                }
                else
                {
                    extra.SetData(jsonProp.Name, jsonProp.Value);
                }
            }
            return extra;
        }
    }
}
