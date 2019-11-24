using Simple.Wpf.DataGrid.ViewModels;

namespace Simple.Wpf.DataGrid.Tests.ViewModels
{
    public sealed class TestViewModel : BaseViewModel
    {
        private int _integerProperty;
        private string _stringProperty;

        public string StringProperty
        {
            get => _stringProperty;
            set { SetPropertyAndNotify(ref _stringProperty, value, () => StringProperty); }
        }

        public int IntegerProperty
        {
            get => _integerProperty;
            set { SetPropertyAndNotify(ref _integerProperty, value, () => IntegerProperty); }
        }
    }
}