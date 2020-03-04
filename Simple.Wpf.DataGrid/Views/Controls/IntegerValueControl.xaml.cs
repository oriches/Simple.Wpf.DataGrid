namespace Simple.Wpf.DataGrid.Views.Controls
{
    public partial class IntegerValueControl : NumberValueControl
    {
        public IntegerValueControl() : base(typeof(int), x => (int) x > 0)
        {
            InitializeComponent();
        }
    }
}