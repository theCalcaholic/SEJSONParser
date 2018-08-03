using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JSONParser;

namespace JSONParserTests
{
    [TestClass]
    public class JSONTest
    {
        [TestMethod]
        public void TestFlatJSON()
        {
            var json = JSON.Parse(
@"{
    key1: value1,
    key2: value2
}");
            Assert.IsTrue(json.ContainsKey("key1"));
            Assert.IsTrue(json.ContainsKey("key2"));
            Assert.AreEqual("value1", json["key1"].GetString());
            Assert.AreEqual("value2", json["key2"].GetString());

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
            var json = JSON.Parse(
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

            Assert.IsTrue(json.ContainsKey("jsonObject1"));
            Assert.IsTrue(json.ContainsKey("flatKey1"));
            Assert.IsTrue(json.ContainsKey("jsonObject2"));

            var jsonObject1 = json["jsonObject1"].GetValue();
            Assert.IsTrue(jsonObject1.ContainsKey("nestedKey1"));
            Assert.IsTrue(jsonObject1.ContainsKey("nestedKey2"));
            Assert.AreEqual("nestedValue1", jsonObject1["nestedKey1"].GetString());
            Assert.AreEqual("nestedValue2", jsonObject1["nestedKey2"].GetString());

            Assert.AreEqual("flatValue1", json["flatKey1"].GetString());

            var jsonObject2 = json["jsonObject2"].GetValue();
            Assert.IsTrue(jsonObject2.ContainsKey("nestedkey1"));
            Assert.IsTrue(jsonObject2.ContainsKey("nestedkey2"));
            Assert.AreEqual("nestedvalue2", jsonObject2["nestedkey2"].GetString());

            var nestedObject = jsonObject2["nestedkey1"].GetValue();
            Assert.IsTrue(nestedObject.ContainsKey("deeplynestedkey1"));
            Assert.AreEqual("deeplynestedvalue1", nestedObject["deeplynestedkey1"].GetString());


        }
    }
}
