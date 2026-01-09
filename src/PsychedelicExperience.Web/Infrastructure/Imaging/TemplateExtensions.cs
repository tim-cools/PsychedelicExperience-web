using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace PsychedelicExperience.Web.Infrastructure
{
    public static class TemplateExtensions
    {
        private static readonly Lazy<Font> Font = new Lazy<Font>(GetFont);

        public static MemoryStream AddTemplateText(this byte[] templateImageData, string text)
        {
            using var stream = new MemoryStream(templateImageData);
            using var bitmap = Bitmap.FromStream(stream);
            using var graphics = Graphics.FromImage(bitmap);

            Brush brush = new SolidBrush(Color.FromArgb(235, 255, 255, 255));

            var font = Font.Value;

            const int marginHorizontal = 60;
            const int marginTop = 400;
            const int width = 1500 - 2 * marginHorizontal;

            var area = new Rectangle(marginHorizontal, marginTop, width , 500);

            graphics.DrawString(text, font, brush, width, 120, area, StringFormat.GenericDefault);

            var output = new MemoryStream();
            bitmap.Save(output, System.Drawing.Imaging.ImageFormat.Png);
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        private static Font GetFont()
        {
            return GetFont("Verdana");
        }

        private static Font GetFont(string name)
        {
            var font = new Font(name, 102, FontStyle.Bold, GraphicsUnit.Pixel);
            return 0 == string.Compare(name, font.Name, StringComparison.InvariantCultureIgnoreCase) ? font : null;
        }

        private static IEnumerable<string> GetWrappedLines(this Graphics that, string text, Font font, double maxWidth = double.PositiveInfinity)
        {
            if (string.IsNullOrEmpty(text)) return new string[0];
            if (font == null) throw new ArgumentNullException(nameof(font), "The 'font' parameter must not be null");
            if (maxWidth <= 0) throw new ArgumentOutOfRangeException(nameof(maxWidth), "Maximum width must be greater than zero");

            // See https://stackoverflow.com/questions/6111298/best-way-to-specify-whitespace-in-a-string-split-operation
            var words = text.Split((char[]) null, StringSplitOptions.RemoveEmptyEntries);

            if (words.Length == 0) return new string[0];

            var lines = new List<string>();

            var spaceCharWidth = that.MeasureString(" ", font).Width;
            float currentWidth = 0;
            var currentLine = string.Empty;
            for (var i = 0; i < words.Length; i++)
            {
                var currentWordWidth = that.MeasureString(words[i], font).Width;
                if (currentWidth != 0)
                {
                    var potentialWordWidth = spaceCharWidth + currentWordWidth;
                    if (currentWidth + potentialWordWidth < maxWidth)
                    {
                        currentWidth += potentialWordWidth;
                        currentLine += " " + words[i];
                    }
                    else
                    {
                        lines.Add(currentLine);
                        currentLine = words[i];
                        currentWidth = currentWordWidth;
                    }
                }
                else
                {
                    currentWidth += currentWordWidth;
                    currentLine = words[i];
                }

                if (i == words.Length - 1)
                {
                    lines.Add(currentLine);
                }
            }

            return lines;
        }

        private static void DrawString(this Graphics that, string text, Font font, Brush brush, int maxWidth,
                                            int lineHeight, RectangleF layoutRectangle, StringFormat format)
        {
            var lines = that.GetWrappedLines(text, font, maxWidth).ToArray();
            var lastDrawn = new Rectangle(Convert.ToInt32(layoutRectangle.X), Convert.ToInt32(layoutRectangle.Y), 0, 0);

            foreach (var line in lines)
            {
                var lineSize = that.MeasureString(line, font);
                var increment = lastDrawn.Height == 0 ? 0 : lineHeight;
                var lineOrigin = new Point(lastDrawn.X, lastDrawn.Y + increment);
                that.DrawString(line, font, brush, lineOrigin);
                lastDrawn = new Rectangle(lineOrigin, Size.Round(lineSize));
            }
        }
    }
}