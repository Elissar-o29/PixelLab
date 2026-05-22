using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PixelLab.Services;

namespace PixelLab.Forms
{
    public partial class Color_Quantization : Form
    {
        Bitmap originalimage;
        public Color_Quantization(Bitmap image)
        {
            InitializeComponent();
            originalimage = image;
            originalImage.Image = originalimage;
            pictureBox1.Image = originalimage;
        }

        private void Color_Quantization_Load(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int colorCount = trackBar1.Value;
            label.Text = $"{colorCount} colors";

            Bitmap quantized = ColorQuantizate.QuantizeColors(originalimage, colorCount);
            
            pictureBox1.Image = quantized;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }
}
