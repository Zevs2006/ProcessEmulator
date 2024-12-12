using System.Collections.ObjectModel;
using System.Text.Json;

namespace ProcessEmulator;

public partial class MainPage : ContentPage
{
    private ObservableCollection<Process> processes = new();
    private int nextId = 1;

    public MainPage()
    {
        InitializeComponent();
        ProcessList.ItemsSource = processes;
        LoadProcesses();
        StartProcessTimer();
    }

    private void OnCreateProcessClicked(object sender, EventArgs e)
    {
        var random = new Random();
        var process = new Process
        {
            Id = nextId++,
            Name = $"Process {nextId}",
            Priority = "Средний",
            State = "Выполняется",
            Memory = $"{random.Next(50, 500)} MB",
            TimeLeft = random.Next(5, 20)
        };
        processes.Add(process);
    }

    private void OnPauseProcessClicked(object sender, EventArgs e)
    {
        var activeProcess = processes.FirstOrDefault(p => p.State == "Выполняется");
        if (activeProcess != null)
        {
            activeProcess.State = "Приостановлен";
        }
    }

    private void OnStopProcessClicked(object sender, EventArgs e)
    {
        var activeProcess = processes.FirstOrDefault(p => p.State == "Выполняется");
        if (activeProcess != null)
        {
            activeProcess.State = "Завершён";
        }
    }

    private void OnChangePriorityClicked(object sender, EventArgs e)
    {
        var random = new Random();
        var process = processes.FirstOrDefault();
        if (process != null)
        {
            var priorities = new[] { "Высокий", "Средний", "Низкий" };
            process.Priority = priorities[random.Next(priorities.Length)];
        }
    }

    private void StartProcessTimer()
    {
        Device.StartTimer(TimeSpan.FromSeconds(1), () =>
        {
            foreach (var process in processes.Where(p => p.State == "Выполняется"))
            {
                process.TimeLeft--;
                if (process.TimeLeft <= 0)
                {
                    process.State = "Завершён";
                }
            }
            return true;
        });
    }

    private void LoadProcesses()
    {
        if (File.Exists("processes.json"))
        {
            var json = File.ReadAllText("processes.json");
            processes = JsonSerializer.Deserialize<ObservableCollection<Process>>(json) ?? new ObservableCollection<Process>();
            ProcessList.ItemsSource = processes;
        }
    }

    private void SaveProcesses()
    {
        var json = JsonSerializer.Serialize(processes);
        File.WriteAllText("processes.json", json);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SaveProcesses();
    }
}

public class Process : BindableObject
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Priority { get; set; }
    public string State { get; set; }
    public string Memory { get; set; }

    private int timeLeft;
    public int TimeLeft
    {
        get => timeLeft;
        set
        {
            timeLeft = value;
            OnPropertyChanged();
        }
    }
}
