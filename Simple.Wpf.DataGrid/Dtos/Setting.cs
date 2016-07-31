namespace Simple.Wpf.DataGrid.Dtos
{
    public sealed class Setting
    {
        public string Name { get; set; }

        public object Value { get; set; }

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
