using Avalonia.Controls;
using System;

namespace PeriscopeSuite;

public partial class StartupWindow : Window
{
    public event EventHandler<TimeOnly>? TimeSelected;
    public StartupWindow()
    {
        InitializeComponent();
    }

    private void StartButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var selectedTime = GetSelectedTime();
        TimeSelected?.Invoke(this, selectedTime);
        Close();
    }

    private TimeOnly GetSelectedTime()
    {
        var timePicker = this.FindControl<TimePicker>("TimePicker");
        if (timePicker is not { SelectedTime: not null }) throw new InvalidOperationException("No time selected");
        
        var selectedTime = TimeOnly.FromTimeSpan(timePicker.SelectedTime.Value);
        Console.WriteLine($"Selected Time: {selectedTime}");
        return selectedTime;
    }
}
