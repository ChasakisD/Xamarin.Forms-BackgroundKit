using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Effects;
using XamarinBackgroundKit.Skia.Controls;

namespace XamarinBackgroundKitSample
{
    public partial class ExplorerPage
    {
        private Background _background;
        public Background Background
        {
            get => _background;
            set
            {
                _background = value;
                OnPropertyChanged();
            }
        }

        private int _offsetCount;
        private int _borderOffsetCount;

        private readonly View _view;

        private readonly ObservableCollection<GradientStop> _gradientsStackItems;
        private readonly ObservableCollection<GradientStop> _borderGradientsStackItems;

        public ExplorerPage(View view)
        {
            InitializeComponent();

            BindingContext = this;

            if (view == null) return;

            Title = $"{view.GetType().Name} Explorer";

            if (view is Button button)
            {
                button.Text = "Test Text";
            }
            
            Background = new Background();
            if (view is MaterialContentView materialView)
            {
                materialView.Background = Background;
            }
            else if (view is SKBackgroundCanvasView skBackgroundCanvasView)
            {
                skBackgroundCanvasView.Padding = 24;
                skBackgroundCanvasView.Background = Background;
            }
            else
            {
                BackgroundEffect.SetBackground(view, Background);
            }

            Background.SetBinding(Background.BorderWidthProperty, new Binding("Value", source: BorderWidthSlider));
            Background.SetBinding(Background.DashGapProperty, new Binding("Value", source: DashGapSlider));
            Background.SetBinding(Background.DashWidthProperty, new Binding("Value", source: DashWidthSlider));
            Background.SetBinding(Background.ElevationProperty, new Binding("Value", source: ElevationSlider));
            Background.SetBinding(Background.IsRippleEnabledProperty, new Binding("IsToggled", source: RippleColorSwitch));
            Background.GradientBrush.SetBinding(LinearGradientBrush.AngleProperty, new Binding("Value", source: GradientAngleSlider));
            Background.BorderGradientBrush.SetBinding(LinearGradientBrush.AngleProperty, new Binding("Value", source: BorderGradientAngleSlider));

            Container.Children.Add(view);

            _gradientsStackItems = new ObservableCollection<GradientStop>();
            Background.GradientBrush.Gradients = _gradientsStackItems;
            BindableLayout.SetItemsSource(GradientsLayout, _gradientsStackItems);

            _borderGradientsStackItems = new ObservableCollection<GradientStop>();
            Background.BorderGradientBrush.Gradients = _borderGradientsStackItems;
            BindableLayout.SetItemsSource(BorderGradientsLayout, _borderGradientsStackItems);

            _view = view;
        }

        private void OnBackgroundColorChanged(object sender, TextChangedEventArgs e)
        {
            Background.Color = GetColorFromString(e.NewTextValue);
        }

        private void OnBorderColorChanged(object sender, TextChangedEventArgs e)
        {
            Background.BorderColor = GetColorFromString(e.NewTextValue);
        }

        private void OnRippleColorChanged(object sender, TextChangedEventArgs e)
        {
            Background.RippleColor = GetColorFromString(e.NewTextValue);
        }
        
        private void OnNewGradientAdded(object sender, EventArgs e)
        {
            _offsetCount++;
            _gradientsStackItems.Add(new GradientStop());
            SyncOffsets(_gradientsStackItems, _offsetCount);
        }
        
        private void OnNewGradientRemoved(object sender, EventArgs e)
        {
            if(_gradientsStackItems.Count <= 0) return;

            _offsetCount--;
            _gradientsStackItems.Remove(_gradientsStackItems.Last());
            SyncOffsets(_gradientsStackItems, _offsetCount);
        }


        private void OnNewBorderGradientAdded(object sender, EventArgs e)
        {
            _borderOffsetCount++;
            _borderGradientsStackItems.Add(new GradientStop());
            SyncOffsets(_borderGradientsStackItems, _borderOffsetCount);
        }

        private void OnNewBorderGradientRemoved(object sender, EventArgs e)
        {
            if (_borderGradientsStackItems.Count <= 0) return;

            _borderOffsetCount--;
            _borderGradientsStackItems.Remove(_borderGradientsStackItems.Last());
            SyncOffsets(_borderGradientsStackItems, _borderOffsetCount);
        }
        
        private void OnGradientChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is BindableObject bindable) || !(bindable.BindingContext is GradientStop gradientStop)) return;

            gradientStop.Color = GetColorFromString(e.NewTextValue);
        }
        
        private static void SyncOffsets(IEnumerable<GradientStop> gradientStops, int offsets)
        {
            var offset = 0f;
            var delta = 1f / (offsets - 1);

            foreach (var gradientStop in gradientStops)
            {
                gradientStop.Offset = offset;
                offset += delta;
            }
        }

        private static Color GetColorFromString(string value)
        {
            if (string.IsNullOrEmpty(value)) return Color.Default;

            try
            {
                return Color.FromHex(value[0].Equals('#') ? value : $"#{value}");
            }
            catch (Exception)
            {
                return Color.Default;
            }
        }

        private void OnWidthChanged(object sender, TextChangedEventArgs e)
        {
            if (_view == null || !double.TryParse(e.NewTextValue, out var width)) return;

            if (width <= 0)
            {
                _view.HorizontalOptions = LayoutOptions.FillAndExpand;
            }
            else
            {
                _view.WidthRequest = width;
                _view.HorizontalOptions = LayoutOptions.Center;
            }
        }

        private void OnHeightChanged(object sender, TextChangedEventArgs e)
        {
            if (_view == null || !double.TryParse(e.NewTextValue, out var height)) return;

            if (height <= 0)
            {
                _view.VerticalOptions = LayoutOptions.FillAndExpand;
            }
            else
            {
                _view.HeightRequest = height;
                _view.VerticalOptions = LayoutOptions.Center;
            }
        }

        private void OnCornerRadiusChanged(object sender, ValueChangedEventArgs e)
        {
            Background.CornerRadius = new CornerRadius(TopLeftCornerSlider.Value, TopRightCornerSlider.Value,
                BottomLeftCornerSlider.Value, BottomRightCornerSlider.Value);
        }

        private async void OnColorPickerClick(object sender, EventArgs e)
        {
            if (!(sender is MaterialContentView contentView) || !(contentView.Parent is Layout<View> layout)) return;

            var selectedColor = await ColorPickerPage.ShowAsync();
            if (selectedColor == null) return;

            if (!(layout.Children.FirstOrDefault() is Entry entry)) return;

            var red = (int) (selectedColor.Value.R * 255);
            var green = (int) (selectedColor.Value.G * 255);
            var blue = (int) (selectedColor.Value.B * 255);
            entry.Text = $"#{red:X2}{green:X2}{blue:X2}";
        }

        private void OnBorderStyleToggled(object sender, ToggledEventArgs e)
        {
            Background.BorderStyle = e.Value ? BorderStyle.Outer : BorderStyle.Inner;
        }
    }
}
