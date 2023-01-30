namespace TopCom
{
    public interface ICell<T>
    {
        int Index { get; set; }
        T Status { get; set; }
    }
}
