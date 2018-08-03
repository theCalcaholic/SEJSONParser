using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSONParser
{
    public class JSON
    {
        enum JSONPart { KEY, KEYEND, VALUE, VALUEEND }

        public static Dictionary<string, JsonObject> Parse(string serialized)
        {
            int lastCharIndex = -1;
            JSONPart Expected = JSONPart.VALUE;
            Stack<Dictionary<string, JsonObject>> ListStack = new Stack<Dictionary<string, JsonObject>>();
            Stack<JsonObject> JsonStack = new Stack<JsonObject>();
            JsonObject CurrentJsonObject = new JsonObject("");
            var trimChars = new char[] { ' ', '\n', '\r', '\t' };

            while (lastCharIndex < serialized.Length - 1)
            {
                var charIndex = -1;
                switch (Expected)
                {
                    case JSONPart.VALUE:
                        charIndex = serialized.IndexOfAny(new char[] { '{', '}', ',' }, lastCharIndex + 1);
                        if (charIndex == -1)
                            throw new UnexpectedCharacterException(new char[] { '{', '}', ',' }, "EOF", lastCharIndex);
                        Console.WriteLine("Expecting Value...");
                        Console.WriteLine("Found " + serialized[charIndex] + " (" + charIndex + ")");
                        switch (serialized[charIndex])
                        {
                            case '{':
                                CurrentJsonObject.SetValue(new Dictionary<string, JsonObject>());
                                JsonStack.Push(CurrentJsonObject);
                                CurrentJsonObject = new JsonObject();
                                Expected = JSONPart.KEY;
                                break;
                            case '}':
                            case ',':
                                var value = serialized.Substring(lastCharIndex + 1, charIndex - lastCharIndex - 1).Trim();
                                Console.WriteLine("value is: '" + value + "'");
                                CurrentJsonObject.SetValue(value);

                                if (serialized[charIndex] == '}')
                                {
                                    if (charIndex < serialized.Length - 1 && serialized[charIndex + 1] == ',')
                                        charIndex++;
                                    CurrentJsonObject = JsonStack.Pop();
                                }

                                Expected = JSONPart.KEY;
                                break;
                        }
                        lastCharIndex = charIndex;
                        break;
                    case JSONPart.KEY:
                        charIndex = serialized.IndexOfAny(new char[] { '}', ':' }, lastCharIndex + 1);
                        Console.WriteLine("Expecting Key...");
                        Console.WriteLine("Found " + serialized[charIndex] + " (" + charIndex + ")");
                        if (charIndex == -1)
                            throw new UnexpectedCharacterException(new char[] { '}', ':' }, "EOF", lastCharIndex);

                        switch (serialized[charIndex])
                        {
                            case '}':
                                if (charIndex < serialized.Length - 1 && serialized[charIndex + 1] == ',')
                                    charIndex++;
                                CurrentJsonObject = JsonStack.Pop();
                                Expected = JSONPart.KEY;
                                break;
                            case ':':
                                var key = serialized.Substring(lastCharIndex + 1, charIndex - lastCharIndex - 1).Trim();
                                Console.WriteLine("key is: '" + key + "'");
                                CurrentJsonObject = new JsonObject(key);
                                JsonStack.Peek().GetValue()
                                    .Add(CurrentJsonObject.Key, CurrentJsonObject);
                                Expected = JSONPart.VALUE;
                                break;
                        }
                        lastCharIndex = charIndex;
                        break;
                }
                Console.WriteLine("Iteration done, CurrentJsonObject is: '" + CurrentJsonObject.Key + "'");
            }

            return CurrentJsonObject.GetValue();
        }

        private class ParseException : Exception
        {
            public ParseException(string message, int position = -1)
                : base("PARSE ERROR" + (position == -1 ? "" : " after char " + position.ToString()) + ": " + message) { }

        }

        private class UnexpectedCharacterException : ParseException
        {
            public UnexpectedCharacterException(char[] expected, string received, int position = -1)
                : base("Expected one of [ '" + string.Join("', '", expected) + "' ] but received " + received + "!", position)
            { }

        }

    }
}
