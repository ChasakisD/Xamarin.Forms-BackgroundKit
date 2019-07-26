using System;
using System.ComponentModel;
using Xamarin.Forms;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Controls.Base;

namespace XamarinBackgroundKit.Skia.Controls
{
    [ContentProperty(nameof(Content))]
    public class SKBackgroundContentView : SKBackgroundCanvasView
    {
        #region Bindable Properties

        public static readonly BindableProperty ContentProperty = BindableProperty.Create(
            nameof(Content), typeof(View), typeof(SKBackgroundContentView));

        public View Content
        {
            get => (View)GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        #endregion

        public event EventHandler<EventArgs> InvalidateClipRequested;

        public override void OnBackgroundChanged(Background oldValue, Background newValue)
        {
            base.OnBackgroundChanged(oldValue, newValue);

            if (oldValue != null)
            {
                oldValue.PropertyChanged -= OnBackgroundPropertyChanged;
            }

            if (newValue != null)
            {
                newValue.PropertyChanged += OnBackgroundPropertyChanged;
            }
        }

        private void OnBackgroundPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != CornerElement.CornerRadiusProperty.PropertyName
                && e.PropertyName != BorderElement.BorderWidthProperty.PropertyName
                && e.PropertyName != BorderElement.BorderStyleProperty.PropertyName) return;

            InvalidateClipRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
