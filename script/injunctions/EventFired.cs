using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class EventFired : Injunction
    {
        public string Target;

        // optional: 
        public string Parameter;
        public string ParameterTag;

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new StringlyEventType(Target);
        }

        public override void FireEvent(GameObject target, Event E) {
            if (E.ID == Target) {
                if (Parameter != null && ParameterTag != null) {
                    var param = E.GetGameObjectParameter(Parameter);
                    if (param != null && !param.HasTag(ParameterTag)) {
                        return;
                    }
                }
                SignalViolation();
            }
        }
    }
}
