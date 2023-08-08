using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoUse : Injunction
    {
        public string Tag;

        public string ValidCommands = "";

        [NonSerialized]
        HashSet<string> _ValidCommandsMap = null;
        HashSet<string> ValidCommandsMap {
            get {
                if (_ValidCommandsMap == null) {
                    _ValidCommandsMap = new HashSet<string>(ValidCommands.Split(','));
                }
                return _ValidCommandsMap;
            }
        }

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(AfterInventoryActionEvent.ID);
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is AfterInventoryActionEvent aiae) {
                UnityEngine.Debug.Log("[Conducts] Got command: " + aiae.Command);
                if (aiae.Item.HasTag(Tag) && !ValidCommandsMap.Contains(aiae.Command)) {
                    SignalViolation();
                }
            }
        }
    }
}
