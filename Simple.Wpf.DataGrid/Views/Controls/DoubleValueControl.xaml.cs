namespace Simple.Wpf.DataGrid.Views.Controls
{
    public partial class DoubleValueControl : NumberValueControl
    {
        public DoubleValueControl() : base(typeof(double), x => (double) x > 0)
        {
            InitializeComponent();
        }
    }
}