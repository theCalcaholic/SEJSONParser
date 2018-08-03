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
        private bool ReadOnly = true;
        public bool IsPrimitive {
            get {
                return StringValue != null;
            }
        }

        public JsonObject(string key, string value, bool readOnly = true)
        {
            Key = key;
            StringValue = value;
            IsFinal = true;
            ReadOnly = readOnly;
        }

        public JsonObject(string key, Dictionary<string, JsonObject> value, bool readOnly = true)
        {
            Key = key;
            NestedValue = value;
            IsFinal = true;
            ReadOnly = readOnly;
        }

        public JsonObject(string key, bool readOnly = true)
        {
            Key = key;
            IsFinal = false;
            ReadOnly = readOnly;
        }

        public JsonObject(bool readOnly = true)
        {
            IsFinal = false;
            ReadOnly = readOnly;
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

        public T GetValue<T>()
        {
            object value = null;
            if( typeof(T) == typeof(string) )
            {
                value = StringValue;
            }
            else if( typeof(T) == typeof(int) )
            {
                value = Int32.Parse(StringValue);
            }
            else if( typeof(T) == typeof(float) )
            {
                value = Single.Parse(StringValue);
            }
            else if( typeof(T) == typeof(double) )
            {
                value = Double.Parse(StringValue);
            }
            else if( typeof(T) == typeof(char) )
            {
                value = Char.Parse(StringValue);
            }
            else if( typeof(T) == typeof(DateTime) )
            {
                value = DateTime.Parse(StringValue);
            }
            else if( typeof(T) == typeof(decimal) )
            {
                value = Decimal.Parse(StringValue);
            }
            else if( typeof(T) == typeof(bool) )
            {
                value = Boolean.Parse(StringValue);
            }
            else if (typeof(T) == typeof(byte) )
            {
                value = Byte.Parse(StringValue);
            }
            else if( typeof(T) == typeof(uint) )
            {
                value = UInt32.Parse(StringValue);
            }
            else if( typeof(T) == typeof(short) )
            {
                value = short.Parse(StringValue);
            }
            else if( typeof(T) == typeof(long) )
            {
                value = long.Parse(StringValue);
            }
            else if( typeof(T) == typeof(List<JsonObject>) )
            {
                var values = GetBody()?.Values;
                if (values == null)
                    value = new List<JsonObject>();
                else
                    value = new List<JsonObject>(values);
            }
            else if( typeof(T) == typeof(Dictionary<string, JsonObject>))
            {
                value =  GetBody();
            }
            else
            {
                throw new ArgumentException("Invalid type '" + typeof(T).ToString() + "' requested!");
            }

            return (T) value;
        }

        public bool TryGetValue<T>(out T result)
        {
            try
            {
                result = GetValue<T>();
                return true;
            }
            catch( Exception )
            {
                result = default(T);
                return false;
            }
        }



        public Dictionary<string, JsonObject> GetBody()
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
                result += StringValue;
            }
            else
            {
                result += "{";
                foreach(var kvp in GetBody())
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
