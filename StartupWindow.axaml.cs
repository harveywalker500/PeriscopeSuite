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
        this.Close();
    }

    private TimeOnly GetSelectedTime()
    {
        var timePicker = this.FindControl<TimePicker>("TimePicker");
        if (timePicker != null && timePicker.SelectedTime.HasValue)
        {
            TimeOnly selectedTime = TimeOnly.FromTimeSpan(timePicker.SelectedTime.Value);
            Console.WriteLine($"Selected Time: {selectedTime}");
            return selectedTime;
        }
        throw new InvalidOperationException("No time selected");
    }
}
