using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace FishAndChips
{
	public class CraftingSystemCraftingService : Singleton<CraftingSystemCraftingService>, IInitializable
	{
		#region -- Properties --
		/// <summary>
		/// Get all CraftItem entities in game.
		/// </summary>
		public List<CraftItemEntity> CraftItemEntities => _craftItemEntities;

		/// <summary>
		/// Get all CraftRecipe entities in game.
		/// </summary>
		public List<CraftRecipeEntity> CraftRecipeEntities => _craftRecipeEntities;

		/// <summary>
		/// Get the gameplay baord.
		/// </summary>
		public SimpleGameplayBoard GameplayBoard => _gameplayBoard;

		/// <summary>
		/// Current recyclign state of the board.
		/// </summary>
		public SimpleGameplayBoard.eRecycleState CurrentRecycleState => GameplayBoard != null ? GameplayBoard.RecycleState : FishAndChips.GameplayBoard.eRecycleState.UndoState;
		#endregion

		#region -- Public Member Vars --
		// A mapping of which recipe creates a CraftItemEntity.
		// With the key being the ID of the CraftItemEntity in which we want to see all of the recipes that create it.
		// Get recipes in which the entity is a product.
		public Dictionary<string, List<CraftRecipeEntity>> ProductRecipes = new();

		// A mapping of which CraftItemEntity is being used as an ingredient.
		// With the key being the ID of the CraftItemEntity in which we want to see all of the recipes in which it is an ingredient.
		// Get recipes in wich the entity is a ingredient.
		public Dictionary<string, List<CraftRecipeEntity>> IngredientRecipes = new();
		#endregion

		#region -- Protected Member Vars --
		protected UIService _uiService;
		protected EntityService _entityService;
		protected CraftingSystemDataService _dataService;
		protected CraftingSystemSavingService _saveService;
		protected CraftingSystemPoolingService _poolingService;
		#endregion

		#region -- Private Member Vars --
		// Collisions.
		private PointerEventData _pointerEventData;
		private List<RaycastResult> _collisionResults = new();
		private List<CraftItemInstance> _collidedWithCraftItemInstances = new();

		// Entities.
		private List<CraftItemEntity> _craftItemEntities = new();
		private List<CraftRecipeEntity> _craftRecipeEntities = new();

		// Board.
		private SimpleGameplayBoard _gameplayBoard;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Subscribe to game events.
		/// </summary>
		private void SubscribeListeners()
		{
			// Life-cycle.
			EventManager.SubscribeEventListener<OnGameSceneReady>(OnGameSceneReady);

			// Board.
			EventManager.SubscribeEventListener<PositionSaveObjectRemovedEvent>(OnObjectRemovedFromBoard);

			// Selection.
			EventManager.SubscribeEventListener<CraftItemSelectionEvent>(OnSelectCraftItemInstance);
			EventManager.SubscribeEventListener<CraftItemReleasedEvent>(OnReleaseCraftItemInstance);
		}

		/// <summary>
		/// Unsubscribe from game events.
		/// </summary>
		private void UnsubscribeListeners()
		{
			// Life-cycle.
			EventManager.UnsubscribeEventListener<OnGameSceneReady>(OnGameSceneReady);

			// Board.
			EventManager.UnsubscribeEventListener<PositionSaveObjectRemovedEvent>(OnObjectRemovedFromBoard);

			//Selection.
			EventManager.UnsubscribeEventListener<CraftItemSelectionEvent>(OnSelectCraftItemInstance);
			EventManager.UnsubscribeEventListener<CraftItemReleasedEvent>(OnReleaseCraftItemInstance);
		}

		/// <summary>
		/// Callback for the when the game scene is ready.
		/// </summary>
		/// <param name="gameEvent">The OnGameSceneReady event that was raised.</param>
		private void OnGameSceneReady(OnGameSceneReady gameEvent)
		{
			var gameplaySceneView = _uiService.GetView<GameplaySceneView>();
			if (gameplaySceneView != null)
			{
				_gameplayBoard = gameplaySceneView.SimpleGameplayBoard;
			}
			PopulateBoardFromSavedState(_saveService.BoardSaveState);
		}

		/// <summary>
		/// Handle what happens when the object should no longer be tracked on the game board.
		/// </summary>
		private void OnObjectRemovedFromBoard(PositionSaveObjectRemovedEvent gameEvent)
		{
			if (gameEvent == null || gameEvent.CraftItemInstance == null)
			{
				return;
			}

			var instance = gameEvent.CraftItemInstance;
			instance.IsSelected = false;
			instance.IsInteractable = false;

			// Add instance back to the pool.
			if (gameEvent.RePoolImmediate == true)
			{
				_poolingService.PoolCraftItemInstance(instance);
			}
			else
			{
				DelayPoolCraftItem(instance, gameEvent.WaitTimeBeforeRepool);
			}
		}

		/// <summary>
		/// Wait a set amount of time before the CraftItemInstance is re-pooled.
		/// </summary>
		/// <param name="instance">The CraftItemInstance that will be re-pooled.</param>
		/// <param name="delay">The delay before re-pooling the CraftItemInstance.</param>
		private async void DelayPoolCraftItem(CraftItemInstance instance, float delay)
		{
			if (instance == null)
			{
				return;
			}
			if (delay > 0)
			{
				await Awaitable.WaitForSecondsAsync(delay);
			}
			if (instance != null)
			{
				_poolingService.PoolCraftItemInstance(instance);
			}
		}


		/// <summary>
		/// Spawn in instances to match the saved state of the board.
		/// </summary>
		/// <param name="state">Save data corresponding to the board.</param>
		private void PopulateBoardFromSavedState(BoardSaveInfo state)
		{
			if (state == null)
			{
				Logger.LogError("CraftingSystemCraftingService.PopulateBoardFromSavedState : state was null.");
				return;
			}

			if (_gameplayBoard == null)
			{
				Logger.LogError("CraftingSystemCraftingService.PopulateBoardFromSavedState : game board was null.");
				return;
			}

			var trackedElements = state.SavedElements;
			foreach (var element in trackedElements)
			{
				if (element == null)
				{
					continue;
				}
				var entity = FetchCraftItemEntity(element.ID);
				if (entity == null)
				{
					Logger.LogWarning($"Could not fetch craft item from ID {element.ID}.");
					continue;
				}

				// TODO : Check on logic here, should a depleted or final item every be spawned in.
				if (IsDepletedItem(entity) == true
					|| IsFinalItem(entity) == true)
				{
					continue;
				}

				// TODO : Trigger board spawn animation.
				var instance = SpawnAndReturnCraftItemInstance(entity,
					element.Position,
					triggerSaveEvent: false,
					spawnAnimation: string.Empty);

				element.RuntimeInstance = instance;
			}
		}


		/// <summary>
		/// Callback for when a CraftItemInstance is selected.
		/// </summary>
		/// <param name="gameEvent">The CraftItemSelectionEvent event that was raised.</param>
		private void OnSelectCraftItemInstance(CraftItemSelectionEvent gameEvent)
		{
			if (gameEvent == null || gameEvent.CraftItemInstance == null)
			{
				return;
			}
			var instance = gameEvent.CraftItemInstance;
			if (instance.IsInteractable == false)
			{
				return;
			}
			PlaceOnDragLayer(instance);
		}

		/// <summary>
		/// Callback for when a CraftItemInstance is released.
		/// </summary>
		/// <param name="gameEvent">The CraftItemReleasedEvent event that was raised.</param>
		private void OnReleaseCraftItemInstance(CraftItemReleasedEvent gameEvent)
		{
			if (gameEvent == null || gameEvent.CraftItemInstance == null)
			{
				return;
			}
			var instance = gameEvent.CraftItemInstance;

			// Clean up item if it was placed out of bounds.
			if (CheckIfReleasedInPlayRegion() == false)
			{
				// TODO : Prevent this item from being selected.
				instance.PlayAnimation(CraftItemInstance.eCraftItemAnimationKeys.InvalidCombo.ToString());
				float waitTime = instance.GetCurrentPlayingDirector() != null ? instance.GetCurrentPlayingDirectorLength() : 1;
				instance.Recycle(false, waitTime);
				return;
			}

			EventManager.TriggerEvent(new PositionSaveObjectAddedEvent(instance));

			PlaceOnCraftingLayer(instance);
			if (AttemptCrafting(instance) == false)
			{
				instance.AttemptCloning();
			}

			// Event to trigger a board save.
			EventManager.TriggerEvent(new GeneralPositionSaveEvent());
		}

		/// <summary>
		/// Determine if the user released the CraftItemInstance in the playable area.
		/// </summary>
		/// <returns>True if released in the playable area, false otherwise.</returns>
		private bool CheckIfReleasedInPlayRegion()
		{
			FetchCollisionResults();
			if (_collisionResults.Count == 0)
			{
				return false;
			}

			// As long as one of the collided with objects is the crafting region they are in the playable region.
			foreach (var result in _collisionResults)
			{
				if (result.gameObject.CompareTag(GameConstants.CraftingRegionTag))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Populate the collision results from under the click.
		/// </summary>
		private void FetchCollisionResults()
		{
			_collisionResults.Clear();

			Vector3 pointerPosition = Vector3.zero;
			pointerPosition = Mouse.current.position.value;
			_pointerEventData.position = pointerPosition;
			EventSystem.current.RaycastAll(_pointerEventData, _collisionResults);
			/*
			if (Input.touchCount > 0)
			{
				_pointerEventData.position = Input.GetTouch(0).position;
				EventSystem.current.RaycastAll(_pointerEventData, _collisionResults);
			}
			else if (Input.mousePresent == true)
			{
				_pointerEventData.position = Input.mousePosition;
				EventSystem.current.RaycastAll(_pointerEventData, _collisionResults);
			}
			*/
		}

		/// <summary>
		/// Set up dictionaries to make recipe queries easier.
		/// </summary>
		private void CreateRecipeMaps()
		{
			if (_craftRecipeEntities == null || _craftRecipeEntities.Count == 0)
			{
				return;
			}

			foreach (var recipe in _craftRecipeEntities)
			{
				var data = recipe.CraftRecipeData;
				// Set up map of which recipes can be used to create a CraftItem.
				foreach (var product in data.RecipeProducts)
				{
					if (ProductRecipes.ContainsKey(product) == false)
					{
						List<CraftRecipeEntity> entityList = new();
						ProductRecipes.Add(product, entityList);
					}
					ProductRecipes[product].Add(recipe);
				}

				// Set up map in which the CraftItem is used as an ingredient
				foreach (var ingredient in data.IngredientMap)
				{
					var id = ingredient.IngredientKey;
					if (IngredientRecipes.ContainsKey(id) == false)
					{
						List<CraftRecipeEntity> entityList = new();
						IngredientRecipes.Add(id, entityList);
					}
					if (IngredientRecipes[id].Contains(recipe) == false)
					{
						IngredientRecipes[id].Add(recipe);
					}
				}
			}
		}

		/// <summary>
		/// On completion of a recipe check if any of the items have now become depleted.
		/// </summary>
		/// <param name="recipe">CraftRecipe that was just constructed.</param>
		private void CheckAddingDepletedKeywordPostRecipe(CraftRecipeEntity recipe)
		{
			if (recipe == null)
			{
				return;
			}

			var recipeData = recipe.CraftRecipeData;
			foreach (var product in recipeData.RecipeProducts)
			{
				var craftItemEntity = FetchCraftItemEntity(product);
				if (craftItemEntity == null)
				{
					continue;
				}
				if (IsDepletedItem(craftItemEntity) == true)
				{
					craftItemEntity.ActivateTempKeyword(eCraftItemKeyword.Depleted);
				}
			}

			foreach (var kvp in recipeData.IngredientMap)
			{
				var key = kvp.IngredientKey;
				var craftItemEntity = FetchCraftItemEntity(key);
				if (craftItemEntity == null)
				{
					continue;
				}
				if (IsDepletedItem(craftItemEntity) == true)
				{
					craftItemEntity.ActivateTempKeyword(eCraftItemKeyword.Depleted);
				}
			}
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			SubscribeListeners();

			_entityService = EntityService.Instance;
			_uiService = UIService.Instance;
			_dataService = CraftingSystemDataService.Instance;
			_saveService = CraftingSystemSavingService.Instance;
			_poolingService = CraftingSystemPoolingService.Instance;

			_pointerEventData = new PointerEventData(EventSystem.current);
		}

		public override void Cleanup()
		{
			base.Cleanup();
			UnsubscribeListeners();
		}

		public virtual void Load()
		{
			_craftItemEntities = _entityService.LoadEntities<CraftItemEntity, CraftItemData>().ToList();
			_craftRecipeEntities = _entityService.LoadEntities<CraftRecipeEntity, CraftRecipeData>().ToList();

			CreateRecipeMaps();

			// Make sure relevant keywords and starting states are set.
			foreach (var entity in _craftItemEntities)
			{
				entity.EnsureStartingValues();
			}

		}

		/// <summary>
		/// Return CraftItemEntity matching identifier.
		/// </summary>
		/// <param name="id">Id of CraftItemEntity.</param>
		/// <returns>Matching CraftItemEntity.</returns>
		public CraftItemEntity FetchCraftItemEntity(string id)
		{
			return _craftItemEntities.FirstOrDefault(e => e.InstanceId == id);
		}

		/// <summary>
		/// Return CraftRecipeEntity matching identifier.
		/// </summary>
		/// <param name="id">Id of CraftRecipeEntity.</param>
		/// <returns>Matching CraftRecipeEntity.</returns>
		public CraftRecipeEntity FetchCraftRecipeEntity(string id)
		{
			return _craftRecipeEntities.FirstOrDefault(e => e.InstanceId == id);
		}

		/// <summary>
		/// Return CraftItemModelData matching identifier.
		/// </summary>
		/// <param name="id">Id of matching CraftItemEntity to get the model data from.</param>
		/// <returns>Matching CraftItemModelData.</returns>
		public CraftItemModelData FetchCraftItemModelData(string id)
		{
			CraftItemModelData data = null;
			var entity = FetchCraftItemEntity(id);
			data = (entity != null) ? entity.CraftItemData.CraftItemModelData : null;
			return data;
		}

		public CraftRecipeEntity FetchCraftRecipeWithIngredients(CraftItemInstance firstIngredient, CraftItemInstance secondIngredient)
		{
			string firstIngredientId = firstIngredient.CraftItemEntity.InstanceId;
			string secondIngredientId = secondIngredient.CraftItemEntity.InstanceId;

			if (IngredientRecipes.ContainsKey(firstIngredientId) == true)
			{
				// Go over the recipes in which the first ingredient is an ingredient.
				// Check if the second ingredient is also part of that recipe.
				var recipeList = IngredientRecipes[firstIngredientId];
				foreach (var recipe in recipeList)
				{
					if (recipe.CraftRecipeData.RecipeSatisfied(firstIngredientId, secondIngredientId))
					{
						return recipe;
					}
				}
			}
			return null;
		}

		/// <summary>
		/// Return whether the CraftItemInstance underlying CraftItemEntity is a final item.
		/// A final item is something that is not used as an ingredient to create anything.
		/// </summary>
		/// <param name="instance">CraftItemInstance being checked to see if its CraftItemEntity is a final item.</param>
		/// <returns>True if underlying CraftItemEntity is a final item.</returns>
		public bool IsFinalItem(CraftItemInstance instance)
		{
			if (instance == null)
			{
				return false;
			}
			return IsFinalItem(instance.CraftItemEntity);
		}

		/// <summary>
		/// Return whether CraftItemEntity is a final item.
		/// A final item is something that is not used as an ingredient to create anything.
		/// </summary>
		/// <param name="entity">CraftItemEntity being checked to see if its CraftItemEntity is a final item.</param>
		/// <returns>True is underlying CraftItemEntity is a final item.</returns>
		public bool IsFinalItem(CraftItemEntity entity)
		{
			if (entity == null)
			{
				return false;
			}
			// If it is never used as an ingredient, then it is a final item.
			if (IngredientRecipes.ContainsKey(entity.InstanceId) == false)
			{
				return true;
			}
			else
			{
				if (IngredientRecipes[entity.InstanceId] == null || IngredientRecipes[entity.InstanceId].Count == 0)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// If all of the recipes in which this instances entity is an ingredient are made then the item is depleted.
		/// </summary>
		/// <param name="instance">The instance in which we are checking its underlying entity for depletion.</param>
		/// <returns>True if underlying entity is depleted, false otherwise.</returns>
		public bool IsDepletedItem(CraftItemInstance instance)
		{
			return false;
			if (instance == null)
			{
				return false;
			}
			return IsDepletedItem(instance.CraftItemEntity);
		}

		/// <summary>
		/// If all of the recipes in which the entity is an ingredient are made then the item is depleted.
		/// </summary>
		/// <param name="entity">The entity that we are checking depletion for.</param>
		/// <returns>True if the entity is depleted, false otherwise.</returns>
		public bool IsDepletedItem(CraftItemEntity entity)
		{
			return false;
			if (entity == null)
			{
				return false;
			}

			if (IsFinalItem(entity) == true)
			{
				return false;
			}

			if (IngredientRecipes.ContainsKey(entity.InstanceId) == true)
			{
				var recipes = IngredientRecipes[entity.InstanceId];
				foreach (var recipe in recipes)
				{
					if (recipe.Unlocked == false)
					{
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Return a CraftItemInstance for use.
		/// </summary>
		/// <param name="instance">CraftItemInstance being used to construct the new instance.</param>
		/// <param name="position">Position to set the CraftItemInstance.</param>
		/// <param name="triggerSaveEvent">Should the object be saved to the board.</param>
		/// <param name="spawnAnimation">Animation that should play after returning the CraftItemInstance.</param>
		/// <param name="activeState">Should the object be on or off in the heiarchy.</param>
		/// <returns>A CraftItemInstance.</returns>
		public CraftItemInstance SpawnAndReturnCraftItemInstance(CraftItemInstance instance,
			Vector3 position,
			bool triggerSaveEvent,
			string spawnAnimation = "",
			bool activeState = true)
		{
			return SpawnAndReturnCraftItemInstance(entity: instance.CraftItemEntity,
				position: position,
				triggerSaveEvent: triggerSaveEvent, 
				spawnAnimation: spawnAnimation, 
				activeState: activeState);
		}

		/// <summary>
		/// Return a CraftItemInstance for use.
		/// </summary>
		/// <param name="entity">CraftItemEntity to base the instance after.</param>
		/// <param name="position">Position to set the CraftItemInstance.</param>
		/// <param name="triggerSaveEvent">Should the object be saved to the board.</param>
		/// <param name="spawnAnimation">Animation that should play after returning the CraftItemInstance.</param>
		/// <param name="activeState">Should the object be on or off in the heiarchy.</param>
		/// <returns>A CraftItemInstance.</returns>
		public CraftItemInstance SpawnAndReturnCraftItemInstance(CraftItemEntity entity, 
			Vector3 position,
			bool triggerSaveEvent,
			string spawnAnimation = "",
			bool activeState = true)
		{
			var instance = _poolingService.GetCraftItemInstancePoolElement();
			instance.transform.localPosition = position;
			instance.SetActiveSafe(activeState);

			// Set up instance.
			instance.Entity = entity;
			instance.Initialize();
			instance.PlayAnimation(spawnAnimation);

			if (triggerSaveEvent == true)
			{
				EventManager.TriggerEvent(new PositionSaveObjectAddedEvent(instance));
			}

			return instance;
		}

		/// <summary>
		/// Return a PreviouslyMadeIndicatorInstance for use.
		/// </summary>
		/// <param name="instance">PreviouslyMadeIndicatorInstance being used to construct the new instance.</param>
		/// <param name="position">Position to set the PreviouslyMadeIndicatorInstance.</param>
		/// <param name="activeState">Active state of gameobject.</param>
		/// <returns>A PreviuslyMadeIndicatorInstance for use./returns>
		public PreviouslyMadeIndicatorInstance SpawnAndReturnPreviouslyMadeIndicatorInstance(PreviouslyMadeIndicatorInstance instance, 
			Vector3 position,
			bool activeState = true)
		{
			return SpawnAndReturnPreviouslyMadeIndicatorInstance(entity: 
				instance.CraftItemEntity,
				position: position,
				activeState : activeState);
		}

		/// <summary>
		/// Return a PreviouslyMadeIndicatorInstance for use.
		/// </summary>
		/// <param name="entity">Entity being used to construct the new instance.</param>
		/// <param name="position">Position to set the PreviouslyMadeIndicatorInstance.</param>
		/// <param name="activeState">Active state of gameobject.</param>
		/// <returns>A PreviuslyMadeIndicatorInstance for use./returns>
		public PreviouslyMadeIndicatorInstance SpawnAndReturnPreviouslyMadeIndicatorInstance(CraftItemEntity entity,
			Vector3 position,
			bool activeState = true)
		{
			var instance = _poolingService.GetPreviouslyMadeInstancePoolElement();
			instance.transform.localPosition = position;

			//instance.Entity = entity;
			instance.gameObject.SetActiveSafe(activeState);
			instance.Initialize(entity);

			return instance;
		}

		/// <summary>
		/// CraftRecipe was already made so, spawn previously made indicatorsn for each product.
		/// </summary>
		/// <param name="entity">The CraftRecipeEntity that was constructed.</param>
		/// <param name="position">Position to spawn the indicators.</param>
		public void SpawnPreviouslyMadeIndicatorsForRecipe(CraftRecipeEntity entity, Vector3 position)
		{
			if (entity == null)
			{
				return;
			}
			foreach (var product in entity.CraftRecipeData.RecipeProducts)
			{
				var instance = SpawnAndReturnPreviouslyMadeIndicatorInstance(FetchCraftItemEntity(product), position);
			}
		}

		/// <summary>
		/// Spawn a clone of CraftItemInstance.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public CraftItemInstance SpawnClone(CraftItemInstance instance)
		{
			if (instance == null)
			{
				return null;
			}
			var newInstance = SpawnAndReturnCraftItemInstance(instance, instance.transform.localPosition, triggerSaveEvent: true, spawnAnimation: CraftItemInstance.eCraftItemAnimationKeys.CloneAppear.ToString());
			return newInstance;
		}

		/// <summary>
		/// On release of the CraftItemInstance, attempt fufilling recipe.
		/// </summary>
		/// <param name="instance">The CraftItemInstance that was released.</param>
		/// <returns>True if something was crafted, false otherwise.</returns>
		public bool AttemptCrafting(CraftItemInstance instance)
		{
			_collidedWithCraftItemInstances.Clear();
			FetchCollisionResults();
			if (_collisionResults.Count == 0)
			{
				return false;
			}

			// Get collection of collided with instances.
			foreach (var collisionResult in _collisionResults)
			{
				collisionResult.gameObject.TryGetComponentInParent<CraftItemInstance>(out var resultInstance);
				if (resultInstance != null
					&& instance != resultInstance 
					&& _collidedWithCraftItemInstances.Contains(resultInstance) == false)
				{
					_collidedWithCraftItemInstances.Add(resultInstance);
				}
			}
			if (_collidedWithCraftItemInstances.Count == 0)
			{
				return false;
			}

			CraftItemInstance combiningInstance = _collidedWithCraftItemInstances.OrderByDescending(x => x.transform.GetSiblingIndex()).First();
			bool hasCraftedSomething = AttemptToCombineIngredients(instance, combiningInstance, combiningInstance.transform.localPosition);
			if (hasCraftedSomething == true)
			{
				// TODO : play animation?
				instance.Recycle(true);
				combiningInstance.Recycle(true);
				return true;
			}
			instance.PlayAnimation(CraftItemInstance.eCraftItemAnimationKeys.InvalidCombo.ToString(), true);
			return false;
		}

		/// <summary>
		/// Attempt to combine two ingredients.
		/// </summary>
		/// <param name="firstIngredient">First CraftItemInstance in combination.</param>
		/// <param name="secondIngredient">Second CraftItemInstance in combination.</param>
		/// <param name="interactionPosition">Where the instances should be combined.</param>
		/// <returns>True if something was crafted, false otherwise.</returns>
		public bool AttemptToCombineIngredients(CraftItemInstance firstIngredient, CraftItemInstance secondIngredient, Vector3 interactionPosition)
		{
			// Do these ingredients correspond to any recipe.
			var recipeEntity = FetchCraftRecipeWithIngredients(firstIngredient, secondIngredient);

			if (recipeEntity == null)
			{
				return false;
			}

			// Handle building recipe for the first time.
			if (recipeEntity.Unlocked == false)
			{
				TriggerCraftRecipeEntityUnlock(recipeEntity);
				SpawnProductsOfRecipe(recipeEntity, interactionPosition);
				CheckAddingDepletedKeywordPostRecipe(recipeEntity);
			}
			else
			{
				// Spawn previously made indicator and give toast.
				SpawnPreviouslyMadeIndicatorsForRecipe(recipeEntity, interactionPosition);
				EventManager.TriggerEvent(new ToastEvent("Already Made!"));
				return false;
			}
			return true;
		}

		/// <summary>
		/// Spawn a CraftItemInstance when crafting a CraftRecipe
		/// </summary>
		/// <param name="entity">CraftItemEntity to spawn.</param>
		/// <param name="pos">Position to place the spawned instance.</param>
		/// <returns>CraftItemInstance that was spawned in.</returns>
		public CraftItemInstance SpawnProductOfRecipe(CraftItemEntity entity, Vector3 pos)
		{
			if (entity == null)
			{
				return null;
			}
			var newInstance = SpawnAndReturnCraftItemInstance(entity, 
				pos,
				triggerSaveEvent: true, 
				CraftItemInstance.eCraftItemAnimationKeys.CloneAppear.ToString(), 
				activeState: true);
			return newInstance;
		}

		/// <summary>
		/// Spawn all products of a CraftRecipeEntity
		/// </summary>
		/// <param name="entity">The CraftRecipeEntity that was crafted.</param>
		/// <param name="pos">Position that the new elements should originate from.</param>
		public void SpawnProductsOfRecipe(CraftRecipeEntity entity, Vector3 pos)
		{
			if (entity == null)
			{
				return;
			}
			foreach (var product in entity.CraftRecipeData.RecipeProducts)
			{
				var craftItemEntity = FetchCraftItemEntity(product);
				if (craftItemEntity == null)
				{
					Debug.LogError($"Could not find CraftItemEntity with ID {product}");
					continue;
				}
				var newInstance = SpawnProductOfRecipe(craftItemEntity, pos);

				// Unlock CraftItemEntity if appropriate.
				if (craftItemEntity.Unlocked == false)
				{
					// TODO : Switch to full screen overlay.
					_uiService.ActivateView(UIEnumTypes.eViewType.GameplayUnlockView.ToString());
					TriggerCraftItemEntityUnlock(craftItemEntity);
				}
			}
		}

		/// <summary>
		/// Unlock CraftItemEntity.
		/// </summary>
		/// <param name="entity">CraftItemEntity to unlock.</param>
		public void TriggerCraftItemEntityUnlock(CraftItemEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			entity.SetUnlockState(state : true);
			EventManager.TriggerEvent(new CraftItemEntityUnlockEvent(entity));
		}

		/// <summary>
		/// Unlock CraftRecipeEntity.
		/// </summary>
		/// <param name="entity">CraftRecipeEntity to unlock.</param>
		public void TriggerCraftRecipeEntityUnlock(CraftRecipeEntity entity)
		{
			if (entity == null)
			{
				return;
			}
			entity.SetUnlockState(state : true);
			EventManager.TriggerEvent(new CraftRecipeUnlockEvent(entity));
		}

		/// <summary>
		/// Place CraftItemInstance on drag layer of gameboard.
		/// </summary>
		/// <param name="instance">CraftItemInstance to place.</param>
		public void PlaceOnDragLayer(CraftItemInstance instance)
		{
			if (instance == null)
			{
				return;
			}
			if (_gameplayBoard == null)
			{
				Logger.LogError("CraftingSystemCraftingService.PlaceOnDragLayer : Game board was null.");
				return;
			}
			instance.transform.SetParent(_gameplayBoard.DragLayer);
		}

		/// <summary>
		/// Place CraftItemInstance on crafting layer of gameboard.
		/// </summary>
		/// <param name="instance">CraftItemInstance to place.</param>
		/// <param name="setAslastSibling">Should SetAsLiastSibling be called on the transform.</param>
		public void PlaceOnCraftingLayer(CraftItemInstance instance, bool setAslastSibling = true)
		{
			if (instance == null)
			{
				return;
			}
			if (_gameplayBoard == null)
			{
				Logger.LogError("CraftingSystemCraftingService.PlaceOnCraftingLayer : Game board was null.");
				return;
			}
			instance.transform.SetParent(_gameplayBoard.CraftingLayer);
			if (setAslastSibling == true)
			{
				instance.transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Return a position on the game board, bounded within a region.
		/// </summary>
		/// <param name="position">Position to attempt to place the entity.</param>
		/// <param name="useDefaultBuffer">Should the bounding buffer be the default boffer value.</param>
		/// <param name="useDefaultRadius">Should the placement radius be the default radius.</param>
		/// <param name="customBuffer">Custom buffer value.</param>
		/// <param name="customRadius">Custom radius value.</param>
		/// <returns>A position bounded to the play region.</returns>
		public Vector3 GetPositionOnGameBoard(Vector3 position,
			bool useDefaultBuffer,
			bool useDefaultRadius, 
			float customBuffer = 0f, 
			float customRadius = 1f)
		{
			if (_gameplayBoard == null)
			{
				return position;
			}
			return _gameplayBoard.GetPositionOnCircle(position, useDefaultBuffer, useDefaultRadius, customBuffer, customRadius);
		}
		#endregion
	}
}
