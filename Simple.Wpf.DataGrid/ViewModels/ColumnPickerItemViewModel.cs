using System.Diagnostics;

namespace Simple.Wpf.DataGrid.ViewModels
{
    [DebuggerDisplay("Name = {Name}, IsSelected = {IsSelected}")]
    public sealed class ColumnPickerItemViewModel : BaseViewModel, ITransientViewModel
    {
        private bool _isSelected;

        public ColumnPickerItemViewModel(string name, string displayName)
        {
            Name = name;
            DisplayName = displayName;
        }

        public string Name { get; }

        public string DisplayName { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set { SetPropertyAndNotify(ref _isSelected, value, () => IsSelected); }
        }
    }
}