using System.Collections.Generic;
using System.Linq;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Extensions;

namespace XamarinBackgroundKit.Skia.Extensions
{
    public static class SKPaintExtensions
    {
        public static void ApplyGradient(this SKPaint paint, SKRect rect, GradientBrush brush)
        {
            if (brush == null) return;

            paint.Shader = GetGradientShader(rect, brush.Gradients, brush.Angle);
        }

        public static void ApplyGradient(this SKPaint paint, SKRect rect, IList<GradientStop> gradients, float angle)
        {
            paint.Shader = GetGradientShader(rect, gradients, angle);
        }

        public static SKShader GetGradientShader(SKRect rect, IList<GradientStop> gradients, float angle)
        {
            if (gradients == null || !gradients.Any()) return null;

            var positions = gradients.Select(x => x.Offset).ToArray();
            var colors = gradients.Select(x => x.Color.ToSKColor()).ToArray();

            var colorPositions = angle.ToStartEndPoint();
            var startPoint = new SKPoint(colorPositions[0] * rect.Width, colorPositions[1] * rect.Height);
            var endPoint = new SKPoint(colorPositions[2] * rect.Width, colorPositions[3] * rect.Height);

            return SKShader.CreateLinearGradient(startPoint, endPoint, colors, positions, SKShaderTileMode.Clamp);
        }
    }
}
