using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLab.Services
{
    public class ColorQuantizate
    {
        public static Bitmap QuantizeColors(Bitmap source, int colorCount)
        {
            Bitmap result = new Bitmap(source.Width, source.Height);
            int step = (int)Math.Ceiling(256.0 / Math.Pow(colorCount, 1.0 / 3));

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
                output[i] = (byte)Math.Min(255, (int)(Math.Round((double)buffer[i] / step) * step));
                output[i + 1] = (byte)Math.Min(255, (int)(Math.Round((double)buffer[i + 1] / step) * step));
                output[i + 2] = (byte)Math.Min(255, (int)(Math.Round((double)buffer[i + 2] / step) * step));
            }

            System.Runtime.InteropServices.Marshal.Copy(output, 0, dstData.Scan0, bytes);

            source.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }
    }
}
