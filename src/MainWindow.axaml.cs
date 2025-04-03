using System;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace PeriscopeSuite;

public partial class MainWindow : Window
{
    private ObservableCollection<string?> Messages { get; } = [];
    private string InGameProcedure { get; set; } = null!;
    private TimeOnly InGameTime { get; set; }
    private FileHandler? FileHandler { get; set; }
    private string Boat1Operator { get; set; } = null!;
    private string Boat2Operator { get; set; } = null!;
    private string Boat3Operator { get; set; } = null!;
    private string Boat4Operator { get; set; } = null!;
    private string Boat1Number { get; set; } = null!;
    private string Boat2Number { get; set; } = null!;
    private string Boat3Number { get; set; } = null!;
    private string Boat4Number { get; set; } = null!;
    private Color Boat1Colour { get; set; } = Color.Green;
    private Color Boat2Colour { get; set; } = Color.Red;
    private Color Boat3Colour { get; set; } = Color.Blue;
    private Color Boat4Colour { get; set; } = Color.Purple;
    private DispatcherTimer? _timer;

    public MainWindow()
    {
        InitializeComponent();
        GetDefaultColours();
        Loaded += OnMainWindowLoaded;
    }

    private void GetDefaultColours()
    {
        Boat1ColourPicker.Text = ColorTranslator.ToHtml(Boat1Colour);
        Boat2ColourPicker.Text = ColorTranslator.ToHtml(Boat2Colour);
        Boat3ColourPicker.Text = ColorTranslator.ToHtml(Boat3Colour);
        Boat4ColourPicker.Text = ColorTranslator.ToHtml(Boat4Colour);
    }

    private async void OnMainWindowLoaded(object? sender, EventArgs e)
    {
        try
        {
            StartupWindow startupWindow = new();
            startupWindow.DataSelected += OnDataSelected;
            startupWindow.TimeSelected += OnTimeSelected;
    
            // Wait for the dialog to close before continuing
            await startupWindow.ShowDialog(this);
    
            // This code will only run after StartupWindow is closed
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            InGameTimeDisplay.Text = InGameTime.ToString();
            _timer.Tick += OnTimerTick;
            _timer.Start();
        }
        catch (Exception error)
        {
            throw new Exception("Error loading the main window: " + error.Message);
        }
    }

    private void OnTimeSelected(object? sender, TimeOnly time)
    {
        InGameTime = time;
        InGameTimeDisplay.Text = InGameTime.ToString();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        InGameTime = InGameTime.AddMinutes(1);
        InGameTimeDisplay.Text = InGameTime.ToString();
        Debug.WriteLine($"Current Time: {InGameTime}");
    }

    private void OnDataSelected(object? sender, StartupData data)
    {
        InGameTime = data.Time;
        Boat1Number = data.Boat1Number;
        Boat2Number = data.Boat2Number;
        Boat3Number = data.Boat3Number;
        Boat4Number = data.Boat4Number;
        Boat1Operator = data.Boat1Operator;
        Boat2Operator = data.Boat2Operator;
        Boat3Operator = data.Boat3Operator;
        Boat4Operator = data.Boat4Operator;
        InGameProcedure = data.Procedure;
    
        FileHandler = new FileHandler(InGameTime, InGameProcedure,
            Boat1Colour, Boat2Colour, Boat3Colour, Boat4Colour,
            Boat1Number, Boat2Number, Boat3Number, Boat4Number,
            Boat1Operator, Boat2Operator, Boat3Operator, Boat4Operator);
    }

    private void ChatInput_OnKeyDown(object? sender, KeyEventArgs e)
    {
        ChatLog.ItemsSource = Messages;

        if (e.Key != Key.Enter) return;
        
        var message = ChatInput.Text;
        if (string.IsNullOrWhiteSpace(message)) return;
            
        Messages.Add(message);
        ChatLog.ItemsSource = Messages;
        FileHandler?.WriteToLog(message, InGameTime);
        Debug.WriteLine("Message sent: " + message);
        ChatInput.Clear();
    }

    private void OpenFile_OnClick(object? sender, RoutedEventArgs e)
    {
        using var fileopener = new Process();

        fileopener.StartInfo.FileName = "explorer";
        fileopener.StartInfo.Arguments = "\"" + FileHandler?.MarkdownFile + "\"";
        fileopener.Start();
    }

    private void ColourPickers_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            Boat1Colour = ColorTranslator.FromHtml(Boat1ColourPicker.Text!);
            if (FileHandler != null)
            {
                FileHandler.Boat1Colour = Boat1Colour;
                Boat2Colour = ColorTranslator.FromHtml(Boat2ColourPicker.Text!);
                FileHandler.Boat2Colour = Boat2Colour;
                Boat3Colour = ColorTranslator.FromHtml(Boat3ColourPicker.Text!);
                FileHandler.Boat3Colour = Boat3Colour;
                Boat4Colour = ColorTranslator.FromHtml(Boat4ColourPicker.Text!);
                FileHandler.Boat4Colour = Boat4Colour;
            }
        }
    }

    private void OpenExplorer_OnClick(object? sender, RoutedEventArgs e)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        Process.Start("explorer.exe", currentDirectory);
    }

    private void ChangeTime_OnClick(object? sender, RoutedEventArgs e)
    {
        StartupWindow startupWindow = new();
        startupWindow.TimeSelected += OnTimeSelected;
        startupWindow.ShowDialog(this);
    }

    private void FinishMission_OnClick(object? sender, RoutedEventArgs e)
    {
        FileHandler?.ConvertToPdf();
    }

    public async void GetWeatherAsync(object? sender, RoutedEventArgs routedEventArgs)
    {
        string lat = LatInput.Text ?? throw new InvalidOperationException("Latitude is null");
        string lon = LongInput.Text ?? throw new InvalidOperationException("Longitude is null");
        string apiKey = ApiKeyInput.Text ?? throw new InvalidOperationException("API Key is null");
        
        string units = UnitInput.SelectedIndex == 0 ? "metric" : "imperial"; // 1 for metric, 0 for imperial
        
        using var client = new HttpClient();
        try
        {
            var response = await client.GetAsync(
                $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units={units}");

            if (!response.IsSuccessStatusCode)
            {
                Debug.WriteLine("ERROR: " + response.StatusCode);
                ReportOutput.Text = "ERROR: " + response.StatusCode;
                return;
            }

            var content = await response.Content.ReadAsStringAsync();
            using JsonDocument doc = JsonDocument.Parse(content);
            JsonElement root = doc.RootElement;

            // Parse weather condition
            var weatherArray = root.GetProperty("weather");
            if (weatherArray.GetArrayLength() == 0)
            {
                Console.WriteLine("No weather data available");
                return;
            }
            string weatherMain = weatherArray[0].GetProperty("main").GetString() ?? throw new InvalidOperationException();
            string weatherFormatted = FormatWeatherCondition(weatherMain);

            // Parse main data
            var main = root.GetProperty("main");
            var city = root.GetProperty("name").GetString();
            double temp = main.GetProperty("temp").GetDouble();
            int humidity = main.GetProperty("humidity").GetInt32();
            double pressure = main.GetProperty("pressure").GetDouble();

            // Parse wind data
            var wind = root.GetProperty("wind");
            double windSpeed = wind.GetProperty("speed").GetDouble();
            double windDeg = wind.TryGetProperty("deg", out var deg) ? deg.GetDouble() : 0;
            string windDirection = GetWindDirection(windDeg);

            // Parse sunrise/sunset
            var sys = root.GetProperty("sys");
            long sunrise = sys.GetProperty("sunrise").GetInt64();
            long sunset = sys.GetProperty("sunset").GetInt64();
            double dayLengthHours = (sunset - sunrise) / 3600.0;

            // Format output
            ReportOutput.Text = $"WX {city?.ToUpper()} = {weatherFormatted} = " +
                              $"TEMP {temp:F1} = " +
                              $"HYGRO {humidity} PCT = " +
                              $"BARO {pressure:F1} HPA = " +
                              $"WIND {windSpeed:F1} FRM {windDirection} = " +
                              $"LENGTH OF DAY {Math.Round(dayLengthHours)} H";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    
    private static string FormatWeatherCondition(string condition)
    {
        var conditionMap = new System.Collections.Generic.Dictionary<string, string>
        {
            { "Clear", "CLEAR SKIES" },
            { "Clouds", "OVERCAST" },
            { "Rain", "RAIN" },
            { "Drizzle", "DRIZZLE" },
            { "Thunderstorm", "THUNDERSTORM" },
            { "Snow", "SNOW" },
            { "Mist", "MIST" },
            { "Smoke", "SMOKE" },
            { "Haze", "HAZE" },
            { "Dust", "DUST" },
            { "Fog", "FOG" },
            { "Sand", "SAND" },
            { "Ash", "ASH" },
            { "Squall", "SQUALL" },
            { "Tornado", "TORNADO" }
        };

        return conditionMap.TryGetValue(condition, out var formatted) 
            ? formatted 
            : condition.ToUpper();
    }

    private static string GetWindDirection(double degrees)
    {
        string[] directions = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", 
            "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
        int index = (int)((degrees + 11.25) / 22.5) % 16;
        return directions[index];
    }
}