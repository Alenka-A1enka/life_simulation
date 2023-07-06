using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ИИ_ЛР8
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        Bitmap bmp;
        Graphics g;
        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Width = 450;
            pictureBox1.Height = 500;
            pictureBox1.Location = new Point(1, 1);


            f = new Field(40, 800, 200, 200);

            PrintField();
            Agent.field = f;
        }
        Field f;
        string what_happen = "";
        private void button1_Click(object sender, EventArgs e)
        {
            int count = Convert.ToInt32(textBox1.Text);
            for (int i = 0; i < count; i++)
            {

                f.GetNextTact(ref what_happen);
            }
            richTextBox1.Text = what_happen;
            PrintField();
        }
        int count1 = 0;
        int count2 = 0;
        private void PrintField()
        {
            count1 = 0;
            count2 = 0;
            pictureBox1.Image = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            bmp = (Bitmap)pictureBox1.Image;
            g = Graphics.FromImage(bmp);
            pictureBox1.Image = bmp;
            int size = f.size;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Color color = Color.White;
                    if (f.field[i, j].type == 0) color = Color.White;
                    if (f.field[i, j].type == 1) color = Color.Green;
                    if (f.field[i, j].type == 2) { color = Color.Blue; count1++; }
                    if (f.field[i, j].type == 3) { color = Color.Red; count2++; }


                    g.DrawString("X", new Font(new FontFamily("Helvetica"), 10, FontStyle.Regular, GraphicsUnit.Point),
                   new SolidBrush(color), new Point(i * 10, j * 12));

                }
            }
            pictureBox1.Image = bmp;
            label3.Text = count1.ToString();
            label4.Text = count2.ToString();
        }
        //public static void PrintMassive(double[] mas)
        //{
        //    string s = "";
        //    for (int i = 0; i < mas.Length; i++)
        //    {
        //        s += mas[i] + "    \n";
        //    }
        //    RichTextBoxPrint(s);
        //}
    }
}
