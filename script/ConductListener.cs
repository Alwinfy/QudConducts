using System;
using XRL;
using XRL.World;
using XRL.World.Parts;
using System.Collections.Generic;

using Alwinfy.Conducts;

namespace XRL.World.Parts {
    public class alwinfy_ConductListener : IPart
    {

        public List<int> WantEvents = new List<int>();

        public override bool WantEvent(int ID, int cascade) => WantEvents.Contains(ID) || base.WantEvent(ID, cascade);
        public override void Register(GameObject Object)
        {
            ConductLoader.System.RebuildCaches();
            foreach (var evt in ConductLoader.System.InterestingStringEvents) {
                Object.RegisterPartEvent(this, evt);
            }
            WantEvents = ConductLoader.System.InterestingMinEvents;
            base.Register(Object);
        }

        public override bool FireEvent(Event E) {
            ConductLoader.System.NotifyEvent(ParentObject, E);
            return base.FireEvent(E);
        }

        public override bool HandleEvent(MinEvent E) {
            ConductLoader.System.NotifyEvent(ParentObject, E);
            return base.HandleEvent(E);
        }
    }
}
