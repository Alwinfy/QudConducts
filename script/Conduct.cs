using System;
using System.Collections.Generic;
using XRL.World;
using Alwinfy.Conducts.Injunctions;

namespace Alwinfy.Conducts {

    [Serializable]
    public class Conduct
    {
        public string Name;
        public string Description;
        // Hide this conduct from display if any of these other conducts are satisfied.
        public string[] HideIf;

        public string Group;

        public bool AppliesToFollowers = false;
        public bool Enforced = false;

        public List<Injunction> Injunctions = new List<Injunction>();

        public void SignalViolation(Injunction violated) {
            ConductLoader.System.SignalViolation(this);
        }

        [NonSerialized]
        private HashSet<EventType> _DesiredEvents;
        [NonSerialized]
        private Dictionary<EventType, List<Injunction>> _InjunctionsByEvent;

        public void BuildCaches() {
            if (_DesiredEvents is null) {
                _DesiredEvents = new HashSet<EventType>();
                foreach (var inj in Injunctions) {
                    foreach (var evt in inj.DesiredEvents()) {
                        _DesiredEvents.Add(evt);
                    }
                }
            }
            if (_InjunctionsByEvent is null) {
                _InjunctionsByEvent = new Dictionary<EventType, List<Injunction>>();
                foreach (var inj in Injunctions) {
                    foreach (var evt in inj.DesiredEvents()) {
                        List<Injunction> targets = null;
                        _InjunctionsByEvent.TryGetValue(evt, out targets);
                        if (targets is null) {
                            targets = new List<Injunction>();
                            _InjunctionsByEvent.Add(evt, targets);
                        }
                        targets.Add(inj);
                    }
                }
            }
        }
        public IEnumerable<EventType> DesiredEvents() {
            BuildCaches();
            return _DesiredEvents;
        }
        public void NotifyEvent(GameObject target, Event E) {
            BuildCaches();
            List<Injunction> injs;
            _InjunctionsByEvent.TryGetValue(new StringlyEventType(E.ID), out injs);
            if (!injs.IsNullOrEmpty()) {
                foreach (var inj in injs) {
                    inj.FireEvent(target, E);
                }
            }
        }
        public void NotifyEvent(GameObject target, MinEvent E) {
            BuildCaches();
            List<Injunction> injs;
            _InjunctionsByEvent.TryGetValue(new MinEventType(E.ID), out injs);
            if (!injs.IsNullOrEmpty()) {
                foreach (var inj in injs) {
                    inj.HandleEvent(target, E);
                }
            }
        }
    }
}
