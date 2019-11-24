using System.Diagnostics;

namespace Simple.Wpf.DataGrid.Models
{
    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    public sealed class Setting
    {
        public Setting()
        {
        }

        public Setting(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }

        public object Value { get; }
    }
}