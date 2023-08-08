using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoUseEnergy : Injunction
    {
        public string Type;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(GetEnergyCostEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is GetEnergyCostEvent ev && ev.Type == Type) {
                SignalViolation();
            }
        }
    }
}
