using System.Text;

namespace TaxCalculator.App;

public class FileWorker : BackgroundService
{
    private readonly string _workersLog = "workersLog.txt";
    private readonly TimeSpan _delay = TimeSpan.FromSeconds(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var path = Path.Combine(Directory.GetCurrentDirectory(), _workersLog);
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var stream = File.Open(path, FileMode.Append, FileAccess.Write, FileShare.Read);
            await using var writer = new StreamWriter(stream, Encoding.UTF8);
            await writer.WriteLineAsync($"{nameof(FileWorker)} is active at {DateTime.UtcNow:R}");

            await Task.Delay(_delay, stoppingToken);
        }
    }
}