using System;
using System.ComponentModel;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using XamarinBackgroundKit.Abstractions;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Controls.Base;
using XamarinBackgroundKit.Extensions;
using XamarinBackgroundKit.Skia.Extensions;

namespace XamarinBackgroundKit.Skia.Controls
{
    public class SKBackgroundCanvasView : SKCanvasView, IBackgroundElement
    {
        #region Bindable Properties

        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(
            nameof(Padding), typeof(Thickness), typeof(SKBackgroundCanvasView), new Thickness(0d),
            propertyChanged: (b, o, n) => ((SKBackgroundCanvasView)b)?.InvalidateSurface());

        public Thickness Padding
        {
            get => (Thickness)GetValue(PaddingProperty);
            set => SetValue(PaddingProperty, value);
        }

        public static readonly BindableProperty BackgroundProperty = BackgroundElement.BackgroundProperty;

        public Background Background
        {
            get => (Background)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        #endregion
        
        #region IBackgroundElement Implementation

        public virtual void OnBackgroundChanged(Background oldValue, Background newValue)
        {          
            if (oldValue != null)
            {
                oldValue.PropertyChanged -= OnBackgroundPropertyChanged;
                newValue.InvalidateGradientRequested -= OnGradientsInvalidateRequested;
                newValue.InvalidateBorderGradientRequested -= OnGradientsInvalidateRequested;
            }

            if (newValue != null)
            {
                newValue.PropertyChanged += OnBackgroundPropertyChanged;
                newValue.InvalidateGradientRequested += OnGradientsInvalidateRequested;
                newValue.InvalidateBorderGradientRequested += OnGradientsInvalidateRequested;
                newValue.SetBinding(BindingContextProperty, new Binding("BindingContext", source: this));
            }
        }

        private void OnGradientsInvalidateRequested(object sender, EventArgs e) => InvalidateSurface();

        private void OnBackgroundPropertyChanged(object sender, PropertyChangedEventArgs e) => InvalidateSurface();

        #endregion

        private double _density;

        private static readonly SKColor ShadowColor = new SKColor(0, 0, 0, 80);

        #region Actual Draw

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            _density = e.Info.Width / Width;

            var canvas = e.Surface.Canvas;
            var bounds = e.Info.Rect.Inset(Padding, _density);

            canvas.Clear();

            using (var clipPath = GetClipPath(bounds))
            {
                DrawShadow(canvas, bounds, clipPath);
                DrawBackground(canvas, bounds, clipPath);
                DrawBackgroundBorder(canvas, bounds, clipPath);
            }            
        }       

        protected virtual void DrawShadow(SKCanvas canvas, SKRect bounds, SKPath clipPath)
        {
            if (!HasShadow()) return;

            var elevationPx = (float)(Background.Elevation * _density);

            using (var shadowPaint = new SKPaint())
            {
                shadowPaint.IsAntialias = true;
                shadowPaint.Style = SKPaintStyle.Fill;

                switch (Background)
                {
                    case SKBackground skBackground:
                        var shadowOffsetXPx = (float)(skBackground.ShadowOffsetX * _density);
                        var shadowOffsetYPx = (float)(skBackground.ShadowOffsetY * _density);
                        shadowPaint.ImageFilter = SKImageFilter.CreateDropShadow(
                            shadowOffsetXPx, shadowOffsetYPx, elevationPx, elevationPx, skBackground.ShadowColor.ToSKColor(),
                            SKDropShadowImageFilterShadowMode.DrawShadowOnly);
                        break;
                    default:
                        shadowPaint.ImageFilter = SKImageFilter.CreateDropShadow(
                            0, elevationPx, elevationPx, elevationPx, ShadowColor,
                            SKDropShadowImageFilterShadowMode.DrawShadowOnly);
                        break;
                }

                canvas.DrawPath(clipPath, shadowPaint);
            }
        }

        protected virtual void DrawBackground(SKCanvas canvas, SKRect bounds, SKPath clipPath)
        {
            using (var backgroundPaint = new SKPaint())
            {
                backgroundPaint.IsAntialias = true;
                backgroundPaint.Style = SKPaintStyle.Fill;
                backgroundPaint.Color = Background.Color.ToSKColor();

                if (HasGradient())
                {
                    backgroundPaint.ApplyGradient(bounds, Background.GradientBrush);
                }

                canvas.DrawPath(clipPath, backgroundPaint);
            }
        }

        protected virtual void DrawBackgroundBorder(SKCanvas canvas, SKRect bounds, SKPath clipPath)
        {
            if (!HasBorder()) return;
            
            using (var borderPaint = new SKPaint())
            {
                borderPaint.IsAntialias = true;
                borderPaint.Style = SKPaintStyle.Stroke;
                borderPaint.Color = Background.BorderColor.ToSKColor();
                borderPaint.StrokeWidth = (float)(Background.BorderWidth * _density * 2);

                if (IsBorderDashed())
                {
                    borderPaint.PathEffect = SKPathEffect.CreateDash(new[]
                    {
                        (float)(Background.DashWidth * _density),
                        (float)(Background.DashGap * _density)
                    }, 0);
                }

                if (HasBorderGradient())
                {
                    borderPaint.ApplyGradient(bounds, Background.BorderGradientBrush);
                }

                canvas.Save();
                canvas.ClipPath(clipPath, antialias: true);
                canvas.DrawPath(clipPath, borderPaint);
                canvas.Restore();
            }
        }

        #endregion

        #region Draw Helpers

        private bool IsBorderDashed()
        {
            return Background.DashGap > 0 && Background.DashWidth > 0;
        }

        private bool HasBorder()
        {
            return Background.BorderWidth > 0;
        }

        private bool HasShadow()
        {
            return Background.Elevation > 0;
        }

        private bool HasGradient()
        {
            return Background.GradientBrush != null
                && Background.GradientBrush.Gradients != null
                && Background.GradientBrush.Gradients.Count > 0;
        }

        private bool HasBorderGradient()
        {
            return Background.BorderGradientBrush != null
                && Background.BorderGradientBrush.Gradients != null
                && Background.BorderGradientBrush.Gradients.Count > 0;
        }

        private SKPath GetClipPath(SKRect bounds)
        {
            var clipPath = new SKPath();

            if (Background.IsEmpty())
            {
                clipPath.AddRect(bounds);
            }
            else if (Background.IsAllRadius())
            {
                var cornerRadiusPx = (float)(Background.CornerRadius.TopLeft * _density);
                clipPath.AddRoundRect(bounds, cornerRadiusPx, cornerRadiusPx);
            }
            else
            {
                clipPath.AddRoundRect(bounds.ToSKRoundRect(Background, _density));
            }

            return clipPath;
        }

        #endregion
    }
}
