using System.Collections.Generic;
using System;
using XRL.World;
using XRL.World.Parts;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoDrink : Injunction
    {
        public string Liquid;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new StringlyEventType("DrinkingFrom");
        }

        public override void FireEvent(GameObject target, Event E) {
            // BROKEN: doesn't catch certain edge cases
            if (E.ID == "DrinkingFrom" && E.GetGameObjectParameter("Container") is GameObject go && go.LiquidVolume is LiquidVolume lv && lv.GetLiquidName() == Liquid) {
                SignalViolation();
            }
        }
    }
}
