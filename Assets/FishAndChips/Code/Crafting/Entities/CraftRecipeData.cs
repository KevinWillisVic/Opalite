using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FishAndChips
{
	[Serializable]
	public class RecipeRequirement
	{
		public string IngredientKey;
		public int RequiredCount;

		public RecipeRequirement(string key, int count)
		{
			IngredientKey = key;
			RequiredCount = count;
		}

		public RecipeRequirement(RecipeRequirement other)
		{
			IngredientKey = other.IngredientKey;
			RequiredCount = other.RequiredCount;
		}
	}

    [Serializable]
    public class CraftRecipeData : IMetaData, IMetaDataSavedData
    {
		#region -- Properties --
		public virtual string ID => _id;
		public List<string> RecipeProducts => _recipeProducts;
		public List<RecipeRequirement> IngredientMap => _ingredientMap;
		#endregion

		#region -- Protected Member Vars --
		[SerializeField] protected string _id;
		[SerializeField] protected List<string> _recipeProducts = new();
		[SerializeField] protected List<RecipeRequirement> _ingredientMap = new();
		#endregion

		#region -- Public Methods --
		public virtual IEntity CreateEntity(string instanceId)
		{
			return new CraftRecipeEntity(instanceId);
		}

		public virtual ISavedData CreateSavedData(string saveId)
		{
			return new CraftRecipeSavedData(saveId);
		}

		public bool RecipeSatisfied(CraftItemInstance firstIngredient, CraftItemInstance secondIngredient)
		{
			if (firstIngredient == null || secondIngredient == null)
			{
				return false;
			}
			return RecipeSatisfied(firstIngredient.InstanceID, secondIngredient.InstanceID);
		}

		public bool RecipeSatisfied(string firstIngredient, string secondIngredient)
		{
			if (firstIngredient.IsNullOrEmpty() || secondIngredient.IsNullOrEmpty())
			{
				return false;
			}
			var ingredients = new List<string>() { firstIngredient, secondIngredient };
			return RecipeSatisfied(ingredients);
		}

		public bool RecipeSatisfied(List<string> ingredients)
		{
			List<RecipeRequirement> copyOfConditions = new();
			foreach (var data in _ingredientMap)
			{
				copyOfConditions.Add(new RecipeRequirement(data));
			}

			foreach (var ingredient in ingredients)
			{
				var requirement = copyOfConditions.FirstOrDefault(i => i.IngredientKey.Equals(ingredient));
				if (requirement == null)
				{
					return false;
				}
				requirement.RequiredCount--;
			}

			foreach (var condition in copyOfConditions)
			{
				if (condition.RequiredCount > 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool ContainsIngredient(CraftItemInstance instance)
		{
			if (instance == null)
			{
				return false;
			}
			return ContainsIngredient(instance.InstanceID);
		}

		public bool ContainsIngredient(string id)
		{
			if (id.IsNullOrEmpty())
			{
				return false;
			}
			var ingredient = _ingredientMap.FirstOrDefault(i => i.IngredientKey.Equals(id));
			return ingredient != null;
		}

		public void SetId(string id)
		{
			_id = id;
		}
		#endregion
	}
}
