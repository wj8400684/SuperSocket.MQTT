using Core;
using Microsoft.Extensions.Logging;
namespace Server;

internal readonly struct TaskItem
{
    public TaskItem(MQTTSession session, MQTTPackage package, Func<MQTTSession, MQTTPackage, CancellationToken, ValueTask> task)
    {
        Package = package;
        Session = session;
        Task = task;
    }

    public readonly MQTTPackage Package { get; }

    public readonly MQTTSession Session { get; }

    public readonly Func<MQTTSession, MQTTPackage, CancellationToken, ValueTask> Task { get; }

    public ValueTask InvokeAsync(CancellationToken cancellationToken)
    {
        return Task.Invoke(Session, Package, cancellationToken);
    }
}

internal sealed class TaskPacketScheduler
{
    private readonly TimeSpan _timeout;
    private ILogger _logger;
    private readonly AsyncQueue<TaskItem> _schedulerQueue = new();

    public TaskPacketScheduler(ILogger logger)
    {
        _logger = logger;
        _timeout = TimeSpan.FromMilliseconds(20);
    }

    /// <summary>
    /// 开始任务
    /// </summary>
    /// <param name="cancellationToken"></param>
    public void Start(CancellationToken cancellationToken)
    {
        Task.Factory.StartNew(async () =>
        {
            try
            {
                await OnHandlerTaskAsync(cancellationToken);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "执行 OnHandlerTaskAsync 抛出一个异常");
            }
            finally
            {
                _schedulerQueue.Dispose();
            }
        }, TaskCreationOptions.LongRunning);
    }

    /// <summary>
    /// 添加任务
    /// </summary>
    /// <param name="item"></param>
    public void Add(TaskItem item)
    {
        _schedulerQueue.Enqueue(item);
    }

    /// <summary>
    /// 处理任务
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    async ValueTask OnHandlerTaskAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var item = await _schedulerQueue.DequeueAsync(cancellationToken);

            try
            {
                using var tokenSource = new CancellationTokenSource(_timeout);
                using var linkedToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, tokenSource.Token);
                await item.InvokeAsync(linkedToken.Token);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                item.Session.LogError(ex, "执行调度任务抛出一个异常");
            }
        }
    }
}
