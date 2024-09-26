using System.Text;

namespace FileGenerator;
public static class Generator
{
    private static readonly Random _random = new();
    private const string Letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private const string FixedPart = ". ";
    private const int MinNumber = 1000;

    public static void GenerateTestFile(int rowsNumber, string filePath)
    {
        rowsNumber = Math.Max(rowsNumber, MinNumber);

        var duplicateFactor = _random.Next(10, 100);
        var stringBuilder = new StringBuilder();

        for (var i = 0; i < rowsNumber; i++)
        {
            var number = _random.Next(1, 30000);
            var name = new string(Enumerable.Range(0, _random.Next(1, 100))
                .Select(_ => Letters[_random.Next(Letters.Length)]).ToArray());

            stringBuilder.AppendLine(number + FixedPart + name);
            if (i % duplicateFactor != 0) continue;

            //We force to have random number of name duplicates
            number = _random.Next(1, 30000);
            stringBuilder.AppendLine(number + FixedPart + name);
        }

        try
        {
            File.WriteAllText(filePath, stringBuilder.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while saving the file: {ex.Message}");
            throw; 
        }
    }
}
