namespace Code1.Common.Persistance
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public class JsonSerializer : IObjectSerializer
    {
        private JsonSerializerSettings jsonSerializerSettings;

        public JsonSerializer()
        {
            jsonSerializerSettings = new JsonSerializerSettings();
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, this.jsonSerializerSettings);
        }

        public T DeserializeObject<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, this.jsonSerializerSettings);
        }
    }
}
