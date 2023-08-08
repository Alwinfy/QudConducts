using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoKill : Injunction
    {
        public string Tag;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(AwardXPEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is AwardXPEvent axe) {
                if (axe.Kill != null && axe.Kill.HasTag(Tag)) {
                    SignalViolation();
                }
            }
        }
    }
}
