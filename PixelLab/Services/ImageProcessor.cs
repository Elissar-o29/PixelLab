using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace PixelLab.Services
{
    public class ImageProcessor
    {
        public static Bitmap ApplyRGB(
            Bitmap originalImage,
            int rVal,
            int gVal,
            int bVal,
            int brightness,
            float saturation,
            bool disableRed,
            bool disableGreen,
            bool disableBlue,
            string previewMode)
        {
            Bitmap img = new Bitmap(originalImage.Width, originalImage.Height);

            BitmapData srcData = originalImage.LockBits(
                new Rectangle(0, 0, originalImage.Width, originalImage.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            BitmapData dstData = img.LockBits(
                new Rectangle(0, 0, img.Width, img.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);

            int bytes = Math.Abs(srcData.Stride) * originalImage.Height;
            byte[] buffer = new byte[bytes];

            System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, buffer, 0, bytes);

            for (int i = 0; i < buffer.Length; i += 3)
            {
                int b = buffer[i] + bVal + brightness;
                int g = buffer[i + 1] + gVal + brightness;
                int r = buffer[i + 2] + rVal + brightness;

                float gray = (r + g + b) / 3f;

                r = (int)(gray + (r - gray) * (1 + saturation));
                g = (int)(gray + (g - gray) * (1 + saturation));
                b = (int)(gray + (b - gray) * (1 + saturation));

                if (disableRed)
                    r = 0;

                if (disableGreen)
                    g = 0;

                if (disableBlue)
                    b = 0;

                switch (previewMode)
                {
                    case "Red":
                        g = 0;
                        b = 0;
                        break;

                    case "Green":
                        r = 0;
                        b = 0;
                        break;

                    case "Blue":
                        r = 0;
                        g = 0;
                        break;
                }

                buffer[i] = (byte)Math.Max(0, Math.Min(255, b));
                buffer[i + 1] = (byte)Math.Max(0, Math.Min(255, g));
                buffer[i + 2] = (byte)Math.Max(0, Math.Min(255, r));
            }

            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, dstData.Scan0, bytes);

            originalImage.UnlockBits(srcData);
            img.UnlockBits(dstData);

            return img;
        }
    }
}