using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoKill : Injunction
    {
        public string Tag;
        public string ExcludeTag;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new StringlyEventType("MurderEvent");
        }

        public override void FireEvent(GameObject target, Event E) {
            if (E.ID == "MurderEvent" && E.GetGameObjectParameter("Victim") is GameObject victim) {
                if (victim.HasTagOrProperty(Tag) && (ExcludeTag == null || !victim.HasTagOrProperty(ExcludeTag))) {
                    SignalViolation();
                }
            }
        }
    }
}
