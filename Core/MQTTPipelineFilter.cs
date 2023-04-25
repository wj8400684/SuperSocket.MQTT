using Server.Internal;
using SuperSocket.ProtoBase;
using System.Buffers;

namespace Core;

public sealed class MQTTPipelineFilter : IPipelineFilter<MQTTPackage>
{
    public IPackageDecoder<MQTTPackage>? Decoder { get; set; }

    public IPipelineFilter<MQTTPackage>? NextFilter { get; private set; }

    public object? Context { get; set; }

    private int _currentLenUnit;

    private int _totalSize;

    private int _headerParsed;

    public MQTTPackage Filter(ref SequenceReader<byte> reader)
    {
        const int v1 = 0x80;
        const int MaxVariablePartCount = 4;

        if (_currentLenUnit >= 0)
        {
            if (_headerParsed > 0)
                reader.Advance(_headerParsed);

            while (reader.TryRead(out byte encodedByte))
            {
                _headerParsed++;

                if (_currentLenUnit == 0)
                {
                    _currentLenUnit = 1;
                }
                else
                {
                    var hasNext = (encodedByte & v1) == v1;
                    _totalSize += (encodedByte & (v1-1)) * _currentLenUnit;

                    if (!hasNext || _headerParsed >= MaxVariablePartCount)
                    {
                        _currentLenUnit = -1;
                        break;
                    }
                    else
                    {
                        _currentLenUnit *= v1;
                    }
                }
            }

            reader.Rewind(_headerParsed);

            // has not loaded the length yet
            if (_currentLenUnit >= 0)
                return default!;

            // got the length, so we can set the totalSize right now
            _totalSize += _headerParsed;
        }

        if (reader.Remaining < _totalSize)
            return default!;

        try
        {
            var buffer = reader.Sequence.Slice(0, _totalSize);
            return Decoder!.Decode(ref buffer, Context);
        }
        finally
        {
            reader.Advance(_totalSize);
        }
    }

    public void Reset()
    {
        _currentLenUnit = 0;
        _headerParsed = 0;
        _totalSize = 0;
    }
}