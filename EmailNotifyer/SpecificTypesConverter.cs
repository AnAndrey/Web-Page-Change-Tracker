using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace NoCompany.EmailNotifier
{

    internal class SpecificTypesConverter : JsonConverter
    {
        public HashSet<Type> Types { get;  private set;}

        public SpecificTypesConverter(IEnumerable<Type> types)
        {
            Types = new HashSet<Type>(types);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var props = value.GetType().GetProperties().Where(x => Types.Contains(x.PropertyType));

            if (!props.Any())
                return;
            try
            {
                writer.WriteStartObject();
                foreach (var prop in props)
                {
                    var v = prop.GetValue(value);
                    if (v != null)
                    {
                        writer.WritePropertyName(prop.Name);
                        writer.WriteValue(v.ToString());
                    }
                }
            }
                finally
            {
                writer.WriteEndObject();
            }

        }
    }
}