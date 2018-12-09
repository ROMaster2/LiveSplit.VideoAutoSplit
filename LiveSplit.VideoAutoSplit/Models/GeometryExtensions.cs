namespace LiveSplit.VAS.Models
{
    public partial struct Geometry
    {
        #region Constructors

        // Additional constructors for conversion.
        /// <summary>
        /// Constructor for converting ImageMagick.MagickGeometry to this format.
        /// </summary>
        public Geometry(ImageMagick.MagickGeometry mGeo,
                    Anchor anchor = Anchor.Undefined)
            : this(mGeo.X, mGeo.Y, mGeo.Width, mGeo.Height, anchor) { }

        /// <summary>
        /// Constructor for converting ImageMagick.MagickGeometry and Gravity to this format.
        /// </summary>
        public Geometry(ImageMagick.MagickGeometry mGeo,
                    ImageMagick.Gravity gravity = ImageMagick.Gravity.Undefined)
            : this(mGeo.X, mGeo.Y, mGeo.Width, mGeo.Height, gravity.ToAnchor()) { }

        #endregion Constructors

        #region Public Methods

        public string ToFFmpegString()
        {
            return Width.ToString() + ':' + Height.ToString() + ':' + X.ToString() + ':' + Y.ToString();
        }

        public ImageMagick.MagickGeometry ToMagick(bool includePoint = true, int rounding = 0)
        {
            var geo = Round(includePoint, rounding);
            return new ImageMagick.MagickGeometry((int)geo.X, (int)geo.Y, (int)geo.Width, (int)geo.Height);
        }

        public System.Drawing.Rectangle ToRectangle(bool includePoint = true, int rounding = 0)
        {
            var geo = Round(includePoint, rounding);
            return new System.Drawing.Rectangle((int)geo.X, (int)geo.Y, (int)geo.Width, (int)geo.Height);
        }

        public System.Drawing.RectangleF ToRectangleF()
        {
            return new System.Drawing.RectangleF((float)X, (float)Y, (float)Width, (float)Height);
        }

        #endregion Public Methods

    }
}
