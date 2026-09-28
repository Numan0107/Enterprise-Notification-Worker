using System.Threading.Channels;

namespace EnterpriseNotificationWorker;

public class NotificationQueue
{
    private readonly Channel<NotificationTask> _queue;

    public NotificationQueue()
    {
        var options = new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.Wait
        };
        _queue = Channel.CreateBounded<NotificationTask>(options);
    }

    public async ValueTask EnqueueAsync(NotificationTask task)
    {
        ArgumentNullException.ThrowIfNull(task);
        await _queue.Writer.WriteAsync(task);
    }

    public async ValueTask<NotificationTask> DequeueAsync(CancellationToken cancellationToken)
    {
        return await _queue.Reader.ReadAsync(cancellationToken);
    }
}
