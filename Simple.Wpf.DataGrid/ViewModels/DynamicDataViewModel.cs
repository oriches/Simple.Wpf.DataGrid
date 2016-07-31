namespace Simple.Wpf.DataGrid.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive.Linq;
    using Extensions;
    using Helpers;
    using Models;
    using Services;

    [DebuggerDisplay("Id = {Id}, CreatedOn = {CreatedOn}, ModifiedOn = {ModifiedOn}")]
    public sealed class DynamicDataViewModel : BaseViewModel, ICustomTypeDescriptor, ITransientViewModel
    {
        public static PropertyDescriptorCollection PropertyDescriptors;

        private static readonly string[] AuditProperties = { Constants.UI.Grids.PredefinedColumns.CreatedOn, Constants.UI.Grids.PredefinedColumns.ModifiedOn };

        private readonly IDictionary<string, string> _valuesAsStrings;
        private readonly DynamicData _data;

        private readonly IDateTimeService _dateTimeService;
        private readonly object _createdOn;

        private object _modifiedOn;
        
        public DynamicDataViewModel(DynamicData data, IDateTimeService dateTimeService) 
        {
            _data = data;
            _dateTimeService = dateTimeService;

            _valuesAsStrings = new Dictionary<string, string>();

            var auditDate = _dateTimeService.Now
                .DateTime
                .Truncate(TimeSpan.TicksPerSecond);
            
            _createdOn = auditDate;

            _modifiedOn = auditDate;

#if DEBUG
            SuppressDebugWriteline = true;
#endif
        }
        
        public object this[string name] => GetValue(name);

        public string Id => _data.Id;

        public DateTime CreatedOn => (DateTime)_createdOn;

        public DateTime ModifiedOn => (DateTime)_modifiedOn;

        public static void Reset()
        {
            PropertyDescriptors = null;
        }

        public object GetValue(string name)
        {
            if (name == Constants.UI.Grids.PredefinedColumns.Id)
            {
                return Id;
            }

            if (name == Constants.UI.Grids.PredefinedColumns.CreatedOn)
            {
                return _createdOn;
            }

            if (name == Constants.UI.Grids.PredefinedColumns.ModifiedOn)
            {
                return _modifiedOn;
            }

            return _data[name];
        }

        public string GetValueAsString(string name)
        {
            string value;
            if (!_valuesAsStrings.TryGetValue(name, out value))
            {
                value = GetValue(name).ToString().ToLower();
                _valuesAsStrings.Add(name, value);
            }

            return value;
        }

        public void Update(string name, object value)
        {
            if (_data.Update(name, value))
            {
                _valuesAsStrings.Remove(name);

                _modifiedOn = _dateTimeService.Now
                    .DateTime
                    .Truncate(TimeSpan.TicksPerSecond);
                
                OnPropertyChanged(name);
                OnPropertyChanged(Constants.UI.Grids.PredefinedColumns.ModifiedOn);
            }
        }
        
        public void ProcessUpdate(DynamicData data)
        {
            var properties = data.Properties.ToArray();

            using (SuspendNotifications())
            {
                // ReSharper disable once ForCanBeConvertedToForeach
                for (var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    Update(property, data[property]);
                }
            }
        }

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            throw new NotImplementedException();
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }
        
        public PropertyDescriptorCollection GetProperties()
        {
            if (PropertyDescriptors != null)
            {
                return PropertyDescriptors;
            }

            var descriptors = Observable.Return(this)
                .Select(x => x.BuildDescriptors(x._data.Properties.Concat(AuditProperties).ToArray()))
                .Wait()
                .ToArray();

            PropertyDescriptors = new PropertyDescriptorCollection(descriptors);
            return PropertyDescriptors;
        }
        
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private List<PropertyDescriptor> BuildDescriptors(string[] properties)
        {
            var descriptors = new List<PropertyDescriptor>();

            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (var i = 0; i < properties.Length; i++)
            {
                var property = properties[i];

                var descriptor = new DynamicDataViewModelPropertyDescriptor(property,
                    ColumnHelper.DisplayName(property),
                    GetValue(property).GetType());

                descriptors.Add(descriptor);
            }

            return descriptors;
        }
    }
}