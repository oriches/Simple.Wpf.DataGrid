namespace Simple.Wpf.DataGrid.Models
{
    public interface ICloneable<out T>
    {
        T Clone();
    }
}