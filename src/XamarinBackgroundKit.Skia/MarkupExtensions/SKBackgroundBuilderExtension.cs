using System;
using Xamarin.Forms.Xaml;
using XamarinBackgroundKit.Skia.Controls;

namespace XamarinBackgroundKit.Skia.MarkupExtensions
{
    public class SKBackgroundBuilderExtension : SKBackground, IMarkupExtension<SKBackground>
    {
        public SKBackground ProvideValue(IServiceProvider serviceProvider) => this;

        object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
    }
}
