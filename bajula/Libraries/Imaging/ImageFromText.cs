using System.Drawing;

namespace tradelr.Libraries.Imaging
{
    public class ImageFromText
    {
        #region Private Variables
        private string _FontFace = "Arial";
        private int _FontSize = 12;
        private Color _FontColor = Color.Black;
        private Color _BackgroundColor = Color.White;
        #endregion
        #region Public Properties
        public Color BackGroundColor
        {
            get { return _BackgroundColor; }
            set { _BackgroundColor = value; }
        }

        public Color FontColor
        {
            get { return _FontColor; }
            set { _FontColor = value; }
        }
        public int FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; }
        }
        public string FontFace
        {
            get { return _FontFace; }
            set { _FontFace = value; }
        }
        #endregion
        #region Constructor
        public ImageFromText() { }
        #endregion
        public Image MakeImage(string Text, SizeF Sf)
        {
            // Create a Bitmap instance that's 468x60, and a Graphics instance
            int width = (int)(Sf.Width + 20);
            int height = (int)(Sf.Height + 20);

            Image imageObj = new Bitmap(width, height);
            Graphics graphicsObj = Graphics.FromImage(imageObj);

            // Create a border in the color of the font
            graphicsObj.FillRectangle(new SolidBrush(_FontColor), 0, 0, width, height);
            // Create a LightBlue background
            graphicsObj.FillRectangle(new SolidBrush(_BackgroundColor), 2, 2, width - 4, height - 4);

            // Specify the font and alignment
            Font fontBanner = new Font(_FontFace, _FontSize, FontStyle.Bold);

            // center align the advertising pitch
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;

            // Draw the adverising pitch
            graphicsObj.DrawString(Text, fontBanner, new SolidBrush(FontColor), new Rectangle(0, 0, width, height), stringFormat);

            return imageObj;
        }

        public SizeF CheckWidth(string mystring)
        {
            Font menuFont = new Font(FontFace, FontSize, FontStyle.Bold);
            SizeF CurrentWidth = new SizeF(0, 0);
            Bitmap b = new Bitmap(12, 12);
            Graphics g = Graphics.FromImage(b);
            CurrentWidth = g.MeasureString(mystring, menuFont);
            g.Dispose();
            b.Dispose();
            return CurrentWidth;
        }
    }
}
