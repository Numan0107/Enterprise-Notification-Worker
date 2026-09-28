namespace EnterpriseNotificationWorker;

public class NotificationTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Recipient { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int RetryCount { get; set; } = 0;
    public bool IsProcessed { get; set; } = false;
}
