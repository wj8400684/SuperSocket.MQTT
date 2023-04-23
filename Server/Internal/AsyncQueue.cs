namespace Server;

internal sealed class CancelableCompletionSource<T> : IDisposable
{
    private readonly TaskCompletionSource<T> _source;
    private CancellationTokenRegistration? _registration;

    public CancelableCompletionSource()
        => _source = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

    public void SetupCancellation(Action callback, CancellationToken token)
        => _registration = token.Register(callback);

    public void SetResult(T result)
        => _ = _source.TrySetResult(result);

    public void SetException(Exception exception)
        => _ = _source.TrySetException(exception);

    public Task<T> Task => _source.Task;

    public void Dispose()
    {
        _ = _source.TrySetCanceled();
        _registration?.Dispose();
    }
}

internal sealed class AsyncQueue<T> : IDisposable
{
    private readonly object _lock;
    private readonly Queue<T> _queue;
    private readonly LinkedList<CancelableCompletionSource<T>> _pendingDequeues;
    private int _isDisposed;

    public AsyncQueue()
    {
        _lock = new object();
        _queue = new Queue<T>();
        _pendingDequeues = new LinkedList<CancelableCompletionSource<T>>();
    }

    public int Count => _queue.Count;

    public void Enqueue(T item)
    {
        lock (_lock)
        {
            ThrowIfDisposed();

            var node = _pendingDequeues.First;
            if (node is not null)
            {
                node.Value.SetResult(item);
                node.Value.Dispose();
                _pendingDequeues.RemoveFirst();
            }
            else
                _queue.Enqueue(item);
        }
    }

    public ValueTask<T> DequeueAsync(CancellationToken cancellationToken = default)
    {
        LinkedListNode<CancelableCompletionSource<T>>? node = null;

        lock (_lock)
        {
            ThrowIfDisposed();

            if (_queue.Count > 0)
                return new ValueTask<T>(_queue.Dequeue());

            node = _pendingDequeues.AddLast(new CancelableCompletionSource<T>());
        }

        node.Value.SetupCancellation(() => Cancel(node), cancellationToken);
        return new ValueTask<T>(node.Value.Task);
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) != 0)
                return;

            foreach (var pendingDequeue in _pendingDequeues)
                pendingDequeue.Dispose();

            _pendingDequeues.Clear();
            _queue.Clear();
        }
    }

    private void Cancel(LinkedListNode<CancelableCompletionSource<T>> node)
    {
        lock (_lock)
        {
            try
            {
                node.Value.Dispose();
                _pendingDequeues.Remove(node);
            }
            catch
            {
                // ignored
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (_isDisposed != 0)
            throw new ObjectDisposedException(typeof(AsyncQueue<>).FullName);
    }
}

