using System.Collections.Concurrent;
using System.Numerics;

namespace ECC
{
    public partial class Form1
    {
        BigInteger MAXIMUMINDEX = 50; //Cannot be higher than 65535
        BigInteger a, b, p, x1, x2, y1, y2, s, k, N, mA, mB;
        readonly Dictionary<BigInteger, BigInteger> dic = new();
        readonly List<Tuple<BigInteger, BigInteger>> bigs = new();
        public DataGridViewCell? lastClick;
        public Color lastStyle = Color.Empty;
        public void Data()
        {
            x1 = BigInteger.Parse(xP.Text);
            y1 = BigInteger.Parse(yP.Text);
            x2 = BigInteger.Parse(xQ.Text);
            y2 = BigInteger.Parse(yQ.Text);
            a = BigInteger.Parse(textBox5.Text);
            p = BigInteger.Parse(textBox4.Text);
            b = BigInteger.Parse(textBox6.Text);
            k = BigInteger.Parse(textBox7.Text);
            s = BigInteger.Parse(textBox8.Text);
            mA = BigInteger.Parse(textBox2.Text);
            mB = BigInteger.Parse(textBox13.Text);
            dic.Clear();
        }
        public static Tuple<BigInteger, BigInteger> Parse(string s)
        {
            try
            {
                var bi = s.Trim('(', ')').Split(',');
                BigInteger x = BigInteger.Parse(bi[0]);
                BigInteger y = BigInteger.Parse(bi[1]);
                return Tuple.Create(x, y);
            }
            catch (Exception)
            {
                return Tuple.Create(BigInteger.One * -1, BigInteger.One * -1);
            }
        }
        public static BigInteger NeedToCrypt(string s)
        {
            if (!BigInteger.TryParse(s, out BigInteger msg))
            {
                for (int i = s.Length - 1; i > -1; i--)
                {
                    var temp = i;
                    var index = s[temp] % 32 - 1;
                    msg += index * BigInteger.Pow(26, temp);
                }
            }
            return msg;
        }
    }
}
