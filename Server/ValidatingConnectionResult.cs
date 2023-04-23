
using Core;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Server;

public sealed class ValidatingConnectionResult
{
    private readonly MQTTConnectPackage _connectPackage;

    public ValidatingConnectionResult(MQTTConnectPackage package)
    {
    }

    /// <summary>
    ///     Gets or sets the assigned client identifier.
    ///     MQTTv5 only.
    /// </summary>
    public string AssignedClientIdentifier { get; set; }

    /// <summary>
    ///     Gets or sets the authentication data.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public byte[]? AuthenticationData => _connectPackage.AuthenticationData;

    /// <summary>
    ///     Gets or sets the authentication method.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public string? AuthenticationMethod => _connectPackage.AuthenticationMethod;

    /// <summary>
    ///     Gets the channel adapter. This can be a _MqttConnectionContext_ (used in ASP.NET), a _MqttChannelAdapter_ (used for
    ///     TCP or WebSockets) or a custom implementation.
    /// </summary>
    public IMqttChannelAdapter ChannelAdapter { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether clean sessions are used or not.
    ///     When a client connects to a broker it can connect using either a non persistent connection (clean session) or a
    ///     persistent connection.
    ///     With a non persistent connection the broker doesn't store any subscription information or undelivered messages for
    ///     the client.
    ///     This mode is ideal when the client only publishes messages.
    ///     It can also connect as a durable client using a persistent connection.
    ///     In this mode, the broker will store subscription information, and undelivered messages for the client.
    /// </summary>
    public bool? CleanSession => _connectPackage.CleanSession;

    public X509Certificate2 ClientCertificate => ChannelAdapter.ClientCertificate;

    /// <summary>
    ///     Gets the client identifier.
    ///     Hint: This identifier needs to be unique over all used clients / devices on the broker to avoid connection issues.
    /// </summary>
    public string? ClientId => _connectPackage.ClientId;

    public string Endpoint => ChannelAdapter.Endpoint;

    public bool IsSecureConnection => ChannelAdapter.IsSecureConnection;

    /// <summary>
    ///     Gets or sets the keep alive period.
    ///     The connection is normally left open by the client so that is can send and receive data at any time.
    ///     If no data flows over an open connection for a certain time period then the client will generate a PINGREQ and
    ///     expect to receive a PINGRESP from the broker.
    ///     This message exchange confirms that the connection is open and working.
    ///     This period is known as the keep alive period.
    /// </summary>
    public ushort? KeepAlivePeriod => _connectPackage.KeepAlivePeriod;

    /// <summary>
    ///     A value of 0 indicates that the value is not used.
    /// </summary>
    public uint MaximumPacketSize => _connectPackage.MaximumPacketSize;

    public string? Password => Encoding.UTF8.GetString(RawPassword ?? EmptyBuffer.Array);

    public MQTTProtocolVersion ProtocolVersion => ChannelAdapter.PacketFormatterAdapter.ProtocolVersion;

    public byte[]? RawPassword => _connectPackage.Password;

    /// <summary>
    ///     Gets or sets the reason code. When a MQTTv3 client connects the enum value must be one which is
    ///     also supported in MQTTv3. Otherwise the connection attempt will fail because not all codes can be
    ///     converted properly.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public MQTTConnectReasonCode ReasonCode { get; set; } = MQTTConnectReasonCode.Success;

    public string? ReasonString { get; set; }

    /// <summary>
    ///     Gets or sets the receive maximum.
    ///     This gives the maximum length of the receive messages.
    ///     A value of 0 indicates that the value is not used.
    /// </summary>
    public ushort ReceiveMaximum => _connectPackage.ReceiveMaximum;

    /// <summary>
    ///     Gets the request problem information.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public bool RequestProblemInformation => _connectPackage.RequestProblemInformation;

    /// <summary>
    ///     Gets the request response information.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public bool RequestResponseInformation => _connectPackage.RequestResponseInformation;

    /// <summary>
    ///     Gets or sets the response authentication data.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public byte[] ResponseAuthenticationData { get; set; }

    /// <summary>
    ///     Gets or sets the response user properties.
    ///     In MQTT 5, user properties are basic UTF-8 string key-value pairs that you can append to almost every type of MQTT
    ///     packet.
    ///     As long as you don’t exceed the maximum message size, you can use an unlimited number of user properties to add
    ///     metadata to MQTT messages and pass information between publisher, broker, and subscriber.
    ///     The feature is very similar to the HTTP header concept.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public List<MQTTUserProperty>? ResponseUserProperties { get; set; }

    /// <summary>
    ///     Gets or sets the server reference. This can be used together with i.e. "Server Moved" to send
    ///     a different server address to the client.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public string? ServerReference { get; set; }

    /// <summary>
    ///     Gets the session expiry interval.
    ///     The time after a session expires when it's not actively used.
    ///     A value of 0 means no expiation.
    /// </summary>
    public uint SessionExpiryInterval => _connectPackage.SessionExpiryInterval;

    /// <summary>
    ///     Gets or sets a key/value collection that can be used to share data within the scope of this session.
    /// </summary>
    public IDictionary<object, object>? SessionItems { get; }

    /// <summary>
    ///     Gets or sets the topic alias maximum.
    ///     This gives the maximum length of the topic alias.
    ///     A value of 0 indicates that the value is not used.
    /// </summary>
    public ushort TopicAliasMaximum => _connectPackage.TopicAliasMaximum;

    [Obsolete("This property name has a typo. Use 'UserName' instead. This one will be removed soon.")]
    public string? Username => _connectPackage.Username;

    public string? UserName => _connectPackage.Username;

    /// <summary>
    ///     Gets or sets the user properties.
    ///     In MQTT 5, user properties are basic UTF-8 string key-value pairs that you can append to almost every type of MQTT
    ///     packet.
    ///     As long as you don’t exceed the maximum message size, you can use an unlimited number of user properties to add
    ///     metadata to MQTT messages and pass information between publisher, broker, and subscriber.
    ///     The feature is very similar to the HTTP header concept.
    ///     <remarks>MQTT 5.0.0+ feature.</remarks>
    /// </summary>
    public List<MQTTUserProperty>? UserProperties => _connectPackage.UserProperties;

    /// <summary>
    ///     Gets or sets the will delay interval.
    ///     This is the time between the client disconnect and the time the will message will be sent.
    ///     A value of 0 indicates that the value is not used.
    /// </summary>
    public uint WillDelayInterval => _connectPackage.WillDelayInterval;

}
