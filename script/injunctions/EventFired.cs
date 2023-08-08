using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class EventFired : Injunction
    {
        public string Event;

        // optional: 
        public string Parameter;
        public string ParameterTag;

        public override IEnumerable<EventType> DesiredEvents() {
            if (Event == null) {
                throw new Exception("Couldn't load an EventFired - target is null!!");
            }
            yield return new StringlyEventType(Event);
        }

        public override void FireEvent(GameObject target, Event E) {
            if (E.ID == Event) {
                if (Parameter != null && ParameterTag != null) {
                    var param = E.GetGameObjectParameter(Parameter);
                    if (param != null && !CheckTag(param, ParameterTag)) {
                        return;
                    }
                }
                SignalViolation();
            }
        }
        public static bool CheckTag(GameObject obj, string param) {
            bool wantTag = true;
            if (param.StartsWith('!')) {
                wantTag = false;
                param = param.Substring(1);
            }
            return obj.HasTagOrProperty(param) == wantTag;
        }
    }
}
