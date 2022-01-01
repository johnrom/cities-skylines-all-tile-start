using ICities;

namespace AllTileStart.Mod
{
	public class LoadingExtension : LoadingExtensionBase
	{
		public override void OnCreated(ILoading loading)
		{
			base.OnCreated(loading);

			Mod.Container.CreateGameScope();
		}

		// Thread: Main
		public override void OnLevelLoaded(LoadMode mode)
		{
			Mod.Container.TileManager.MaybeUpdateTerrainOnLevelLoaded(mode);
		}

		public override void OnLevelUnloading()
		{
			Mod.Container.DestroyGameScope();
		}
	}
}
