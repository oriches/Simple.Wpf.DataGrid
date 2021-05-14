using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Simple.Wpf.DataGrid.Collections
{
    public sealed class CustomTypeRangeObservableCollection<T> : RangeObservableCollection<T>, ITypedList
        where T : ICustomTypeDescriptor
    {
        private PropertyDescriptorCollection _properties;

        PropertyDescriptorCollection ITypedList.GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return _properties;
        }

        string ITypedList.GetListName(PropertyDescriptor[] listAccessors)
        {
            return null;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Reset)
                _properties = null;
            else if (args.Action == NotifyCollectionChangedAction.Add && Count == 1)
                _properties = this.First()
                    .GetProperties();

            base.OnCollectionChanged(args);
        }

        public override void AddRange(IEnumerable<T> items)
        {
            if (!items.Any()) return;

            var firstItem = items.First();
            var allOtherItems = items.Skip(1)
                .ToArray();

            Add(firstItem);

            base.AddRange(allOtherItems);
        }
    }
}