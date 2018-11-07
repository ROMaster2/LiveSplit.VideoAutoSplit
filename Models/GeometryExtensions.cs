using System;

namespace LiveSplit.VFM.Models
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

        // Todo: Handle fractional pixels.

        public ImageMagick.MagickGeometry ToMagick(bool includePoint = true, int rounding = 0)
        {

            const double rounder = 0.4999999999; // Difficult to be precise without posible error.
            int x;
            int y;
            int width;
            int height;

            switch (rounding)
            {
                case 2: // Ceiling
                    x = includePoint ? (int)Math.Ceiling(X) : 0;
                    y = includePoint ? (int)Math.Ceiling(Y) : 0;
                    width = (int)Math.Ceiling(Width);
                    height = (int)Math.Ceiling(Height);
                    break;
                case 1: // Roundup
                    x = includePoint ? (int)Math.Round(X + (Math.Sign(X) > 0 ? rounder : -rounder)) : 0;
                    y = includePoint ? (int)Math.Round(Y + (Math.Sign(Y) > 0 ? rounder : -rounder)) : 0;
                    width = (int)Math.Round(Width + (Math.Sign(Width) > 0 ? rounder : -rounder));
                    height = (int)Math.Round(Height + (Math.Sign(Height) > 0 ? rounder : -rounder));
                    break;
                case 0: // Round
                default:
                    x = includePoint ? (int)Math.Round(X) : 0;
                    y = includePoint ? (int)Math.Round(Y) : 0;
                    width = (int)Math.Round(Width);
                    height = (int)Math.Round(Height);
                    break;
                case -1: // Rounddown
                    x = includePoint ? (int)Math.Round(X + (Math.Sign(X) < 0 ? rounder : -rounder)) : 0;
                    y = includePoint ? (int)Math.Round(Y + (Math.Sign(Y) < 0 ? rounder : -rounder)) : 0;
                    width = (int)Math.Round(Width + (Math.Sign(Width) < 0 ? rounder : -rounder));
                    height = (int)Math.Round(Height + (Math.Sign(Height) < 0 ? rounder : -rounder));
                    break;
                case -2: // Floor
                    x = includePoint ? (int)Math.Floor(X) : 0;
                    y = includePoint ? (int)Math.Floor(Y) : 0;
                    width = (int)Math.Floor(Width);
                    height = (int)Math.Floor(Height);
                    break;
            }

            return new ImageMagick.MagickGeometry(x, y, width, height);
        }

        #endregion Public Methods

    }
}
