using SkiaSharp;
using Xamarin.Forms;
using XamarinBackgroundKit.Abstractions;

namespace XamarinBackgroundKit.Skia.Extensions
{
    public static class SKRectExtensions
    {
        public static SKRect Inset(this SKRect rect, Thickness thickness, double density = 1d)
        {
            return new SKRect(
                (float)(rect.Left + (thickness.Left * density)),
                (float)(rect.Top + (thickness.Top * density)),
                (float)(rect.Right - (thickness.Right * density)),
                (float)(rect.Bottom - (thickness.Bottom * density)));
        }

        public static SKRect Inset(this SKRectI rect, Thickness thickness, double density = 1d)
        {
            return new SKRect(
                (float)(rect.Left + (thickness.Left * density)),
                (float)(rect.Top + (thickness.Top * density)),
                (float)(rect.Right - (thickness.Right * density)),
                (float)(rect.Bottom - (thickness.Bottom * density)));
        }

        public static SKRoundRect ToSKRoundRect(this SKRect rect, ICornerElement cornerElement, double density = 1d)
        {
            var roundRect = new SKRoundRect();
            roundRect.SetRectRadii(rect, cornerElement.ToSKRadii(density));
            return roundRect;
        }

        public static SKPoint[] ToSKRadii(this ICornerElement cornerElement, double density)
        {
            return cornerElement.CornerRadius.ToSKRadii(density);
        }

        public static SKPoint[] ToSKRadii(this CornerRadius cornerRadius, double density)
        {
            return new[]
            {
                new SKPoint(ToPixels(cornerRadius.TopLeft, density), ToPixels(cornerRadius.TopLeft, density)),
                new SKPoint(ToPixels(cornerRadius.TopRight, density), ToPixels(cornerRadius.TopRight, density)),
                new SKPoint(ToPixels(cornerRadius.BottomRight, density), ToPixels(cornerRadius.BottomRight, density)),
                new SKPoint(ToPixels(cornerRadius.BottomLeft, density), ToPixels(cornerRadius.BottomLeft, density))
            };
        }

        public static float ToPixels(double units, double density)
        {
            return (float)(units * density);
        }
    }
}
