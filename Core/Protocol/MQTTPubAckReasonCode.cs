﻿namespace Core;

public enum MQTTPubAckReasonCode
{
    Success = 0,

    /// <summary>
    /// The message is accepted but there are no subscribers. This is sent only by the Server. If the Server knows that there are no matching subscribers, it MAY use this Reason Code instead of 0x00 (Success).
    /// </summary>
    NoMatchingSubscribers = 16,

    UnspecifiedError = 128,
    ImplementationSpecificError = 131,
    NotAuthorized = 135,
    TopicNameInvalid = 144,
    PacketIdentifierInUse = 145,
    QuotaExceeded = 151,
    PayloadFormatInvalid = 153
}
