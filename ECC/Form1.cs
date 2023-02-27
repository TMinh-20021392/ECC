using System.Diagnostics;
using System.Numerics;

namespace ECC
{
    public partial class Form1 : Form
    {
        BigInteger count = 1;
        public void M1_MouseClick(object sender, MouseEventArgs e)
        {
            Data();
            while (mA < N)
            {
                if (BigInteger.GreatestCommonDivisor(mA, N) == 1)
                    break;
                else
                    mA++;
            }
            var M = Parse(pointM.Text);
            M1.Text = Multiply(M, mA, a, p).ToString();
        }
        public void M2_MouseClick(object sender, MouseEventArgs e)
        {
            Data();
            while (mB < N)
            {
                if (BigInteger.GreatestCommonDivisor(mB, N) == 1)
                    break;
                else
                    mB++;
            }
            var M = Parse(M1.Text);
            M2.Text = Multiply(M, mB, a, p).ToString();
        }
        public void M3_MouseClick(object sender, MouseEventArgs e)
        {
            var M = Parse(M2.Text);
            M3.Text = Multiply(M, TryModInverse(mA, N), a, p).ToString();
        }
        public void M4_MouseClick(object sender, MouseEventArgs e)
        {
            var M = Parse(M3.Text);
            M4.Text = Multiply(M, TryModInverse(mB, N), a, p).ToString();
        }
        public Form1()
        {
            InitializeComponent();
            BangCuuChuong.SelectedIndex = 1;
            comboBox1.SelectedIndex = 0;
        }
        public void ADD_Click(object sender, EventArgs e)
        {
            Data();
            pointR.Text = Calculate(Tuple.Create(x1, y1), Tuple.Create(x2, y2), a, p).ToString();
        }
        public void MULTIPLY_Click(object sender, EventArgs e)
        {
            Data();
            var tuple = Multiply(Tuple.Create(x1, y1), k, a, p);
            pointR.Text = tuple.ToString();
        }
        public void NumberOfPoints_Click(object sender, EventArgs e)
        {
            var watch = Stopwatch.StartNew();
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Refresh();
            Data();
            string directory = AppDomain.CurrentDomain.BaseDirectory[..^25];
            string py = directory + @"Ellinit.py {0} {1} {2}";
            //string py = directory + @"pointcounting.py {0} {1} {2}";
            N = P(py, a.ToString(), b.ToString(), p.ToString());
            textBox5.Text = a.ToString();
            textBox6.Text = b.ToString();
            NumOfPoints.Text = N.ToString();

            TableActivation();
            bigs.Clear();
            //watch.Stop();
            //MessageBox.Show(watch.ElapsedMilliseconds.ToString());
        }
        public void PointB_TextChanged(object sender, EventArgs e)
        {
            Data();
            pointB.Text = Multiply(Tuple.Create(x1, y1), s, a, p).ToString();
        }
        public void ECElgamalSolve_Click(object sender, EventArgs e)
        {
            Data();
            var M1 = Parse(pointM1.Text);
            var sM1 = Multiply(M1, s, a, p);
            sM1 = Tuple.Create(sM1.Item1, Rev(-sM1.Item2, p));
            var M2 = Parse(pointM2.Text);
            ECElgamalDecrypt.Text = Calculate(M2, sM1, a, p).ToString();
        }
        public void ECElgamalEncrypt_Click(object sender, EventArgs e)
        {
            Data();
            var tuple = Multiply(Tuple.Create(x1, y1), k, a, p);
            pointM1.Text = tuple.ToString();
            var B = Parse(pointB.Text);
            var M = Parse(pointM.Text);
            tuple = Multiply(B, k, a, p);
            tuple = Calculate(tuple, M, a, p);
            pointM2.Text = tuple.ToString();
        }
        public void ECCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            eCCToolStripMenuItem.Checked = true;
            digitalSignatureECCToolStripMenuItem.Checked = false;
            panel1.Enabled = false;
            panel1.Visible = false;
            splitContainer1.Visible = true;
            splitContainer1.Enabled = true;
            splitContainer1.Size = new Size(1378, 170);
            splitContainer1.Location = new Point(12, 425);
        }
        public void DigitalSignatureECCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            digitalSignatureECCToolStripMenuItem.Checked = true;
            eCCToolStripMenuItem.Checked = false;
            panel1.Enabled = true;
            panel1.Visible = true;
            splitContainer1.Visible = false;
            splitContainer1.Enabled = false;
        }

    }
}