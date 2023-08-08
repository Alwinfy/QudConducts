using System;
using System.Collections.Generic;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts {

    public enum ConductType {
        SELF,
        FOLLOWERS,
    }

    [Serializable]
    public class ConductSystem : IGameSystem
    {
        public HashSet<string> BrokenConducts;

        [NonSerialized]
        public Dictionary<ConductType, Dictionary<EventType, List<Conduct>>> ConductsByEventAndType;
        [NonSerialized]
        public HashSet<string> InterestingStringEvents;
        public List<int> InterestingMinEvents;
        
        private bool Dirty = true;

        public void MarkDirty() { Dirty = true; }


        private static TValue GetOrCreate<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey key) 
            where TValue : new()
        {
            if (!dict.TryGetValue(key, out TValue val))
            {
                val = new TValue();
                dict.Add(key, val);
            }

            return val;
        }
        public void RebuildCaches() {
            if (ConductsByEventAndType != null && !Dirty) {
                return;
            }
            ConductsByEventAndType = new Dictionary<ConductType, Dictionary<EventType, List<Conduct>>>();
            foreach (var conduct in ConductLoader.Conducts) {
                if (BrokenConducts.Contains(conduct.Name)) {
                    continue;
                }
                var type = conduct.AppliesToFollowers ? ConductType.FOLLOWERS : ConductType.SELF;
                var eventDict = GetOrCreate(ConductsByEventAndType, type);
                var minEvents = new HashSet<int>();
                foreach (var evt in conduct.DesiredEvents()) {
                    GetOrCreate(eventDict, evt).Add(conduct);
                    if (evt is StringlyEventType strEv) {
                        InterestingStringEvents.Add(strEv.ID);
                    } else if (evt is MinEventType minEv) {
                        minEvents.Add(minEv.ID);
                    }
                }
                InterestingMinEvents = new List<int>(minEvents);
            }
            Dirty = false;
        }

        public void NotifyEvent(GameObject target, Event E) {
            foreach (var conductType in ConductTypes(target)) {
                var listeners = ConductsByEventAndType[conductType][new StringlyEventType(E.ID)];
                if (listeners != null) {
                    foreach (var listener in listeners) {
                        listener.NotifyEvent(target, E);
                    }
                }
            }
            RebuildCaches();
        }

        public void NotifyEvent(GameObject target, MinEvent E) {
            foreach (var conductType in ConductTypes(target)) {
                var listeners = ConductsByEventAndType[conductType][new MinEventType(E.ID)];
                if (listeners != null) {
                    foreach (var listener in listeners) {
                        listener.NotifyEvent(target, E);
                    }
                }
            }
            RebuildCaches();
        }


        public static IEnumerable<ConductType> ConductTypes(GameObject candidate) {
            if (candidate.IsPlayer()) {
                yield return ConductType.SELF;
            }
            if (candidate.IsPlayerLed()) {
                yield return ConductType.FOLLOWERS;
            }
        }

        public void SignalViolation(Conduct conduct) {
            BrokenConducts.Add(conduct.Name);
            MarkDirty();
        }
    }
}
