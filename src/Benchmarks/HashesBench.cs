using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    public class HashesBench
    {
        private static readonly SHA256 _sha256 = SHA256.Create();
        private static readonly SHA256Cng _sha256Cng = new SHA256Cng();
        private static readonly SHA256CryptoServiceProvider _sha256CSP = new SHA256CryptoServiceProvider();
        private static readonly SHA512 _sha512 = SHA512.Create();
        private static readonly SHA512Cng _sha512Cng = new SHA512Cng();
        private static readonly SHA512Managed _sha512Managed = new SHA512Managed();
        private static readonly SHA512CryptoServiceProvider _sha512CSP = new SHA512CryptoServiceProvider();

        private static readonly string _s =
            "CommentDto;null;null;>;e;214;39;b;>;{Id,ParentId,CreateDate,Description,IsPrivate,IsPinned,General:{General.Id},Owner:{Owner.Id,Owner.FirstName,Owner.LastName,Owner.Kind,FullName:String.Concat(Owner.FirstName,\" \",Owner.LastName).Trim()}}";
        
        private static readonly byte[] _inputBytes = Encoding.UTF8.GetBytes(_s);

        
        [Benchmark]
        public void Sha256()
        {
            _sha256.ComputeHash(_inputBytes);
        }
        
        [Benchmark]
        public void Sha256Cng()
        {
            _sha256Cng.ComputeHash(_inputBytes);
        }
        
        [Benchmark]
        public void Sha256CSP()
        {
            _sha256CSP.ComputeHash(_inputBytes);
        }
        
        [Benchmark]
        public void Sha512()
        {
            _sha512.ComputeHash(_inputBytes);
        }
        
        [Benchmark]
        public void Sha512Cng()
        {
            _sha512Cng.ComputeHash(_inputBytes);
        }
        
        [Benchmark]
        public void Sha512Managed()
        {
            _sha512Managed.ComputeHash(_inputBytes);
        }

        [Benchmark]
        public void Sha512CSP()
        {
            _sha512CSP.ComputeHash(_inputBytes);
        }
    }

    public class Hashes2Bench
    {
        private static readonly SHA256Managed _sharedValue = new SHA256Managed();

        private static readonly byte[] _bytes = Encoding.UTF8.GetBytes("This is a test string");
        
        [Benchmark]
        public void SharedInstance()
        {
            _sharedValue.ComputeHash(_bytes);
        }

        [Benchmark]
        public void CreateNew()
        {
            new SHA256Managed().ComputeHash(_bytes);
        }

        [Benchmark]
        public void ThreadStaticValue()
        {
            ShaContainer.Value.ComputeHash(_bytes);
        }

        [Benchmark]
        public void ThreadLocal()
        {
            ShaContainer.Value2.Value.ComputeHash(_bytes);
        }
        
        private class ShaContainer
        {
            public static readonly ThreadLocal<SHA256Managed> Value2 = new ThreadLocal<SHA256Managed>(() => new SHA256Managed());
            
            [ThreadStatic] private static SHA256Managed _value;

            public static SHA256Managed Value
            {
                get
                {
                    if (_value is null)
                    {
                        _value = new SHA256Managed();
                    }

                    return _value;
                }
            }
        }
    }
}