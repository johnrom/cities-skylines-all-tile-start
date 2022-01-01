using ICities;

namespace AllTileStart.Mod
{
	public class Mod : IUserMod 
	{
		public string Name { get; } = "All Tile Start";
		public string Description { get; } = "All Tiles will be unlocked when starting a new game. Old saves should be preserved!";
		public static Container Container { get; private set; }

		public void OnEnabled()
		{
			Container = new Container();
		}

		public void OnDisabled()
		{
			Container = null;
		}
	}
}
