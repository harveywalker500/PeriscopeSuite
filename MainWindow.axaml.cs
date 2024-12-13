using System;
using Avalonia.Controls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;

namespace PeriscopeSuite;

public partial class MainWindow : Window
{
    private ObservableCollection<string?> Messages { get; } = [];
    private TimeOnly InGameTime { get; set; }
    private FileHandler FileHandler { get; set; }
    private Color U96Colour { get; set; } = Color.Green;
    private Color U552Colour { get; set; } = Color.Red;
    private Color U564Colour { get; set; } = Color.Blue;
    private Color U307Colour { get; set; } = Color.Purple;
    private DispatcherTimer _timer;

    public MainWindow()
    {
        Console.WriteLine(U96Colour);
        InitializeComponent();
        GetDefaultColours();
        Loaded += OnMainWindowLoaded;
    }

    private void GetDefaultColours()
    {
        U96ColourPicker.Text = ColorTranslator.ToHtml(U96Colour);
        U552ColourPicker.Text = ColorTranslator.ToHtml(U552Colour);
        U564ColourPicker.Text = ColorTranslator.ToHtml(U564Colour);
        U307ColourPicker.Text = ColorTranslator.ToHtml(U307Colour);
    }

    private async void OnMainWindowLoaded(object? sender, EventArgs e)
    {
        StartupWindow startupWindow = new();
        startupWindow.TimeSelected += OnTimeSelected;
        await startupWindow.ShowDialog(this);

        _timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMinutes(1)
        };
        _timer.Tick += OnTimerTick;
        _timer.Start();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        InGameTime = InGameTime.AddMinutes(1);
        Console.WriteLine($"Current Time: {InGameTime}");
    }

    private void OnTimeSelected(object? sender, TimeOnly e)
    {
        InGameTime = e;
        Console.WriteLine($"Selected Time: {InGameTime}");
        FileHandler = new FileHandler(InGameTime, U96Colour, U552Colour, U564Colour, U307Colour);
    }

    private void ChatInput_OnKeyDown(object? sender, KeyEventArgs e)
    {
        ChatLog.ItemsSource = Messages;

        if (e.Key != Key.Enter) return;
        
        var message = ChatInput.Text;
        if (string.IsNullOrWhiteSpace(message)) return;
            
        Messages.Add(message);
        ChatLog.ItemsSource = Messages;
        FileHandler.WriteToLog(message, InGameTime);
        Console.WriteLine("Message sent: " + message);
        ChatInput.Clear();
    }

    private void OpenFile_OnClick(object? sender, RoutedEventArgs e)
    {
        using var fileopener = new Process();

        fileopener.StartInfo.FileName = "explorer";
        fileopener.StartInfo.Arguments = "\"" + FileHandler.MarkdownFile + "\"";
        fileopener.Start();
    }

    private void ColourPickers_OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            U96Colour = ColorTranslator.FromHtml(U96ColourPicker.Text);
            FileHandler.U96Colour = U96Colour;
            U552Colour = ColorTranslator.FromHtml(U552ColourPicker.Text);
            FileHandler.U552Colour = U552Colour;
            U564Colour = ColorTranslator.FromHtml(U564ColourPicker.Text);
            FileHandler.U564Colour = U564Colour;
            U307Colour = ColorTranslator.FromHtml(U307ColourPicker.Text);
            FileHandler.U307Colour = U307Colour;
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
        FileHandler.ConvertToPdf();
    }
}