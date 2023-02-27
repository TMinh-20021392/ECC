using System.Numerics;

namespace ECC
{
    public partial class Form1
    {
        public void H_m_MouseClick(object sender, MouseEventArgs e)
        {
            if (!string.IsNullOrEmpty(pointM.Text))
            {
                H_m_.Text = NeedToCrypt(pointM.Text).ToString();
            }
        }
        public void Sign_Click(object sender, EventArgs e)
        {
            Data();
            var k = BigInteger.Parse(textBox7.Text);
            var h = BigInteger.Parse(H_m_.Text);
            var d = BigInteger.Parse(textBox1.Text);
            var R = Multiply(Parse(pointg.Text), k, a, p);
            kA.Text = R.ToString();
            var R1 = R.Item1 % N;
            BigInteger S = -1;
            while (R1 == 0 || S == 0)
            {
                k++;
                if (k % N == 0) k++;
                k %= N;
                R = Multiply(Parse(pointg.Text), k, a, p);
                R1 = R.Item1 % N;
                if (comboBox1.SelectedIndex == 1)
                {
                    S = (k * R1 - h) * d % N;
                }
                else
                {
                    S = (h + d * R.Item1) * TryModInverse(k, N) % N;
                }
                if (S == 0) continue;
                break;
            }
            if (comboBox1.SelectedIndex == 1)
            {
                S = (k * R1 - h) * d % N;
            }
            else
            {
                S = (h + d * R.Item1) * TryModInverse(k, N) % N;
            }
            while (S < 0)
            {
                S += N;
            }
            textBox7.Text = k.ToString();
            kA.Text = R.ToString();
            r.Text = R1.ToString();
            Sig.Text = S.ToString();
        }
        public void Verify_Click(object sender, EventArgs e)
        {
            Data();
            BigInteger u1, u2, w;
            if (comboBox1.SelectedIndex == 1)
            {
                w = TryModInverse(BigInteger.Parse(r.Text), N);
                u2 = w * BigInteger.Parse(Sig.Text) % N;
            }
            else
            {
                w = TryModInverse(BigInteger.Parse(Sig.Text), N);
                u2 = w * BigInteger.Parse(r.Text) % N;
            }
            textBox11.Text = w.ToString();
            u1 = w * BigInteger.Parse(H_m_.Text) % N;
            textBox12.Text = u1.ToString();
            textBox14.Text = u2.ToString();
            var P = Calculate(Multiply(Parse(pointg.Text), u1, a, p), Multiply(Parse(pointQ.Text), u2, a, p), a, p);
            textBox15.Text = P.ToString();
            textBox16.Text = (P.Item1 == BigInteger.Parse(r.Text)).ToString();
        }
        public void KEYPAIR_Click(object sender, EventArgs e)
        {
            Data();
            if (!string.IsNullOrEmpty(textBox1.Text) && !string.IsNullOrEmpty(pointg.Text))
            {
                if (comboBox1.SelectedIndex == 0)
                {
                    pointQ.Text = Multiply(Parse(pointg.Text), BigInteger.Parse(textBox1.Text), a, p).ToString();
                }
                else
                {
                    pointQ.Text = Multiply(Parse(pointg.Text), TryModInverse(BigInteger.Parse(textBox1.Text), N), a, p).ToString();
                }
            }
        }
        public void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            panel1.Controls.OfType<TextBox>()
                           .ToList()
                           .ForEach(t =>
                           {
                               if (t.Name != "H_m_") t.Text = "";
                           });
        }
    }
}
