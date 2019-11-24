namespace Simple.Wpf.DataGrid.Dtos
{
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

        public string Name { get; set; }

        public object Value { get; set; }
    }
}