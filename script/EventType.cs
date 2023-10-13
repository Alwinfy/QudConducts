using System;

namespace Alwinfy.Conducts {
    // HI GANG WHO LIKES SUM TYPES
    public abstract record EventType() { }
    [Serializable]
    public record MinEventType(int ID) : EventType {}

    [Serializable]
    public record StringlyEventType(string ID) : EventType {}
}
