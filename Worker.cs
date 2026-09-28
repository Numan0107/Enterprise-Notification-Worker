using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EnterpriseNotificationWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly NotificationQueue _notificationQueue;

    public Worker(ILogger<Worker> logger, NotificationQueue notificationQueue)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationQueue = notificationQueue ?? throw new ArgumentNullException(nameof(notificationQueue));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = Task.Run(() => SimulateIncomingTrafficAsync(stoppingToken), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var task = await _notificationQueue.DequeueAsync(stoppingToken);
                await ProcessNotificationWithRetryAsync(task, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Critical failure inside global background worker loop.");
            }
        }
    }

    private async Task ProcessNotificationWithRetryAsync(NotificationTask task, CancellationToken cancellationToken)
    {
        const int maxRetryAttempts = 3;
        while (task.RetryCount < maxRetryAttempts && !task.IsProcessed)
        {
            try
            {
                if (Random.Shared.Next(1, 100) <= 20)
                {
                    throw new InvalidOperationException("Simulated external network gateway failure.");
                }

                task.IsProcessed = true;
                _logger.LogInformation("[ReqID:{Id}] [TYPE:{Type}] Notification delivered to {Recipient}.", task.Id, task.Type, task.Recipient);
            }
            catch (Exception)
            {
                task.RetryCount++;
                _logger.LogWarning("[ReqID:{Id}] Delivery failed. Retry attempt {Count}/{Max} triggered.", task.Id, task.RetryCount, maxRetryAttempts);
                
                if (task.RetryCount >= maxRetryAttempts)
                {
                    _logger.LogError("[ReqID:{Id}] [CRITICAL] Notification pushed to Dead Letter Queue (DLQ) after max retries.", task.Id);
                    break;
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }

    private async Task SimulateIncomingTrafficAsync(CancellationToken cancellationToken)
    {
        var channels = new[] { "EMAIL", "SMS", "PUSH_NOTIFICATION" };
        var users = new[] { "user_102@global.com", "+4471234567", "device_token_abc123" };

        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(400), cancellationToken);
            
            var index = Random.Shared.Next(0, channels.Length);
            var task = new NotificationTask
            {
                Type = channels[index],
                Recipient = users[index],
                Message = "Enterprise operational synchronization alert trigger."
            };

            await _notificationQueue.EnqueueAsync(task);
        }
    }
}
