using ICities;
using System;

namespace AllTileStart.Library
{
    public class TileManager : ITileManager
    {
        private readonly SimulationManager _simulationManager;
        private readonly GameAreaManager _gameAreaManager;
        private readonly EconomyManager _economyManager;
        private readonly UnlockManager _unlockManager;

		private bool _isUnlockingTiles;
		private bool _hasUnlockedTiles;

		public TileManager(
			SimulationManager simulationManager,
			GameAreaManager gameAreaManager,
			EconomyManager economyManager,
			UnlockManager unlockManager
		) {
			_simulationManager = simulationManager;
			_gameAreaManager = gameAreaManager;
			_economyManager = economyManager;
			_unlockManager = unlockManager;
        }

        public bool CanUnlockTile(int x, int z, bool originalResult)
		{

			if (_isUnlockingTiles)
			{
				return !_gameAreaManager.IsUnlocked(x, z);
			}

			return originalResult;
		}

        public int GetTilePrice(int originalPrice)
		{
			if (_isUnlockingTiles)
			{
				return 0;
			}

			return originalPrice;
		}

		/// <summary>
		/// Update terrain data so that the game knows to print detailed versions of each tile instead of the basic "locked" versions.
		/// When unlocking tiles before the game is loaded, it skips this step since the game hasn't rendered yet.
		/// </summary>
		public void MaybeUpdateTerrainOnLevelLoaded(LoadMode mode)
        {
			if (mode == LoadMode.NewGame)
			{
				// refresh all tiles, even those which weren't unlocked
				var totalRows = 5;
				var totalColumns = 5;

				for (var currentRow = 0; currentRow < totalRows; currentRow++)
				{
					for (var currentColumn = 0; currentColumn < totalColumns; currentColumn++)
					{
						// Skip the outer tiles because they are not available to unlock in the base game.
						var tilesToSkip = 2;
						var sectorsPerTile = 120;

						TerrainModify.UpdateArea(
							(currentRow + tilesToSkip) * sectorsPerTile - 4, 
							(currentColumn + tilesToSkip) * sectorsPerTile - 4,
							(currentRow + tilesToSkip + 1) * sectorsPerTile + 4, 
							(currentColumn + tilesToSkip + 1) * sectorsPerTile + 4, 
							true, 
							true, 
							true
						);
					}
				}
			}
		}

        public void MaybeUnlockAllTilesOnStart()
		{
			if (_simulationManager.m_metaData.m_updateMode == SimulationManager.UpdateMode.NewGameFromMap)
			{
				if (!_hasUnlockedTiles)
				{
					// calculate original cash
					var originalCash = _economyManager.InternalCashAmount;

					// causes rendering issue in new areas
					_unlockManager.UnlockAllProgressionMilestones();

					UnlockAllTiles();

					// copy milestone info so we can reset it to default
					var MilestoneInfos = new MilestoneInfo[_unlockManager.m_allMilestones.Count];
					_unlockManager.m_allMilestones.Values.CopyTo(MilestoneInfos, 0);

					UnlockManager.ResetMilestones(MilestoneInfos, false);

					// calculated added cash
					var finalCash = _economyManager.InternalCashAmount;
					var cashDifference = Convert.ToInt32(originalCash - finalCash);

					// remove cash difference
					_economyManager.AddResource(
						EconomyManager.Resource.LoanAmount, 
						cashDifference, 
						ItemClass.Service.None, 
						ItemClass.SubService.None, 
						ItemClass.Level.None
					);

					_hasUnlockedTiles = true;
				}
			}
		}

		private void UnlockAllTiles()
        {
			_isUnlockingTiles = true;

			var totalRows = 5;
			var totalColumns = totalRows;
			
			for (var currentRow = 0; currentRow < totalRows; currentRow++)
			{
				var isFirstOrLastRow = currentRow == 0 || currentRow == 4;

				// skip the first and last row when we don't have 25 tiles enabled
				if (isFirstOrLastRow && _gameAreaManager.MaxAreaCount < 25)
				{
					continue;
				}
				for (var currentColumn = 0; currentColumn < totalColumns; currentColumn++)
				{
					var isFirstOrLastColumn = currentColumn == 0 || currentColumn == 4;

					// skip the first and last row when we don't have 25 tiles enabled
					if (isFirstOrLastColumn && _gameAreaManager.MaxAreaCount < 25)
					{
						continue;
					}
					if (!_gameAreaManager.IsUnlocked(currentColumn, currentRow))
					{
						_gameAreaManager.UnlockArea(currentRow * totalColumns + currentColumn);
					}
				}
			}

			_isUnlockingTiles = false;
		}
    }
}
