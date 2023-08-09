using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoBodyPart : Injunction
    {
        public string Part;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(AfterPlayerBodyChangeEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is AfterPlayerBodyChangeEvent ev && ev.NewBody.HasPart(Part)) {
                SignalViolation();
            }
        }
    }
}
