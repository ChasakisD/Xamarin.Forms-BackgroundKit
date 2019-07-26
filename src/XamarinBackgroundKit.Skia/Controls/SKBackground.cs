using Xamarin.Forms;
using XamarinBackgroundKit.Controls;

namespace XamarinBackgroundKit.Skia.Controls
{
    public class SKBackground : Background
    {
        #region Bindable Properties

        public static readonly BindableProperty ShadowColorProperty = BindableProperty.Create(
            nameof(ShadowColor), typeof(Color), typeof(SKBackground), Color.Default);

        public Color ShadowColor
        {
            get => (Color)GetValue(ShadowColorProperty);
            set => SetValue(ShadowColorProperty, value);
        }

        public static readonly BindableProperty ShadowOffsetXProperty = BindableProperty.Create(
            nameof(ShadowOffsetX), typeof(double), typeof(SKBackground), 0d);

        public double ShadowOffsetX
        {
            get => (double)GetValue(ShadowOffsetXProperty);
            set => SetValue(ShadowOffsetXProperty, value);
        }

        public static readonly BindableProperty ShadowOffsetYProperty = BindableProperty.Create(
            nameof(ShadowOffsetY), typeof(double), typeof(SKBackground), 0d);

        public double ShadowOffsetY
        {
            get => (double)GetValue(ShadowOffsetYProperty);
            set => SetValue(ShadowOffsetYProperty, value);
        }

        #endregion
    }
}
