using System;
using System.Collections.Generic;
using XRL.World;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public abstract class Injunction
    {
        public abstract IEnumerable<EventType> DesiredEvents();

        public Conduct ParentConduct;

        public virtual void FireEvent(GameObject target, Event E) {}

        public virtual void HandleEvent(GameObject target, MinEvent E) {}

        public void SignalViolation() {
            ParentConduct.SignalViolation(this);
        }
    }
}
