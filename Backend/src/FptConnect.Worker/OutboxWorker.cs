namespace FptConnect.Worker;

/// <summary>
/// Khung background worker (Bible 7.8): xử lý outbox/reminder/route aggregation/retention.
/// Sprint 0 chỉ heartbeat; các consumer thực thi sẽ thêm ở S7/S9/S16.
/// </summary>
public class OutboxWorker : BackgroundService
{
    private readonly ILogger<OutboxWorker> _logger;
    public OutboxWorker(ILogger<OutboxWorker> logger) => _logger = logger;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("OutboxWorker heartbeat {TimeUtc}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
        }
    }
}
