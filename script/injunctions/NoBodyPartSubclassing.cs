using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoBodyPartSubclassing : Injunction
    {
        public string Part;

        [NonSerialized]
        private Type _partType;
        public Type PartType {
            get {
                if (_partType is null) {
                    var (ns, clazz) = ConductLoader.GetNamespaceAndClassName("XRL.World.Parts", Part);
                    _partType = ModManager.ResolveType(ns, clazz);
                    if (_partType == null) {
                        throw new Exception("Can't find part: " + Part);
                    }
                }
                return _partType;
            }
        }

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(AfterPlayerBodyChangeEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is AfterPlayerBodyChangeEvent ev && CheckForPart(ev.NewBody)) {
                SignalViolation();
            }
        }
        public bool CheckForPart(GameObject obj) {
            foreach (var part in obj.PartsList) {
                if (PartType.IsInstanceOfType(part)) {
                    return true;
                }
            }
            return false;
        }
    }
}
