namespace FileSorter;
public static class Sorter
{
    private const string TempPath = "ExternalSortTemp";

    public static void Sort(string inputFilePath, string outputFilePath, int maxLinesInMemory)
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), TempPath);

        if (!Directory.Exists(tempDirectory))
            Directory.CreateDirectory(tempDirectory);

        var runFiles = CreateInitialRuns(inputFilePath, tempDirectory, maxLinesInMemory);
        MergeRuns(runFiles, outputFilePath);
        Directory.Delete(tempDirectory, true);
    }

    private static List<string> CreateInitialRuns(string inputFilePath,string tempDirectory, int maxLinesInMemory)
    {
        var runFiles = new List<string>();
        var linesBuffer = new List<Line>(maxLinesInMemory);

        using var reader = new StreamReader(inputFilePath);

        while (reader.ReadLine() is { } line)
        {
            var entry = ParseLine(line);
            linesBuffer.Add(entry);

            if (linesBuffer.Count < maxLinesInMemory) continue;

            runFiles.Add(SortAndSaveRun(linesBuffer, tempDirectory));
            linesBuffer.Clear();
        }

        if (linesBuffer.Count <= 0) return runFiles;

        runFiles.Add(SortAndSaveRun(linesBuffer, tempDirectory));
        linesBuffer.Clear();

        return runFiles;
    }

    private static string SortAndSaveRun(List<Line> linesBuffer, string tempDirectory)
    {
        linesBuffer.Sort();

        var runFileName = Path.Combine(tempDirectory, Guid.NewGuid() + ".txt");
        using var writer = new StreamWriter(runFileName);

        foreach (var entry in linesBuffer)
            writer.WriteLine(entry.OriginalLine);

        return runFileName;
    }

    private static Line ParseLine(string line)
    {
        var dotIndex = line.IndexOf(". ", StringComparison.Ordinal);

        if (dotIndex == -1)
            throw new FormatException($"Unrecognised format: {line}");

        var numberPart = line[..dotIndex];
        var stringPart = line[(dotIndex + 2)..];

        if (!int.TryParse(numberPart, out int number))
            throw new FormatException($"Incorrect number in line: {line}");

        return new Line
        {
            Number = number,
            StringPart = stringPart,
            OriginalLine = line
        };
    }

    private static void MergeRuns(List<string> runFiles, string outputFilePath)
    {
        var heap = new SortedSet<LineIndexed>(new LineIndexedComparer());
        var readers = new List<StreamReader>();

        try
        {
            for (var i = 0; i < runFiles.Count; i++)
            {
                var reader = new StreamReader(runFiles[i]);
                readers.Add(reader);

                if (reader.EndOfStream)
                    continue;

                var line = ParseLine(reader.ReadLine());

                heap.Add(new LineIndexed
                {
                    Line = line,
                    ReaderIndex = i
                });
            }

            using var writer = new StreamWriter(outputFilePath);

            while (heap.Count > 0)
            {
                var minEntry = heap.Min;
                heap.Remove(minEntry);

                writer.WriteLine(minEntry.Line.OriginalLine);

                var reader = readers[minEntry.ReaderIndex];
                if (reader.EndOfStream) continue;

                var line = ParseLine(reader.ReadLine());

                heap.Add(new LineIndexed
                {
                    Line = line,
                    ReaderIndex = minEntry.ReaderIndex
                });
            }
        }
        finally
        {
            foreach (var reader in readers)
                reader.Dispose();
        }
    }
}
