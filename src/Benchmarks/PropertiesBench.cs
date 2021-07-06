using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class PropertiesBench
    {
        private readonly Data _untyped = new Data(new object());
        private readonly DataInherited<object> _inherited = new DataInherited<object>(new object());
        private readonly DataTypeCasted<object> _typeCasted = new DataTypeCasted<object>(new object());
        private readonly Data<object> _typed = new Data<object>(new object());

        [Benchmark]
        public void Untyped()
        {
            var _ = _untyped.Value;
        }

        [Benchmark]
        public void Typed()
        {
            var _ = _typed.Value;
        }

        [Benchmark]
        public void Inherited()
        {
            var _ = _inherited.Value;
        }

        [Benchmark]
        public void TypeCasted()
        {
            var _ = _typeCasted.Value;
        }

        public class DataInherited<T> : Data
        {
            public DataInherited(T value) : base(value)
            {
            }

            public new T Value => (T) base.Value;
        }

        public class DataTypeCasted<T>
        {
            private readonly object _value;

            public DataTypeCasted(object value)
            {
                _value = value;
            }

            public T Value => (T) _value;
        }

        public class Data
        {
            public Data(object value)
            {
                Value = value;
            }

            public object Value { get; }
        }

        public class Data<T>
        {
            public Data(T value)
            {
                Value = value;
            }

            public T Value { get; }
        }
    }
}
