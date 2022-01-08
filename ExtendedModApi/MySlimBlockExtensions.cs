using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using System.Collections.Generic;

namespace ExtendedModApi
{
	internal static class MySlimBlockExtensions
	{
		private static readonly Dictionary<string, int> missingComponents = new Dictionary<string, int>();

		public static bool TryGetBuildingPlayer(this MySlimBlock block, out MyPlayer player)
		{
			player = null;
			MyPlayer.PlayerId playerId;
			return
				MySession.Static.Players.TryGetPlayerId(block.BuiltBy, out playerId) &&
				MySession.Static.Players.TryGetPlayerById(playerId, out player);
		}

		public static bool TryFillStockpile(this MySlimBlock block, MyInventory inventory)
		{
			block.MoveItemsToConstructionStockpile(inventory);
			block.MoveUnneededItemsFromConstructionStockpile(inventory);

			missingComponents.Clear();
			block.GetMissingComponents(missingComponents);

			return missingComponents.Count == 0;
		}

		public static bool TryDrainStockpile(this MySlimBlock block, MyInventory inventory)
		{
			return block.MoveItemsFromConstructionStockpile(inventory);
		}

		public static ushort GetSubBlockId(this MySlimBlock slimBlock)
		{
			MySlimBlock cubeBlock = slimBlock.CubeGrid.GetCubeBlock(slimBlock.Position);
			MyCompoundCubeBlock myCompoundCubeBlock = null;
			if (cubeBlock != null)
			{
				myCompoundCubeBlock = (cubeBlock.FatBlock as MyCompoundCubeBlock);
			}

			if (myCompoundCubeBlock != null)
			{
				return myCompoundCubeBlock.GetBlockId(slimBlock) ?? 0;
			}

			return 0;
		}
	}
}
