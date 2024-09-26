using FileSorter;

const string inputFilePath = "HERE WRITE YOUR NOT SORTED FILE DIRECTORY";
const string outputFilePath = "HERE WRITE YOUR SORTED FILE OUTPUT DIRECTORY";

//MaxLinesInMemory can be adjusted based on available RAM memory
const int maxLinesInMemory = 50000;

var start = DateTime.Now;
Sorter.Sort(inputFilePath, outputFilePath, maxLinesInMemory);
var sortingTime = DateTime.Now - start;
Console.WriteLine($"Sorting took {sortingTime}");
