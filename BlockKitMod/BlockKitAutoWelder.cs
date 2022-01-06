using Sandbox.ModAPI;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.ModAPI;
using VRage.ObjectBuilders;
using VRageMath;

namespace BlockKitMod
{

    [MyEntityComponentDescriptor(typeof(MyObjectBuilder_CubeGrid), false)]
    class BlockKitAutoWelder : MyGameLogicComponent
    {
        private IMyHudNotification _Notify = null;
        private bool isFirstBlock = true;

        public override void Init(MyComponentDefinitionBase definition)
        {
            base.Init(definition);
            //ShowOnHud("[BlockKitAutoWelder]: Initialized");
        }

        public override void Init(MyObjectBuilder_EntityBase objectBuilder)
        {
            base.Init(objectBuilder);
            isFirstBlock = true;
            if(Entity == null)
            {
                //ShowOnHud("[BlockKitAutoWelder]: entity null");
                return;
            }

            IMyCubeGrid grid = Entity as IMyCubeGrid;
            
            if(grid != null)
                grid.OnBlockAdded += OnBlockAdded;

        }

        private void OnBlockAdded(IMySlimBlock block)
        {
            if (isFirstBlock)
            {
                isFirstBlock = false;
                return;
            }

            if (block.IsFullIntegrity || block.HasDeformation || !Globals.Initialized)
                return;

            //ShowOnHud("Valid Block");

            //ShowOnHud("" + block.BuiltBy);

            var playerId = block.OwnerId;

            var player = GetPlayer(block);

            if (player == null)
            {
                //ShowOnHud("No Player in range");
                return;
            }

            //BlockCanBeConstructed(player, block);
            Dictionary<string, int> items;
            MoveItemsToBlock(player, block, out items);


            if (items.Count > 0)
            {
                ShowOnHud("You do not have the required amount of components!");
                MoveItemsToPlayer(player, block);
                RazeBlock(block);

            }
            else
            {
                BuildBlock(block, player);
            }
        }

        private IMyPlayer GetPlayer(IMySlimBlock target)
        {
            var players = new List<IMyPlayer>();
            MyAPIGateway.Multiplayer.Players.GetPlayers(players);
            if (players == null || players.Count == 0)
                ShowOnHud("No Players");

            foreach (var p in players)
            {
                if (target.BuiltBy == p.IdentityId)
                    return p;
            }

            return null;
        }

        private IMyPlayer GetClosestPlayer(IMySlimBlock target)
        {
            var players = new List<IMyPlayer>();
            double distance = 200f;
            IMyPlayer player = null;

            MyAPIGateway.Multiplayer.Players.GetPlayers(players);
            if (players == null || players.Count == 0)
                ShowOnHud("No Players");

            foreach (var p in players)
            {
                var pos = p.GetPosition();
                Vector3D endPosition;
                target.ComputeWorldCenter(out endPosition);
                var dist = Vector3D.DistanceSquared(endPosition, pos);
                if (dist < distance)
                {
                    distance = dist;
                    player = p;
                }
            }

            return player;
        }

        private void ShowOnHud(string text, int displayms = 3000)
        {
            if (_Notify == null)
            {
                _Notify = MyAPIGateway.Utilities.CreateNotification(text, displayms, MyFontEnum.Red);
            
            }
            else
            {
                _Notify.Text = text;
                _Notify.ResetAliveTime();
            }

            _Notify.Show();
        }

        private bool BlockCanBeConstructed(IMyPlayer player, IMySlimBlock block)
        {
            if (player.Character == null)
                return false;

            var inv = player.Character.GetInventory();
            if (inv == null)
                return false;

            var items = new Dictionary<string, int>();

            block.GetMissingComponents(items);
            
            foreach(var item in items)
            {
                //ShowOnHud(item.Key);
                //VRage.Game.ModAPI.Ingame.MyItemType it = new VRage.Game.ModAPI.Ingame.MyItemType("Component", "item.Key");
                //inv.ContainItems(item.Value, it);
            }

            return true;
        }

        private bool MoveItemsToBlock(IMyPlayer player, IMySlimBlock block, out Dictionary<string, int> items)
        {
            items = new Dictionary<string, int>();

            if (player.Character == null)
                return false;

            var inv = player.Character.GetInventory();
            if (inv == null)
                return false;

            block.MoveItemsToConstructionStockpile(inv);
            block.GetMissingComponents(items);

            return true;
        }

        private bool MoveItemsToPlayer(IMyPlayer player, IMySlimBlock block)
        {
            if (player.Character == null)
                return false;

            var inv = player.Character.GetInventory();
            if (inv == null)
                return false;

            block.MoveItemsFromConstructionStockpile(inv);
            return true;
        }

        private bool RazeBlock(IMySlimBlock block)
        {
            var grid = this.Entity as IMyCubeGrid;
            if (grid == null)
                return false;

            grid.RazeBlock(block.Position);
            return true;
        }

        private bool BuildBlock(IMySlimBlock block, IMyPlayer player)
        {
            if (player != null)
            {
                //ShowOnHud(block.MaxIntegrity.ToString() + player.Identity.DisplayName);
                block.IncreaseMountLevel(block.MaxIntegrity, player.IdentityId);
                //ShowOnHud("Block build");
                return true;
            }
            else
            {
                ShowOnHud("Player build block: null");
                return false;
            }
        }
    }
}
