using System.Collections.Generic;
using System;
using XRL.World;
using XRL.World.Effects;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoUseCampfire : Injunction
    {
        public override IEnumerable<EventType> DesiredEvents() {
            yield return new StringlyEventType("RemoveEffect");
        }

        public override void FireEvent(GameObject target, Event E) {
            if (E.ID == "RemoveEffect" && E.GetParameter<Effect>("Effect") is Famished) {
                SignalViolation();
            }
        }
    }
}
