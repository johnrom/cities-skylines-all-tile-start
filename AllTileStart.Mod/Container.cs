using AllTileStart.Library;
using ColossalFramework;

namespace AllTileStart
{
    public class Container
    {
        public ITileManager TileManager { get; private set; }

        public void CreateGameScope()
        {
            TileManager = new TileManager(
                Singleton<SimulationManager>.instance,
                Singleton<GameAreaManager>.instance,
                Singleton<EconomyManager>.instance,
                Singleton<UnlockManager>.instance
            );
        }

        public void DestroyGameScope()
        {
            TileManager = null;
        }
    }
}
