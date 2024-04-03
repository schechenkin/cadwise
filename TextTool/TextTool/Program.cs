using Cocona;
using System.ComponentModel.DataAnnotations;
using TextTool;


var app = CoconaApp.Create();
app.AddCommand("file", (
    [Argument][FileExists] string source,
    [Argument][FileNotExists] string destination,
    [Argument][Range(0, long.MaxValue)] long wordMinLength,
    [Option] bool removePunctuation = false) =>
    {
        new FileConverter(source, destination, wordMinLength, removePunctuation).Convert();
    })
    .WithDescription("process file");

app.AddCommand("folder", (
    [Argument][FolderExists] string folderPath, 
    [Argument][Range(0, long.MaxValue)] long wordMinLength,
    [Option] bool removePunctuation = false) =>
    {
        var files = Directory.GetFiles(folderPath, "*.txt");
        Parallel.ForEach(files, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, (string file) => {
            new FileConverter(file, Path.Combine(Path.GetDirectoryName(file), $"{Path.GetFileNameWithoutExtension(file)}_out.txt"), wordMinLength, removePunctuation).Convert();
        });
    })
    .WithDescription("process folder");
app.Run();
