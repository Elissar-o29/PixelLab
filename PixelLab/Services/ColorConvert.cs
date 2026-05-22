using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLab.Services
{
    public static class ColorConvert
    {
        private static int Clamp(int val) => Math.Max(0, Math.Min(255, val));

        // ------------------- RGB <==> HSV -------------------
        public static (float H, float S, float V) RGBtoHSV(int r, int g, int b)
        {
            float rf = r / 255f, gf = g / 255f, bf = b / 255f;
            float max = Math.Max(rf, Math.Max(gf, bf));
            float min = Math.Min(rf, Math.Min(gf, bf));
            float delta = max - min;

            float h = 0, s = 0, v = max;

            if (delta > 0)
            {
                s = delta / max;
                if (max == rf) h = 60 * (((gf - bf) / delta) % 6);
                else if (max == gf) h = 60 * (((bf - rf) / delta) + 2);
                else h = 60 * (((rf - gf) / delta) + 4);
                if (h < 0) h += 360;
            }
            return (h, s, v);
        }

        public static (int R, int G, int B) HSVtoRGB(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            float m = v - c;
            float rf, gf, bf;

            if (h < 60) { rf = c; gf = x; bf = 0; }
            else if (h < 120) { rf = x; gf = c; bf = 0; }
            else if (h < 180) { rf = 0; gf = c; bf = x; }
            else if (h < 240) { rf = 0; gf = x; bf = c; }
            else if (h < 300) { rf = x; gf = 0; bf = c; }
            else { rf = c; gf = 0; bf = x; }

            return (
     Clamp((int)((rf + m) * 255)),
     Clamp((int)((gf + m) * 255)),
     Clamp((int)((bf + m) * 255))
                   );
        }
        // ------------------- RGB <==> CMYK -------------------
        public static (float C, float M, float Y, float K) RGBtoCMYK(int r, int g, int b)
        {
            float rf = r / 255f, gf = g / 255f, bf = b / 255f;
            float k = 1 - Math.Max(rf, Math.Max(gf, bf));
            if (k == 1) return (0, 0, 0, 1);
            float c = (1 - rf - k) / (1 - k);
            float m = (1 - gf - k) / (1 - k);
            float y = (1 - bf - k) / (1 - k);
            return (c, m, y, k);
        }

        public static (int R, int G, int B) CMYKtoRGB(float c, float m, float y, float k)
        {
            int r = (int)(255 * (1 - c) * (1 - k));
            int g = (int)(255 * (1 - m) * (1 - k));
            int b = (int)(255 * (1 - y) * (1 - k));
            return (
    Clamp(r),
    Clamp(g),
    Clamp(b)
);
        }

        // ------------------- RGB <==> YUV -------------------
        public static (float Y, float U, float V) RGBtoYUV(int r, int g, int b)
        {
            float Y = 0.299f * r + 0.587f * g + 0.114f * b;
            float U = -0.147f * r - 0.289f * g + 0.436f * b;
            float V = 0.615f * r - 0.515f * g - 0.100f * b;
            return (Y, U, V);
        }

        public static (int R, int G, int B) YUVtoRGB(float y, float u, float v)
        {
            int r = (int)(y + 1.140f * v);
            int g = (int)(y - 0.395f * u - 0.581f * v);
            int b = (int)(y + 2.032f * u);
            return (Clamp(r), Clamp(g), Clamp(b));
        }

        // ------------------- RGB <==> YCbCr -------------------
        public static (float Y, float Cb, float Cr) RGBtoYCbCr(int r, int g, int b)
        {
            float Y = 16 + 0.257f * r + 0.504f * g + 0.098f * b;
            float Cb = 128 - 0.148f * r - 0.291f * g + 0.439f * b;
            float Cr = 128 + 0.439f * r - 0.368f * g - 0.071f * b;
            return (Y, Cb, Cr);
        }

        public static (int R, int G, int B) YCbCrtoRGB(float y, float cb, float cr)
        {
            float r = 1.164f * (y - 16) + 1.596f * (cr - 128);
            float g = 1.164f * (y - 16) - 0.813f * (cr - 128) - 0.392f * (cb - 128);
            float b = 1.164f * (y - 16) + 2.017f * (cb - 128);
            return (Clamp((int)r), Clamp((int)g), Clamp((int)b));
        }

        // ------------------- RGB <==> LAB
        // أولاً RGB → XYZ → LAB
        public static (float L, float A, float B) RGBtoLAB(int r, int g, int b)
        {
            // RGB → XYZ
            float rf = r / 255f, gf = g / 255f, bf = b / 255f;
            rf = rf > 0.04045f ? (float)Math.Pow((rf + 0.055f) / 1.055f, 2.4f) : rf / 12.92f;
            gf = gf > 0.04045f ? (float)Math.Pow((gf + 0.055f) / 1.055f, 2.4f) : gf / 12.92f;
            bf = bf > 0.04045f ? (float)Math.Pow((bf + 0.055f) / 1.055f, 2.4f) : bf / 12.92f;

            float x = (rf * 0.4124f + gf * 0.3576f + bf * 0.1805f) / 0.95047f;
            float y = (rf * 0.2126f + gf * 0.7152f + bf * 0.0722f) / 1.00000f;
            float z = (rf * 0.0193f + gf * 0.1192f + bf * 0.9505f) / 1.08883f;

            // XYZ → LAB
            x = x > 0.008856f ? (float)Math.Pow(x, 1 / 3f) : 7.787f * x + 16 / 116f;
            y = y > 0.008856f ? (float)Math.Pow(y, 1 / 3f) : 7.787f * y + 16 / 116f;
            z = z > 0.008856f ? (float)Math.Pow(z, 1 / 3f) : 7.787f * z + 16 / 116f;

            float L = 116 * y - 16;
            float A = 500 * (x - y);
            float B2 = 200 * (y - z);
            return (L, A, B2);
        }
    }
}
