using ImageMagick;
using System;

namespace LiveSplit.VFM.Models
{
    [Flags]
    public enum Anchor
    {
        Undefined = 0,
        Top = 1,
        Bottom = 2,
        Left = 4,
        Right = 8,
        TopLeft = Top | Left,
        TopRight = Top | Right,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,
        Center = 16
    }

    public static partial class Extensions
    {
        public static Gravity ToGravity(this Anchor anchor)
        {
            switch (anchor)
            {
                case Anchor.TopLeft: return Gravity.Northwest;
                case Anchor.Top: return Gravity.North;
                case Anchor.TopRight: return Gravity.Northeast;
                case Anchor.Left: return Gravity.West;
                case Anchor.Center: return Gravity.Center;
                case Anchor.Right: return Gravity.East;
                case Anchor.BottomLeft: return Gravity.Southwest;
                case Anchor.Bottom: return Gravity.South;
                case Anchor.BottomRight: return Gravity.Southeast;
                default: return Gravity.Undefined;
            }
        }

        public static Anchor ToAnchor(this Gravity gravity)
        {
            switch (gravity)
            {
                case Gravity.Northwest: return Anchor.TopLeft;
                case Gravity.North: return Anchor.Top;
                case Gravity.Northeast: return Anchor.TopRight;
                case Gravity.West: return Anchor.Left;
                case Gravity.Center: return Anchor.Center;
                case Gravity.East: return Anchor.Right;
                case Gravity.Southwest: return Anchor.BottomLeft;
                case Gravity.South: return Anchor.Bottom;
                case Gravity.Southeast: return Anchor.BottomRight;
                default: return Anchor.Undefined;
            }
        }

    }
}
