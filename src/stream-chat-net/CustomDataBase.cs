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
            this._data.SetData(name, data);
        }

        internal JObject ToJObject()
        {
            var root = JObject.FromObject(this);
            this._data.AddToJObject(root);
            return root;
        }
    }
}
