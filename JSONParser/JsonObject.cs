using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IngameScript
{
    public class JsonObject
    {
        public string Key { get; private set; }
        private string StringValue;
        private Dictionary<string, JsonObject> NestedValue;
        public bool IsFinal { get; private set; }

        public JsonObject(string key, string value)
        {
            Key = key;
            StringValue = value;
            IsFinal = true;
        }

        public JsonObject(string key, Dictionary<string, JsonObject> value)
        {
            Key = key;
            NestedValue = value;
            IsFinal = true;
        }

        public JsonObject(string key)
        {
            Key = key;
            IsFinal = false;
        }

        public JsonObject()
        {
            IsFinal = false;
        }

        public void SetKey(string key)
        {
            if (IsFinal)
                throw new Exception("JSON object can't be modified!");
            Key = key;
            if (StringValue != null || NestedValue != null)
                IsFinal = true;
        }

        public void SetValue(string value)
        {
            if (IsFinal)
                throw new Exception("JSON object can't be modified!");
            if (NestedValue != null)
                throw new Exception("Can't define JSON value and string value at the same time!");
            StringValue = value;
            IsFinal = (Key != null);
        }

        public void SetValue(Dictionary<string, JsonObject> value)
        {
            if (IsFinal)
                throw new Exception("JSON object can't be modified!");
            if (StringValue != null)
                throw new Exception("Can't define JSON value and string value at the same time!");
            NestedValue = value;
            IsFinal = (Key != null);
        }

        public string GetString()
        {
            return StringValue;
        }

        public Dictionary<string, JsonObject> GetValue()
        {
            return NestedValue;
        }

        override
        public string ToString()
        {
            return ToString(true);

        }

        public string ToString(bool pretty = true)
        {
            var result = "";
            if(Key != "" || StringValue != null)
                result = Key + (pretty ? ": " : ":");
            if (StringValue != null)
            {
                result += GetString();
            }
            else
            {
                result += "{";
                foreach(var kvp in GetValue())
                {
                    var childResult = kvp.Value.ToString(pretty);
                    if (pretty)
                        childResult = childResult.Replace("\n", "\n  ");
                    result += (pretty ? "\n  " : "") + childResult + ",";
                }
                result = result.Substring(0, result.Length - 1);
                result += (pretty ? "\n}" : "}");
            }

            return result;
        }

    }
}
