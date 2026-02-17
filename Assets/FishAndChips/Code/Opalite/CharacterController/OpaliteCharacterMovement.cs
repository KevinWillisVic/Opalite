using UnityEngine;
using Rewired;

namespace FishAndChips
{
	[RequireComponent(typeof(Rigidbody))]
	public class OpaliteCharacterMovement : MonoBehaviour
	{
		#region -- Inspector --
		[Header("Movement")]
		public float MoveSpeed;
		public Transform Orientation;
		public int PlayerId = 0;
		#endregion

		#region -- Private Member Vars --
		private Rigidbody _rigidBody;
		private Vector3 _moveDirection;

		private float _horizontalInput;
		private float _verticalInput;

		private Player _rewiredPlayer;
		private Vector2 _movementInputs = Vector2.zero;
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
			FetchPlayer();
		}

		private void Start()
		{
			_rigidBody = GetComponent<Rigidbody>();
			_rigidBody.freezeRotation = true;
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
			_movementInputs.x = _rewiredPlayer.GetAxis("HorizontalMovement");
			_movementInputs.y = _rewiredPlayer.GetAxis("VerticalMovement");

			_horizontalInput = _movementInputs.x;
			_verticalInput = _movementInputs.y;
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

			if (Orientation == null || _rigidBody == null)
			{
				return;
			}
			GetInput();
		}

		private void FixedUpdate()
		{
			ProcessInput();
		}

		private void ProcessInput()
		{
			_moveDirection = (Orientation.forward * _verticalInput) + (Orientation.right * _horizontalInput);
			_rigidBody.AddForce(_moveDirection * MoveSpeed, ForceMode.Force);
		}
		#endregion
	}
}
