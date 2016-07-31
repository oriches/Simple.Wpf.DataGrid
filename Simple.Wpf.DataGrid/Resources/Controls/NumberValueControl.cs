namespace Simple.Wpf.DataGrid.Resources.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Extensions;

    public abstract class NumberValueControl : UserControl
    {
        private readonly Type _type;
        private readonly Func<object, bool> _isGreaterThanZero;

        private bool _mouseDown;
        private DispatcherTimer _scrollWheelTimer;
        private bool _scrollWheel;
        private ScrollViewer _scrollViewer;

        protected NumberValueControl(Type type,  Func<object, bool> isGreaterThanZero)
        {
            _type = type;
            _isGreaterThanZero = isGreaterThanZero;

            DataContextChanged += HandleDataContextChanged;
            Loaded += HandleLoaded;
            Unloaded += HandleUnloaded;
        }

        private void HandleLoaded(object sender, RoutedEventArgs args)
        {
            _scrollViewer = this.FindAncestor<ScrollViewer>();
            if (_scrollViewer != null)
            {
                _scrollViewer.PreviewMouseDown += HandlePreviewMouseDown;
                _scrollViewer.PreviewMouseUp += HandlePreviewMouseUp;

                _scrollWheelTimer = new DispatcherTimer { Interval = Constants.UI.Grids.ScrollingThrottle };
                _scrollWheelTimer.Tick += HandleScrollWheelTimerTick;

                _scrollViewer.PreviewMouseWheel += HandlePreviewMouseWheel;
            }
        }

        private void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_scrollWheelTimer.IsEnabled)
            {
                _scrollWheelTimer.Stop();
            }
            else
            {
                _scrollWheel = true;
            }

            _scrollWheelTimer.Start();
        }

        private void HandleScrollWheelTimerTick(object sender, EventArgs e)
        {
            _scrollWheelTimer.Stop();

            _scrollWheel = false;
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        {
            if (_scrollWheelTimer != null)
            {
                _scrollWheelTimer.Stop();
                _scrollWheelTimer.Tick -= HandleScrollWheelTimerTick;

                _scrollWheelTimer = null;

                _scrollWheel = false;
            }

            if (_scrollViewer != null)
            {
                _scrollViewer.PreviewMouseDown -= HandlePreviewMouseDown;
                _scrollViewer.PreviewMouseUp -= HandlePreviewMouseUp;
                _scrollViewer.PreviewMouseWheel -= HandlePreviewMouseWheel;
            }

            DataContextChanged -= HandleDataContextChanged;
            Loaded -= HandleLoaded;
            Unloaded -= HandleUnloaded;
        }
        
        private void HandlePreviewMouseUp(object sender, MouseButtonEventArgs args)
        {
            _mouseDown = false;
        }

        private void HandlePreviewMouseDown(object sender, MouseButtonEventArgs args)
        {
            _mouseDown = true;
        }

        private void HandleDataContextChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            VisualStateManager.GoToState(this, Constants.UI.Grids.Transitions.Default, true);

            // do nothing when...
            // not visible or
            // no previous value or
            // mouse is down or
            // scrolling or
            // new value type is wrong...

            if (!IsVisible ||
                args.OldValue == null || 
                _mouseDown ||
                _scrollWheel ||
                !(args.NewValue.GetType() == _type))
            {
                return;
            }
            
            VisualStateManager.GoToState(this, _isGreaterThanZero(args.NewValue) ? Constants.UI.Grids.Transitions.NewPositive : Constants.UI.Grids.Transitions.NewNegative, true);
        }
    }
}
