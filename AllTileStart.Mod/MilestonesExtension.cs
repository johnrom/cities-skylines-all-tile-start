using ICities;

namespace AllTileStart.Mod
{
	public class MilestonesExtension : MilestonesExtensionBase
	{
		// Thread: Main
		public override void OnRefreshMilestones()
		{
			Mod.Container.TileManager.MaybeUnlockAllTilesOnStart();
		}
	}
}
