using Sandbox;
using Sandbox.Engine.Multiplayer;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.Multiplayer;
using Sandbox.Game.World;
using System;
using System.IO;
using System.Reflection;
using VRage.Game;
using VRage.Network;
using VRage.Plugins;
using VRage.Utils;

namespace ExtendedModApi
{
	public class BlockAutoWelder : IPlugin, IDisposable
	{
		private readonly Assembly self;
		private readonly MyModContext modContext;

		private readonly MyLog Logger;

		public BlockAutoWelder()
		{
			Logger = MySandboxGame.Log;
			self = Assembly.GetExecutingAssembly();
			modContext = new MyModContext();
			modContext.Init(self.FullName, self.Location, Path.Combine(Path.GetDirectoryName(self.Location), Path.GetFileNameWithoutExtension(self.Location)));
		}

		void IPlugin.Init(object gameInstance) => Init((MySandboxGame)gameInstance);
		public void Init(MySandboxGame gameInstance)
		{
			MyCubeGrids.BlockBuilt += OnBlockBuilt;
			Info("Initialized");
		}

		public void Dispose()
		{
			Info("Disposed");
			MyCubeGrids.BlockBuilt -= OnBlockBuilt;
		}

		public void Update() { }

		private void OnBlockBuilt(MyCubeGrid grid, MySlimBlock block)
		{
			if (!Sync.IsServer)
				return;

			MyPlayer player = null;
			MyInventory inventory = null;
			if (
				block.TryGetBuildingPlayer(out player) &&
				player.TryGetInventory(out inventory)
			) {
				if(block.TryFillStockpile(inventory))
				{
					var finishBlockMountAmount = (block.MaxIntegrity - block.Integrity) / block.BlockDefinition.IntegrityPointsPerSec;

					if (!block.IncreaseMountLevel(finishBlockMountAmount, player.Identity.IdentityId, inventory, 0, false, MyOwnershipShareModeEnum.Faction, true))
					{
						Warn($"Could not increase mount level of '{block.BlockDefinition.DisplayNameText}' from '{player.DisplayName}'");
						// TODO: what should happen? when does this happen?
					}
					else
					{
						Info($"Successfully autowelded '{block.BlockDefinition.DisplayNameText}' at'{block.Position}'");
						grid.OnIntegrityChanged(block, true);
						// TODO: this doesn't update the client, why? What event do we need to send to the client?
					}
				}
				else
				{
					Warn($"Could not fill stockpile of '{block.BlockDefinition.DisplayNameText}' from '{player.DisplayName}'");
					block.TryDrainStockpile(inventory);
					grid.RemoveBlock(block, true);
					MyMultiplayer.RaiseEvent(grid, g => g.BuildBlocksFailedNotify, new EndpointId(player.Id.SteamId));
				}
			}
			else if (player  == null)
			{
				Warn($"Could not find player {block.BuiltBy}");
			}
			else if (inventory == null)
			{
				Warn($"Could not find inventory of '{player.DisplayName}'");
			}
		}

		private void Log(MyLogSeverity severity, string message) => Logger.WriteLineToConsole($"[{nameof(BlockAutoWelder)}, {severity}] {message}");
		private void Warn(string message) => Log(MyLogSeverity.Warning, message);
		private void Info(string message) => Log(MyLogSeverity.Info, message);
	}
}
