using System.Collections.Generic;
using System;
using XRL.World;
using XRL.World.Parts;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class FistsOnly : Injunction
    {

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new StringlyEventType("AttackerAfterMelee");
        }

        public override void FireEvent(GameObject target, Event E) {
            if (E.ID == "AttackerAfterMelee") {
                foreach (var weapon in E.GetParameter<List<GameObject>>("Weapons")) {
                    if (!weapon.HasTagOrProperty("NaturalGear")) {
                        SignalViolation();
                    }
                }
            }
        }
    }
}
