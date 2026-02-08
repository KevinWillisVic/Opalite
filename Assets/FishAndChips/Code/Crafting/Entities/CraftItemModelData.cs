using System;
using UnityEngine;

namespace FishAndChips
{
	[CreateAssetMenu(menuName = "FishAndChips/Models/CraftItemModelData")]
	[Serializable]
	public class CraftItemModelData : GameObjectModelData
    {
		#region -- Inspector --
		public string Blurb;
		#endregion
	}
}
