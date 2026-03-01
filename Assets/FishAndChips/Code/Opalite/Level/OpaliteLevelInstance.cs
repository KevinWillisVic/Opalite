using UnityEngine;
using System;
using System.Collections.Generic;

namespace FishAndChips
{
    public class OpaliteLevelInstance : FishScript
    {
		#region -- Inspector --
		[Header("Position Helpers")]
		public Transform PlayerTransform;
		public Transform StartingTransform;
		public BoxCollider LevelEndTrigger;

		[Header("Puzzle Collection")]
		public List<OpalitePuzzleData> PuzzleList = new();
		#endregion

		#region -- Private Member Vars --
		private int _currentPuzzleIndex;
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
			Initialize();
		}

		private void OnResetLevel(OpaliteResetLevelEvent resetEvent)
		{
			ResetScene(resetEvent.Passed);
		}

		private void ResetPlayerPosition()
		{
			var currentPosition = PlayerTransform.position;

			// New z position.
			float adjustedZPosition = StartingTransform.position.z;

			// New y position.
			// Remap
			float levelEndTriggerXPosition = LevelEndTrigger.transform.position.x;
			float levelEndTriggerXScale = LevelEndTrigger.size.x;

			float min = levelEndTriggerXPosition - (levelEndTriggerXScale / 2.0f);
			float max = levelEndTriggerXPosition + (levelEndTriggerXScale / 2.0f);

			// TODO : This only works on the -1 to 1 mapping, if its a different value it must be updated.
			float adjustedXPosition = currentPosition.x.Remap(min, max, -1f, 1f);

			currentPosition = currentPosition.WithX(adjustedXPosition).WithZ(adjustedZPosition);
			PlayerTransform.position = currentPosition;
		}

		private void ResetLevelVisuals()
		{
		}


		private void Initialize()
		{
			_currentPuzzleIndex = 0;
			ResetPuzzleData();
			ResetLevelVisuals();
		}

		private void ResetPuzzleData()
		{
			foreach (var puzzle in PuzzleList)
			{
				if (puzzle == null)
				{
					continue;
				}
				puzzle.IsPuzzleSolved = false;
				puzzle.IsActivePuzzle = false;
			}
		}

		private void OnFailLevel()
		{
			_currentPuzzleIndex = 0;
			ResetPuzzleData();
			ResetLevelVisuals();
		}
		#endregion

		#region -- Protected Methods --
		protected override void SubscribeEventListeners()
		{
			base.SubscribeEventListeners();
			EventManager.SubscribeEventListener<OpaliteResetLevelEvent>(OnResetLevel);
		}

		protected override void UnsubscribeEventListeners()
		{
			base.UnsubscribeEventListeners();
			EventManager.UnsubscribeEventListener<OpaliteResetLevelEvent>(OnResetLevel);
		}

		protected virtual void AdvancePuzzle()
		{
			if (_currentPuzzleIndex == PuzzleList.Count - 1)
			{
				Debug.Log("Should be finishing the game.");
			}
			else
			{
				_currentPuzzleIndex++;
				_currentPuzzleIndex = Math.Clamp(_currentPuzzleIndex, 0, PuzzleList.Count - 1);
			}
		}
		#endregion

		#region -- Public Methods --
		public void ResetScene(bool passed)
		{
			ResetPlayerPosition();

			if (passed == true)
			{
				AdvancePuzzle();
			}
			else
			{
				OnFailLevel();
			}
		}
		#endregion
	}
}
