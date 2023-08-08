using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoMapTravel : Injunction
    {
        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(ObjectLeavingCellEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is ObjectLeavingCellEvent && target.OnWorldMap()) {
                SignalViolation();
            }
        }
    }
}
