using System.Collections.Generic;
using System;
using XRL.World;
using XRL;

namespace Alwinfy.Conducts.Injunctions {

    [Serializable]
    public class Inviolable : Injunction
    {
        public override IEnumerable<EventType> DesiredEvents() { 
            return new EventType[0];
        }
    }
}
