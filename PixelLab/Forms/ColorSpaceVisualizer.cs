using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PixelLab.Services;
namespace PixelLab.Forms
{
    public partial class ColorSpaceVisualizer : Form
    {
        private bool loaded = false;
        private float rotX = 30f, rotY = 45f;
        private System.Drawing.Point lastMouse;
        private bool isDragging = false;

        public ColorSpaceVisualizer()
        {
            InitializeComponent();
        }

        private void ColorSpaceVisualizer_Load(object sender, EventArgs e)
        {
            glControl1.MakeCurrent();
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1f);
            GL.Enable(EnableCap.DepthTest);
            loaded = true;

            comboBox1.Items.AddRange(new string[] { "RGB", "HSV", "YUV", "LAB" });
            comboBox1.SelectedIndex = 0;
            glControl1.MouseWheel += glControl1_MouseWheel;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            glControl1.Invalidate();  
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!loaded) return;

            glControl1.MakeCurrent();
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            float aspect = (float)glControl1.Width / glControl1.Height;
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45f), aspect, 0.1f, 100f);
            GL.LoadMatrix(ref perspective);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(0f, 0f, -3f);
            GL.Rotate(rotX, 1f, 0f, 0f);
            GL.Rotate(rotY, 0f, 1f, 0f);
            GL.Translate(0f, 0f, -zoomLevel); 

            // ← حسب الاختيار
            switch (comboBox1.SelectedItem?.ToString())
            {
                case "HSV": DrawHSVCylinder(); break;
                case "YUV": DrawYUVSpace(); break;
                case "LAB": DrawLABSpace(); break;
                default: DrawRGBCube(); break;
            }

            DrawAxes();
            glControl1.SwapBuffers();
        }

        private void DrawRGBCube()
        {
            GL.Begin(PrimitiveType.Points);
            GL.PointSize(2f);

            int steps = 20; // عدد النقاط
            for (int r = 0; r <= steps; r++)
            {
                for (int g = 0; g <= steps; g++)
                {
                    for (int b = 0; b <= steps; b++)
                    {
                        float rf = r / (float)steps;
                        float gf = g / (float)steps;
                        float bf = b / (float)steps;

                        // بس ارسم النقاط على سطح المكعب
                        if (r == 0 || r == steps ||
                            g == 0 || g == steps ||
                            b == 0 || b == steps)
                        {
                            GL.Color3(rf, gf, bf);
                            GL.Vertex3(rf, gf, bf);
                        }
                    }
                }
            }
            GL.End();
        }

        private void DrawAxes()
        {
            string space = comboBox1.SelectedItem?.ToString() ?? "RGB";
            GL.LineWidth(2f);
            GL.Begin(PrimitiveType.Lines);

            switch (space)
            {
                case "HSV":
                    GL.Color3(1f, 1f, 0f); GL.Vertex3(0, 0, 0); GL.Vertex3(1.2f, 0, 0); // H
                    GL.Color3(0f, 1f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 1.2f, 0); // S
                    GL.Color3(1f, 0f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 1.2f); // V
                    break;
                case "YUV":
                    GL.Color3(1f, 1f, 0f); GL.Vertex3(0, 0, 0); GL.Vertex3(1.2f, 0, 0); // Y
                    GL.Color3(0f, 1f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 1.2f, 0); // U
                    GL.Color3(1f, 0f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 1.2f); // V
                    break;
                case "LAB":
                    GL.Color3(1f, 1f, 0f); GL.Vertex3(0, 0, 0); GL.Vertex3(1.2f, 0, 0); // L
                    GL.Color3(0f, 1f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 1.2f, 0); // A
                    GL.Color3(1f, 0f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 1.2f); // B
                    break;
                default: // RGB
                    GL.Color3(1f, 0f, 0f); GL.Vertex3(0, 0, 0); GL.Vertex3(1.2f, 0, 0); // R
                    GL.Color3(0f, 1f, 0f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 1.2f, 0); // G
                    GL.Color3(0f, 0f, 1f); GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 1.2f); // B
                    break;
            }
            GL.End();
        }
        private void DrawHSVCylinder()
        {
            GL.Begin(PrimitiveType.Points);
            int steps = 30;

            for (int h = 0; h < steps; h++)
            {
                for (int s = 0; s <= steps; s++)
                {
                    for (int v = 0; v <= steps; v++)
                    {
                        float hf = h / (float)steps * 360f;
                        float sf = s / (float)steps;
                        float vf = v / (float)steps;

                        // بس السطح
                        if (s == steps || v == 0 || v == steps || s == 0)
                        {
                            // حول لـ RGB للون
                            var (r, g, b) = ColorConvert.HSVtoRGB(hf, sf, vf);
                            GL.Color3(r / 255f, g / 255f, b / 255f);

                            // حول لإحداثيات أسطوانة
                            float angle = MathHelper.DegreesToRadians(hf);
                            float x = 0.5f + sf * 0.5f * (float)Math.Cos(angle);
                            float z = 0.5f + sf * 0.5f * (float)Math.Sin(angle);
                            float y = vf;

                            GL.Vertex3(x, y, z);
                        }
                    }
                }
            }
            GL.End();
        }

        private void DrawYUVSpace()
        {
            GL.Begin(PrimitiveType.Points);
            int steps = 15;

            for (int r = 0; r <= steps; r++)
            {
                for (int g = 0; g <= steps; g++)
                {
                    for (int b = 0; b <= steps; b++)
                    {
                        if (r == 0 || r == steps ||
                            g == 0 || g == steps ||
                            b == 0 || b == steps)
                        {
                            float rf = r / (float)steps * 255f;
                            float gf = g / (float)steps * 255f;
                            float bf = b / (float)steps * 255f;

                            var (Y, U, V) = ColorConvert.RGBtoYUV((int)rf, (int)gf, (int)bf);

                            GL.Color3(rf / 255f, gf / 255f, bf / 255f);

                            // نرمّل القيم
                            float x = (Y / 255f);
                            float y = (U + 0.5f);
                            float z = (V + 0.5f);

                            GL.Vertex3(
                                Math.Max(0, Math.Min(1, x)),
                                Math.Max(0, Math.Min(1, y)),
                                Math.Max(0, Math.Min(1, z)));
                        }
                    }
                }
            }
            GL.End();
        }

        private void DrawLABSpace()
        {
            GL.Begin(PrimitiveType.Points);
            int steps = 15;

            for (int r = 0; r <= steps; r++)
            {
                for (int g = 0; g <= steps; g++)
                {
                    for (int b = 0; b <= steps; b++)
                    {
                        if (r == 0 || r == steps ||
                            g == 0 || g == steps ||
                            b == 0 || b == steps)
                        {
                            float rf = r / (float)steps * 255f;
                            float gf = g / (float)steps * 255f;
                            float bf = b / (float)steps * 255f;

                            var (L, A, B2) = ColorConvert.RGBtoLAB((int)rf, (int)gf, (int)bf);

                            GL.Color3(rf / 255f, gf / 255f, bf / 255f);

                            // نرمّل القيم
                            float x = L / 100f;
                            float y = (A + 128f) / 256f;
                            float z = (B2 + 128f) / 256f;

                            GL.Vertex3(
                                Math.Max(0, Math.Min(1, x)),
                                Math.Max(0, Math.Min(1, y)),
                                Math.Max(0, Math.Min(1, z)));
                        }
                    }
                }
            }
            GL.End();
        }

        // تدوير بالماوس
        private void glControl1_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastMouse = e.Location;
        }

        private void glControl1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging) return;

            rotY += (e.X - lastMouse.X) * 0.5f;
            rotX += (e.Y - lastMouse.Y) * 0.5f;
            lastMouse = e.Location;

            glControl1.Invalidate();
        }

        private float zoomLevel = 3f; 

        private void glControl1_MouseWheel(object sender, MouseEventArgs e)
        {
            zoomLevel -= e.Delta / 120f * 0.2f; // كل scroll = 0.2 وحدة
            zoomLevel = Math.Max(1f, Math.Min(10f, zoomLevel)); // بين 1 و 10
            glControl1.Invalidate();
        }

        private void glControl1_Load(object sender, EventArgs e)
        {

        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!loaded) return;
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height);
            glControl1.Invalidate();
        }
    }
}