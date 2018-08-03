﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IngameScript;

namespace JSONParserTests
{
    [TestClass]
    public class JSONTest
    {
        [TestMethod]
        public void TestFlatJSON()
        {
            var json = new JSON(
@"{
    key1: value1,
    key2: value2
}");
            while (!json.ParsingComplete())
            {
                Console.WriteLine("Parsing (" + json.Progress + "%)...");
            }
            var result = json.Result.GetBody();

            Assert.IsTrue(result.ContainsKey("key1"));
            Assert.IsTrue(result.ContainsKey("key2"));
            Assert.AreEqual("value1", result["key1"].GetValue<string>());
            Assert.AreEqual("value2", result["key2"].GetValue<string>());

            /*Console.WriteLine("{");
            foreach(var kvp in json)
            {
                Console.WriteLine("    " + kvp.Key + ": " + kvp.Value.GetString() + ",");
            }
            Console.WriteLine("}");*/
        }

        [TestMethod]
        public void TestNestedJSON()
        {
            var json = new JSON(
@"{
    jsonObject1: {
        nestedKey1: nestedValue1,
        nestedKey2: nestedValue2
    },
    flatKey1: flatValue1,
    jsonObject2: {
        nestedkey1: {
            deeplynestedkey1: deeplynestedvalue1,
        },
        nestedkey2: nestedvalue2
    }
}");

            while (!json.ParsingComplete())
            {
                Console.WriteLine("Parsing (" + json.Progress + "%)...");
            }

            var result = json.Result.GetBody();

            Assert.IsTrue(result.ContainsKey("jsonObject1"));
            Assert.IsTrue(result.ContainsKey("flatKey1"));
            Assert.IsTrue(result.ContainsKey("jsonObject2"));

            var jsonObject1 = result["jsonObject1"].GetBody();
            Assert.IsTrue(jsonObject1.ContainsKey("nestedKey1"));
            Assert.IsTrue(jsonObject1.ContainsKey("nestedKey2"));
            Assert.AreEqual("nestedValue1", jsonObject1["nestedKey1"].GetValue<string>());
            Assert.AreEqual("nestedValue2", jsonObject1["nestedKey2"].GetValue<string>());

            Assert.AreEqual("flatValue1", result["flatKey1"].GetValue<string>());

            var jsonObject2 = result["jsonObject2"].GetBody();
            Assert.IsTrue(jsonObject2.ContainsKey("nestedkey1"));
            Assert.IsTrue(jsonObject2.ContainsKey("nestedkey2"));
            Assert.AreEqual("nestedvalue2", jsonObject2["nestedkey2"].GetValue<string>());

            var nestedObject = jsonObject2["nestedkey1"].GetBody();
            Assert.IsTrue(nestedObject.ContainsKey("deeplynestedkey1"));
            Assert.AreEqual("deeplynestedvalue1", nestedObject["deeplynestedkey1"].GetValue<string>());


        }



        [TestMethod]
        public void TestJsonObjectToString()
        {
            var json = new JSON(
@"{
    jsonObject1: {
        nestedKey1: nestedValue1,
        nestedKey2: nestedValue2
    },
    flatKey1: flatValue1,
    jsonObject2: {
        nestedkey1: {
            deeplynestedkey1: deeplynestedvalue1,
        },
        nestedkey2: nestedvalue2
    }
}");

            while (!json.ParsingComplete())
            {
                Console.WriteLine("Parsing (" + json.Progress + "%)...");
            }

            var result = json.Result;

            var serialized = result.ToString(true);
            Console.WriteLine(serialized);
            var expected = @"{
  jsonObject1: {
    nestedKey1: nestedValue1,
    nestedKey2: nestedValue2
  },
  flatKey1: flatValue1,
  jsonObject2: {
    nestedkey1: {
      deeplynestedkey1: deeplynestedvalue1
    },
    nestedkey2: nestedvalue2
  }
}".Replace("\r\n", "\n");
            
            Assert.IsTrue(expected.Equals(serialized));


            serialized = result.ToString(false);
            Console.WriteLine(serialized);
            expected = "{jsonObject1:{nestedKey1:nestedValue1,nestedKey2:nestedValue2},flatKey1:flatValue1,jsonObject2:{nestedkey1:{deeplynestedkey1:deeplynestedvalue1},nestedkey2:nestedvalue2}}";
            
            Assert.AreEqual(expected, serialized);

        }
    }
}
