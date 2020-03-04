namespace Simple.Wpf.DataGrid.Resources.Controls
{
    public partial class DoubleValueControl : NumberValueControl
    {
        public DoubleValueControl() : base(typeof(double), x => (double) x > 0)
        {
            InitializeComponent();
        }
    }
}