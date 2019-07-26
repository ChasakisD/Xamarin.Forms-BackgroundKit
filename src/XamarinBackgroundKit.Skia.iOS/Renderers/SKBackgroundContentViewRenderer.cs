using System;
using System.ComponentModel;
using CoreAnimation;
using CoreGraphics;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Skia.Controls;
using XamarinBackgroundKit.Skia.iOS.Renderers;

[assembly: ExportRenderer(typeof(SKBackgroundContentView), typeof(SKBackgroundContentViewRenderer))]
namespace XamarinBackgroundKit.Skia.iOS.Renderers
{
    public class SKBackgroundContentViewRenderer : SKCanvasViewRenderer
    {
        private bool _disposed;
        private IVisualElementRenderer _contentRenderer;

        private SKBackgroundContentView ElementController => Element as SKBackgroundContentView;

        protected override void OnElementChanged(ElementChangedEventArgs<SKCanvasView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement is SKBackgroundContentView oldElement)
            {
                oldElement.InvalidateClipRequested -= OnInvalidateClipRequested;
            }

            if (e.NewElement is SKBackgroundContentView newElement)
            {
                newElement.InvalidateClipRequested += OnInvalidateClipRequested;
            }

            UpdateContent();
        }

        private void OnInvalidateClipRequested(object sender, EventArgs e) => InvalidateContentMask();

        #region Property Changed

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == SKBackgroundContentView.ContentProperty.PropertyName) UpdateContent();
        }

        private void UpdateContent()
        {
            if (ElementController?.Content == null && _contentRenderer != null)
            {
                _contentRenderer.NativeView?.RemoveFromSuperview();
                _contentRenderer.Dispose();
                _contentRenderer = null;
            }

            _contentRenderer = Platform.GetRenderer(ElementController.Content);
            if (_contentRenderer == null)
            {
                _contentRenderer = Platform.CreateRenderer(ElementController.Content);
                Platform.SetRenderer(ElementController.Content, _contentRenderer);
            }

            if (_contentRenderer?.NativeView == null) return;

            AddSubview(_contentRenderer.NativeView);
        }

        #endregion

        #region Layout Content

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (_contentRenderer?.NativeView == null) return;

            var padding = ElementController.Padding;
            var contentMargin = ElementController.Content?.Margin ?? new Thickness(0d);

            var startX = padding.Left + contentMargin.Left;
            var startY = padding.Top + contentMargin.Top;
            var endX = Bounds.Right - Bounds.Left - padding.Right - contentMargin.Right;
            var endY = Bounds.Bottom - Bounds.Top - padding.Bottom - contentMargin.Bottom;

            _contentRenderer.NativeView.Frame = new CGRect(
                startX, startY, endX - startX, endY - startY);

            _contentRenderer.Element.Layout(new Rectangle(
                startX, startY, endX - startX, endY - startY));

            InvalidateContentMask();
        }

        #endregion

        #region Clip Content

        private void InvalidateContentMask()
        {
            if (ElementController?.Background == null || _contentRenderer?.NativeView == null) return;

            if (!(_contentRenderer.Element is View view)) return;
            var transform = CGAffineTransform.MakeTranslation(
                -(nfloat)view.Margin.Left,
                -(nfloat)view.Margin.Top);

            var bounds = new CGRect(
                Bounds.Left,
                Bounds.Top,
                Bounds.Width - ElementController.Padding.HorizontalThickness,
                Bounds.Height - ElementController.Padding.VerticalThickness);

            CGPath maskPath;
            switch (ElementController.Background.BorderStyle)
            {
                case BorderStyle.Inner:
                    maskPath = SkiaBackgroundKit.GetRoundCornersPath(
                        bounds, ElementController.Background.CornerRadius).CGPath;
                    break;
                default:
                    var borderWidth = (float)ElementController.Background.BorderWidth;
                    bounds = bounds.Inset(borderWidth, borderWidth);
                    maskPath = SkiaBackgroundKit.GetRoundCornersPath(
                        bounds, ElementController.Background.CornerRadius, borderWidth).CGPath;
                    break;
            }

            _contentRenderer.NativeView.Layer.Mask?.Dispose();
            _contentRenderer.NativeView.Layer.Mask = new CAShapeLayer
            {
                Frame = _contentRenderer.NativeView.Bounds,
                Path = new CGPath(maskPath, transform)
            };

            _contentRenderer.NativeView.Layer.MasksToBounds = true;
        }

        #endregion

        #region LifeCycle

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            _disposed = true;

            if (disposing)
            {
                if (_contentRenderer != null)
                {
                    _contentRenderer?.Dispose();
                    _contentRenderer = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
