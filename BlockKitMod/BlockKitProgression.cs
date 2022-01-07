using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.Game.SessionComponents;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Components.Session;
using VRage.Game.ModAPI;
using VRage.Network;
using VRage.ObjectBuilders;
using VRage.Plugins;

namespace BlockKitMod
{
    class BlockKitProgression
    {
        public static bool Progress(IMySlimBlock block)
        {
            MySession session;
            MySessionComponentResearch researchComponent;
            session = MySession.Static;
            researchComponent = MySessionComponentResearch.Static;

            if (session == null || researchComponent == null)
                return false;

            long player = block.BuiltBy;
            long builtBy = block.BuiltBy;
            MyDefinitionId id = block.BlockDefinition.Id;

            var faction = session.Factions.TryGetPlayerFaction(player);
            if (faction != null)
            {
                foreach (var memberId in faction.Members.Keys)
                {
                    researchComponent.UnlockResearch(memberId, id, builtBy);
                }
            }
            else
            {
                researchComponent.UnlockResearch(builtBy, id, builtBy);
            }

            return true;
        }
    }
}
