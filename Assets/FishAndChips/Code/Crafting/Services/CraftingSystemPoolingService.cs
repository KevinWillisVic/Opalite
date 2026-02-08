using UnityEngine;
using UnityEngine.Pool;

namespace FishAndChips
{
	/// <summary>
	/// Service handling pooling objects.
	/// </summary>
	public class CraftingSystemPoolingService : Singleton<CraftingSystemPoolingService>, IInitializable, ICleanable
	{
		#region -- Inspector --
		[Header("Craft Item Instance Pool")]
		[Tooltip("The number of items to pre-populate the CraftItemInstance pool with.")]
		public int DefaultCraftItemInstancePoolSize = 50;
		[Tooltip("The template for CraftItemInstance's")]
		public CraftItemInstance CraftItemInstanceTemplate;

		[Header("Previously Made Indicator Instance Pool")]
		[Tooltip("The number of items to pre-populate the PreviouslyMadeIndicatorInstance pool with.")]
		public int DefaultPreviouslyMadeIndicatorInstancePoolSize = 50;
		[Tooltip("The template for PreviouslyMadeIndicatorInstance's")]
		public PreviouslyMadeIndicatorInstance PreviouslyMadeIndicatorInstanceTemplate;
		#endregion

		#region -- Protected Member Vars --
		// Services.
		protected UIService _uiService;
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Member Vars --
		private SimpleGameplayBoard _gameplayBoard;

		// Pools.
		private ObjectPool<CraftItemInstance> _craftItemInstancePool;
		private ObjectPool<PreviouslyMadeIndicatorInstance> _previouslyMadeIndicatorInstancePool;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Subscribe to events.
		/// </summary>
		private void SubscribeListeners()
		{
			EventManager.SubscribeEventListener<PoolPopulationReady>(OnPoolPopulationReady);
		}

		/// <summary>
		/// Unsubscribe from events.
		/// </summary>
		private void UnsubscribeListeners()
		{
			EventManager.UnsubscribeEventListener<PoolPopulationReady>(OnPoolPopulationReady);
		}

		/// <summary>
		/// Callback for event to create pools.
		/// </summary>
		private void OnPoolPopulationReady(PoolPopulationReady gameEvent)
		{
			var gameplaySceneView = _uiService.GetView<GameplaySceneView>();
			if (gameplaySceneView != null)
			{
				_gameplayBoard = gameplaySceneView.SimpleGameplayBoard;
			}
			CreatePools();
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Setup service.
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			SubscribeListeners();
			// Services.
			_uiService = UIService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;
		}

		/// <summary>
		/// Cleanup service.
		/// </summary>
		public override void Cleanup()
		{
			base.Cleanup();
			UnsubscribeListeners();
		}

		/// <summary>
		/// Create PreviouslyMadeIndicatorInstance pool, and CraftItemInstance pool.
		/// </summary>
		public virtual void CreatePools()
		{
			// CraftItemInstance pool.
			_craftItemInstancePool = new ObjectPool<CraftItemInstance>(() =>
			{
				var instance = Instantiate(CraftItemInstanceTemplate);
				instance.transform.SetParent(_gameplayBoard.CraftingLayer);
				instance.transform.ResetScale();
				return instance;
			}
			, craftItem =>
			{
			}
			, craftItem => craftItem.SetActiveSafe(false)
			, craftItem => Destroy(craftItem.gameObject)
			, false
			, DefaultCraftItemInstancePoolSize);

			// PreviouslyMadeIndicatorInstance pool.
			_previouslyMadeIndicatorInstancePool = new ObjectPool<PreviouslyMadeIndicatorInstance>(() =>
			{
				var instance = Instantiate(PreviouslyMadeIndicatorInstanceTemplate);
				instance.transform.SetParent(_gameplayBoard.PopupLayer);
				instance.transform.ResetScale();
				return instance;
			},
			indicator =>
			{
			}
			, indicator => indicator.SetActiveSafe(false)
			, indicator => Destroy(indicator.gameObject)
			, false
			, DefaultPreviouslyMadeIndicatorInstancePoolSize);
		}

		/// <summary>
		/// Return a PreviouslyMadeIndicatorInstance from the pool.
		/// </summary>
		/// <returns>A PreviouslyMadeIndicatorInstance from the pool.</returns>
		public PreviouslyMadeIndicatorInstance GetPreviouslyMadeInstancePoolElement()
		{
			return _previouslyMadeIndicatorInstancePool.Get();
		}

		/// <summary>
		/// Add a PreviouslyMadeIndicatorInstance back to the pool.
		/// </summary>
		/// <param name="instance">Instance being added back to pool.</param>
		public void PoolPreviouslyMadeIndicatorInstance(PreviouslyMadeIndicatorInstance instance)
		{
			_previouslyMadeIndicatorInstancePool.Release(instance);
		}

		/// <summary>
		/// Clear PreviouslyMadeIndicatorInstance pool.
		/// </summary>
		public void ClearPreviouslyMadeIndicatorInstancePool()
		{
			_previouslyMadeIndicatorInstancePool.Clear();
		}

		/// <summary>
		/// Return a CraftItemInstance from the pool.
		/// </summary>
		/// <returns>A CraftItemInstance from the pool.</returns>
		public CraftItemInstance GetCraftItemInstancePoolElement()
		{
			return _craftItemInstancePool.Get();
		}

		/// <summary>
		/// Add a CraftItemInstance back to the pool.
		/// </summary>
		/// <param name="instance">Instance being added back to pool.</param>
		public void PoolCraftItemInstance(CraftItemInstance instance)
		{
			_craftItemInstancePool.Release(instance);
		}

		/// <summary>
		/// Clear CraftItemInstance pool.
		/// </summary>
		public void ClearCraftItemInstancePool()
		{
			_craftItemInstancePool.Clear();
		}
		#endregion
	}
}
