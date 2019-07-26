# Xamarin.Forms-BackgroundKit
🎨 🔨 A powerful Kit for customizing the background of Xamarin.Forms views

📐 Corner Radius | 🎨 Background Gradients | 🍩 Borders | 🌈 Border Gradients | 🙏 Shadows

[![NuGet Version](https://buildstats.info/nuget/Xamarin.Forms.BackgroundKit)](https://www.nuget.org/packages/Xamarin.Forms.BackgroundKit)

* [Screenshots](#Screenshots)
* [Setup](#Setup)
* [Usage](#Usage)
* [What was hard](#What-was-hard)
* [Break some limitations](#Break-some-limitations)
* [What is Supported](#What-is-supported)
* [Ripple Support](#Ripple-Support)

### ScreenShots

#### Android
<img src="https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/art/contentViewAndroid.gif" width="280" /> | 
<img src="https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/art/searchBarAndroid.gif" width="280" />

#### iOS (Gifs coming soon...)

### Clipping and Shadow Support
<img src="https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/art/cornerRadiiElevation.png" width="280" /> | 
<img src="https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/art/cornerRadiiElevation-iOS.png" height="560" width="280" /> 

<img src="https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/art/corner_clipping_android.png" width="280" /> |
<img src="https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/art/corner_clipping_ios.png" height="560" width="280" />

## Setup
#### Xamarin.Android
Initialize the renderer below the Xamarin.Forms.Init
```cs
XamarinBackgroundKit.Android.BackgroundKit.Init();
```

#### Xamarin.iOS
Initialize the renderer below the Xamarin.Forms.Init
```cs
XamarinBackgroundKit.Android.BackgroundKit.Init();
```

### Actual Usage
Everything you need to do is coming from XAML!

#### Material Card/ContentView
```xml
<controls:MaterialContentView HeightRequest="60">

    <controls:MaterialContentView.Background>    

        <!-- Provide the Background Instance -->

        <controls:Background
            Angle="0"
            BorderColor="Brown"
            BorderWidth="4"
            CornerRadius="30"
            IsRippleEnabled="True"
            RippleColor="White">

            <!-- Provide the Background Gradient Brush -->

            <background:Background.GradientBrush>

                <background:LinearGradientBrush Angle="45">

                    <!-- Provide the Background Gradient Stops -->

                    <background:GradientStop Offset="0" Color="DarkBlue" />
                    <background:GradientStop Offset="1" Color="DarkRed" />
                </background:LinearGradientBrush>

            </background:Background.GradientBrush>

            <!-- Provide the Border Gradient Brush -->

            <background:Background.BorderGradientBrush>

                <background:LinearGradientBrush Angle="45">

                    <!-- Provide the Border Gradient Stops -->

                    <background:GradientStop Offset="0" Color="Blue" />
                    <background:GradientStop Offset="1" Color="Red" />
                </background:LinearGradientBrush>

            </background:Background.BorderGradientBrush>

        </controls:Background>

    </controls:MaterialContentView.Background>

    <Label Text="Material ContentView with Gradient and Offsets" />
</controls:MaterialContentView>
```

### New Way by using the Markup extensions
```xml
<controls:MaterialContentView 
    HeightRequest="60" 
    Background="{controls:BackgroundBuilder Color=White, CornerRadius=8, Elevation=8}" />
```

#### Do you need just a color? 
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder Color=White}" />
```    

#### Do you need rounded cornders?
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder CornerRadius='LeftTop, RightTop, RightBottom, LeftBottom'}" />
```   

#### Do you need shadow? 
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder Elevation=8}" />
```    

#### Do you need ripple? 
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder IsRippleEnabled=True, RippleColor=#80000000}" />
```   

#### Do you need gradient?
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder GradientBrush={controls:LinearGradient Start=Red, End=Green, Angle=70}}" />
``` 

#### Do you need more gradients?
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder GradientBrush={controls:LinearGradient Gradients='Red,Green,Blue', Angle=70}}" />
```

#### Do you need border?
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder BorderWidth=4, BorderColor=Red}" />
```

#### Do you need dashed border?
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder BorderWidth=4, BorderColor=Red, DashGap=5, DashWidth=10}" />
```

#### Do you need gradient border?
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder BorderWidth=4, BorderGradientBrush={controls:LinearGradient Start=Red, End=Green, Angle=70}}" />
```

#### Do you need outer border?
```xml
<controls:MaterialContentView 
    Background="{controls:BackgroundBuilder BorderStyle=Outer}" />
```

#### Don't you like MaterialContentView? Feel free to attach it anywhere by
```xml
<StackLayout 
    controls:BackgroundEffect.Background="{controls:BackgroundBuilder BorderStyle=Outer}" />
```

#### Do you need a very complex background? You can achieve it by using only one view!

### What was hard

Mixing Gradient on border with gradient on background was a really painful challenge. On Android I had to deal with custom Drawables and CALayers on iOS. Also, on Android, making elevation and clipping work for different radius on each corner was a trial and error pain. An outline provider has been made in order to support cornerRadii. GradientStrokeDrawable extends GradientDrawable and draws a custom paint(to replicate the border) into the shape of the view. On iOS, the GradientStrokeLayer also extends CAGradientLayer and it has a ShadowLayer and another one CAGradientLayer for the border. Clipping on iOS is done through a CAShapeLayer.

### Break some limitations 

### Android

ViewOutlineProvider only supports clipping that can be represented as a rectangle, circle, or round rect. See more at [documentation](https://developer.android.com/reference/android/graphics/Outline.html#canClip()).
Setting ```SetConvexPath()``` to the outline has no effect on clipping the view, since the ```CanClip()``` method returns false.

#### Clipping SubViews

Clipping subviews (e.g. clip an image inside a stacklayout with rounded corners) is only supported by ```MaterialContentView``` and it clips its subviews by clipping the Canvas on ```DispatchDraw()```. Since ```DispatchDraw``` must be overwritten in order to make clip to a path happen, clipping on xamarin.forms views is not supported. If there is any other way, i'm glad to accept it as a PR.

#### Clipping Drawable

On Android, the GradientStrokeDrawable is clipping the outer stroke by ```Canvas.ClipPath()```, and sets the stroke the double value. Although, this seems to work on API <= 27, on API 28, outer stroke was not clipped by the ```ClipPath()``` method. To resolve this issue, instead of drawing directly to the canvas, it draws to a bitmap, then clips the canvas by ```ClipPath()``` and then ```DrawBitmap()``` is called.

### iOS

#### Clipping SubViews

There is a method called ```InvalidateClipToBounds()``` inside [MaterialVisualElementTracker](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.iOS/Renderers/MaterialVisualElementTracker.cs). This method manually clip each subview of the rendered parent view, using as mask the BezierPath that is calculated by the CornerRadius.

#### CAGradientLayer vs GradientStrokeLayer

In the old version, when using sublayers to get the job done for the gradient and the border gradient, things went bad and weird flickerings happened.  
The new version only extends ```CALayer``` and draws manually everything on ```DrawInContext()``` method. First of all, it clips the path using the BezierPath provided by the CornerRadius, as explained above, and then draws the Gradient or Color and the Border Gradient or Border Color. Finally, it has only 1 sublayer, the MDCShadowLayer.   

By using the ```GradientStrokeLayer``` every view's background is identical to Android! 

### What is used for the Background

BackgroundKit Provides a consistent way for adding Background to your views.  
What is supported out of the box: 

| Background | Android | iOS |
| ------ | ------ | ------ |
| Elevation | Android.Views.View.Elevation | MDCShadowLayer
| Ripple | Android.Graphics.Drawable.RippleDrawable | MDCInkViewController |
| CornerRadius | [GradientStrokeDrawable](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.Android/Renderers/GradientStrokeDrawable.cs) | [GradientStrokeLayer](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.iOS/Renderers/GradientStrokeLayer.cs) |
| Gradients | [GradientStrokeDrawable](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.Android/Renderers/GradientStrokeDrawable.cs) | [GradientStrokeLayer](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.iOS/Renderers/GradientStrokeLayer.cs) |
| Border | [GradientStrokeDrawable](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.Android/Renderers/GradientStrokeDrawable.cs) | [GradientStrokeLayer](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.iOS/Renderers/GradientStrokeLayer.cs) |
| Gradient Border | [GradientStrokeDrawable](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.Android/Renderers/GradientStrokeDrawable.cs) | [GradientStrokeLayer](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.iOS/Renderers/GradientStrokeLayer.cs) |

### Native Views (FastRenderers Pattern on Android)

| Parent Views | Android | iOS |
| ------ | ------ | ------ |
| MaterialContentView | Android.Views.View | UIView |
| MaterialCard | CardView* | MaterialComponents.Card |  

* I used MaterialCardView but there is [bug](https://github.com/material-components/material-components-android/commit/09673a5de798241860e5cecd051e0caa19397ca0) with the RippleEffect and it is not in the codebase yet.

### Native Background Managers
| Android | iOS |
| ------ | ------ |
| [MaterialVisualElementTracker](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.Android/Renderers/MaterialVisualElementTracker.cs) | [MaterialVisualElementTracker](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/blob/master/src/XamarinBackgroundKit.iOS/Renderers/MaterialVisualElementTracker.cs) |

### What is supported

#### Xamarin.Forms Layouts

| Xamarin.Forms Layouts | Android | iOS |
| ------ | ------ | ------ |
| AbsoluteLayout | Yes | Yes |
| CarouselView | Yes | Yes |
| CollectionView | Yes | Yes |
| ContentView | Yes | Yes |
| FlexLayout | Yes | Yes |
| Frame | No, see more at this [issue](https://github.com/ChasakisD/Xamarin.Forms-BackgroundKit/issues/12) | Yes |
| Grid | Yes | Yes |
| ListView | Yes | Yes |
| RelativeLayout | Yes | Yes |
| ScrollView | Yes | Yes |
| StackLayout | Yes | Yes |

#### Xamarin.Forms Views

| Xamarin.Forms Views | Android | iOS |
| ------ | ------ | ------ |
| ActivityIndicator | Yes | Yes |
| BoxView | Yes | Yes |
| Button | Yes | Yes |
| DatePicker | Yes | Yes |
| Editor | Yes | Yes |
| Entry | Yes | Yes |
| Image | Yes | Yes |
| ImageButton | Yes | Yes |
| Label | Yes | Yes |
| Picker | Yes | Yes |
| ProgressBar | Yes | Yes |
| SearchBar | Yes | Yes |
| Slider | Yes | Yes |
| Stepper | Yes | Yes |
| Switch | Yes | Yes |
| TimePicker | Yes | Yes |
| WebView | Yes | Yes |

#### Custom Views from BackgroundKit

| BackgroundKit View | Android | iOS |
| ------ | ------ | ------ |
| MaterialCard | Yes | Yes |
| MaterialContentView | Yes | Yes |

**PS.: Material Components on Android and iOS are not supporting changing the background, so gradients and gradient borders are not supported when Visual="Material" is used.**

### Ripple Support

| BackgroundKit View | Android | iOS |
| ------ | ------ | ------ |
| MaterialCard | Yes through RippleDrawable | Yes through RippleDrawable |
| MaterialContentView | Yes through MDCInkViewController | Yes through MDCInkViewController |

## <a name="usage"></a>Usage
### API Documentation
#### Background
| Property | Type | Description | Why do I need it? |
| ------ | ------ | ------ | ------ |
| `Elevation` | `double` | The elevation of the Background | It adds shadow to view that depends on the value of elevation |
| `TranslationZ` | `double` | The translation in Z axis of the background (only affects on Android) | In android you need to overlap views without adding shadows. TranslationZ is what you need! |
| `CornerRadius` | `CornerRadius` | The corner radius of the background | Exactly as Thickness. Sets the radius to each corner |
| `GradientBrush` | [LinearGradientBrush](#LinearGradientBrush) | The gradient brush that will be used for the background |
| `BorderColor` | `Color` | The border color of the background |
| `BorderWidth` | `double` | The border width of the background |
| `DashGap` | `double` | Adds dashed border to the background. Dash gap specifies the pixels that the gap of the dash will be |
| `DashWidth` | `double` | Adds dashed border to the background. Dash width specifies the pixels that the width of a filled line will be |
| `BorderGradientBrush` | [LinearGradientBrush](#LinearGradientBrush) | The gradient brush that will be used for the border of the background |
| `IsRippleEnabled` | `bool` | Whether or not the ripple will be enabled |
| `RippleColor` | `Color` | The ripple color of the background |
| `IsClippedToBounds` | `bool` | Whether or not the parent view clips its subviews |

#### LinearGradientBrush
| Property | Type | Description | Why do I need it? |
| ------ | ------ | ------ | ------ |
| `Angle` | `double` | The gradient angle of the background | The -360 to 360 angle of the gradient |
| `GradientType` | `GradientType` | The type of gradient of the background | Linear or Radial. Currently Linear is supported only |
| `Gradients` | `IList<`[GradientStop](#GradientStop)`>` | The gradients that will be used for the background |

#### GradientStop
| Property | Type | Description | Why do I need it? |
| ------ | ------ | ------ | ------ |
| `Offset` | `double` | The offset of the gradient stop | Sets where the gradient stop ends |
| `Color` | `Color` | The color of the gradient stop |  |


#### Material Content View
| Property | Type | Description |
| ------ | ------ | ------ |
| `Background` | `Background` | The background of the view | |
| `IsCircle` | `bool` | By using this property you do not have to set the same WidthRequest and HeightRequest and manually add the CornerRadius. It measures all the child views and then becomes a circle |
| `IsCornerRadiusHalfHeight` | `bool` | The corner radius will ALWAYS become the half of the height |

#### Material Card
Same as MaterialContentView

#### Background Effect
Background Effect has an attached property in order to add background to your views without the need of a custom renderer!
