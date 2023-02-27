using System.Diagnostics;
using System.Numerics;
namespace ECC
{
    public static class BigIntegerExtension
    {
        public static readonly ThreadLocal<Random> s_Gen = new(() => new Random());

        // Random generator (thread safe)
        public static Random? Gen => s_Gen.Value;
        public static BigInteger NextBigInteger(BigInteger minValue, BigInteger maxValue)
        {
            if (minValue > maxValue) throw new ArgumentException("wrong order");
            if (minValue == maxValue) return minValue;
            BigInteger zeroBasedUpperBound = maxValue - 1 - minValue; // Inclusive
            Debug.Assert(zeroBasedUpperBound.Sign >= 0);
            byte[] bytes = zeroBasedUpperBound.ToByteArray();
            Debug.Assert(bytes.Length > 0);
            Debug.Assert((bytes[^1] & 0b10000000) == 0);

            // Search for the most significant non-zero bit
            byte lastByteMask = 0b11111111;
            for (byte mask = 0b10000000; mask > 0; mask >>= 1, lastByteMask >>= 1)
            {
                if ((bytes[^1] & mask) == mask) break; // We found it
            }

            while (true)
            {
                Gen?.NextBytes(bytes);
                bytes[^1] &= lastByteMask;
                var result = new BigInteger(bytes);
                Debug.Assert(result.Sign >= 0);
                if (result <= zeroBasedUpperBound) return result + minValue;
            }
        }
        public static bool IsProbablyPrime(this BigInteger value, int witnesses = 10)
        {
            if (value <= 1)
                return false;

            if (witnesses <= 0)
                witnesses = 10;

            BigInteger d = value - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            byte[] bytes = new byte[value.GetByteCount()];
            BigInteger a;

            for (int i = 0; i < witnesses; i++)
            {
                do
                {
                    Gen?.NextBytes(bytes);

                    a = new BigInteger(bytes);
                }
                while (a < 2 || a >= value - 2);

                BigInteger x = BigInteger.ModPow(a, d, value);
                if (x == 1 || x == value - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, value);
                    if (x == 1) return false;
                    if (x == value - 1) break;
                }

                if (x != value - 1) return false;
            }
            return true;
        }
        public static string BigIntegerToBinaryString(BigInteger x)
        {
            // Setup source
            ReadOnlySpan<byte> srcBytes = x.ToByteArray();
            int srcLoc = srcBytes.Length - 1;

            // Find the first bit set in the first byte so we don't print extra zeros.
            int msb = BitOperations.Log2(srcBytes[srcLoc]);

            // Setup Target
            Span<char> dstBytes = stackalloc char[srcLoc * 8 + msb + 2];
            int dstLoc = 0;

            // Add leading '-' sign if negative.
            if (x.Sign < 0)
            {
                dstBytes[dstLoc++] = '-';
            }
            //else if (!x.IsZero) dstBytes[dstLoc++] = '0'; // add adding leading '0' (optional)

            // The first byte is special because we don't want to print leading zeros.
            byte b = srcBytes[srcLoc--];
            for (int j = msb; j >= 0; j--)
            {
                dstBytes[dstLoc++] = (char)('0' + ((b >> j) & 1));
            }

            // Add the remaining bits.
            for (; srcLoc >= 0; srcLoc--)
            {
                byte b2 = srcBytes[srcLoc];
                for (int j = 7; j >= 0; j--)
                {
                    dstBytes[dstLoc++] = (char)('0' + ((b2 >> j) & 1));
                }
            }

            return dstBytes.ToString();
        }
        public static BigInteger NewtonPlusSqrt(BigInteger x)
        {
            if (x < 144838757784765629)    // 1.448e17 = ~1<<57
            {
                uint vInt = (uint)Math.Sqrt((ulong)x);
                if ((x >= 4503599761588224) && ((ulong)vInt * vInt > (ulong)x))  // 4.5e15 =  ~1<<52
                {
                    vInt--;
                }
                return vInt;
            }

            double xAsDub = (double)x;
            if (xAsDub < 8.5e37)   //  long.max*long.max
            {
                ulong vInt = (ulong)Math.Sqrt(xAsDub);
                BigInteger v = (vInt + ((ulong)(x / vInt))) >> 1;
                return (v * v <= x) ? v : v - 1;
            }

            if (xAsDub < 4.3322e127)
            {
                BigInteger v = (BigInteger)Math.Sqrt(xAsDub);
                v = (v + (x / v)) >> 1;
                if (xAsDub > 2e63)
                {
                    v = (v + (x / v)) >> 1;
                }
                return (v * v <= x) ? v : v - 1;
            }

            int xLen = (int)x.GetBitLength();
            int wantedPrecision = (xLen + 1) / 2;
            int xLenMod = xLen + (xLen & 1) + 1;

            //////// Do the first Sqrt on hardware ////////
            long tempX = (long)(x >> (xLenMod - 63));
            double tempSqrt1 = Math.Sqrt(tempX);
            ulong valLong = (ulong)BitConverter.DoubleToInt64Bits(tempSqrt1) & 0x1fffffffffffffL;
            if (valLong == 0)
            {
                valLong = 1UL << 53;
            }

            ////////  Classic Newton Iterations ////////
            BigInteger val = ((BigInteger)valLong << 52) + (x >> xLenMod - (3 * 53)) / valLong;
            int size = 106;
            for (; size < 256; size <<= 1)
            {
                val = (val << (size - 1)) + (x >> xLenMod - (3 * size)) / val;
            }

            if (xAsDub > 4e254) // 4e254 = 1<<845.76973610139
            {
                int numOfNewtonSteps = BitOperations.Log2((uint)(wantedPrecision / size)) + 2;

                //////  Apply Starting Size  ////////
                int wantedSize = (wantedPrecision >> numOfNewtonSteps) + 2;
                int needToShiftBy = size - wantedSize;
                val >>= needToShiftBy;
                size = wantedSize;
                do
                {
                    ////////  Newton Plus Iterations  ////////
                    int shiftX = xLenMod - (3 * size);
                    BigInteger valSqrd = (val * val) << (size - 1);
                    BigInteger valSU = (x >> shiftX) - valSqrd;
                    val = (val << size) + (valSU / val);
                    size *= 2;
                } while (size < wantedPrecision);
            }

            /////// There are a few extra digits here, lets save them ///////
            int oversidedBy = size - wantedPrecision;
            BigInteger saveDroppedDigitsBI = val & ((BigInteger.One << oversidedBy) - 1);
            int downby = (oversidedBy < 64) ? (oversidedBy >> 2) + 1 : (oversidedBy - 32);
            ulong saveDroppedDigits = (ulong)(saveDroppedDigitsBI >> downby);


            ////////  Shrink result to wanted Precision  ////////
            val >>= oversidedBy;


            ////////  Detect a round-ups  ////////
            if ((saveDroppedDigits == 0) && (val * val > x))
            {
                val--;
            }

            ////////// Error Detection ////////
            //// I believe the above has no errors but to guarantee the following can be added.
            //// If an error is found, please report it.
            //BigInteger tmp = val * val;
            //if (tmp > x)
            //{
            //    Console.WriteLine($"Missed  , {ToolsForOther.ToBinaryString(saveDroppedDigitsBI, oversidedBy)}, {oversidedBy}, {size}, {wantedPrecision}, {saveDroppedDigitsBI.GetBitLength()}");
            //    if (saveDroppedDigitsBI.GetBitLength() >= 6)
            //        Console.WriteLine($"val^2 ({tmp}) < x({x})  off%:{((double)(tmp)) / (double)x}");
            //    //throw new Exception("Sqrt function had internal error - value too high");
            //}
            //if ((tmp + 2 * val + 1) <= x)
            //{
            //    Console.WriteLine($"(val+1)^2({((val + 1) * (val + 1))}) >= x({x})");
            //    //throw new Exception("Sqrt function had internal error - value too low");
            //}

            return val;
        }
    }
}
