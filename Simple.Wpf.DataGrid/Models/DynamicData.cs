using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Simple.Wpf.DataGrid.Models
{
    public sealed class DynamicData : DynamicObject, ICloneable<DynamicData>, IEnumerable
    {
        private readonly Dictionary<string, object> _dictionary;

        public DynamicData() : this(20)
        {
        }

        public DynamicData(int size)
        {
            _dictionary = new Dictionary<string, object>(size);
        }

        public DynamicData(Dictionary<string, object> properties)
        {
            if (properties == null)
            {
                _dictionary = new Dictionary<string, object>();
            }
            else
            {
                _dictionary = properties;

                if (!_dictionary.ContainsKey(Constants.UI.Grids.PredefinedColumns.Id))
                    throw new ArgumentException("DynamicData - Id is not defined in properties collection!");

                Id = _dictionary[Constants.UI.Grids.PredefinedColumns.Id]
                    .ToString();
            }
        }

        public object this[string name]
        {
            get
            {
                if (name == Constants.UI.Grids.PredefinedColumns.Id) return Id;

                return _dictionary.TryGetValue(name, out var value) ? value : null;
            }
            private set => _dictionary[string.Intern(name)] = value;
        }

        public int Count => _dictionary.Count;

        public string Id { get; private set; }

        public IEnumerable<string> Properties => _dictionary.Keys;

        public DynamicData Clone()
        {
            return new DynamicData(_dictionary.ToDictionary(x => x.Key, x => x.Value));
        }

        public IEnumerator GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var name = binder.Name.ToLower();

            return _dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return Update(binder.Name.ToLower(), value);
        }

        public bool Update(string name, object value)
        {
            if (!_dictionary.ContainsKey(name) || _dictionary[name] != value)
            {
                this[name] = value;

                if (name == Constants.UI.Grids.PredefinedColumns.Id)
                    Id = _dictionary[Constants.UI.Grids.PredefinedColumns.Id]
                        .ToString();

                return true;
            }

            return false;
        }

        public void Add(string name, object value)
        {
            Update(name, value);
        }
    }
}