using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinBackgroundKit.Controls;
using XamarinBackgroundKit.Effects;
using XamarinBackgroundKit.Skia.Controls;

namespace XamarinBackgroundKitSample
{
    public partial class ExploreViewsPage
    {
        private CancellationTokenSource _cancellationTokenSource;

        public ExploreViewsPage()
        {
            InitializeComponent();

            var customViews = new ObservableCollection<string>
            {
                "MaterialCard",
                "MaterialContentView",
                "MaterialCardShowCase1",
                "SKBackgroundCanvasView",
                "SKMaterialCardShowCase1"
            };

            var xamarinViews = new ObservableCollection<string>
            {
                "ActivityIndicator",
                "BoxView",
                "Button",
                "DatePicker",
                "Editor",
                "Entry",
                "Image",
                "ImageButton",
                "Label",
                "Picker",
                "ProgressBar",
                "SearchBar",
                "Slider",
                "Stepper",
                "Switch",
                "TimePicker",
                "WebView"
            };

            var xamarinLayouts = new ObservableCollection<string>
            {
                "AbsoluteLayout",
                "CarouselView",
                "CollectionView",
                "ContentView",
                "FlexLayout",
                "Frame",
                "Grid",
                "ListView",
                "RelativeLayout",
                "ScrollView",
                "StackLayout"
            };

            CustomViewsCollectionView.ItemsSource = customViews;
            XamarinControlsCollectionView.ItemsSource = xamarinViews;
            XamarinLayoutsCollectionView.ItemsSource = xamarinLayouts;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            StartGradientAnimation();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _cancellationTokenSource.Cancel();
            BackgroundView.AbortAnimation("name");
        }

        private async void StartGradientAnimation()
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var gradientStates = new List<(Color, Color)>
            {
                (Color.FromHex("#4A148C"), Color.FromHex("#D500F9")),
                (Color.FromHex("#FF6F00"), Color.FromHex("#FFC400")),
                (Color.FromHex("#1B5E20"), Color.FromHex("#00E676")),
                (Color.FromHex("#B71C1C"), Color.FromHex("#FF1744"))
            };

            var state = 0;
            
            while (!_cancellationTokenSource.Token.IsCancellationRequested)
            {
                var fromColorStart = gradientStates[state].Item1;
                var fromColorEnd = gradientStates[state].Item2;

                var nextState = state + 1 > 3 ? 0 : state + 1;
                var toColorStart = gradientStates[nextState].Item1;
                var toColorEnd = gradientStates[nextState].Item2;

                var taskCompletionSource = new TaskCompletionSource<bool>();

                new Animation
                {
                    {
                        0, 1, new Animation(d =>
                        {
                            BackgroundEffect.GetBackground(BackgroundView).GradientBrush.Gradients =
                                new List<GradientStop>
                                {
                                    new GradientStop(GetColor(d, fromColorStart, toColorStart), 0),
                                    new GradientStop(GetColor(d, fromColorEnd, toColorEnd), 1)
                                };
                        })
                    }
                }.Commit(BackgroundView, "name", 16, 4000, Easing.Linear,
                    (d, b) => taskCompletionSource.TrySetResult(true));

                await taskCompletionSource.Task;

                if (_cancellationTokenSource.Token.IsCancellationRequested) break;

                state++;

                if (state > 3) state = 0;
            }
        }

        private static Color GetColor(double t, Color fromColor, Color toColor)
        {
            return Color.FromRgba(fromColor.R + t * (toColor.R - fromColor.R),
                fromColor.G + t * (toColor.G - fromColor.G),
                fromColor.B + t * (toColor.B - fromColor.B),
                fromColor.A + t * (toColor.A - fromColor.A));
        }

        private View GetDataTemplate(string control)
        {
            try
            {
                switch (control)
                {
                    case "MaterialCard":
                    case "MaterialContentView":
                        var type = Type.GetType($"XamarinBackgroundKit.Controls.{control}, {typeof(MaterialCard).Assembly.GetName().Name}");
                        if (type == null) return null;
                        return (View)Activator.CreateInstance(type);
                    case "MaterialCardShowCase1":
                        return new MaterialContentView { Content = GetMaterialShowCaseContent() };
                    case "SKMaterialCardShowCase1":
                        return new SKBackgroundContentView { Content = GetMaterialShowCaseContent() };
                    case "SKBackgroundCanvasView":
                        return new SKBackgroundCanvasView();
                    default:
                        type = Type.GetType($"Xamarin.Forms.{control}, {typeof(Grid).Assembly.GetName().Name}");
                        if (type == null) return null;
                        return (View)Activator.CreateInstance(type);
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async void OnItemClicked(object sender, EventArgs e)
        {
            if (!(sender is BindableObject bindable) || bindable.BindingContext == null) return;
            
            var view = GetDataTemplate(bindable.BindingContext.ToString());
            if (view is ContentView contentView && contentView.Content == null)
            {
                view.HeightRequest = 120;
            }

            view.Margin = new Thickness(24, 0);
            view.HorizontalOptions = LayoutOptions.FillAndExpand;

            await Navigation.PushAsync(new ExplorerPage(view));
        }

        private View GetMaterialShowCaseContent()
        {
            return new StackLayout
            {
                Spacing = 0,
                Margin = 8,
                Children =
                {
                    new Image
                    {
                        HeightRequest = 194,
                        Aspect = Aspect.AspectFill,
                        VerticalOptions = LayoutOptions.Start,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        Source = new UriImageSource
                        {
                            Uri = new Uri(
                                "https://devblogs.microsoft.com/xamarin/wp-content/uploads/sites/44/2018/11/Codemonkeys2.jpg")
                        }
                    },
                    new Label
                    {
                        FontSize = 22,
                        Text = "Material Card",
                        TextColor = Color.FromHex("4A4A4A"),
                        Margin = new Thickness(16, 16, 0, 4)
                    },
                    new Label
                    {
                        FontSize = 16,
                        Text = "Xamarin Forms Material Showcase",
                        TextColor = Color.Gray,
                        Margin = new Thickness(16, 0, 0, 4)
                    },
                    new Label
                    {
                        FontSize = 14,
                        Text = "Welcome to Xamarin.Forms Background Kit Material ShowCase",
                        TextColor = Color.Gray,
                        Margin = new Thickness(16, 16, 24, 4)
                    },
                    new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        Children =
                        {
                            new Button
                            {
                                Margin = new Thickness(16),
                                TextColor = Color.Purple,
                                FontSize = 18,
                                Text = "Action",
                                FontAttributes = FontAttributes.Bold,
                                BackgroundColor = Color.Transparent
                            }
                        }
                    }
                }
            };
        }
            
    }
}