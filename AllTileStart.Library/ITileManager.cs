using ICities;

namespace AllTileStart.Library
{
    public interface ITileManager
    {
        void MaybeUnlockAllTilesOnStart();
        void MaybeUpdateTerrainOnLevelLoaded(LoadMode mode);
        int GetTilePrice(int originalPrice);
        bool CanUnlockTile(int x, int y, bool originalResult);
    }
}
