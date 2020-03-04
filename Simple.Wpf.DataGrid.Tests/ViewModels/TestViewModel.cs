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
            set => SetProperty(ref _stringProperty, value);
        }

        public int IntegerProperty
        {
            get => _integerProperty;
            set => SetProperty(ref _integerProperty, value);
        }
    }
}