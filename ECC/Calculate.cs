using System.Diagnostics;
using System.Numerics;
namespace ECC
{
    public partial class Form1
    {
        public static Tuple<BigInteger, BigInteger> Multiply(Tuple<BigInteger, BigInteger> tuple, BigInteger k, BigInteger a, BigInteger p)
        {
            Tuple<BigInteger, BigInteger> temp = tuple;
            while (k - 1 > 0)
            {
                tuple = Calculate(tuple, temp, a, p);
                k--;
            }
            return tuple;
        }
        public static BigInteger TryModInverse(BigInteger number, BigInteger modulo)
        {
            if (number < 1)
            {
                number = Rev(number, modulo);
            }
            if (BigIntegerExtension.IsProbablyPrime(modulo)) return BigInteger.ModPow(number, modulo - 2, modulo);
            //if (modulo < 2) throw new ArgumentOutOfRangeException(nameof(modulo));
            BigInteger n = number;
            BigInteger m = modulo, v = 0, d = 1;
            while (n > 0)
            {
                BigInteger t = m / n, x = n;
                n = m % x;
                m = x;
                x = d;
                d = checked(v - t * x); // Just in case
                v = x;
            }
            BigInteger result = v % modulo;
            if (result < 0) result += modulo;
            if (number * result % modulo == 1) return result;
            return 0;
        }
        public static Tuple<BigInteger, BigInteger> Calculate(Tuple<BigInteger, BigInteger> tuple1, Tuple<BigInteger, BigInteger> tuple2,
            BigInteger a, BigInteger p)
        {
            BigInteger lambda;
            var x1 = tuple1.Item1;
            var y1 = tuple1.Item2;
            var x2 = tuple2.Item1;
            var y2 = tuple2.Item2;
            if (x1 == -1 && y1 == -1)
            {
                return Tuple.Create(x2, y2);
            }
            else if (x2 == -1 && y2 == -1)
            {
                return Tuple.Create(x1, y1);
            }
            else
            {
                if (x1 == x2 && y1 == y2)
                {
                    if (y1 == 0)
                    {
                        return Tuple.Create(BigInteger.One * -1, BigInteger.One * -1);
                    }
                    lambda = (3 * BigInteger.Pow(x1, 2) + a) * TryModInverse(2 * y1, p) % p;
                    var x3 = (BigInteger.Pow(lambda, 2) - 2 * x1) % p;
                    var y3 = Rev(lambda * (x1 - x3) - y1, p) % p;
                    return Tuple.Create(Rev(x3, p), y3);
                }
                else
                {
                    if (x1 == x2 && y1 != y2)
                    {
                        return Tuple.Create(BigInteger.One * -1, BigInteger.One * -1);
                    }
                    lambda = Rev((y1 - y2) * TryModInverse(x1 - x2, p), p) % p;
                    var x3 = (BigInteger.Pow(lambda, 2) - x1 - x2) % p;
                    var y3 = Rev(lambda * (x1 - x3) - y1, p) % p;
                    return Tuple.Create(Rev(x3, p), y3);
                }
            }
        }
        static BigInteger Rev(BigInteger a, BigInteger p)
        {
            if (a < 0)
            {
                a %= p;
                a += p;
            }
            return a;
        }
        public static bool PerfectSQR(BigInteger A)
        {
            BigInteger[] ar = { 0, 1, 4, 5, 6, 9 };
            if (ar.Contains(BigInteger.Remainder(A, 10)))
            {
                var B = Math.Exp(BigInteger.Log(A) / 2);
                return (B * B).Equals(A);
            }
            return false;
        }
        static IEnumerable<BigInteger> Range(BigInteger fromInclusive, BigInteger toExclusive)
        {
            for (BigInteger i = fromInclusive; i < toExclusive; i++) yield return i;
        }
        public static void ParallelFor(BigInteger fromInclusive, BigInteger toExclusive, Action<BigInteger, ParallelLoopState> body)
        {
            Parallel.ForEach(Range(fromInclusive, toExclusive), body);
        }
        static BigInteger P(string py, string ar, string im, string mod)
        {
            string arg = string.Format(py, ar, im, mod);
            Process pro = new()
            {
                StartInfo = new ProcessStartInfo(@"python3.10.exe", arg)
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            pro.Start();
            string output = pro.StandardOutput.ReadToEnd();
            pro.WaitForExit();
            var p = BigInteger.Parse(output);
            return p;
        }
        static BigInteger QuadraticNonresidue(BigInteger p)
        {
            while (true)
            {
                var a = BigIntegerExtension.NextBigInteger(0, p - 1);
                if (L(a, p) == -1)
                {
                    return a;
                }
            }
        }
        public static BigInteger L(BigInteger n, BigInteger k)
        {
            var a = BigInteger.ModPow(n, (k - 1) / 2, k);
            if (a == k - 1) return -1;
            return a;
        }
    }
}
