namespace Simple.Wpf.DataGrid.ViewModels
{
    using System;
    using System.ComponentModel;

    public sealed class DynamicDataViewModelPropertyDescriptor : PropertyDescriptor
    {
        private readonly Type _type;

        public DynamicDataViewModelPropertyDescriptor(string columnName, string displayName, Type type)
            : base(columnName, new Attribute[]
            {
                new DisplayNameAttribute(displayName),
            })
        {
            _type = type;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return ((DynamicDataViewModel)component)[Name];
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType => typeof(DynamicDataViewModel);

        public override bool IsReadOnly => true;

        public override Type PropertyType => _type;
    }
}