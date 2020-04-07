using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WinForms.NetFramework
{
    public partial class Form1 : Form
    {
        private List<Bitmap> _bitmaps = new List<Bitmap>();
        private Random _random= new Random();
        public Form1()
        {
            InitializeComponent();
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }
        private List<Pixel> GetPixels (Bitmap bitmap)
        {
            var pixels = new List<Pixel>(bitmap.Width*bitmap.Height);
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    pixels.Add(new Pixel()
                    {
                        Color = bitmap.GetPixel(x, y),
                        Point = new Point()
                        {
                            X = x, Y = y
                        }
                    }
                        ) ;

                }
            }
            return pixels;
        }
        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var time = Stopwatch.StartNew();
                menuStrip1.Enabled = trackBar1.Enabled = false;
                pictureBox1.Image = null;
                _bitmaps.Clear();
                var bitmap = new Bitmap(openFileDialog1.FileName);
                await Task.Run(() => { RunProcessing(bitmap); });
                menuStrip1.Enabled = trackBar1.Enabled = true;
                time.Stop();
                Text = time.Elapsed.ToString();
                                            
            }
        }
        private void RunProcessing(Bitmap Image)
        {
            var pixels = GetPixels(Image);
            var pixelsInStep = (Image.Width * Image.Height)/100;
            var CurrentPixelsSet = new List<Pixel>(pixels.Count - pixelsInStep);
            for(int i =1; i < trackBar1.Maximum; i++)
            {
                for (int j = 0; j < pixelsInStep; j++)
                {
                    var index = _random.Next(pixels.Count);
                    CurrentPixelsSet.Add(pixels[index]);
                    pixels.RemoveAt(index);
                    
                }
                var CurrentImage = new Bitmap(Image.Width, Image.Height);
                foreach (var pixel in CurrentPixelsSet)
                    CurrentImage.SetPixel(pixel.Point.X, pixel.Point.Y, pixel.Color);
                _bitmaps.Add(CurrentImage);
                this.Invoke(new Action(() => { Text = $"{i} %"; }));
               
            }
            _bitmaps.Add(Image);       
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            if (_bitmaps == null || _bitmaps.Count == 0)
                return;
            
            pictureBox1.Image = _bitmaps[trackBar1.Value - 1];

        }
    }
}
