using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.World;

namespace ExtendedModApi
{
	internal static class MyPlayerExtensions
	{
		public static bool TryGetInventory(this MyPlayer player, out MyInventory inventory)
		{
			inventory = player.Character?.GetInventory();
			return inventory != null;
		}
	}
}
