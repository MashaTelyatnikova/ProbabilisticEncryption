using System;
using System.Numerics;

namespace ProbabilisticEncryption
{
    public static class BigIntegerExtensions
    {
        public static BigInteger Mode(this BigInteger a, BigInteger modular)
        {
            var result = a % modular;
            if (result < 0)
            {
                result += modular;
            }
            return result;
        }

        public static BigInteger Invert(this BigInteger a, BigInteger m)
        {
            BigInteger x = 0;
            BigInteger y = 0;
            var g = Gcd(a, m, ref x, ref y);
            if (g != 1)
            {
                throw new ArgumentException($"Нет решения для a = {a} и m = {m}");
            }
            return (x % m + m) % m;
        }

        public static BigInteger Pow(this BigInteger a, int pow)
        {
            BigInteger result = a;
            while (pow > 0)
            {
                result *= a;
                --pow;
            }

            return result;
        }

        private static BigInteger Gcd(BigInteger a, BigInteger b, ref BigInteger x, ref BigInteger y)
        {
            if (a == 0)
            {
                x = 0;
                y = 1;
                return b;
            }

            BigInteger x1 = 0;
            BigInteger y1 = 0;
            var d = Gcd(b % a, a, ref x1, ref y1);
            x = y1 - (b / a) * x1;
            y = x1;
            return d;
        }
    }
}
