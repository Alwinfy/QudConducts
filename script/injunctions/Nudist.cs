using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class Nudist : Injunction
    {
        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(EquippedEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is EquippedEvent eqe) {
                if (eqe.Item.HasTag("Armor")) {
                    SignalViolation();
                }
            }
        }
    }
}
