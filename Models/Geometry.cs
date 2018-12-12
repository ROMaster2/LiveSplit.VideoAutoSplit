/**
 * Since I couldn't find any shape class that included alignment/gravity/anchor logic, I
 * implemented my own. The below is derived from System.Windows.Rect.
 * 
 * This ultimately should be made very portable.
 */

using System;
using System.Windows;
using System.Xml.Serialization;

namespace LiveSplit.VAS.Models
{
    /// <summary>
    /// Geometry - The primitive which represents a rectangle.  Geometry are stored as
    /// X, Y (Location) and Width and Height (Size). Unlike System.Windows.Rect, child Geometry
    /// can have negative Width and Height to be relational to its parent's Width and Height.
    /// This also implements Alignment, setting the corner or side where the location starts
    /// from.
    /// </summary>
    public partial struct Geometry
    {
        #region Constructors

        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters.
        /// </summary>
        public Geometry(double x,
                    double y,
                    double width,
                    double height,
                    Anchor anchor = Anchor.Undefined)
        {
            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _anchor = anchor;
            ValidateAll();
        }

        /// <summary>
        /// Constructor which sets the initial values to the values of the parameters
        /// </summary>
        public Geometry(Point location, Size size, Anchor anchor = Anchor.Undefined)
            : this(location.X, location.Y, size.Width, size.Height, anchor) { }

        /// <summary>
        /// Constructor which sets the initial values to bound the two points provided.
        /// </summary>
        public Geometry(Point point1, Point point2, Anchor anchor = Anchor.Undefined)
        {
            _x = Math.Min(point1.X, point2.X);
            _y = Math.Min(point1.Y, point2.Y);

            //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
            _width = Math.Max(Math.Max(point1.X, point2.X) - _x, 0);
            _height = Math.Max(Math.Max(point1.Y, point2.Y) - _y, 0);

            _anchor = anchor;
            ValidateAll();
        }

        /// <summary>
        /// Constructor which sets the initial values to bound the point provided and the point
        /// which results from point + vector.
        /// </summary>
        public Geometry(Point point, Vector vector, Anchor anchor = Anchor.Undefined)
            : this(point, point + vector, anchor) { }

        /// <summary>
        /// Constructor which sets the initial values to bound the (0,0) point and the point 
        /// that results from (0,0) + Width and Height. 
        /// </summary>
        public Geometry(double width, double height, Anchor anchor = Anchor.Undefined)
            : this(0, 0, width, height, anchor) { }

        /// <summary>
        /// Constructor which sets the initial values to bound the (0,0) point and the point 
        /// that results from (0,0) + size. 
        /// </summary>
        public Geometry(System.Windows.Size size, Anchor anchor = Anchor.Undefined)
            : this(size.Width, size.Height, anchor) { }

        #endregion Constructors

        #region Statics

        /// <summary>
        /// Blank - a static property which provides an Blank geometry. True blanks aren't used
        /// to avoid headaches.
        /// </summary>
        public static readonly Geometry Blank = new Geometry(0, 0, Anchor.Undefined);

        #endregion Statics

        #region Public Properties

        /// <summary>
        /// HasPoint - this returns true if this geometry has set coordinates.
        /// </summary>
        public bool HasPoint
        {
            get
            {
                return (X != 0) || (Y != 0);
            }
        }

        /// <summary>
        /// HasSize - this returns true if this geometry has dimensions.
        /// Note: This will still return false if either Width or Height equals 0 and the other
        /// does not.
        /// </summary>
        public bool HasSize
        {
            get
            {
                return Width != 0 || Height != 0;
            }
        }

        /// <summary>
        /// HasAnchor - this returns true if the anchor is set.
        /// </summary>
        public bool HasAnchor
        {
            get
            {
                return Anchor != Anchor.Undefined;
            }
        }

        /// <summary>
        /// IsBlank - this returns true all variables are unset.
        /// Except Anchor, it's not as important.
        /// </summary>
        public bool IsBlank
        {
            get
            {
                return !(HasPoint || HasSize);
            }
        }

        /// <summary>
        /// X - The X coordinate of the Location.
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                Validate(value);
                _x = value;
            }

        }

        /// <summary>
        /// Y - The Y coordinate of the Location.
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                Validate(value);
                _y = value;
            }
        }

        /// <summary>
        /// Width - The Width component of the Size.
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                Validate(value);
                _width = value;
            }
        }

        /// <summary>
        /// Height - The Height component of the Size.
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                Validate(value);
                _height = value;
            }
        }

        /// <summary>
        /// Anchor - The side or corner the coordiates are relative to.
        /// </summary>
        public Anchor Anchor
        {
            get
            {
                return _anchor;
            }
            set
            {
                _anchor = value;
            }
        }

        /// <summary>
        /// Location - The Point representing the origin of the Geometry.
        /// Note: This does not tell you what anchor it's relative to.
        /// </summary>
        [XmlIgnore]
        public Point Location
        {
            get
            {
                return new Point(X, Y);
            }
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }

        /// <summary>
        /// Size - The Size representing the area of the Geometry
        /// </summary>
        [XmlIgnore]
        public Size Size
        {
            get
            {
                if (!HasSize)
                {
                    return Size.Empty;
                }
                else
                {
                    return new Size(Width, Height);
                }
            }
            set
            {
                if (value.IsEmpty)
                {
                    Width = 0;
                    Height = 0;
                }
                else
                {
                    Width = value.Width;
                    Height = value.Height;
                }
            }
        }

        /// <summary>
        /// Left Property - Distance the left edge is from its anchor.
        /// </summary>
        public double Left
        {
            get
            {
                if (Anchor == Anchor.Undefined || Anchor.HasFlag(Anchor.Left))
                {
                    return X;
                }
                else if (Anchor.HasFlag(Anchor.Right))
                {
                    return Width + X;
                }
                else
                {
                    return Width / 2 + X;
                }
            }
        }

        /// <summary>
        /// Top Property - Distance the top edge is from its anchor.
        /// </summary>
        public double Top
        {
            get
            {
                if (Anchor == Anchor.Undefined || Anchor.HasFlag(Anchor.Top))
                {
                    return Y;
                }
                else if (Anchor.HasFlag(Anchor.Bottom))
                {
                    return Height + Y;
                }
                else
                {
                    return Height / 2 + Y;
                }
            }
        }

        /// <summary>
        /// Right Property - Distance the right edge is from its anchor.
        /// </summary>
        public double Right
        {
            get
            {
                if (Anchor == Anchor.Undefined || Anchor.HasFlag(Anchor.Left))
                {
                    return Width + X;
                }
                else if (Anchor.HasFlag(Anchor.Right))
                {
                    return X;
                }
                else
                {
                    return Width / 2 + X;
                }
            }
        }

        /// <summary>
        /// Bottom Property - Distance the bottom edge is from its anchor.
        /// </summary>
        public double Bottom
        {
            get
            {
                if (Anchor == Anchor.Undefined || Anchor.HasFlag(Anchor.Top))
                {
                    return Height + Y;
                }
                else if (Anchor.HasFlag(Anchor.Bottom))
                {
                    return Y;
                }
                else
                {
                    return Height / 2 + Y;
                }
            }
        }

        /// <summary>
        /// CenterX Property - Distance the horizontal center is from its anchor.
        /// </summary>
        public double CenterX
        {
            get
            {
                if (Anchor.HasFlag(Anchor.Left) || Anchor.HasFlag(Anchor.Right))
                {
                    return X + Width / 2;
                }
                else
                {
                    return X;
                }
            }
        }

        /// <summary>
        /// CenterY Property - Distance the vertical center is from its anchor.
        /// </summary>
        public double CenterY
        {
            get
            {
                if (Anchor.HasFlag(Anchor.Top) || Anchor.HasFlag(Anchor.Bottom))
                {
                    return Y + Height / 2;
                }
                else
                {
                    return Y;
                }
            }
        }

        /// <summary>
        /// TopLeft Property - Location of the Top Left point relative to its anchor.
        /// </summary>
        public Point TopLeft
        {
            get
            {
                return new Point(Left, Top);
            }
        }

        /// <summary>
        /// TopRight Property - Location of the Top Right point relative to its anchor.
        /// </summary>
        public Point TopRight
        {
            get
            {
                return new Point(Right, Top);
            }
        }

        /// <summary>
        /// BottomLeft Property - Location of the Bottom Left point relative to its anchor.
        /// </summary>
        public Point BottomLeft
        {
            get
            {
                return new Point(Left, Bottom);
            }
        }

        /// <summary>
        /// BottomRight Property - Location of the Bottom Right point relative to its anchor.
        /// </summary>
        public Point BottomRight
        {
            get
            {
                return new Point(Right, Bottom);
            }
        }

        /// <summary>
        /// Center Property - Location of the Center point relative to its anchor.
        /// </summary>
        public Point Center
        {
            get
            {
                return new Point(CenterX, CenterY);
            }
        }
        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// LocationWithoutAnchor - The Top Left Point relative to the Top Left of the parent geometry.
        /// </summary>
        /// <param name="width"> The width of the parent geometry. </param>
        /// <param name="height"> The height of the parent geometry. </param>
        /// <returns>
        /// Returns the Top Left Point relative to the Top Left of the parent geometry.
        /// </returns>
        public Point LocationWithoutAnchor(double width, double height)
        {
            double x;
            double y;

            if (Anchor == Anchor.Undefined || Anchor.HasFlag(Anchor.Left))
            {
                x = X;
            }
            else if (Anchor.HasFlag(Anchor.Right))
            {
                x = width - Width + X;
            }
            else
            {
                x = (width - Width) / 2 + X;
            }

            if (Anchor == Anchor.Undefined || Anchor.HasFlag(Anchor.Top))
            {
                y = Y;
            }
            else if (Anchor.HasFlag(Anchor.Bottom))
            {
                y = height - Height + Y;
            }
            else
            {
                y = (height - Height) / 2 + Y;
            }

            return new Point(x, y);
        }

        public Point LocationWithoutAnchor(Size parentSize)
        {
            return LocationWithoutAnchor(parentSize.Width, parentSize.Height);
        }

        public Point LocationWithoutAnchor(Geometry parentGeometry)
        {
            return LocationWithoutAnchor(parentGeometry.Width, parentGeometry.Height);
        }

        public void RemoveAnchor(double width, double height)
        {
            Location = LocationWithoutAnchor(width, height);
            Anchor = Anchor.Undefined;
        }

        public void RemoveAnchor(Size parentSize)
        {
            RemoveAnchor(parentSize.Width, parentSize.Height);
        }

        public void RemoveAnchor(Geometry parentGeometry)
        {
            RemoveAnchor(parentGeometry.Width, parentGeometry.Height);
        }

        // Todo: test
        public void ChangeAnchor(double width, double height, Anchor anchor)
        {
            RemoveAnchor(width, height);

            switch(anchor)
            {
                case Anchor.Undefined:
                    break;
                case Anchor.TopLeft:
                    break;
                case Anchor.Top:
                    Location = new Point(CenterX, Top); break;
                case Anchor.TopRight:
                    Location = TopRight; break;
                case Anchor.Left:
                    Location = new Point(Left, CenterY); break;
                case Anchor.Center:
                    Location = Center; break;
                case Anchor.Right:
                    Location = new Point(Right, CenterY); break;
                case Anchor.BottomLeft:
                    Location = BottomLeft; break;
                case Anchor.Bottom:
                    Location = new Point(Bottom, CenterY); break;
                case Anchor.BottomRight:
                    Location = BottomRight; break;
                default:
                    break;
            }

            Anchor = anchor;
        }

        public void ChangeAnchor(Size parentSize, Anchor anchor)
        {
            ChangeAnchor(parentSize.Width, parentSize.Height, anchor);
        }

        public void ChangeAnchor(Geometry parentGeometry, Anchor anchor)
        {
            ChangeAnchor(parentGeometry.Width, parentGeometry.Height, anchor);
        }

        public void Resize(double width, double height)
        {
            Width += width;
            Height += height;
        }

        public void Resize(Size newSize)
        {
            Resize(newSize.Width, newSize.Height);
        }

        public void Resize(Geometry newGeometry)
        {
            Resize(newGeometry.Width, newGeometry.Height);
        }

        public void ResizeTo(double parentWidth, double parentHeight, double childWidth, double childHeight)
        {
            double xScale = childWidth / parentWidth;
            double yScale = childHeight / parentHeight;
            X /= xScale;
            Y /= yScale;
            Width /= xScale;
            Height /= yScale;
        }

        public void ResizeTo(Size parentSize, Size childSize)
        {
            ResizeTo(parentSize.Width, parentSize.Height, childSize.Width, childSize.Height);
        }

        public void ResizeTo(Geometry parentGeometry, Geometry childGeometry)
        {
            ResizeTo(parentGeometry.Width, parentGeometry.Height, childGeometry.Width, childGeometry.Height);
        }

        public static Geometry Max(
            double? x1 = null, double? y1 = null, double? width1 = null, double? height1 = null,
            double? x2 = null, double? y2 = null, double? width2 = null, double? height2 = null)
        {
            double x = Math.Max(x1 ?? double.MinValue, x2 ?? double.MinValue);
            double y = Math.Max(y1 ?? double.MinValue, y2 ?? double.MinValue);
            double width = Math.Max(width1 ?? double.MinValue, width2 ?? double.MinValue);
            double height = Math.Max(height1 ?? double.MinValue, height2 ?? double.MinValue);
            if (x == double.MinValue) x = Blank.X;
            if (y == double.MinValue) y = Blank.Y;
            if (width == double.MinValue) width = Blank.Width;
            if (height == double.MinValue) height = Blank.Height;
            return new Geometry(x, y, width, height);
        }

        public static Geometry Max(Geometry geo1, Geometry geo2)
        {
            return Max(geo1.X, geo1.Y, geo1.Width, geo1.Height, geo2.X, geo2.Y, geo2.Width, geo2.Height);
        }

        public Geometry Max(double? x2, double? y2, double? width2, double? height2)
        {
            return Max(X, Y, Width, Height, x2, y2, width2, height2);
        }

        public Geometry Max(Geometry geo2)
        {
            return Max(geo2.X, geo2.Y, geo2.Width, geo2.Height);
        }

        public static Geometry Min(
            double? x1 = null, double? y1 = null, double? width1 = null, double? height1 = null,
            double? x2 = null, double? y2 = null, double? width2 = null, double? height2 = null)
        {
            double x = Math.Min(x1 ?? double.MaxValue, x2 ?? double.MaxValue);
            double y = Math.Min(y1 ?? double.MaxValue, y2 ?? double.MaxValue);
            double width = Math.Min(width1 ?? double.MaxValue, width2 ?? double.MaxValue);
            double height = Math.Min(height1 ?? double.MaxValue, height2 ?? double.MaxValue);
            if (x == double.MaxValue) x = Blank.X;
            if (y == double.MaxValue) y = Blank.Y;
            if (width == double.MaxValue) width = Blank.Width;
            if (height == double.MaxValue) height = Blank.Height;
            return new Geometry(x, y, width, height);
        }

        public static Geometry Min(Geometry geo1, Geometry geo2)
        {
            return Min(geo1.X, geo1.Y, geo1.Width, geo1.Height, geo2.X, geo2.Y, geo2.Width, geo2.Height);
        }

        public Geometry Min(double? x2, double? y2, double? width2, double? height2)
        {
            return Min(X, Y, Width, Height, x2, y2, width2, height2);
        }

        public Geometry Min(Geometry geo2)
        {
            return Min(geo2.X, geo2.Y, geo2.Width, geo2.Height);
        }

        // @TODO: Explain more clearly.
        /// <summary>
        /// Clamp - Ensure the geometry is no bigger or or smaller than the given geometry.
        /// </summary>
        public Geometry Clamp(Geometry geo1, Geometry geo2)
        {
            var gMax = Max(geo1, geo2);
            var gMin = Min(geo1, geo2);
            return Min(gMax).Max(gMin);
        }

        public void Update(double x = 0, double y = 0, double width = 0, double height = 0)
        {
            X += x;
            Y += y;
            Width += width;
            Height += height;
        }

        public void Update(Geometry geo)
        {
            Update(geo.X, geo.Y, geo.Width, geo.Height);
        }

        // Don't use this lol
        public override int GetHashCode()
        {
            throw new Exception("How could this happen?");
        }

        public override bool Equals(object obj)
        {
            return obj is Geometry && Equals(this, (Geometry)obj);
        }

        public static bool Equals(Geometry geo1, Geometry geo2)
        {
            return geo1._x == geo2._x
                && geo1._y == geo2._y
                && geo1._width == geo2._width
                && geo1._height == geo2._height
                // Hope northwest/undefined don't have trouble getting along...
                && geo1._anchor == geo2._anchor;
        }

        public static bool operator ==(Geometry geo1, Geometry geo2)
        {
            return geo1.Equals(geo2);
        }

        public static bool operator !=(Geometry geo1, Geometry geo2)
        {
            return !geo1.Equals(geo2);
        }

        // @TODO: Handle fractional pixels.

        public Geometry Round(bool includePoint = true, int rounding = 0)
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

            return new Geometry(x, y, width, height);
        }

        public static Geometry FromString(string str)
        {
            var split = str.Split('x');
            var width = double.Parse(split[0]);
            str = split[1];

            var xPos = str.IndexOfAny(new char[] { '+', '-' });
            var height = double.Parse(str.Substring(0, xPos));
            str = str.Substring(xPos);

            var yPos = str.IndexOfAny(new char[] { '+', '-' }, 1);
            var x = double.Parse(str.Substring(0, yPos));
            var y = double.Parse(str.Substring(yPos));

            return new Geometry(x, y, width, height, Anchor.Undefined);
        }


        // I do not trust these contains and related. They need testing.

        /// <summary>
        /// Contains - Returns true if the Point represented by x,y is within the rectangle inclusive of the edges.
        /// Returns false otherwise.
        /// </summary>
        /// <param name="x"> X coordinate of the point which is being tested </param>
        /// <param name="y"> Y coordinate of the point which is being tested </param>
        /// <returns>
        /// Returns true if the Point represented by x,y is within the rectangle.
        /// Returns false otherwise.
        /// </returns>
        public bool Contains(double x, double y)
        {
            return ContainsInternal(x, y);
        }

        /// <summary>
        /// Contains - Returns true if the Point is within the rectangle, inclusive of the edges.
        /// Returns false otherwise.
        /// </summary>
        /// <param name="point"> The point which is being tested </param>
        /// <returns>
        /// Returns true if the Point is within the rectangle.
        /// Returns false otherwise
        /// </returns>
        public bool Contains(Point point)
        {
            return Contains(point.X, point.Y);
        }

        /// <summary>
        /// Contains - Returns true if the Rect non-Empty and is entirely contained within the
        /// rectangle, inclusive of the edges.
        /// Returns false otherwise
        /// </summary>
        public bool Contains(Geometry childGeometry)
        {
            if (!HasSize && !childGeometry.HasSize)
            {
                return false;
            }

            return (_x <= childGeometry._x &&
                    _y <= childGeometry._y &&
                    _x + _width >= childGeometry._x + childGeometry._width &&
                    _y + _height >= childGeometry._y + childGeometry._height);
            /*
            return (childGeometry.X >= 0 &&
                    childGeometry.Y >= 0 &&
                    childGeometry.X + childGeometry.Width <= Width &&
                    childGeometry.Y + childGeometry.Height <= Height);
            */

        }

        /// <summary>
        /// IntersectsWith - Returns true if the Rect intersects with this rectangle
        /// Returns false otherwise.
        /// Note that if one edge is coincident, this is considered an intersection.
        /// </summary>
        /// <returns>
        /// Returns true if the Rect intersects with this rectangle
        /// Returns false otherwise.
        /// or Height
        /// </returns>
        /// <param name="rect"> Rect </param>
        public bool IntersectsWith(Geometry rect)
        {
            if (!HasSize && !rect.HasSize)
            {
                return false;
            }

            return (rect.Left <= Right) &&
                   (rect.Right >= Left) &&
                   (rect.Top <= Bottom) &&
                   (rect.Bottom >= Top);
        }

        /// <summary>
        /// Intersect - Update this rectangle to be the intersection of this and rect
        /// If either this or rect are Empty, the result is Empty as well.
        /// </summary>
        /// <param name="rect"> The rect to intersect with this </param>
        public void Intersect(Geometry rect)
        {
            if (!this.IntersectsWith(rect))
            {
                this = Blank;
            }
            else
            {
                double left = Math.Max(Left, rect.Left);
                double top = Math.Max(Top, rect.Top);

                //  Max with 0 to prevent double weirdness from causing us to be (-epsilon..0)
                _width = Math.Max(Math.Min(Right, rect.Right) - left, 0);
                _height = Math.Max(Math.Min(Bottom, rect.Bottom) - top, 0);

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Intersect - Return the result of the intersection of rect1 and rect2.
        /// If either this or rect are Empty, the result is Empty as well.
        /// </summary>
        public static Geometry Intersect(Geometry rect1, Geometry rect2)
        {
            rect1.Intersect(rect2);
            return rect1;
        }

        /// <summary>
        /// Union - Update this rectangle to be the union of this and rect.
        /// </summary>
        public void Union(Geometry rect)
        {
            if (!HasSize)
            {
                this = rect;
            }
            else if (!rect.HasSize)
            {
                double left = Math.Min(Left, rect.Left);
                double top = Math.Min(Top, rect.Top);

                double maxRight = Math.Max(Right, rect.Right);
                double maxBottom = Math.Max(Bottom, rect.Bottom);
                _width = Math.Max(maxRight - left, 0);
                _height = Math.Max(maxBottom - top, 0);

                _x = left;
                _y = top;
            }
        }

        /// <summary>
        /// Union - Return the result of the union of rect1 and rect2.
        /// </summary>
        public static Geometry Union(Geometry rect1, Geometry rect2)
        {
            rect1.Union(rect2);
            return rect1;
        }

        /// <summary>
        /// Union - Update this rectangle to be the union of this and point.
        /// </summary>
        public void Union(Point point)
        {
            Union(new Geometry(point, point));
        }

        /// <summary>
        /// Union - Return the result of the union of rect and point.
        /// </summary>
        public static Geometry Union(Geometry rect, Point point)
        {
            rect.Union(new Geometry(point, point));
            return rect;
        }

        /// <summary>
        /// Offset - translate the Location by the offset provided.
        /// </summary>
        public void Offset(Vector offsetVector)
        {
            _x += offsetVector.X;
            _y += offsetVector.Y;
        }

        /// <summary>
        /// Offset - translate the Location by the offset provided
        /// </summary>
        public void Offset(double offsetX, double offsetY)
        {
            _x += offsetX;
            _y += offsetY;
        }

        /// <summary>
        /// Offset - return the result of offsetting rect by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static Geometry Offset(Geometry rect, Vector offsetVector)
        {
            rect.Offset(offsetVector.X, offsetVector.Y);
            return rect;
        }

        /// <summary>
        /// Offset - return the result of offsetting rect by the offset provided
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static Geometry Offset(Geometry rect, double offsetX, double offsetY)
        {
            rect.Offset(offsetX, offsetY);
            return rect;
        }

        /// <summary>
        /// Inflate - inflate the bounds by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Inflate(Size size)
        {
            Inflate(size.Width, size.Height);
        }

        /// <summary>
        /// Inflate - inflate the bounds by the size provided, in all directions.
        /// If -width is > Width / 2 or -height is > Height / 2, this Rect becomes Empty
        /// If this is Empty, this method is illegal.
        /// </summary>
        public void Inflate(double width, double height)
        {
            if (!HasSize)
            {
                throw new System.InvalidOperationException("Rect_CannotCallMethod");
            }

            _x -= width;
            _y -= height;

            // Do two additions rather than multiplication by 2 to avoid spurious overflow
            // That is: (A + 2 * B) != ((A + B) + B) if 2*B overflows.
            // Note that multiplication by 2 might work in this case because A should start
            // positive & be "clamped" to positive after, but consider A = Inf & B = -MAX.
            _width += width;
            _width += width;
            _height += height;
            _height += height;

            // We catch the case of inflation by less than -width/2 or -height/2 here.  This also
            // maintains the invariant that either the Rect is Empty or _width and _height are
            // non-negative, even if the user parameters were NaN, though this isn't strictly maintained
            // by other methods.
            if (!(_width >= 0 && _height >= 0))
            {
                this = Blank;
            }
        }

        override public string ToString()
        {
            return Math.Round(Width, 4).ToString() + "x" + Math.Round(Height, 4).ToString() +
                Math.Round(X, 4).ToString("+0.####;-#.####") + Math.Round(Y, 4).ToString("+0.####;-#.####");
        }

        /// <summary>
        /// Inflate - return the result of inflating rect by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static Geometry Inflate(Geometry geometry, Size size)
        {
            geometry.Inflate(size.Width, size.Height);
            return geometry;
        }

        /// <summary>
        /// Inflate - return the result of inflating rect by the size provided, in all directions
        /// If this is Empty, this method is illegal.
        /// </summary>
        public static Geometry Inflate(Geometry rect, double width, double height)
        {
            rect.Inflate(width, height);
            return rect;
        }

        // The Transform methods were removed since we'll have no use for Matrices.

        /// <summary>
        /// Scale the rectangle in the X and Y directions
        /// </summary>
        /// <param name="scaleX"> The scale in X </param>
        /// <param name="scaleY"> The scale in Y </param>
        public void Scale(double scaleX, double scaleY)
        {
            if (!HasSize)
            {
                return;
            }

            _x *= scaleX;
            _y *= scaleY;
            _width *= scaleX;
            _height *= scaleY;

            // If the scale in the X dimension is negative, we need to normalize X and Width
            if (scaleX < 0)
            {
                // Make X the left-most edge again
                _x += _width;

                // and make Width positive
                _width *= -1;
            }

            // Do the same for the Y dimension
            if (scaleY < 0)
            {
                // Make Y the top-most edge again
                _y += _height;

                // and make Height positive
                _height *= -1;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Validate - Check if the numbers are within the Pixel limit. Throw exception if any are not.
        /// </summary>
        /// <param name="num"> The number(s) to validate </param>
        public void Validate(params double[] nums)
        {
            foreach (double num in nums)
            {
                if (num > PIXEL_LIMIT || num < -PIXEL_LIMIT)
                {
                    throw new System.InvalidOperationException(
                        "Dimensions and Coordinates cannot be greater than " + PIXEL_LIMIT.ToString() +
                        "or less than -" + PIXEL_LIMIT.ToString() + "."
                        );
                }
            }
        }

        /// <summary>
        /// ValidateAll - Check if the numeric variables are within the Pixel limit. Throw exception if any are not.
        /// </summary>
        private void ValidateAll()
        {
            var variables = new double[] { X, Y, Width, Height };
            Validate(variables);
        }

        /// <summary>
        /// ContainsInternal - Performs just the "point inside" logic
        /// </summary>
        /// <returns>
        /// bool - true if the point is inside the rect
        /// </returns>
        /// <param name="x"> The x-coord of the point to test </param>
        /// <param name="y"> The y-coord of the point to test </param>
        private bool ContainsInternal(double x, double y)
        {
            // We include points on the edge as "contained".
            // We do "x - _width <= _x" instead of "x <= _x + _width"
            // so that this check works when _width is PositiveInfinity
            // and _x is NegativeInfinity.
            return ((x >= _x) && (x - _width <= _x) &&
                    (y >= _y) && (y - _height <= _y));
        }

        static private Geometry CreateEmptyGeometry()
        {
            Geometry geometry = new Geometry();
            // We can't set these via the property setters because negatives widths
            // are rejected in those APIs.
            geometry._x = 0;
            geometry._y = 0;
            geometry._width = 0;
            geometry._height = 0;
            geometry._anchor = Anchor.Undefined;
            return geometry;
        }

        #endregion Private Methods

        #region Private Fields

        private double _x;
        private double _y;
        private double _width;
        private double _height;
        private Anchor _anchor;

        #endregion Private Fields

        #region Constants

        private const double PIXEL_LIMIT = 8192;

        #endregion Constants
    }
}