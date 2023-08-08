using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class Tethered : Injunction
    {
        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(AfterPlayerBodyChangeEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is AfterPlayerBodyChangeEvent ape) {
                if (ape.OldBody != null && ape.OldBody != ape.NewBody) {
                    SignalViolation();
                }
            }
        }
    }
}
