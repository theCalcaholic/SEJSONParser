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

        private int LastCharIndex;
        private IEnumerator<bool> Enumerator;
        public string Serialized { get; private set; }
        public JsonElement Result { get; private set; }
        private bool ReadOnly;
        private Func<bool> ShouldPause;

        public int Progress
        {
            get
            {
                return 100 * Math.Max(0, LastCharIndex) / Serialized.Length;
            }
        }


        public JSON(string serialized, Func<bool> shouldPause, bool readOnly = true)
        {
            Serialized = serialized;
            Enumerator = Parse().GetEnumerator();
            ReadOnly = readOnly;
            ShouldPause = shouldPause;
        }


        public bool ParsingComplete()
        {
            return !Enumerator.MoveNext();
        }

        public IEnumerable<bool> Parse()
        {
            LastCharIndex = -1;
            JSONPart Expected = JSONPart.VALUE;
            Stack<Dictionary<string, JsonElement>> ListStack = new Stack<Dictionary<string, JsonElement>>();
            Stack<IJsonNonPrimitive> JsonStack = new Stack<IJsonNonPrimitive>();
            IJsonNonPrimitive CurrentNestedJsonObject = null;
            //Func<object, JsonObject> Generator = JsonObject.NewJsonObject("", readOnly);
            var trimChars = new char[] { '"', '\'', ' ', '\n', '\r', '\t', '\f' };
            string Key = "";
            var keyDelims = new char[] { '}', ':' };
            var valueDelims = new char[] { '{', '}', ',', '[', ']' };
            var expectedDelims = valueDelims;
            var charIndex = -1;
            bool insideList = false;

            while (LastCharIndex < Serialized.Length - 1)
            {
                charIndex = Serialized.IndexOfAny(expectedDelims, LastCharIndex + 1);
                if (charIndex == -1)
                    throw new UnexpectedCharacterException(expectedDelims, "EOF", LastCharIndex);

                if (Expected == JSONPart.VALUE)
                {
                    //Console.WriteLine("Expecting Value...");
                    //Console.WriteLine("Found " + Serialized[charIndex] + " (" + charIndex + ")");
                    switch (Serialized[charIndex])
                    {
                        case '[':
                            CurrentNestedJsonObject = new JsonList(Key);
                            if (JsonStack.Count > 0)
                                JsonStack.Peek().Add(CurrentNestedJsonObject as JsonElement);
                            JsonStack.Push(CurrentNestedJsonObject);
                            insideList = true;
                            //Console.WriteLine("List started");
                            break;
                        case '{':
                            CurrentNestedJsonObject = new JsonObject(Key);
                            if (JsonStack.Count > 0)
                                JsonStack.Peek().Add(CurrentNestedJsonObject as JsonElement);
                            JsonStack.Push(CurrentNestedJsonObject);
                            Expected = JSONPart.KEY;
                            expectedDelims = keyDelims;
                            break;
                        case ',':
                        case '}':
                        case ']':
                            var value = Serialized.Substring(LastCharIndex + 1, charIndex - LastCharIndex - 1).Trim(trimChars);
                            //Console.WriteLine("value is: '" + value + "'");
                            JsonStack.Peek().Add(new JsonPrimitive(Key, value));
                            break;
                    }
                    if (insideList && Serialized[charIndex] != ']')
                    {
                        Key = null;
                    }
                    else
                    {
                        insideList = false;
                        Expected = JSONPart.KEY;
                        expectedDelims = keyDelims;
                    }
                }
                else if ( Expected == JSONPart.KEY )
                {
                    //Console.WriteLine("Expecting Key...");
                    //Console.WriteLine("Found " + Serialized[charIndex] + " (" + charIndex + ")");

                    switch ( Serialized[charIndex] )
                    {
                        case ':':
                            Key = Serialized.Substring(LastCharIndex + 1, charIndex - LastCharIndex - 1).Trim(trimChars);
                            //Console.WriteLine("key is: '" + Key + "'");
                            //Generator = JsonObject.NewJsonObject(Key, readOnly);
                            Expected = JSONPart.VALUE;
                            expectedDelims = valueDelims;
                            break;
                    }
                }
                if ( Serialized[charIndex] == '}' || Serialized[charIndex] == ']' )
                {
                    if (charIndex < Serialized.Length - 1 && Serialized[charIndex + 1] == ',')
                        charIndex++;
                    CurrentNestedJsonObject = JsonStack.Pop();
                }
                LastCharIndex = charIndex;
                //Console.WriteLine("Iteration done, CurrentJsonObject is: '" + CurrentNestedJsonObject.Key + "'");
                if ( ShouldPause() )
                {
                    yield return false;
                }
            }

            Result = CurrentNestedJsonObject as JsonElement;
            yield return true;
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
