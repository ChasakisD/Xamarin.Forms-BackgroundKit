using System;
using CoreGraphics;
using UIKit;
using Xamarin.Forms;
using XamarinBackgroundKit.Extensions;

namespace XamarinBackgroundKit.Skia.iOS
{
    public static class SkiaBackgroundKit
    {
        public static void Init() { }

        internal static UIBezierPath GetRoundCornersPath(CGRect bounds, CornerRadius cornerRadius, float borderWidth = 0f)
        {
            if(cornerRadius.IsEmpty())
            {
                return UIBezierPath.FromRect(bounds);
            }

            if(cornerRadius.IsAllRadius())
            {
                return UIBezierPath.FromRoundedRect(bounds, InsetCorner(cornerRadius.TopLeft, borderWidth));
            }

            var topLeft = InsetCorner(cornerRadius.TopLeft, borderWidth);
            var topRight = InsetCorner(cornerRadius.TopRight, borderWidth);
            var bottomLeft = InsetCorner(cornerRadius.BottomLeft, borderWidth);
            var bottomRight = InsetCorner(cornerRadius.BottomRight, borderWidth);

            var bezierPath = new UIBezierPath();
            bezierPath.AddArc(new CGPoint((float)bounds.X + bounds.Width - topRight, (float)bounds.Y + topRight), topRight, (float)(Math.PI * 1.5), (float)Math.PI * 2, true);
            bezierPath.AddArc(new CGPoint((float)bounds.X + bounds.Width - bottomRight, (float)bounds.Y + bounds.Height - bottomRight), bottomRight, 0, (float)(Math.PI * .5), true);
            bezierPath.AddArc(new CGPoint((float)bounds.X + bottomLeft, (float)bounds.Y + bounds.Height - bottomLeft), bottomLeft, (float)(Math.PI * .5), (float)Math.PI, true);
            bezierPath.AddArc(new CGPoint((float)bounds.X + topLeft, (float)bounds.Y + topLeft), topLeft, (float)Math.PI, (float)(Math.PI * 1.5), true);
            bezierPath.ClosePath();

            return bezierPath;
        }

        private static float InsetCorner(double corner, float borderWidth)
        {
            var temp = corner - borderWidth;
            return temp < 0 ? 0 : (float)temp;
        }
    }
}