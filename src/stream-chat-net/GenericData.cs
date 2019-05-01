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

        internal JObject ToJObject()
        {
            var root = new JObject();
            this.AddToJObject(ref root);
            return root;
        }

        internal void AddToJObject(ref JObject root)
        {
            var tmp = root;
            this._data.ForEach(x => tmp.Add(x.Key, x.Value));
            root = tmp;
        }
    }
}
