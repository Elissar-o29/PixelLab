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
            originall.Image = originalimage;
            nnew.Image = originalimage;
        }

        private void Color_Quantization_Load(object sender, EventArgs e)
        {

        }
        // number of colors scroll
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            int colorCount = trackBar1.Value;
            label.Text = $"{colorCount} colors";

            Bitmap quantized = ColorQuantizate.QuantizeColors(originalimage, colorCount);
            
            nnew.Image = quantized;
        }
        // save the picture
        private void button1_Click(object sender, EventArgs e)
        {
            if (originalimage == null)
            {
                MessageBox.Show("Please open an image first");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                originalimage.Save(sfd.FileName);

                MessageBox.Show("Image Saved Successfully");
            }
        }

        private void newImage_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

    }
}
