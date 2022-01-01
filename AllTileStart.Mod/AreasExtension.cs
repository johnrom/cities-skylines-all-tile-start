using ICities;

namespace AllTileStart.Mod
{
	public class AreasExtension : AreasExtensionBase
	{
		public override int OnGetAreaPrice(
			uint ore,
			uint oil,
			uint forest,
			uint fertility,
			uint water,
			bool road,
			bool train,
			bool ship,
			bool plane,
			float landFlatness,
			int originalPrice
		) {
			return Mod.Container.TileManager.GetTilePrice(originalPrice);
		}

		public override bool OnCanUnlockArea(
			int x,
			int z,
			bool originalResult
		) {
			return Mod.Container.TileManager.CanUnlockTile(x, z, originalResult);
		}
	}
}
