using System.Collections.Generic;
using System;
using XRL.World;
using XRL.World.Parts;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoEat : Injunction
    {
        public string Tag;
        public bool InvertTag = false;
        public bool CookedOnly = false;
        public bool RawOnly = false;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new StringlyEventType("alwinfy_ConsumeFood");
        }

        public bool CheckTag(GameObject go) => Tag.Equals("*anything") || go.HasTagOrProperty(Tag) != InvertTag;
        public bool CheckCookedness(bool Raw) => !(Raw ? CookedOnly : RawOnly);

        public override void FireEvent(GameObject target, Event E) {
            if (E.ID == "alwinfy_ConsumeFood" && CheckCookedness(E.HasFlag("Raw")) && E.GetGameObjectParameter("Ingredient") is GameObject go && CheckTag(go)) {
                SignalViolation();
            }
        }
    }
}
