using System;
using System.Collections.Generic;
using System.Text;

namespace JSONParser
{
    class JsonList : JsonElement, IJsonNonPrimitive
    {
        private List<JsonElement> Values;

        public override JSONValueType ValueType
        {
            get
            {
                return JSONValueType.LIST;
            }
        }

        public JsonElement this[int i]
        {
            get
            {
                return Values[i];
            }
        }

        public int Count
        {
            get
            {
                return Values.Count;
            }
        }

        public JsonList(string key, bool readOnly)
        {
            Key = key;
            Values = new List<JsonElement>();
        }

        public void Add(JsonElement value)
        {
            Values.Add(value);
        }


        public override string ToString(bool pretty)
        {
            var result = "";
            if (Key != "")
                result = Key + (pretty ? ": " : ":");
            result += "[";
            foreach(var jsonObj in Values)
            {
                var childResult = jsonObj.ToString(pretty);
                if (pretty)
                    childResult = childResult.Replace("\n", "\n  ");
                result += (pretty ? "\n  " : "") + childResult + ",";
            }
            result = result.Substring(0, result.Length - 1);
            result += (pretty ? "\n]" : "]");

            return result;
        }
    }
}
