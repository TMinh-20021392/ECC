using System.Numerics;
namespace ECC
{
    class CipollaAlgorithm
    {
        public static BigInteger C(BigInteger n, BigInteger p)
        {
            // Step 1: Find a, omega2
            BigInteger a = 0;
            BigInteger omega2;
            while (true)
            {
                omega2 = (a * a + p - n) % p;
                if (Form1.L(omega2, p) == -1)
                {
                    break;
                }
                a += 1;
            }

            // Multiplication in Fp2
            BigInteger finalOmega = omega2;
            Tuple<BigInteger, BigInteger> mul(Tuple<BigInteger, BigInteger> aa, Tuple<BigInteger, BigInteger> bb)
            {
                return new Tuple<BigInteger, BigInteger>(
                    (aa.Item1 * bb.Item1 + aa.Item2 * bb.Item2 * finalOmega) % p,
                    (aa.Item1 * bb.Item2 + bb.Item1 * aa.Item2) % p
                );
            }

            // Step 2: Compute power
            Tuple<BigInteger, BigInteger> r = new(1, 0);
            Tuple<BigInteger, BigInteger> s = new(a, 1);
            BigInteger nn = ((p + 1) >> 1) % p;
            while (nn > 0)
            {
                if ((nn & 1) == 1)
                {
                    r = mul(r, s);
                }
                s = mul(s, s);
                nn >>= 1;
            }

            // Step 3: Check x in Fp
            if (r.Item2 != 0)
            {
                return 0;
            }

            // Step 5: Check x * x = n
            if (r.Item1 * r.Item1 % p != n)
            {
                return 0;
            }

            // Step 4: Solutions
            return r.Item1;
        }
    }
}
