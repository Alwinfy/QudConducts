using XRL; // to abbreviate XRL.PlayerMutator and XRL.IPlayerMutator
using XRL.World; // to abbreviate XRL.World.GameObject
using XRL.World.Parts; // to abbreviate XRL.World.GameObject

namespace Alwinfy.Conducts {
    
    [PlayerMutator]
    public class AttachConductManager : IPlayerMutator
    {
        public void mutate(GameObject player)
        {
            player.AddPart<alwinfy_ConductListener>();
        }
    }
}
