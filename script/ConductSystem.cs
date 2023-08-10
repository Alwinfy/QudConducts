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
        public HashSet<string> BrokenConducts = new HashSet<string>();

        [NonSerialized]
        public Dictionary<string, Conduct> ConductsByName;
        [NonSerialized]
        public Dictionary<ConductType, Dictionary<EventType, List<Conduct>>> ConductsByEventAndType;
        [NonSerialized]
        public HashSet<string> InterestingStringEvents;
        [NonSerialized]
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
            ConductsByName = new Dictionary<string, Conduct>();
            var minEvents = new HashSet<int>();
            InterestingStringEvents = new HashSet<string>();
            foreach (var conduct in ConductLoader.Conducts) {
                if (BrokenConducts.Contains(conduct.Name)) {
                    continue;
                }
                ConductsByName.Add(conduct.Name, conduct);
                var type = conduct.AppliesToFollowers ? ConductType.FOLLOWERS : ConductType.SELF;
                var eventDict = GetOrCreate(ConductsByEventAndType, type);
                foreach (var evt in conduct.DesiredEvents()) {
                    GetOrCreate(eventDict, evt).Add(conduct);
                    if (evt is StringlyEventType strEv) {
                        InterestingStringEvents.Add(strEv.ID);
                    } else if (evt is MinEventType minEv) {
                        minEvents.Add(minEv.ID);
                    }
                }
            }
            InterestingMinEvents = new List<int>(minEvents);
            Dirty = false;
        }

        public List<Conduct> FindListeners(ConductType ct, EventType et) {
            RebuildCaches();
            Dictionary<EventType, List<Conduct>> conductSet;
            ConductsByEventAndType.TryGetValue(ct, out conductSet);
            if (conductSet is null) return null;

            List<Conduct> listeners;
            conductSet.TryGetValue(et, out listeners);
            return listeners;
        }

        public void NotifyEvent(GameObject target, Event E) {
            foreach (var conductType in ConductTypes(target)) {
                var listeners = FindListeners(conductType, new StringlyEventType(E.ID));
                if (!listeners.IsNullOrEmpty()) {
                    foreach (var listener in listeners) {
                        listener.NotifyEvent(target, E);
                    }
                }
            }
            RebuildCaches();
        }

        public void NotifyEvent(GameObject target, MinEvent E) {
            foreach (var conductType in ConductTypes(target)) {
                var listeners = FindListeners(conductType, new MinEventType(E.ID));
                if (!listeners.IsNullOrEmpty()) {
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
