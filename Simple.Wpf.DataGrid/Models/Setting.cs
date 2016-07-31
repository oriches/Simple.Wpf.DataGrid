namespace Simple.Wpf.DataGrid.Models
{
    using System.Diagnostics;

    [DebuggerDisplay("Name = {Name}, Value = {Value}")]
    public sealed class Setting
    {
        public string Name { get; }

        public object Value { get; }

        public Setting()
        {
        }

        public Setting(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}