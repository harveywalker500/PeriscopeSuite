using Avalonia.Controls;
using System;
using Avalonia.Interactivity;

namespace PeriscopeSuite
{
    public partial class StartupWindow : Window
    {
        public event EventHandler<TimeOnly>? TimeSelected;
        public event EventHandler<StartupData>? DataSelected;

        public StartupWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object? sender, RoutedEventArgs e)
        {
            var time = GetSelectedTime();
            
            var data = new StartupData
            {
                Time = time,
                Boat1Number = Boat1Number.Text ?? string.Empty,
                Boat2Number = Boat2Number.Text ?? string.Empty,
                Boat3Number = Boat3Number.Text ?? string.Empty,
                Boat4Number = Boat4Number.Text ?? string.Empty,
                Boat1Operator = Boat1Operator.Text ?? string.Empty,
                Boat2Operator = Boat2Operator.Text ?? string.Empty,
                Boat3Operator = Boat3Operator.Text ?? string.Empty,
                Boat4Operator = Boat4Operator.Text ?? string.Empty,
                Procedure = ((ComboBoxItem)Procedure.SelectedItem!)?.Content?.ToString() ?? string.Empty
            };

            TimeSelected?.Invoke(this, time);
            DataSelected?.Invoke(this, data);
            Close();
        }

        private TimeOnly GetSelectedTime()
        {
            if (InGameTimePicker.SelectedTime != null)
            {
                int hour = InGameTimePicker.SelectedTime.Value.Hours;
                int minute = InGameTimePicker.SelectedTime.Value.Minutes;
                return new TimeOnly(hour, minute);
            }

            throw new Exception("ERROR: No time selected");
        }
    }

    public class StartupData
    {
        public TimeOnly Time { get; set; }
        public string Boat1Number { get; set; } = string.Empty;
        public string Boat2Number { get; set; } = string.Empty;
        public string Boat3Number { get; set; } = string.Empty;
        public string Boat4Number { get; set; } = string.Empty;
        public string Boat1Operator { get; set; } = string.Empty;
        public string Boat2Operator { get; set; } = string.Empty;
        public string Boat3Operator { get; set; } = string.Empty;
        public string Boat4Operator { get; set; } = string.Empty;
        public string Procedure { get; set; } = string.Empty;
    }
}