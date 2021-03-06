﻿using System;
using ICities;
using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace AllTileStartMod {

	public class AllTileStart: IUserMod {

		public string Name {
			get { return "Unlock All Tiles at Start"; }
		}

		public string Description {
			get { return "All Tiles will be Unlocked When Starting a New Game. Old saves should be preserved!"; }
		}
	}

	public class ATSAreasExtensionBase: AreasExtensionBase {

		public override int OnGetAreaPrice(uint ore, uint oil, uint forest, uint fertility, uint water, bool road, bool train, bool ship, bool plane, float landFlatness, int originalPrice) {

			if ( MilestonesExtension.Instance == null || ! MilestonesExtension.Instance.unlockedAreas) {
				originalPrice = 0;
			}

			return originalPrice;
		}

		public override bool OnCanUnlockArea(int x, int z, bool originalResult) {

			if ( MilestonesExtension.Instance == null || ! MilestonesExtension.Instance.unlockedAreas) {

				if (! GameAreaManager.instance.IsUnlocked(x, z) ) {
					originalResult = true;
				}
			}

			return originalResult;
		}
	}

	public class MilestonesExtension: MilestonesExtensionBase {
		public static MilestonesExtension Instance { get; private set; }
		public IManagers Managers;	

		public bool unlockedAreas = false;

		// Thread: Main
		public override void OnCreated(IMilestones milestones) {
			Instance = this;
			Managers = milestones.managers;
		}

		// Thread: Main
		public override void OnRefreshMilestones() {

			if (Singleton<SimulationManager>.instance.m_metaData.m_updateMode == SimulationManager.UpdateMode.NewGame) {
				
				// only do once
				if (! unlockedAreas) {
					IAreas AreasManager = Managers.areas;

					// calculate original cash
					long originalCash = EconomyManager.instance.InternalCashAmount;

					// causes rendering issue in new areas
					Singleton<UnlockManager>.instance.UnlockAllProgressionMilestones();

					// unlock all tiles
					int rows = (int)Math.Sqrt (AreasManager.maxAreaCount);

					for (int x = 0; x < rows; x++) {

						for (int y = 0; y < rows; y++) {
							int column = x;
							int row = y;

							if (rows.Equals (3)) {
								column = x + 1;
								row = y + 1;
							}

							if (!AreasManager.IsAreaUnlocked (column, row) && AreasManager.CanUnlockArea(column, row) ) {
								AreasManager.UnlockArea(column, row, false);
							}
						}         
					}

					// copy milestone info so we can reset it to default
					MilestoneInfo[] MilestoneInfos = new MilestoneInfo[UnlockManager.instance.m_allMilestones.Count];
					UnlockManager.instance.m_allMilestones.Values.CopyTo (MilestoneInfos, 0);

					UnlockManager.ResetMilestones(MilestoneInfos, false);

					// calculated added cash
					long finalCash = EconomyManager.instance.InternalCashAmount;
					int cashDifference = Convert.ToInt32(originalCash - finalCash);

					// remove cash difference
					EconomyManager.instance.AddResource(EconomyManager.Resource.LoanAmount, cashDifference, ItemClass.Service.None, ItemClass.SubService.None, ItemClass.Level.None);
				}
			}

			unlockedAreas = true;
		}
	}


	public class LoadingExtension: LoadingExtensionBase {

        // Thread: Simulation
		public override void OnLevelLoaded(LoadMode mode) {

            // check it is new game
			if ( mode.Equals( ICities.LoadMode.NewGame ) ) {
				// update terrain data so that the game knows to print detailed versions of each tile instead of the basic "locked" versions
				// when unlocking tiles before the game is loaded, it skips this step since the game hasn't rendered yet

				// refresh all tiles, even those which weren't unlocked
				int rows = 5;

				for (int x = 0; x < rows; x++) {

					for (int z = 0; z < rows; z++) {
						int column = x;
						int row = z;

						if (rows.Equals (3)) {
							column = x + 1;
							row = z + 1;
						}
							
						int skip = 2;
						int num = 120;
						TerrainModify.UpdateArea( ( x + skip ) * num - 4, ( z + skip ) * num - 4, ( ( x + skip ) + 1) * num + 4, ( ( z + skip ) + 1) * num + 4, true, true, true);
					}         
				}
            }
		}
	}
}
