using System;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;
using System.Collections.Generic;

using Alwinfy.Conducts;

namespace XRL.World.Parts {
    public class alwinfy_ConductListener : IPart
    {

        public List<int> WantEvents = new List<int>();

        public override bool WantEvent(int ID, int cascade) => ID == AfterPlayerBodyChangeEvent.ID || WantEvents.Contains(ID) || base.WantEvent(ID, cascade);
        public override void Register(GameObject Object)
        {
            ConductLoader.System.RebuildCaches();
            foreach (var evt in ConductLoader.System.InterestingStringEvents) {
                Object.RegisterPartEvent(this, evt);
            }
            Object.RegisterPartEvent(this, "alwinfy_CmdShowConduct");
            WantEvents = ConductLoader.System.InterestingMinEvents;
            base.Register(Object);
        }

        public override bool FireEvent(Event E) {
            ConductLoader.System.NotifyEvent(ParentObject, E);
            if (E.ID == "alwinfy_CmdShowConduct") {
                Popup.WaitNewPopupMessage(ConductDisplay.MarkUpConducts(false), title: "Conducts");
            }
            return base.FireEvent(E);
        }

        public override bool HandleEvent(AfterPlayerBodyChangeEvent E) {
            if (E.OldBody != null) {
                E.OldBody.RemovePart<alwinfy_ConductListener>();
            }
            E.NewBody.RequirePart<alwinfy_ConductListener>();
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(MinEvent E) {
            ConductLoader.System.NotifyEvent(ParentObject, E);
            return base.HandleEvent(E);
        }
    }
}
