using System.Collections.Generic;
using System;
using XRL.World;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class NoInteract : Injunction
    {
        public string Tag = null;
        public string Part = null;

        public string ValidCommands = "";

        public static string DefaultValidCommands = "Remove,Remove Notes,Add Notes,AutoCollect,Clean,CleanAll,Close,Open,Disarm,Disassemble,Disassemble All,Get,Look,Examine,Mark Important,Mark Unimportant,Pet,Pour,Fill,Read,Repair,RepairVehicle,Rifle,Sacrifice,Stand,Stand Up,Unload Ammo,Load Ammo";

        public bool Invert = false;

        [NonSerialized]
        HashSet<string> _ValidCommandsMap = null;
        HashSet<string> ValidCommandsMap {
            get {
                if (_ValidCommandsMap == null) {
                    _ValidCommandsMap = new HashSet<string>((ValidCommands == "*default" ? DefaultValidCommands : ValidCommands).Split(','));
                }
                return _ValidCommandsMap;
            }
        }

        public override IEnumerable<EventType> DesiredEvents() {
            yield return new MinEventType(OwnerAfterInventoryActionEvent.ID);
        }

        public bool MatchItem(GameObject go) {
            return (Tag == null || go.HasTagOrProperty(Tag)) && (Part == null || go.HasPart(Part));
        }

        public override void HandleEvent(GameObject target, MinEvent E) {
            if (E is OwnerAfterInventoryActionEvent aiae) {
                if (MatchItem(aiae.Item) && ValidCommandsMap.Contains(aiae.Command) == Invert) {
                    SignalViolation();
                }
            }
        }
    }
}
