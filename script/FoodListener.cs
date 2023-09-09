using System;
using XRL.UI;
using XRL.World;
using XRL.World.Parts;
using System.Collections.Generic;

using Alwinfy.Conducts;

namespace XRL.World.Parts {
    public class alwinfy_FoodListener : IPart
    {
        public override void Register(GameObject Object)
        {
            Object.RegisterPartEvent(this, "UsedAsIngredient");
            Object.RegisterPartEvent(this, "Eaten");
            base.Register(Object);
        }

        public override bool FireEvent(Event E) {
            if (E.ID == "UsedAsIngredient") {
                Send(E.GetGameObjectParameter("Actor"), false);
            } else if (E.ID == "Eaten") {
                Send(E.GetGameObjectParameter("Eater"), true);
            }
            return base.FireEvent(E);
        }

        public void Send(GameObject target, bool raw) {
            var evt = new Event("alwinfy_ConsumeFood", "Ingredient", ParentObject);
            evt.SetFlag("Raw", raw);
            target.FireEvent(evt);
        }
    }
}
