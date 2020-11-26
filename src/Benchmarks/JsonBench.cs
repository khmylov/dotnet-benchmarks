using System.Collections.Generic;
using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class JsonBench
    {
        private readonly StaticData _staticData = new StaticData {Foo = 1, Bar = "Baz"};

        private readonly DynamicData _dynamicData = new DynamicData
            {Foo = 2, DynamicProperties = new Dictionary<string, object> {["Custom1"] = 20, ["Custom2"] = "A"}};

        [Benchmark]
        public void StaticData()
        {
            JsonConvert.SerializeObject(_staticData);
        }

        [Benchmark]
        public void DynamicData()
        {
            JsonConvert.SerializeObject(_dynamicData);
        }

        [Benchmark]
        public void DeserializeStatic()
        {
            JsonConvert.DeserializeObject<StaticData>("{\"Foo\":1, \"Bar\": \"Baz\"}");
        }

        [Benchmark]
        public void DeserializeDynamic()
        {
            JsonConvert.DeserializeObject<DynamicData>("{\"Foo\":1, \"Custom1\": \"Baz\"}");
        }
    }

    public class StaticData
    {
        public int Foo { get; set; }

        public string Bar { get; set; }
    }

    public class DynamicData
    {
        public int Foo { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> DynamicProperties { get; set; } = new Dictionary<string, object>();
    }

}
