using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelLab.Helpers
{
    public static class ZoomHelper
    {
        public static Size CalcZoomSize(int originalWidth, int originalHeight, float zoomLevel)
        {
            return new Size(
                (int)(originalWidth * zoomLevel),
                (int)(originalHeight * zoomLevel)
            );
        }

        public static float ClampZoom(float zoom, float min = 0.1f, float max = 5.0f)
        {
            return Math.Max(min, Math.Min(max, zoom));
        }
    }
}
