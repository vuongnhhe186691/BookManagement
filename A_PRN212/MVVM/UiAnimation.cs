using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace A_PRN212.MVVM
{
    public enum EntranceKind
    {
        None,
        Dialog,
        Surface,
        StaggerItem
    }

    /// <summary>
    /// Attached behavior helpers for lightweight WPF press, hover, and entrance animations.
    /// </summary>
    public static class UiAnimation
    {
        public const double ScalePress = 0.97;
        public const double ScaleChoice = 0.985;
        private const double BlurIconRadius = 2.0;
        private const double BlurSurfaceRadius = 4.0;
        private const double EntranceShiftPx = 12.0;
        private const double CardHoverLiftPx = -2.0;
        private const int StaggerStepMs = 55;

        private static bool ShouldReduceMotion => !SystemParameters.ClientAreaAnimation;

        #region Dependency Properties

        public static readonly DependencyProperty IsPressableProperty =
            DependencyProperty.RegisterAttached(
                "IsPressable",
                typeof(bool),
                typeof(UiAnimation),
                new PropertyMetadata(false, OnIsPressableChanged));

        public static bool GetIsPressable(DependencyObject obj) => (bool)obj.GetValue(IsPressableProperty);
        public static void SetIsPressable(DependencyObject obj, bool value) => obj.SetValue(IsPressableProperty, value);

        public static readonly DependencyProperty PressScaleProperty =
            DependencyProperty.RegisterAttached(
                "PressScale",
                typeof(double),
                typeof(UiAnimation),
                new PropertyMetadata(ScalePress));

        public static double GetPressScale(DependencyObject obj) => (double)obj.GetValue(PressScaleProperty);
        public static void SetPressScale(DependencyObject obj, double value) => obj.SetValue(PressScaleProperty, value);

        public static readonly DependencyProperty IsHoverLiftCardProperty =
            DependencyProperty.RegisterAttached(
                "IsHoverLiftCard",
                typeof(bool),
                typeof(UiAnimation),
                new PropertyMetadata(false, OnIsHoverLiftCardChanged));

        public static bool GetIsHoverLiftCard(DependencyObject obj) => (bool)obj.GetValue(IsHoverLiftCardProperty);
        public static void SetIsHoverLiftCard(DependencyObject obj, bool value) => obj.SetValue(IsHoverLiftCardProperty, value);

        public static readonly DependencyProperty EntranceProperty =
            DependencyProperty.RegisterAttached(
                "Entrance",
                typeof(EntranceKind),
                typeof(UiAnimation),
                new PropertyMetadata(EntranceKind.None, OnEntranceChanged));

        public static EntranceKind GetEntrance(DependencyObject obj) => (EntranceKind)obj.GetValue(EntranceProperty);
        public static void SetEntrance(DependencyObject obj, EntranceKind value) => obj.SetValue(EntranceProperty, value);

        public static readonly DependencyProperty StaggerIndexProperty =
            DependencyProperty.RegisterAttached(
                "StaggerIndex",
                typeof(int),
                typeof(UiAnimation),
                new PropertyMetadata(0));

        public static int GetStaggerIndex(DependencyObject obj) => (int)obj.GetValue(StaggerIndexProperty);
        public static void SetStaggerIndex(DependencyObject obj, int value) => obj.SetValue(StaggerIndexProperty, value);

        private static readonly DependencyProperty IsPressedInternalProperty =
            DependencyProperty.RegisterAttached(
                "IsPressedInternal",
                typeof(bool),
                typeof(UiAnimation),
                new PropertyMetadata(false));

        private static readonly DependencyProperty OriginalEffectProperty =
            DependencyProperty.RegisterAttached(
                "OriginalEffect",
                typeof(Effect),
                typeof(UiAnimation),
                new PropertyMetadata(null));

        #endregion

        #region Transform Helpers

        private static (ScaleTransform Scale, TranslateTransform Translate) EnsureTransforms(UIElement element)
        {
            if (element.RenderTransform is TransformGroup group &&
                group.Children.Count >= 2 &&
                group.Children[0] is ScaleTransform existingScale &&
                group.Children[1] is TranslateTransform existingTranslate)
            {
                return (existingScale, existingTranslate);
            }

            var scale = new ScaleTransform(1.0, 1.0);
            var translate = new TranslateTransform(0.0, 0.0);
            var newGroup = new TransformGroup();
            newGroup.Children.Add(scale);
            newGroup.Children.Add(translate);

            element.RenderTransform = newGroup;
            element.RenderTransformOrigin = new Point(0.5, 0.5);
            return (scale, translate);
        }

        private static KeySpline EaseOutSpline => new(0.23, 1.0, 0.32, 1.0);
        private static KeySpline EaseSoftSpline => new(0.215, 0.61, 0.355, 1.0);

        #endregion

        #region Press Animation

        private static void OnIsPressableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element || DesignerProperties.GetIsInDesignMode(element))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                EnsureTransforms(element);
                element.AddHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPressDown), true);
                element.AddHandler(UIElement.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(OnPressUp), true);
                element.AddHandler(UIElement.MouseLeaveEvent, new MouseEventHandler(OnPressMouseLeave), true);
                element.AddHandler(UIElement.LostMouseCaptureEvent, new MouseEventHandler(OnPressMouseLeave), true);
                element.AddHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPressKeyDown), true);
                element.AddHandler(UIElement.PreviewKeyUpEvent, new KeyEventHandler(OnPressKeyUp), true);
            }
            else
            {
                element.RemoveHandler(UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(OnPressDown));
                element.RemoveHandler(UIElement.PreviewMouseLeftButtonUpEvent, new MouseButtonEventHandler(OnPressUp));
                element.RemoveHandler(UIElement.MouseLeaveEvent, new MouseEventHandler(OnPressMouseLeave));
                element.RemoveHandler(UIElement.LostMouseCaptureEvent, new MouseEventHandler(OnPressMouseLeave));
                element.RemoveHandler(UIElement.PreviewKeyDownEvent, new KeyEventHandler(OnPressKeyDown));
                element.RemoveHandler(UIElement.PreviewKeyUpEvent, new KeyEventHandler(OnPressKeyUp));
            }
        }

        private static void OnPressDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.IsEnabled)
            {
                TriggerPress(element);
            }
        }

        private static void OnPressUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                ReleasePress(element);
            }
        }

        private static void OnPressMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is FrameworkElement element && (bool)element.GetValue(IsPressedInternalProperty))
            {
                ReleasePress(element);
            }
        }

        private static void OnPressKeyDown(object sender, KeyEventArgs e)
        {
            if (!e.IsRepeat && (e.Key == Key.Space || e.Key == Key.Enter) &&
                sender is FrameworkElement element && element.IsEnabled)
            {
                TriggerPress(element);
            }
        }

        private static void OnPressKeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Key == Key.Space || e.Key == Key.Enter) && sender is FrameworkElement element)
            {
                ReleasePress(element);
            }
        }

        private static void TriggerPress(FrameworkElement element)
        {
            if (ShouldReduceMotion)
            {
                return;
            }

            element.SetValue(IsPressedInternalProperty, true);
            double targetScale = GetPressScale(element);
            var (scale, _) = EnsureTransforms(element);

            var anim = new DoubleAnimation
            {
                To = targetScale,
                Duration = TimeSpan.FromMilliseconds(105),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        private static void ReleasePress(FrameworkElement element)
        {
            element.SetValue(IsPressedInternalProperty, false);
            if (ShouldReduceMotion)
            {
                return;
            }

            var (scale, _) = EnsureTransforms(element);

            var anim = new DoubleAnimation
            {
                To = 1.0,
                Duration = TimeSpan.FromMilliseconds(190),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.25 }
            };

            scale.BeginAnimation(ScaleTransform.ScaleXProperty, anim);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, anim);
        }

        #endregion

        #region Card Hover Animation

        private static void OnIsHoverLiftCardChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element || DesignerProperties.GetIsInDesignMode(element))
            {
                return;
            }

            if ((bool)e.NewValue)
            {
                EnsureTransforms(element);
                element.MouseEnter += OnCardMouseEnter;
                element.MouseLeave += OnCardMouseLeave;
            }
            else
            {
                element.MouseEnter -= OnCardMouseEnter;
                element.MouseLeave -= OnCardMouseLeave;
            }
        }

        private static void OnCardMouseEnter(object sender, MouseEventArgs e)
        {
            if (ShouldReduceMotion || sender is not FrameworkElement element || !element.IsEnabled)
            {
                return;
            }

            var (_, translate) = EnsureTransforms(element);
            var anim = new DoubleAnimation
            {
                To = CardHoverLiftPx,
                Duration = TimeSpan.FromMilliseconds(150),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            translate.BeginAnimation(TranslateTransform.YProperty, anim);
        }

        private static void OnCardMouseLeave(object sender, MouseEventArgs e)
        {
            if (ShouldReduceMotion || sender is not FrameworkElement element)
            {
                return;
            }

            var (_, translate) = EnsureTransforms(element);
            var anim = new DoubleAnimation
            {
                To = 0.0,
                Duration = TimeSpan.FromMilliseconds(165),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            translate.BeginAnimation(TranslateTransform.YProperty, anim);
        }

        #endregion

        #region Entrance Animation

        private static void OnEntranceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element || DesignerProperties.GetIsInDesignMode(element))
            {
                return;
            }

            var kind = (EntranceKind)e.NewValue;
            element.Loaded -= OnElementLoadedForEntrance;

            if (kind == EntranceKind.None)
            {
                return;
            }

            if (element.IsLoaded)
            {
                PlayEntrance(element, kind);
            }
            else
            {
                element.Opacity = 0.0;
                element.Loaded += OnElementLoadedForEntrance;
            }
        }

        private static void OnElementLoadedForEntrance(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Loaded -= OnElementLoadedForEntrance;
                PlayEntrance(element, GetEntrance(element));
            }
        }

        private static void PlayEntrance(FrameworkElement element, EntranceKind kind)
        {
            bool reduce = ShouldReduceMotion;
            int staggerIndex = Math.Max(0, GetStaggerIndex(element));
            TimeSpan delay = reduce ? TimeSpan.Zero : TimeSpan.FromMilliseconds(staggerIndex * StaggerStepMs);

            bool isDialog = kind == EntranceKind.Dialog;
            TimeSpan duration = isDialog ? TimeSpan.FromMilliseconds(360) : TimeSpan.FromMilliseconds(300);
            KeySpline curve = isDialog ? EaseSoftSpline : EaseOutSpline;
            double initialBlur = isDialog ? BlurSurfaceRadius : BlurIconRadius;

            // Preserve any existing DropShadowEffect while running the entrance blur
            if (element.Effect != null && element.Effect is not BlurEffect)
            {
                element.SetValue(OriginalEffectProperty, element.Effect);
            }

            element.Opacity = 0.0;
            var opacityAnim = new DoubleAnimationUsingKeyFrames { BeginTime = delay };
            opacityAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            opacityAnim.KeyFrames.Add(new SplineDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(duration), curve));
            element.BeginAnimation(UIElement.OpacityProperty, opacityAnim);

            var blur = new BlurEffect { Radius = initialBlur, RenderingBias = RenderingBias.Performance };
            element.Effect = blur;
            var blurAnim = new DoubleAnimationUsingKeyFrames { BeginTime = delay };
            blurAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame(initialBlur, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            blurAnim.KeyFrames.Add(new SplineDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(duration), curve));
            blurAnim.Completed += (_, _) =>
            {
                element.Effect = (Effect?)element.GetValue(OriginalEffectProperty);
            };
            blur.BeginAnimation(BlurEffect.RadiusProperty, blurAnim);

            if (reduce)
            {
                return;
            }

            var (scale, translate) = EnsureTransforms(element);

            translate.Y = EntranceShiftPx;
            var moveAnim = new DoubleAnimationUsingKeyFrames { BeginTime = delay };
            moveAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame(EntranceShiftPx, KeyTime.FromTimeSpan(TimeSpan.Zero)));
            moveAnim.KeyFrames.Add(new SplineDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(duration + TimeSpan.FromMilliseconds(30)), curve));
            translate.BeginAnimation(TranslateTransform.YProperty, moveAnim);

            if (isDialog)
            {
                const double initialDialogScale = 0.95;
                scale.ScaleX = initialDialogScale;
                scale.ScaleY = initialDialogScale;

                var scaleAnim = new DoubleAnimationUsingKeyFrames { BeginTime = delay };
                scaleAnim.KeyFrames.Add(new DiscreteDoubleKeyFrame(initialDialogScale, KeyTime.FromTimeSpan(TimeSpan.Zero)));
                scaleAnim.KeyFrames.Add(new SplineDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(duration), curve));

                scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnim);
                scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnim);
            }
        }

        #endregion
    }
}
