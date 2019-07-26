using System;
using System.ComponentModel;
using Android.Content;
using Android.Graphics;
using Android.Views;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Extensions;
using XamarinBackgroundKit.Skia.Android.Renderers;
using XamarinBackgroundKit.Skia.Controls;
using ASKCanvasView = SkiaSharp.Views.Android.SKCanvasView;
using AView = Android.Views.View;
using SKCanvasView = SkiaSharp.Views.Forms.SKCanvasView;

[assembly: ExportRenderer(typeof(SKBackgroundContentView), typeof(SKBackgroundContentViewRenderer))]
namespace XamarinBackgroundKit.Skia.Android.Renderers
{
    public class SKBackgroundContentViewRenderer : SKCanvasViewRenderer
    {
        private bool _disposed;
        private Path _clipPath;
        private IVisualElementRenderer _contentRenderer;

        private SKBackgroundContentView ElementController => Element as SKBackgroundContentView;

        public SKBackgroundContentViewRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<SKCanvasView> e)
        {
            base.OnElementChanged(e);

            if (_clipPath == null)
            {
                _clipPath = new Path();
            }

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

        private void OnInvalidateClipRequested(object sender, EventArgs e) => PostInvalidate();

        #region Property Changed

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == SKBackgroundContentView.ContentProperty.PropertyName) UpdateContent();
        }

        private void UpdateContent()
        {
            if (ElementController.Content == null && _contentRenderer != null)
            {
                RemoveView(_contentRenderer?.View);

                _contentRenderer.Dispose();
                _contentRenderer = null;
            }

            _contentRenderer = Platform.GetRenderer(ElementController.Content);
            if (_contentRenderer == null)
            {
                _contentRenderer = Platform.CreateRendererWithContext(ElementController.Content, Context);
                Platform.SetRenderer(ElementController.Content, _contentRenderer);
            }

            if (_contentRenderer?.View == null) return;

            AddView(_contentRenderer.View, LayoutParams.MatchParent);
        }

        #endregion

        #region Layout Content

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (_contentRenderer?.View != null)
            {
                var padding = ElementController.Padding;
                var contentMargin = ElementController.Content?.Margin ?? new Thickness(0d);

                var startX = (int)Context.ToPixels(padding.Left + contentMargin.Left);
                var startY = (int)Context.ToPixels(padding.Top + contentMargin.Top);
                var endX = r - l - (int)Context.ToPixels(padding.Right + contentMargin.Right);
                var endY = b - t - (int)Context.ToPixels(padding.Bottom + contentMargin.Bottom);

                _contentRenderer.Element.Layout(new Rectangle(
                    Context.FromPixels(startX),
                    Context.FromPixels(startY),
                    Context.FromPixels(endX - startX),
                    Context.FromPixels(endY - startY)));

                var msw = MeasureSpec.MakeMeasureSpec(endX, MeasureSpecMode.Exactly);
                var msh = MeasureSpec.MakeMeasureSpec(endY, MeasureSpecMode.Exactly);

                _contentRenderer.View.Measure(msw, msh);
                _contentRenderer.View.Layout(startX, startY, endX, endY);
                _contentRenderer.UpdateLayout();
            }

            if (changed)
            {
                PostInvalidate();
            }
        }

        #endregion        

        #region Clip Content

        protected override bool DrawChild(Canvas canvas, AView child, long drawingTime)
        {
            if (child is ASKCanvasView)
                return base.DrawChild(canvas, child, drawingTime);

            var background = ElementController?.Background;
            if (background == null || background.CornerRadius.IsEmpty())
                return base.DrawChild(canvas, child, drawingTime);

            var strokeWidth = (int)Context.ToPixels(background.BorderWidth);
            var cornerRadii = background.CornerRadius.ToRadii(Context.Resources.DisplayMetrics.Density);

            for (var i = 0; i < cornerRadii.Length; i++)
                cornerRadii[i] -= cornerRadii[i] <= 0 ? 0 : strokeWidth;

            _clipPath.Reset();

            var paddingTopPx = (int)Context.ToPixels(ElementController.Padding.Top);
            var paddingLeftPx = (int)Context.ToPixels(ElementController.Padding.Left);
            var paddingRightPx = (int)Context.ToPixels(ElementController.Padding.Right);
            var paddingBottomPx = (int)Context.ToPixels(ElementController.Padding.Bottom);

            switch (background.BorderStyle)
            {
                case BorderStyle.Inner:
                    _clipPath.AddRoundRect(
                        paddingLeftPx,
                        paddingTopPx,
                        canvas.Width - paddingRightPx,
                        canvas.Height - paddingBottomPx,
                        cornerRadii, Path.Direction.Cw);
                    break;
                case BorderStyle.Outer:
                    _clipPath.AddRoundRect(
                        strokeWidth + paddingLeftPx,
                        strokeWidth + paddingTopPx,
                        canvas.Width - strokeWidth - paddingRightPx,
                        canvas.Height - strokeWidth - paddingBottomPx,
                        cornerRadii, Path.Direction.Cw);
                    break;
            }

            canvas.Save();
            canvas.ClipPath(_clipPath);

            var drawChildResult = base.DrawChild(canvas, child, drawingTime);

            canvas.Restore();

            return drawChildResult;
        }

        #endregion

        #region LifeCycle

        protected override void Dispose(bool disposing)
        {
            if (_disposed) return;

            _disposed = true;

            if (disposing)
            {
                if (_clipPath != null)
                {
                    _clipPath.Dispose();
                    _clipPath = null;
                }

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
