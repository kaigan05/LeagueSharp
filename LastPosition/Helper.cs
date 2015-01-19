using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using LeagueSharp;
using SharpDX;
using SharpDX.Direct3D9;
using Color = SharpDX.Color;
using Font = SharpDX.Direct3D9.Font;
using Rectangle = SharpDX.Rectangle;

namespace LastPosition
{
    internal class Helper
    {
        public static Bitmap CropCircleImage(Bitmap image)
        {
            var cropRect = new System.Drawing.Rectangle(0, 0, image.Width, image.Height);
            using (Bitmap cropImage = image.Clone(cropRect, image.PixelFormat))
            {
                using (var tb = new TextureBrush(cropImage))
                {
                    var target = new Bitmap(cropRect.Width, cropRect.Height);
                    using (Graphics g = Graphics.FromImage(target))
                    {
                        g.FillEllipse(tb, new System.Drawing.Rectangle(0, 0, cropRect.Width, cropRect.Height));
                        var p = new Pen(System.Drawing.Color.Red, 8) { Alignment = PenAlignment.Inset };
                        g.DrawEllipse(p, 0, 0, cropRect.Width, cropRect.Width);
                        return target;
                    }
                }
            }
        }

        /// <summary>
        ///     http://www.codeproject.com/Tips/201129/Change-Opacity-of-Image-in-C
        /// </summary>
        /// <returns></returns>
        public static Bitmap ChangeOpacity(Bitmap image, float opacity)
        {
            var bmp = new Bitmap(image.Width, image.Height);
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                var matrix = new ColorMatrix { Matrix33 = opacity };
                var attributes = new ImageAttributes();
                attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
                gfx.DrawImage(
                    image, new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel, attributes);
            }
            return bmp;
        }

        public static string FormatTime(double time)
        {
            TimeSpan t = TimeSpan.FromSeconds(time);
            if (t.Minutes > 0)
            {
                return string.Format("{0:D1}:{1:D2}", t.Minutes, t.Seconds);
            }
            return string.Format("{0:D}", t.Seconds);
        }

    }
}