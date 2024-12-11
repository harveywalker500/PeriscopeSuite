using System;
using System.Drawing;
using System.IO;
using Markdown2Pdf;

namespace PeriscopeSuite;

public class FileHandler
{
    public string MarkdownFile { get; set; }
    public string TextFile { get; set; }
    public TimeOnly InGameTime { get; set; }
    public Color U96Colour { get; set; }
    public Color U552Colour { get; set; }
    public Color U564Colour { get; set; }
    public Color U307Colour { get; set; }
    
    public FileHandler(TimeOnly inGameTime, Color u96Colour, Color u552Colour, Color u564Colour, Color u307Colour)
    {
        CreateMarkdownFile();
        CreateTextFile();
        InGameTime = inGameTime;
        U96Colour = u96Colour;
        U552Colour = u552Colour;
        U564Colour = u564Colour;
        U307Colour = u307Colour;
    }

    public void CreateMarkdownFile()
    {
        string fileName = $"{DateTime.Now:yyyy-MM-dd} Radio Log.md";
        File.Create(fileName).Dispose();
        MarkdownFile = fileName;
    }

    public void CreateTextFile()
    {
        string fileName = $"{DateTime.Now:yyyy-MM-dd} Radio Log.txt";
        File.Create(fileName).Dispose();
        TextFile = fileName;
    }

    public void WriteToLog(string message)
    {
        Console.WriteLine(InGameTime.ToString());
        if (message.Contains("DE 96") || message.Contains("DE SW"))
        {
            File.AppendAllLines(MarkdownFile,
            [$"<span style=\"color:rgba({U96Colour.R},{U96Colour.G},{U96Colour.B},{U96Colour.A})\">{InGameTime}: {message}</span>" +
             Environment.NewLine]);
        }
        else if (message.Contains("DE 52") || message.Contains("DE 552") || message.Contains("DE DV"))
        {
            File.AppendAllLines(MarkdownFile,
            [$"<span style=\"color:rgba({U552Colour.R},{U552Colour.G},{U552Colour.B},{U552Colour.A})\">{InGameTime}: {message}</span>" +
             Environment.NewLine]);
        }
        else if (message.Contains("DE 64") || message.Contains("DE 564") || message.Contains("DE TC"))
        {
            File.AppendAllLines(MarkdownFile,
            [$"<span style=\"color:rgba({U564Colour.R},{U564Colour.G},{U564Colour.B},{U564Colour.A})\">{InGameTime}: {message}</span>" +
             Environment.NewLine]);
        }
        else if (message.Contains("DE 07") || message.Contains("DE 307") || message.Contains("DE 07"))
        {
            File.AppendAllLines(MarkdownFile,
            [
                $"<span style=\"color:rgba({U307Colour.R},{U307Colour.G},{U307Colour.B},{U307Colour.A})\">{InGameTime}: {message}</span>" +
                Environment.NewLine
            ]);
        }
        else
        {
            File.AppendAllLines(MarkdownFile, [$"{InGameTime}: [NOTE] {message}" + Environment.NewLine]);
        }
        File.AppendAllText(TextFile, $"{InGameTime}: {message}\n" + Environment.NewLine);
    }

    public void ConvertToPDF()
    {
        var converter = new Markdown2PdfConverter();
    }
}