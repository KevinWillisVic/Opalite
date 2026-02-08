using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
    /// <summary>
    /// Set ScrollRect velocity to zero if velocity is under a specific velocity.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectVelocityClamper : MonoBehaviour
    {
        #region -- Inspector --
        public float MinimumVelocity = 1.0f;
        #endregion

        #region -- Private Member Vars --
        private bool _isHorizontal = false;
        private bool _isVertical = false;

        private ScrollRect _scrollRect;
		#endregion

		#region -- Private Methods --
		private void Awake()
		{
            _scrollRect = GetComponent<ScrollRect>();
            if (_scrollRect == null)
            {
                return;
            }
            if (_scrollRect.horizontal == true)
            {
                _isHorizontal = true;
            }
            else
            {
                _isVertical = true;
            }
		}

		private void Update()
		{
            if (_scrollRect == null)
            {
                return;
            }
            if (_scrollRect.velocity == Vector2.zero)
            {
                return;
            }

            if (_isVertical == true)
            {
                if (Mathf.Abs(_scrollRect.velocity.y) < MinimumVelocity)
                {
                    KillVelocity();
                }
            }

            if (_isHorizontal == true)
            {
                if (Mathf.Abs(_scrollRect.velocity.x) < MinimumVelocity)
                {
                    KillVelocity();
                }
            }
		}
        #endregion

        #region -- Public Methods --
        /// <summary>
        /// Set ScrollRect velocity to zero.
        /// </summary>
        public void KillVelocity()
        {
            if (_scrollRect != null)
            {
				_scrollRect.velocity = Vector2.zero;
			}
        }
        #endregion
    }
}
