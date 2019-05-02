using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StreamChat
{
    public class GenericData
    {
        readonly IDictionary<string, JToken> _data = new Dictionary<string, JToken>();

        public GenericData() { }

        public T GetData<T>(string name)
        {
            JToken val;
            return this._data.TryGetValue(name, out val) ? val.ToObject<T>() : default(T);
        }

        public void SetData<T>(string name, T data)
        {
            this._data[name] = JValue.FromObject(data);
        }

        public void RemoveData(string name)
        {
            this._data.Remove(name);
        }

        internal JObject ToJObject()
        {
            var root = new JObject();
            this.AddToJObject(root);
            return root;
        }

        internal void AddToJObject(JObject root)
        {
            this._data.ForEach(x => root.Add(x.Key, x.Value));
        }
    }
}
