using System;
using System.Collections.Generic;
using System.Text;

namespace JSONParser
{
    interface IJsonNonPrimitive
    {
        void Add(JsonElement child);
    }
}
