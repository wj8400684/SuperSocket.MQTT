using System.Buffers;

namespace Core;

/// <summary>
/// 定义数组持有者的接口
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IArrayOwner<T> : IDisposable
{
    /// <summary>
    /// 获取数据有效数据长度
    /// </summary>
    int Length { get; }

    /// <summary>
    /// 获取持有的数组
    /// </summary>
    T[] Array { get; }

    /// <summary>
    /// 转换为Span
    /// </summary>
    /// <returns></returns>
    Span<T> AsSpan();

    /// <summary>
    /// 转换为Memory
    /// </summary>
    /// <returns></returns>
    Memory<T> AsMemory();
}

public sealed class ArrayOwner<T> : IArrayOwner<T>
{
    private bool _disposed = false;
    private readonly ArrayPool<T> _arrayPool;

    /// <summary>
    /// 获取数据有效数据长度
    /// </summary>
    public int Length { get; }

    /// <summary>
    /// 获取持有的数组
    /// </summary>
    public T[] Array { get; }

    public Span<T> AsSpan()
    {
        return Array.AsSpan(0, Length);
    }

    public Memory<T> AsMemory()
    {
        return Array.AsMemory(0, Length);
    }

    /// <summary>
    /// 数组持有者
    /// </summary>
    /// <param name="arrayPool"></param>
    /// <param name="length"></param> 
    public ArrayOwner(ArrayPool<T> arrayPool, int length)
    {
        _arrayPool = arrayPool;
        Length = length;
        Array = arrayPool.Rent(length);
    }

    /// <summary>
    /// 将对象进行回收
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
            return;

        _arrayPool.Return(Array);
        GC.SuppressFinalize(this);

        _disposed = true;
    }

    ~ArrayOwner()
    {
        Dispose();
    }
}
