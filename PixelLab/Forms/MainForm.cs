using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using PixelLab.Forms;
using PixelLab.Helpers;
using PixelLab.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PixelLab
{
    public partial class MainForm : Form
    {
        Bitmap currentImage;
        Bitmap originalImage;
        string previewMode = "All";
        private string currentColorSpace = "RGB";
        private float zoomLevel = 1.0f;

        private float Clamp(float val)
        {
            return Math.Max(0f, Math.Min(1f, val));
        }
        public MainForm()
        {
            InitializeComponent();
        }
        private void MainForm_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = "RGB";
            currentColorSpace = "RGB";
            UpdateChannelLabels();

        }
        private void MainForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void MainForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            if (files.Length > 0)
            {
                originalImage = new Bitmap(files[0]);

                currentImage = new Bitmap(originalImage);

                pictureBox1.Image = currentImage;
            }
        }
        //apply RGB
        private void UpdateImage()
        {
            currentImage = ImageProcessor.ApplyRGB(
                originalImage,
                trackBarC1.Value,
                trackBarC2.Value,
                 trackBarC3.Value,
                trackBarBrightness.Value,
                trackBarSaturation.Value / 100f,
                disableRed.Checked,
                disableGreen.Checked,
                disableBlue.Checked,
                previewMode
            );
            UpdateColorInfo(currentImage);
            pictureBox1.Image = currentImage;
        }
                
        //open Button 
        private void OpenButton_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
               
                zoomLevel = 1.0f;
                ApplyZoom();
                ResetSliders();
                using (Bitmap temp = new Bitmap(ofd.FileName))
                {
                    originalImage = new Bitmap(temp);
                    currentImage = new Bitmap(temp); 
                }
                pictureBox1.Image = currentImage;
                pictureBox2.Image = currentImage;
                UpdatePictureInfo(ofd.FileName);
            }
        }

        // save button
        private void SaveButton_Click_1(object sender, EventArgs e)
        {
            if (currentImage == null)
            {
                MessageBox.Show("Please open an image first");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                currentImage.Save(sfd.FileName);

                MessageBox.Show("Image Saved Successfully");
            }
        }
        // reset button
        private void resetBtn_Click(object sender, EventArgs e)
        {
            currentImage = new Bitmap(originalImage);
            ResetSliders();
            zoomLevel = 1.0f;
            ApplyZoom();
            pictureBox1.Size = new Size(originalImage.Width, originalImage.Height);
            pictureBox1.Image = currentImage;
        }

        private void tbRGB_Scroll(object sender, EventArgs e)
        {
            CVal1.Text = trackBarC1.Value.ToString();
            CVal2.Text = trackBarC2.Value.ToString();
            CVal3.Text = trackBarC3.Value.ToString();
            CVal4.Text = trackBarC4.Value.ToString();
            brightVal.Text= trackBarBrightness.Value.ToString();
            saturationVal.Text = trackBarSaturation.Value.ToString();

            if (currentColorSpace == "RGB")
                UpdateImage();
            else
                ApplyColorSpace();
        }

        //channel options
        private void allBtn_Click(object sender, EventArgs e)
        {
            previewMode = "All";
            UpdateImage();
        }

        private void redBtn_Click(object sender, EventArgs e)
        {
            previewMode = "Red";
            UpdateImage();
        }

        private void greenBtn_Click(object sender, EventArgs e)
        {
            previewMode = "Green";
            UpdateImage();
        }

        private void blueBtn_Click(object sender, EventArgs e)
        {
            previewMode = "Blue";
            UpdateImage();
        }

        // color system (RGB , CMYK .... )
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentColorSpace = comboBox1.SelectedItem.ToString();
            UpdateChannelLabels();
            ResetSliders();
            if (currentColorSpace == "RGB")
                UpdateImage();
            else
                ApplyColorSpace();
        }
        private void UpdateChannelLabels()
        {
            switch (currentColorSpace)
            {
                case "RGB":
                    label1.Text = "R"; label2.Text = "G"; label3.Text = "B";
                    panelC1.BackColor = Color.Red; panelC2.BackColor = Color.Green; panelC3.BackColor = Color.Blue;
                    trackBarC4.Visible = false; label4.Visible = false; CVal4.Visible = false; panelC4.Visible = false; // ما في channel 4
                    break;
                case "HSV":
                    label1.Text = "H (Hue)";
                    label2.Text = "S (Saturation)";
                    label3.Text = "V (Value)";
                    panelC1.Visible = false; panelC2.Visible = false; panelC3.Visible = false;
                    trackBarC4.Visible = false; label4.Visible = false; CVal4.Visible = false; panelC4.Visible = false;
                    break;
                case "CMYK":
                    label1.Text = "C (Cyan)"; 
                    label2.Text = "M (Magenta)";
                    label3.Text = "Y (Yellow)";
                    label4.Text = "K (Black)";
                    panelC1.BackColor = Color.Cyan;
                    panelC2.BackColor = Color.Magenta;
                    panelC3.BackColor = Color.Yellow;
                    panelC4.BackColor = Color.Black;
                    panelC1.Visible = true; panelC2.Visible = true; panelC3.Visible = true;
                    trackBarC4.Visible = true; // CMYK عندها 4
                    CVal4.Visible=true;
                    panelC4.Visible=true;
                    label4.Visible=true;
                    break;
                case "YUV":
                    label1.Text = "Y"; label2.Text = "U"; label3.Text = "V";
                    panelC1.Visible = false; panelC2.Visible = false; panelC3.Visible = false;
                    trackBarC4.Visible = false; label4.Visible = false; CVal4.Visible = false; panelC4.Visible = false;
                    break;
                case "YCbCr":
                    label1.Text = "Y"; label2.Text = "Cb"; label3.Text = "Cr";
                    panelC1.Visible = false; panelC2.Visible = true; panelC3.Visible = true;
                    panelC2.BackColor = Color.Blue; panelC3.BackColor = Color.Red;
                    trackBarC4.Visible = false; label4.Visible = false; CVal4.Visible = false; panelC4.Visible = false;
                    break;
                case "LAB":
                    label1.Text = "L"; label2.Text = "A"; label3.Text = "B";
                    panelC1.Visible = false; panelC2.Visible = false; panelC3.Visible = false;
                    trackBarC4.Visible = false; label4.Visible = false; CVal4.Visible = false; panelC4.Visible = false;
                    break;


            }
        }
        private void ApplyColorSpace()
        {
            if (originalImage == null) return;

            Bitmap source = new Bitmap(originalImage);
            Bitmap result = new Bitmap(source.Width, source.Height);

            int v1 = trackBarC1.Value;
            int v2 = trackBarC2.Value;
            int v3 = trackBarC3.Value;
            int v4 = trackBarC4.Value;
            int brightness = trackBarBrightness.Value;       
            float saturation = trackBarSaturation.Value / 100f;

            BitmapData srcData = source.LockBits(
                new Rectangle(0, 0, source.Width, source.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

            BitmapData dstData = result.LockBits(
                new Rectangle(0, 0, result.Width, result.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

            int bytes = Math.Abs(srcData.Stride) * source.Height;
            byte[] buffer = new byte[bytes];
            byte[] output = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, buffer, 0, bytes);

            for (int i = 0; i < buffer.Length; i += 3)
            {
                int bOrig = buffer[i];
                int gOrig = buffer[i + 1];
                int rOrig = buffer[i + 2];
                int r = rOrig, g = gOrig, b = bOrig;

                switch (currentColorSpace)
                {
                    case "HSV":
                        var (H, S, V) = ColorConvert.RGBtoHSV(rOrig, gOrig, bOrig);
                        H = ((H + v1) % 360 + 360) % 360;
                        S = Clamp(S + v2 / 100f);
                        V = Clamp(V + v3 / 100f);
                        (r, g, b) = ColorConvert.HSVtoRGB(H, S, V);
                        break;

                    case "CMYK":
                        var (C, M, Y, K) = ColorConvert.RGBtoCMYK(rOrig, gOrig, bOrig);
                        C = Clamp(C + v1 / 100f);
                        M = Clamp(M + v2 / 100f);
                        Y = Clamp(Y + v3 / 100f);
                        K = Clamp(K + v4 / 100f);
                        (r, g, b) = ColorConvert.CMYKtoRGB(C, M, Y, K);
                        break;

                    case "YUV":
                        var (Yv, U, Vv) = ColorConvert.RGBtoYUV(rOrig, gOrig, bOrig);
                        Yv += v1; U += v2; Vv += v3;
                        (r, g, b) = ColorConvert.YUVtoRGB(Yv, U, Vv);
                        break;
                    case "YCbCr":

                        var (Yc, Cb, Cr) =
                            ColorConvert.RGBtoYCbCr(rOrig, gOrig, bOrig);

                        Yc += v1;
                        Cb += v2;
                        Cr += v3;

                        (r, g, b) =
                            ColorConvert.YCbCrtoRGB(Yc, Cb, Cr);

                        break;
                }
                r += brightness;
                g += brightness;
                b += brightness;

                float gray = (r + g + b) / 3f;
                r = (int)(gray + (r - gray) * (1 + saturation));
                g = (int)(gray + (g - gray) * (1 + saturation));
                b = (int)(gray + (b - gray) * (1 + saturation));

                if (disableRed.Checked) r = 0;
                if (disableGreen.Checked) g = 0;
                if (disableBlue.Checked) b = 0;

                switch (previewMode)
                {
                    case "Red": g = 0; b = 0; break;
                    case "Green": r = 0; b = 0; break;
                    case "Blue": r = 0; g = 0; break;
                }

                output[i] = (byte)Math.Max(0, Math.Min(255, b));
                output[i + 1] = (byte)Math.Max(0, Math.Min(255, g));
                output[i + 2] = (byte)Math.Max(0, Math.Min(255, r));
            }

            System.Runtime.InteropServices.Marshal.Copy(output, 0, dstData.Scan0, bytes);

            source.UnlockBits(srcData);
            result.UnlockBits(dstData);
            source.Dispose();

            currentImage = result;
            pictureBox1.Image = currentImage;
           // UpdateColorInfo(currentImage);
        }
        // نسخة بتقبل Bitmap - للاستدعاء الداخلي
        private void UpdateColorInfo(Bitmap img)
        {
            Color c = img.GetPixel(img.Width / 2, img.Height / 2);
            UpdateColorInfo(c); // بتستدعي النسخة الثانية
        }

        // نسخة بتقبل Color - للماوس
        private void UpdateColorInfo(Color c)
        {
            var (H, S, V) = ColorConvert.RGBtoHSV(c.R, c.G, c.B);
            var (Cm, M, Y, K) = ColorConvert.RGBtoCMYK(c.R, c.G, c.B);
            var (Yuv, U, Vv) = ColorConvert.RGBtoYUV(c.R, c.G, c.B);
            var (L, A, B) = ColorConvert.RGBtoLAB(c.R, c.G, c.B);

            fileRGB.Text = $"({c.R}, {c.G}, {c.B})";
            fileHSV.Text = $"({H:F0}°, {S * 100:F0}%, {V * 100:F0}%)";
            fileCMYK.Text = $"({Cm * 100:F0}%, {M * 100:F0}%, {Y * 100:F0}%, {K * 100:F0}%)";
            fileYUV.Text = $"({Yuv:F0}, {U:F0}, {Vv:F0})";
            fileLAB.Text = $"({L:F0}, {A:F0}, {B:F0})";

            currentcolorPanel.BackColor = c;
        }
        private void ResetSliders()
        {
            trackBarC1.Value = 0;
            trackBarC2.Value = 0;
            trackBarC3.Value = 0;
            trackBarC4.Value = 0;
            trackBarBrightness.Value = 0;
            trackBarSaturation.Value = 0;
            zoomtrackBar.Value = 10;

            CVal1.Text = "0";
            CVal2.Text = "0";
            CVal3.Text = "0";
            CVal4.Text = "0";
            brightVal.Text = "0";
            saturationVal.Text = "0";
            persentLabel.Text = "10%";
            zoomLevel = 1.0f;
        }
       
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (currentImage == null) return;

            float scaleX = (float)currentImage.Width / pictureBox1.Width;
            float scaleY = (float)currentImage.Height / pictureBox1.Height;

            int imgX = Math.Max(0, Math.Min(currentImage.Width - 1, (int)(e.X * scaleX)));
            int imgY = Math.Max(0, Math.Min(currentImage.Height - 1, (int)(e.Y * scaleY)));

            Color c = currentImage.GetPixel(imgX, imgY);
            UpdateColorInfo(c);
        }

        private void zoomBBut_Click(object sender, EventArgs e)
        {
            zoomLevel = ZoomHelper.ClampZoom(zoomLevel + 0.1f);
            ApplyZoom();
        }
        private void ApplyZoom()
        {
            if (currentImage == null) return;
            int newWidth = (int)(originalImage.Width * zoomLevel);
            int newHeight = (int)(originalImage.Height * zoomLevel);

            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.Size = new Size(newWidth, newHeight);
            pictureBox1.Image = currentImage; // ← مش Bitmap جديد
            pictureBox1.Location = new Point(0, 0);



            persentLabel.Text = $"{(int)(zoomLevel * 100)}%";
            zoomtrackBar.Value = (int)(zoomLevel * 100);
            UpdateThumbnail();
        }
        private void UpdateThumbnail()
        {
            if (currentImage == null) return;
            pictureBox2.Image = new Bitmap(currentImage,
                pictureBox2.Width,
                pictureBox2.Height);
        }

        private void zoomMBut_Click(object sender, EventArgs e)
        {
            zoomLevel = ZoomHelper.ClampZoom(zoomLevel - 0.1f);
            ApplyZoom();
        }

        private void zoomtrackBar_Scroll(object sender, EventArgs e)
        {
            zoomLevel = zoomtrackBar.Value / 100f;
            ApplyZoom();
        }
        private void UpdatePictureInfo(string filePath)
        {
            FileInfo fi = new FileInfo(filePath);

            string name = fi.Name;
            filename.Text = name.Length > 16 ? name.Substring(0, 16) + "..." : name;
            fileformat.Text = fi.Extension.TrimStart('.').ToUpper();
            filesaze.Text = $"{fi.Length / 1024} KB";
            fileresolution.Text = $"{originalImage.HorizontalResolution} x {originalImage.VerticalResolution} DPI";
            filewidth.Text = $"{originalImage.Width} px";
            fileheight.Text = $"{originalImage.Height} px";
        }

        private void ColorQuant_Click(object sender, EventArgs e)
        {
            if (originalImage == null)
            {
                MessageBox.Show("Please open an image first");
                return;
            }

            Color_Quantization qf = new Color_Quantization(originalImage);
            qf.Show();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ColorSpaceVisualizer csv = new ColorSpaceVisualizer();
            csv.Show();
        }
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void rightPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void colorChannels_Enter(object sender, EventArgs e)
        {

        }
        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }
        private void filewidth_Click(object sender, EventArgs e)
        {

        }

        private void fresolution_Click(object sender, EventArgs e)
        {

        }

        private void filename_Click(object sender, EventArgs e)
        {

        }
        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label4_Click_2(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }
        private void groupBox3_Enter_1(object sender, EventArgs e)
        {

        }
        private void persentLabel_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }
        private void imagepanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void redVal_Click(object sender, EventArgs e)
        {

        }

        private void greenVal_Click(object sender, EventArgs e)
        {

        }

        private void blueVal_Click(object sender, EventArgs e)
        {

        }

        private void brightVal_Click(object sender, EventArgs e)
        {

        }

        private void saturationVal_Click(object sender, EventArgs e)
        {

        }
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }


    }
}

