namespace Simple.Wpf.DataGrid.Resources.Behaviors
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Interactivity;
    using System.Windows.Threading;
    using Extensions;

    public sealed class IsGridScrollingBehavior : Behavior<DataGrid>
    {
        private DispatcherTimer _timer;

        public static readonly DependencyProperty IsScrollingProperty = DependencyProperty.Register("IsScrolling",
            typeof(bool),
            typeof(IsGridScrollingBehavior),
            new PropertyMetadata(default(bool)));

        public bool IsScrolling
        {
            get { return (bool) GetValue(IsScrollingProperty); }
            set { SetValue(IsScrollingProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += HandleLoaded;
            AssociatedObject.Unloaded += HandleUnloaded;
        }

        private void HandleUnloaded(object sender, RoutedEventArgs e)
        { 
            _timer.Stop();
            _timer.Tick -= HandleTimerTick;
        }

        private void HandleLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            _timer = new DispatcherTimer { Interval = Constants.UI.Grids.ScrollingThrottle };
            _timer.Tick += HandleTimerTick;

            var scrollViewer = AssociatedObject.FindDescendant<ScrollViewer>();
            if (scrollViewer != null)
            {
                scrollViewer.ScrollChanged += HandleScrollChanged;
            }
        }

        private void HandleScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_timer.IsEnabled)
            {
                _timer.Stop();
            }
            else
            {
                IsScrolling = true;
            }

            _timer.Start();
        }

        private void HandleTimerTick(object sender, EventArgs e)
        {
            _timer.Stop();
            IsScrolling = false;
        }
    }
}