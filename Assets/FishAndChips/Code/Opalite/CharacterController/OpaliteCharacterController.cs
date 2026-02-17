using UnityEngine;
using Rewired;

namespace FishAndChips
{
    public class OpaliteCharacterController : FishScript
    {
		#region -- Inspector --
		public int PlayerId = 0;

		public float XSensitivity = 1;
		public float YSensitivity = 1;

		public Transform Orientation;

		public float CameraXRotation;
		public float CameraYRotation;
		#endregion

		#region -- Private Member Vars --
		private Player _rewiredPlayer;
		private CharacterController _characterController;

		private bool _interactionTriggered;
		private Vector2 _viewInputs = Vector2.zero;
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
			HandleCursor();
			FetchPlayer();
		}

		private void Update()
		{
			if (ReInput.isReady == false)
			{
				return;
			}
			if (_rewiredPlayer == null)
			{
				FetchPlayer();
			}
			if (_rewiredPlayer == null)
			{
				return;
			}

			GetInput();
			ProcessInput();
		}

		private void HandleCursor()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void FetchPlayer()
		{
			if (ReInput.isReady == true)
			{
				_rewiredPlayer = ReInput.players.GetPlayer(PlayerId);
			}
		}

		private void GetInput()
		{
			_interactionTriggered = _rewiredPlayer.GetButtonDown("Interact");
			_viewInputs.x = _rewiredPlayer.GetAxis("HorizontalView");
			_viewInputs.y = _rewiredPlayer.GetAxis("VerticalView");
		}

		private void ProcessInput()
		{
			if (_interactionTriggered == true)
			{
				Debug.Log("Interaction Triggered");
			}

			CameraXRotation -= _viewInputs.y * Time.deltaTime * XSensitivity;
			CameraYRotation += _viewInputs.x * Time.deltaTime * YSensitivity;

			CameraXRotation = Mathf.Clamp(CameraXRotation, -90f, 90f);

			transform.rotation = Quaternion.Euler(CameraXRotation, CameraYRotation,0);
			Orientation.rotation = Quaternion.Euler(0, CameraYRotation, 0);
		}
		#endregion
	}
}
