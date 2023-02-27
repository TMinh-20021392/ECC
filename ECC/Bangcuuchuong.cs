using System.Numerics;

namespace ECC
{
    public partial class Form1
    {
        bool clicked;
        public void Populate2DtoGridview(object[,] twodim)
        {
            //height = twodim.GetLength(0);
            //width = twodim.GetLength(1);
            dataGridView1.ColumnCount = twodim.GetLength(1);

            for (int r = 0; r < twodim.GetLength(0); r++)
            {
                DataGridViewRow row = new();
                row.CreateCells(dataGridView1);

                for (int c = 0; c < twodim.GetLength(1); c++)
                {
                    if (twodim[r, c].Equals(Tuple.Create(BigInteger.One * -2, BigInteger.One * -2)))
                    {
                        if (BangCuuChuong.SelectedIndex == 0)
                        {
                            row.Cells[c].Value = "+";
                        }
                        else
                        {
                            row.Cells[c].Value = "x";
                        }
                        row.Cells[c].Style.BackColor = Color.Red;
                    }
                    else if (twodim[r, c].Equals(Tuple.Create(BigInteger.One * -1, BigInteger.One * -1)))
                    {
                        row.Cells[c].Value = "O";
                    }
                    else
                    {
                        row.Cells[c].Value = twodim[r, c];
                        if (r == 0 || c == 0)
                        {
                            row.Cells[c].Style.BackColor = Color.Orange;
                        }
                    }
                }
                dataGridView1.Rows.Add(row);
            }
        }
        public Tuple<BigInteger, BigInteger>[,] AdditionTableArray()
        {
            var max = bigs.Count + 1;
            if (max >= 50)
            {
                max = 50;
            }
            Tuple<BigInteger, BigInteger>[,] twodim = new Tuple<BigInteger, BigInteger>[max, max];
            for (int k = 0; k < twodim.GetLength(0); k++)
            {
                for (int l = 0; l < twodim.GetLength(1); l++)
                {
                    if (k == 0 && l == 0)
                    {
                        twodim[k, l] = Tuple.Create(BigInteger.One * -2, BigInteger.One * -2);
                    }
                    else if (k == 0)
                    {
                        twodim[k, l] = bigs.ElementAt(l - 1);
                    }
                    else if (l == 0)
                    {
                        twodim[k, l] = bigs.ElementAt(k - 1);
                    }
                    else
                    {
                        twodim[k, l] = Calculate(bigs.ElementAt(k - 1), bigs.ElementAt(l - 1), a, p);
                    }
                }
            }
            return twodim;
        }
        public object[,] MultiplyTableArray()
        {
            var maxn = N + 1;
            if (maxn >= (bigs.Count + 1))
            {
                maxn = 50;
            }
            object[,] twodim = new object[(int)maxn, bigs.Count + 1];
            for (int k = 0; k < twodim.GetLength(0); k++)
            {
                for (int l = 0; l < twodim.GetLength(1); l++)
                {
                    if (k == 0 && l == 0)
                    {
                        twodim[k, l] = Tuple.Create(BigInteger.One * -2, BigInteger.One * -2);
                    }
                    else if (k == 0)
                    {
                        twodim[k, l] = bigs.ElementAt(l - 1);
                    }
                    else if (l == 0)
                    {
                        twodim[k, l] = k;
                    }
                    else
                    {
                        twodim[k, l] = Multiply(bigs.ElementAt(l - 1), k, a, p);
                    }
                }
            }
            return twodim;
        }
        public void ListOfPointsUpdate()
        {
            BigInteger p1 = p;
            for (BigInteger x = 0; x <= (p - 1) / 2; x++)
            {
                dic.Add(BigInteger.ModPow(x, 2, p), x);
                if (x > 5000000) break;
            }
            if (p > ulong.MaxValue)
            {
                p1 = 5000000;
                MAXIMUMINDEX = 4;
            }
            for (BigInteger x = 0; x < p1; x++)
            {
                var y2 = (BigInteger.Pow(x, 3) + a * x + b) % p;
                if (dic.TryGetValue(y2, out BigInteger value))
                {
                    bigs.Add(Tuple.Create(x, value));
                    if (value != 0) bigs.Add(Tuple.Create(x, p - value));
                    if (p > ulong.MaxValue && bigs.Count >= MAXIMUMINDEX) break;
                }
            }
        }
        public void DataGridView1_DoubleCellClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            clicked = false;
            var cell = dataGridView1[e.ColumnIndex, e.RowIndex];
            if (lastClick != null)
            {
                lastClick.Style.BackColor = lastStyle;
            }
            if (cell.HasStyle)
            {
                lastStyle = cell.Style.BackColor;
            }
            else
            {
                lastStyle = Color.Empty;
            }
            cell.Style.BackColor = Color.CornflowerBlue;
            xP.Text = Parse(cell.Value.ToString()!).Item1.ToString();
            yP.Text = Parse(cell.Value.ToString()!).Item2.ToString();
            pointg.Text = Parse(cell.Value.ToString()!).ToString();
            lastClick = cell;
        }
        public async void DataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (clicked) return;
            clicked = true;
            await Task.Delay(SystemInformation.DoubleClickTime);
            if (!clicked) return;
            clicked = false;
            if (eCCToolStripMenuItem.Checked == true) pointM.Text = dataGridView1.CurrentCell.Value.ToString();
        }
        public void TableActivation()
        {
            ListOfPointsUpdate();
            object[,] twodim;
            if (BangCuuChuong.SelectedIndex == 1)
            {
                twodim = MultiplyTableArray();
            }
            else
            {
                twodim = AdditionTableArray();
            }
            Populate2DtoGridview(twodim);
        }
    }
}
