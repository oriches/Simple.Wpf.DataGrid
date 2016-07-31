namespace Simple.Wpf.DataGrid.Models
{
    using System.ComponentModel;

    public enum MemoryUnits
    {
        [Description("bytes")] Bytes = 1,
        [Description("Kb")] Kilo = 1024,
        [Description("Mb")] Mega = 1024*1000,
        [Description("Gb")] Giga = 1024*1000*1000
    }
}