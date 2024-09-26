namespace FileSorter;
public class Line : IComparable<Line>
{
    public int Number { get; set; }
    public string? StringPart { get; set; }
    public string? OriginalLine { get; set; }

    public int CompareTo(Line other)
    {
        var stringComparison = string.Compare(StringPart, other.StringPart, StringComparison.Ordinal);
        return stringComparison != 0 ? stringComparison : Number.CompareTo(other.Number);
    }
}
