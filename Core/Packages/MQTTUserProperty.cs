namespace Core;

public sealed class MQTTUserProperty
{
    public MQTTUserProperty(string name, string value)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public string Name { get; }

    public string Value { get; }

    public override bool Equals(object? other)
    {
        if (other is not MQTTUserProperty userProperty)
            return false;

        return Equals(userProperty);
    }

    public bool Equals(MQTTUserProperty other)
    {
        if (other == null)
            return false;

        if (ReferenceEquals(other, this))
            return true;

        return string.Equals(Name, other.Name, StringComparison.Ordinal) && string.Equals(Value, other.Value, StringComparison.Ordinal);
    }

    public override int GetHashCode()
    {
        return Name.GetHashCode() ^ Value.GetHashCode();
    }

    public override string ToString()
    {
        return $"{Name} = {Value}";
    }
}
