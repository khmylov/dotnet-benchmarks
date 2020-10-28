using BenchmarkDotNet.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Benchmarks
{
    public class StringifyBench
    {
        private const string _value = "first\nsecond";

        [Benchmark]
        public void JsonNetJValue()
        {
            JValue.CreateString(_value).ToString(Formatting.None);
        }

        [Benchmark]
        public void JsonNetSerialize()
        {
            JsonConvert.SerializeObject(_value);
        }

        [Benchmark]
        public void StringReplace()
        {
            var _ = "\"" + _value.Replace("\"", "\\\"").Replace("\n", "\\\n").Replace("\\", "\\\\") + "\"";
        }

        [Benchmark]
        public void SystemTextJson()
        {
            System.Text.Json.JsonSerializer.Serialize(_value);
        }
    }
}
