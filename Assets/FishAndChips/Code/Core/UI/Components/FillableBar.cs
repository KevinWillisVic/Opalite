using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

namespace FishAndChips
{
    public class FillableBar : MonoBehaviour
    {
		#region -- Properties --
		public float MaxValue { get { return (float)_maxValueDouble; } }
		public float MinValue { get { return (float)_minValueDouble; } }
		public float CurrentValue { get { return (float)_currentValueDouble; } }
		#endregion

		#region -- Inspector --
		public Slider Slider;
		public Image Foreground;
		public Image Handle;
		public bool ShowMaxValue = true;
		public bool AlwaysAnimateFromZero = false;
		public bool CanGoAboveMax = false;
		public bool JumpToValue = false;
		public float FillDuration = 0.1f;
		public Action OnFillCompleted;
		#endregion

		#region -- Private Member Vars --
		private double _maxValueDouble = 100d;
		private double _minValueDouble = 0;
		private double _currentValueDouble = 0;
		private double _previousPercent = 0;
		private double _tweenTarget = 0;
		#endregion

		#region -- Private Methods --
		private void UpdateHandle(double handlePercentOfMax)
		{
			if (Handle == null)
			{
				return;
			}
			Handle.rectTransform.anchorMin = new Vector2((float)handlePercentOfMax, 0.5f);
			Handle.rectTransform.anchorMax = new Vector2((float)handlePercentOfMax, 0.5f);
			Handle.rectTransform.anchoredPosition = Vector3.zero;
		}

		private void UpdateValue(double value)
		{
			float newValue = (float)value;

			if (Foreground != null)
			{
				if (Foreground.fillAmount.CompareEqual(newValue) == false)
				{
					Foreground.fillAmount = newValue;
				}
			}

			if (Slider != null)
			{
				if (Slider.normalizedValue.CompareEqual(newValue) == false)
				{
					Slider.normalizedValue = newValue;
				}
			}

			UpdateHandle(newValue);
		}

		private async Task FillBarAsync(double startValue, double target)
		{
			float timeRemaining = FillDuration;

			float percentComplete = 0;
			while (timeRemaining > 0)
			{
				timeRemaining -= Time.unscaledDeltaTime;
				percentComplete = Mathf.Clamp01((FillDuration - timeRemaining) / FillDuration);
				double currentValue = startValue + ((target - startValue) * percentComplete);
				UpdateValue(currentValue);

				await Awaitable.EndOfFrameAsync();
			}

			UpdateValue(target);
			OnFillCompleted.FireSafe();
		}

		private void SetCurrentValueInternal()
		{
			UpdateValue(_tweenTarget);
			OnFillCompleted.FireSafe();
		}

		private async Task SetCurrentValueInternalAsync()
		{
			double startValue = 0;

			if (AlwaysAnimateFromZero == false)
			{
				startValue = _previousPercent;
			}

			UpdateValue(startValue);
			await FillBarAsync(startValue, _tweenTarget);
		}

		private void UpdateCurrentValue(double value)
		{
			_previousPercent = (_currentValueDouble - _minValueDouble) / (_maxValueDouble - _minValueDouble);
			_currentValueDouble = value;

			_tweenTarget = (value - _minValueDouble) / (_maxValueDouble - _minValueDouble);
			_tweenTarget = CanGoAboveMax == true ? _tweenTarget : _tweenTarget.Clamp(0, 1);
		}
		#endregion

		#region -- Public Methods --
		public void SetCurrentValue(float value)
		{
			SetCurrentValue((double)value);
		}

		public async void SetCurrentValue(double value)
		{
			if (JumpToValue == false && FillDuration > 0)
			{
				await SetCurrentValueAsync(value);
			}
			else
			{
				SetCurrentValueNonAsync(value);
			}
		}

		public void SetCurrentValueNonAsync(float value)
		{
			SetCurrentValueNonAsync((double)value);
		}

		public void SetCurrentValueNonAsync(double value)
		{
			UpdateCurrentValue(value);
			SetCurrentValueInternal();
		}

		public async Task SetCurrentValueAsync(float value)
		{
			await SetCurrentValueAsync((double)value);
		}

		public async Task SetCurrentValueAsync(double value)
		{
			GameObject objectToUse = null;
			if (Foreground != null)
			{
				objectToUse = Foreground.gameObject;
			}
			else if (Slider != null)
			{
				objectToUse = Slider.gameObject;
			}


			UpdateCurrentValue(value);

			if (objectToUse != null && JumpToValue == false && FillDuration > 0)
			{
				await SetCurrentValueInternalAsync();
			}
			else
			{
				SetCurrentValueInternal();
			}
		}

		public void SetMinValue(float value)
		{
			SetMinValue((double)value);
		}

		public void SetMinValue(double value)
		{
			_minValueDouble = value;
		}

		public void SetMaxValue(float value, bool setCurrentAsMax = true)
		{
			SetMaxValue((double)value, setCurrentAsMax);
		}

		public void SetMaxValue(double value, bool setCurrentAsMax = true)
		{
			value = value.IsZero() == true ? 1 : value;
			_maxValueDouble = value;

			if (setCurrentAsMax == true)
			{
				_currentValueDouble = _maxValueDouble;
			}
		}
		#endregion
	}
}
