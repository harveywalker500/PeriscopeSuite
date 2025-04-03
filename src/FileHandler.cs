using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using Markdown2Pdf;

namespace PeriscopeSuite;

public class FileHandler
{
    public string MarkdownFile { get; set; }
    private string TextFile { get; set; }
    private TimeOnly InGameTime { get; set; }
    private string InGameProcedure { get; set; }
    private string Boat1Operator { get; set; }
    private string Boat2Operator { get; set; }
    private string Boat3Operator { get; set; }
    private string Boat4Operator { get; set; }

    private string Boat1Number { get; set; }
    private string Boat2Number { get; set; }
    private string Boat3Number { get; set; }
    private string Boat4Number { get; set; }
    
    public Color Boat1Colour { get; set; }
    public Color Boat2Colour { get; set; }
    public Color Boat3Colour { get; set; }
    public Color Boat4Colour { get; set; }
    
    public FileHandler(TimeOnly inGameTime, string inGameProcedure, Color boat1Colour, Color boat2Colour, Color boat3Colour, Color boat4Colour,
        string boat1Number, string boat2Number, string boat3Number, string boat4Number,
        string boat1Operator, string boat2Operator, string boat3Operator, string boat4Operator)
    {
        MarkdownFile = CreateMarkdownFile();
        TextFile = CreateTextFile();
        InGameTime = inGameTime;
        InGameProcedure = inGameProcedure;

        Boat1Colour = boat1Colour;
        Boat2Colour = boat2Colour;
        Boat3Colour = boat3Colour;
        Boat4Colour = boat4Colour;

        Boat1Number = boat1Number;
        Boat2Number = boat2Number;
        Boat3Number = boat3Number;
        Boat4Number = boat4Number;

        Boat1Operator = boat1Operator;
        Boat2Operator = boat2Operator;
        Boat3Operator = boat3Operator;
        Boat4Operator = boat4Operator;

        CreateHeader();
    }

    private void CreateHeader()
    {
        File.AppendAllText(MarkdownFile, 
            $"""
             # WOLFPACK RADIO LOG {Environment.NewLine} 
             ## {DateTime.Now:yyyy-MM-dd}                            
             ## {InGameTime}
             ## Procedure: {InGameProcedure}
                            
             ## Operators:
             - <span style="color:rgba({Boat1Colour.R}, {Boat1Colour.G},{Boat1Colour.B},{Boat1Colour.A})">{Boat1Number} - {Boat1Operator}
             - <span style="color:rgba({Boat2Colour.R}, {Boat2Colour.G},{Boat2Colour.B},{Boat2Colour.A})">{Boat2Number} - {Boat2Operator}
             - <span style="color:rgba({Boat3Colour.R}, {Boat3Colour.G},{Boat3Colour.B},{Boat3Colour.A})">{Boat3Number} - {Boat3Operator}
             - <span style="color:rgba({Boat4Colour.R}, {Boat4Colour.G},{Boat4Colour.B},{Boat4Colour.A})">{Boat4Number} - {Boat4Operator}
                            
             ## NOTES:
             - All "mis-clicks" are omitted, and messages are cleaned to provide overall meaning.
             - Messages with the prefix (CHAT) were sent over steam.
                - This does not count if steam chat is used by default.
     
             ## Log
            """ + Environment.NewLine);
    }

    private string CreateMarkdownFile()
    {
        var fileName = $"{DateTime.Now:yyyy-MM-dd} Radio Log.md";
        File.Create(fileName).Dispose();
        return fileName;
    }

    private string CreateTextFile()
    {
        var fileName = $"{DateTime.Now:yyyy-MM-dd} Radio Log.txt";
        File.Create(fileName).Dispose();
        return fileName;
    }

    public void WriteToLog(string message, TimeOnly inGameTime)
    {
        InGameTime = inGameTime;
        Debug.WriteLine(InGameTime.ToString());
        message = message.Trim();

        // Log the boat numbers and the message
        Debug.WriteLine($"Boat1Number: {Boat1Number}");
        Debug.WriteLine($"Boat2Number: {Boat2Number}");
        Debug.WriteLine($"Boat3Number: {Boat3Number}");
        Debug.WriteLine($"Boat4Number: {Boat4Number}");
        Debug.WriteLine($"Message: {message}");

        var pattern = $@"DE ({Regex.Escape(Boat1Number)}|{Regex.Escape(Boat2Number)}|{Regex.Escape(Boat3Number)}|{Regex.Escape(Boat4Number)})";
        var regex = new Regex(pattern);

        if (regex.IsMatch(message))
        {
            var match = regex.Match(message).Groups[1].Value;
            Color color = match switch
            {
                var m when m == Boat1Number => Boat1Colour,
                var m when m == Boat2Number => Boat2Colour,
                var m when m == Boat3Number => Boat3Colour,
                var m when m == Boat4Number => Boat4Colour,
                _ => Color.Black
            };

            File.AppendAllLines(MarkdownFile,
            [$"<span style=\"color:rgba({color.R},{color.G},{color.B},{color.A})\">{InGameTime}: {message}</span>" +
             Environment.NewLine]);
        }
        else
        {
            File.AppendAllLines(MarkdownFile, [$"{InGameTime}: [NOTE] {message}" + Environment.NewLine]);
        }
        File.AppendAllText(TextFile, $"{InGameTime}: {message}\n" + Environment.NewLine);
    }

    public void ConvertToPdf()
    {
        var converter = new Markdown2PdfConverter();
        converter.Convert(MarkdownFile); 
    }
}
