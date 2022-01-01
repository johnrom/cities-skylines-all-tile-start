using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllTileStart.Library
{
    public class TileManager : ITileManager
    {
        private readonly SimulationManager _simulationManager;
        private readonly GameAreaManager _gameAreaManager;

        private bool _hasUnlockedTiles;

        public TileManager(
			SimulationManager simulationManager,
			GameAreaManager gameAreaManager
		) {
			_simulationManager = simulationManager;
			_gameAreaManager = gameAreaManager;
        }

        public void MaybeUnlockAllTilesOnStart()
		{
			if (_simulationManager.m_metaData.m_updateMode == SimulationManager.UpdateMode.NewGameFromMap)
			{
				if (!_hasUnlockedTiles)
				{
					// calculate original cash
					var originalCash = EconomyManager.instance.InternalCashAmount;

					// causes rendering issue in new areas
					Singleton<UnlockManager>.instance.UnlockAllProgressionMilestones();

					UnlockAllTiles();

					// copy milestone info so we can reset it to default
					MilestoneInfo[] MilestoneInfos = new MilestoneInfo[UnlockManager.instance.m_allMilestones.Count];
					UnlockManager.instance.m_allMilestones.Values.CopyTo(MilestoneInfos, 0);

					UnlockManager.ResetMilestones(MilestoneInfos, false);

					// calculated added cash
					long finalCash = EconomyManager.instance.InternalCashAmount;
					int cashDifference = Convert.ToInt32(originalCash - finalCash);

					// remove cash difference
					EconomyManager.instance.AddResource(EconomyManager.Resource.LoanAmount, cashDifference, ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);

					_hasUnlockedTiles = true;
				}
			}
		}

		private void UnlockAllTiles()
        {
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
		}
    }
}
