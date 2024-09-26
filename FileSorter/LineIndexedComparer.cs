namespace FileSorter;
public class LineIndexedComparer : IComparer<LineIndexed>
{
    public int Compare(LineIndexed x, LineIndexed y)
    {
        var comparison = x.Line.CompareTo(y.Line);
        return comparison != 0 ? comparison : x.ReaderIndex.CompareTo(y.ReaderIndex);
    }
}
