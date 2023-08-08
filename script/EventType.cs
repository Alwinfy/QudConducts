using System;

namespace Alwinfy.Conducts {
    // HI GANG WHO LIKES SUM TYPES
    public interface EventType { }
    [Serializable]
    class MinEventType : EventType {
        public readonly int ID;
        public MinEventType(int ID) { this.ID = ID; }
    }

    [Serializable]
    class StringlyEventType : EventType {
        public readonly string ID;
        public StringlyEventType(string ID) { this.ID = ID; }
    }
}
