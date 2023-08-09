using System.Collections.Generic;
using System;
using XRL.World;
using XRL.World.Parts;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class Pacifism : Injunction
    {

        public bool NeutralsAcceptable;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(AttackerDealtDamageEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is AttackerDealtDamageEvent adde) {
                if (adde.Actor == target && adde.Actor != adde.Object && adde.Object.pBrain is Brain brain && !HarmAcceptable(brain.GetOpinion(target))) {
                    SignalViolation();
                }
            }
        }
        public bool HarmAcceptable(Brain.CreatureOpinion opinion) {
            switch (opinion) {
                case Brain.CreatureOpinion.allied: return false;
                case Brain.CreatureOpinion.neutral: return NeutralsAcceptable;
                case Brain.CreatureOpinion.hostile: return true;
                default: return false;
            }
        }
    }
}
