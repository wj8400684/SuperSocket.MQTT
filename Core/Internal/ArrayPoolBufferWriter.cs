using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Core;

/// <summary>
/// 定义支持获取已写入数据的BufferWriter
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IWrittenBufferWriter<T> : IBufferWriter<T>
{
    /// <summary>
    /// 获取已数入的数据长度
    /// </summary>
    int WrittenCount { get; }

    /// <summary>
    /// 获取已数入的数据
    /// </summary>
    ReadOnlySpan<T> WrittenSpan { get; }

    /// <summary>
    /// 获取已数入的数据
    /// </summary>
    ReadOnlyMemory<T> WrittenMemory { get; }

    /// <summary>
    /// 获取已数入的数据
    /// </summary>
    /// <returns></returns>
    ArraySegment<T> WrittenSegment { get; }
}

/// <summary>
/// 表示基于ArrayPool的BufferWriter
/// </summary>
[DebuggerDisplay("WrittenCount = {WrittenCount}")]
public sealed class ArrayPoolBufferWriter<T> : IWrittenBufferWriter<T>, IDisposable
{
    private int _index = 0;
    private T[] _buffer;
    private bool _disposed = false;
    private const int DefaultSizeHint = 256;

    /// <summary>
    /// 获取已数入的数据长度
    /// </summary>
    public int WrittenCount => _index;

    /// <summary>
    /// 获取已数入的数据
    /// </summary>
    public ReadOnlySpan<T> WrittenSpan => _buffer.AsSpan(0, _index);

    /// <summary>
    /// 获取已数入的数据
    /// </summary>
    public ReadOnlyMemory<T> WrittenMemory => _buffer.AsMemory(0, _index);

    /// <summary>
    /// 获取已数入的数据
    /// </summary>
    /// <returns></returns>
    public ArraySegment<T> WrittenSegment => new(_buffer, 0, _index);

    /// <summary>
    /// 获取容量
    /// </summary>
    public int Capacity => _buffer.Length;

    /// <summary>
    /// 获取剩余容量
    /// </summary>
    public int FreeCapacity => _buffer.Length - _index;


    /// <summary>
    /// 基于ArrayPool的BufferWriter
    /// </summary>
    /// <param name="initialCapacity">初始容量</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public ArrayPoolBufferWriter(int initialCapacity)
    {
        if (initialCapacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(initialCapacity));

        _buffer = ArrayPool<T>.Shared.Rent(initialCapacity);
    }

    /// <summary>
    /// 清除数据
    /// </summary>
    public void Clear()
    {
        _buffer.AsSpan(0, _index).Clear();
        _index = 0;
    }

    /// <summary>
    /// 设置向前推进
    /// </summary>
    /// <param name="count"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void Advance(int count)
    {
        if (count < 0 || _index > _buffer.Length - count)
            throw new ArgumentOutOfRangeException(nameof(count));

        _index += count;
    }

    /// <summary>
    /// 返回用于写入数据的Memory
    /// </summary>
    /// <param name="sizeHint">意图大小</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns></returns>
    public Memory<T> GetMemory(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return _buffer.AsMemory(_index);
    }

    /// <summary>
    /// 返回用于写入数据的Span
    /// </summary>
    /// <param name="sizeHint">意图大小</param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns></returns>
    public Span<T> GetSpan(int sizeHint = 0)
    {
        CheckAndResizeBuffer(sizeHint);
        return _buffer.AsSpan(_index);
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="value"></param>
    public void Write(T value)
    {
        GetSpan(1)[0] = value;
        _index += 1;
    }

    /// <summary>
    /// 写入数据
    /// </summary>
    /// <param name="value">值</param> 
    public void Write(ReadOnlySpan<T> value)
    {
        if (!value.IsEmpty)
            return;

        value.CopyTo(GetSpan(value.Length));
        _index += value.Length;
    }

    /// <summary>
    /// 检测和扩容
    /// </summary>
    /// <param name="sizeHint"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CheckAndResizeBuffer(int sizeHint)
    {
        if (sizeHint < 0)
            throw new ArgumentOutOfRangeException(nameof(sizeHint));

        if (sizeHint == 0)
            sizeHint = DefaultSizeHint;

        if (sizeHint <= FreeCapacity)
            return;

        var currentLength = _buffer.Length;
        var growBy = Math.Max(sizeHint, currentLength);
        var newSize = checked(currentLength + growBy);

        var newBuffer = ArrayPool<T>.Shared.Rent(newSize);
        Array.Copy(_buffer, newBuffer, _index);

        ArrayPool<T>.Shared.Return(_buffer);
        _buffer = newBuffer;
    }

    /// <summary>
    /// 将对象进行回收
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        ArrayPool<T>.Shared.Return(_buffer);
        GC.SuppressFinalize(this);

        _disposed = true;
    }

    /// <summary>
    /// 析构函数
    /// </summary>
    ~ArrayPoolBufferWriter()
    {
        Dispose();
    }
}
