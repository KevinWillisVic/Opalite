using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace FishAndChips
{
    public class CraftingEditorWindow : EditorWindow
    {
		public static event Action<CraftItemScriptableData> OnItemSelected;

		#region -- Private Member Vars --
		private Texture2D _unkownTexture = null;
		private List<Texture2D> _craftItemTextures = new();
		private List<CraftItemScriptableData> _craftItems = new();
		private List<CraftRecipeScriptableData> _craftRecipes = new();
		private Dictionary<CraftItemScriptableData, Texture2D> _visualMapping = new();


		// CraftItem context.
		private VisualElement _mainCraftItemContent;

		// Left side of the screen.
		private ScrollView _craftItemSelectableList;

		// Right side of the scrren.
		private ScrollView _craftItemInformationList;

		// Context for creation.
		private TextField _newCraftItemNameField;
		private Foldout _craftItemCreationContext;
		private InspectorElement _creatableCraftItemInspector;
		private CraftItemScriptableData _creatableCraftItemData;
		private SerializedObject _serializedCreatableCraftItemData;

		// Context for modification.
		private Foldout _craftItemMofificationContext;
		private InspectorElement _modifiableCraftItemInspector;
		private CraftItemScriptableData _modifiableCraftItemData;
		private SerializedObject _serializedModifiableCraftItemData;


		// CraftRecipe context.
		private VisualElement _mainCraftRecipeContent;

		// Left side of the screen.
		private ScrollView _craftRecipeSelectableList;

		// Right side of the screen.
		private ScrollView _craftRecipeInformationList;

		// Context for creation.
		private TextField _newCraftRecipeNameField;
		private Foldout _craftRecipeCreationContext;
		private InspectorElement _creatableCraftRecipeInspector;
		private CraftRecipeScriptableData _creatableCraftRecipeData;
		private SerializedObject _serializedCreatableCraftRecipeData;

		// Context for modification.
		private Foldout _craftRecipeMofificationContext;
		private InspectorElement _modifiableCraftRecipeInspector;
		private CraftRecipeScriptableData _modifiableCraftRecipeData;
		private SerializedObject _serializedModifiableCraftRecipeData;
		#endregion

		#region -- Private Methods --
		private void OnEnable()
		{
			LoadUnkownTexture();

			_visualMapping.Clear();

			_craftItems.Clear();
			_craftRecipes.Clear();
			_craftItemTextures.Clear();

			LoadCraftItems();
			LoadCraftRecipes();
		}

		private void LoadUnkownTexture()
		{
			var textureGUIDs = AssetDatabase.FindAssets($"t:Texture2D ui_icon_unknown", new[] { $"Assets/FishAndChips/Art/Sprites/Icons" });
			if (textureGUIDs != null && textureGUIDs.Length > 0)
			{
				var texturePath = AssetDatabase.GUIDToAssetPath(textureGUIDs[0]);
				_unkownTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
			}
		}

		private void LoadCraftItems()
		{
			var craftItemGUIDS = AssetDatabase.FindAssets("t:CraftItemScriptableData", new[] {"Assets/FishAndChips/Data/Crafting/CraftItems"} );

			foreach (var guid in craftItemGUIDS)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<CraftItemScriptableData>(assetPath);

				if (asset != null)
				{
					if (_craftItems.Contains(asset) == false)
					{
						_craftItems.Add(asset);
						if (asset.Data != null && asset.Data.ModelData != null)
						{
							if (asset.Data.ModelData.VisualKey.IsNullOrEmpty() == false)
							{
								var textureGUIDs = AssetDatabase.FindAssets($"t:Texture2D {asset.Data.ModelData.VisualKey}", new[] { $"Assets/FishAndChips/Art/Sprites/CraftItemIcons" });
								if (textureGUIDs != null && textureGUIDs.Length > 0)
								{
									var texturePath = AssetDatabase.GUIDToAssetPath(textureGUIDs[0]);
									var textureAsset = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);

									if (_visualMapping.ContainsKey(asset) == false)
									{
										_visualMapping.Add(asset, textureAsset);
									}
								}
							}
						}
					}

					if (_visualMapping.ContainsKey(asset) == false)
					{
						_visualMapping.Add(asset, _unkownTexture);
					}
				}
			}
		}

		private void LoadCraftRecipes()
		{
			var craftRecipeGuids = AssetDatabase.FindAssets("t:CraftRecipeScriptableData", new[] { "Assets/FishAndChips/Data/Crafting/CraftRecipes" });

			foreach (var guid in craftRecipeGuids)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var asset = AssetDatabase.LoadAssetAtPath<CraftRecipeScriptableData>(assetPath);

				if (asset != null)
				{
					if (_craftRecipes.Contains(asset) == false)
					{
						_craftRecipes.Add(asset);
					}
				}
			}
		}

		private void RepopulateCraftRecipeScrollView(string searchFilter)
		{
			if (_craftRecipeSelectableList == null)
			{
				return;
			}
			_craftRecipeSelectableList.Clear();
			foreach (var recipe in _craftRecipes)
			{
				string name = recipe.ID.Replace("CraftRecipe", "");
				var comparison = name.ToLowerInvariant();
				if (searchFilter.IsNullOrEmpty() == false)
				{
					if (comparison.StartsWith(searchFilter) == false)
					{
						continue;
					}
				}
				Button button = new Button(() =>
				{
					_modifiableCraftRecipeData = recipe;
					UpdateSelectedCraftRecipeUI();
				});
				button.style.height = 50;
				button.style.flexDirection = FlexDirection.Row;

				button.RegisterCallback<FocusInEvent>(evt =>
				{
					var b = evt.target as VisualElement;
					b.style.backgroundColor = new Color(143 / 255f, 80 / 255f, 98 / 255f);
				});

				button.RegisterCallback<FocusOutEvent>(evt =>
				{
					var b = evt.target as VisualElement;
					b.style.backgroundColor = StyleKeyword.Null;
				});

				Label label = new Label(name);
				label.style.flexGrow = 1;
				button.Add(label);

				_craftRecipeSelectableList.Add(button);
			}
		}

		private void RepopulateCraftItemScrollView(string searchFilter)
		{
			if (_craftItemSelectableList == null)
			{
				return;
			}
			_craftItemSelectableList.Clear();
			foreach (var kvp in _visualMapping)
			{
				string name = (kvp.Key.Data.ModelData != null) ? kvp.Key.Data.ModelData.DisplayName : kvp.Key.name;
				var comparison = name.ToLowerInvariant();
				if (searchFilter.IsNullOrEmpty() == false)
				{
					if (comparison.StartsWith(searchFilter) == false)
					{
						continue;
					}
				}
				Button button = new Button(() =>
				{
					_modifiableCraftItemData = kvp.Key;
					UpdateSelectedCraftItemUI();
				});
				button.style.height = 50;
				button.style.flexDirection = FlexDirection.Row;

				button.RegisterCallback<FocusInEvent>(evt =>
				{
					var b = evt.target as VisualElement;
					b.style.backgroundColor = new Color(143 / 255f, 80 / 255f, 98 / 255f);
				});

				button.RegisterCallback<FocusOutEvent>(evt =>
				{
					var b = evt.target as VisualElement;
					b.style.backgroundColor = StyleKeyword.Null;
				});

				Label label = new Label(name);
				label.style.flexGrow = 1;
				Image icon = new Image()
				{
					image = kvp.Value,
					style =
						{
							marginTop = 10,
							height = 25,
							width = 25,
						}
				};
				button.Add(icon);
				button.Add(label);
				_craftItemSelectableList.Add(button);
			}
		}

		private void UpdateSelectedCraftItemUI()
		{
			if (_craftItemInformationList != null && _craftItemMofificationContext != null)
			{
				if (_craftItemInformationList.Contains(_craftItemMofificationContext))
				{
					_craftItemInformationList.Remove(_craftItemMofificationContext);
				}
			}
			if (_modifiableCraftItemData == null)
			{
				return;
			}
			else
			{
				_craftItemMofificationContext = new Foldout()
				{
					text = "Craft Item Modification"
				};

				_serializedModifiableCraftItemData = new SerializedObject(_modifiableCraftItemData);
				_modifiableCraftItemInspector = new InspectorElement();
				_modifiableCraftItemInspector.Bind(_serializedModifiableCraftItemData);
				_modifiableCraftItemInspector.style.flexGrow = 0;
				_craftItemMofificationContext.Add(_modifiableCraftItemInspector);

				_craftItemInformationList.Add(_craftItemMofificationContext);
			}
		}

		private void UpdateSelectedCraftRecipeUI()
		{
			if (_craftRecipeInformationList != null && _craftRecipeMofificationContext != null)
			{
				if (_craftRecipeInformationList.Contains(_craftRecipeMofificationContext))
				{
					_craftRecipeInformationList.Remove(_craftRecipeMofificationContext);
				}
			}
			if (_modifiableCraftRecipeData == null)
			{
				return;
			}
			else
			{
				_craftRecipeMofificationContext = new Foldout()
				{
					text = "Craft Recipe Modification"
				};

				_serializedModifiableCraftRecipeData = new SerializedObject(_modifiableCraftRecipeData);
				_modifiableCraftRecipeInspector = new InspectorElement();
				_modifiableCraftRecipeInspector.Bind(_serializedModifiableCraftRecipeData);
				_modifiableCraftRecipeInspector.style.flexGrow = 0;
				_craftRecipeMofificationContext.Add(_modifiableCraftRecipeInspector);

				_craftRecipeInformationList.Add(_craftRecipeMofificationContext);
			}
		}

		private void CreateGUI()
		{
			VisualElement root = rootVisualElement;

			StyleSheet borderStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FishAndChips/StyleSheets/BoxedContainer.uss");
			root.styleSheets.Add(borderStyleSheet);

			VisualElement boxedContainer = new VisualElement();
			boxedContainer.AddToClassList("borderbox");

			// Create a TabView with Tabs that only contains a label.
			var mainCategoryLabels = new TabView();
			mainCategoryLabels.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/FishAndChips/StyleSheets/TabViewStyleSheet.uss"));
			mainCategoryLabels.style.flexShrink = 1;
			mainCategoryLabels.style.flexGrow = 1;
			// CraftItem section.
			var craftItemTab = new Tab("Craft Items");

			// Info Label.
			Label craftItemLabel = new Label("This section is for CraftItem tooling.")
			{
				style =
				{
					marginTop = 10
				}
			};

			TextField craftItemSearchFilter = new TextField("Craft Item Search");
			craftItemSearchFilter.textEdition.placeholder = "...";
			craftItemSearchFilter.textEdition.hidePlaceholderOnFocus = true;
			craftItemSearchFilter.RegisterValueChangedCallback(evt =>
			{
				var search = craftItemSearchFilter.text.ToLowerInvariant();
				RepopulateCraftItemScrollView(search);
			});

			_mainCraftItemContent = new VisualElement();
			_mainCraftItemContent.style.flexGrow = 1;
			_mainCraftItemContent.style.flexDirection = FlexDirection.Row;

			// Add contents to tab.
			craftItemTab.Add(craftItemLabel);
			craftItemTab.Add(craftItemSearchFilter);
			craftItemTab.Add(_mainCraftItemContent);


			// The left side of the screen showing craft item list.
			_craftItemSelectableList = new ScrollView()
			{
				mode = ScrollViewMode.Vertical,
				style =
				{
					flexDirection = FlexDirection.Column,
					backgroundColor = new Color(70/255f,92/255f,120/255f),
					width = 150,

					marginTop = 10,
					marginRight = 10,
					marginBottom = 30,
					marginLeft = 10,

					paddingLeft = 10,
					paddingBottom = 10,
					paddingTop = 10,
					paddingRight = 10,
					
				}
			};

			var searchItem = craftItemSearchFilter.text.ToLowerInvariant();
			RepopulateCraftItemScrollView(searchItem);
			_mainCraftItemContent.Add(_craftItemSelectableList);

			// Right side context.
			_craftItemInformationList = new ScrollView();
			_craftItemInformationList.mode = ScrollViewMode.Vertical;
			_craftItemInformationList.style.flexGrow = 1;
			_craftItemInformationList.style.flexDirection = FlexDirection.Column;
			_mainCraftItemContent.Add(_craftItemInformationList);


			// Creation Context.
			_craftItemCreationContext = new Foldout()
			{
				text = "Craft Item Creation"
			};


			_newCraftItemNameField = new TextField("New Craft Item Name");
			_newCraftItemNameField.textEdition.placeholder = "NewCraftItem";
			_newCraftItemNameField.RegisterValueChangedCallback(evt =>
			{
				var nameValue = _newCraftItemNameField.text;
				if (_creatableCraftItemData != null)
				{
					_creatableCraftItemData.Data.SetId(nameValue);
					_creatableCraftItemData.Data.SetModelId($"{nameValue}ModelData");
				}
			});

			_craftItemCreationContext.Add(_newCraftItemNameField);

			_creatableCraftItemData = CreateInstance<CraftItemScriptableData>();
			_serializedCreatableCraftItemData = new SerializedObject(_creatableCraftItemData);

			_creatableCraftItemInspector = new InspectorElement();
			_creatableCraftItemInspector.Bind(_serializedCreatableCraftItemData);
			_creatableCraftItemInspector.style.flexGrow = 0;
			_craftItemCreationContext.Add(_creatableCraftItemInspector);

			Button craftItemCreationButton = new Button(() => CreateNewItemInstance(_creatableCraftItemInspector, false))
			{
				text = "Create New Instance At Selected Folder"
			};

			Button craftItemCreationButtonBuiltInPath = new Button(() => CreateNewItemInstance(_creatableCraftItemInspector, true))
			{
				text = "Create New Instance At Default Folder"
			};

			craftItemCreationButton.style.marginLeft = 18;
			craftItemCreationButtonBuiltInPath.style.marginLeft = 18;
			_craftItemCreationContext.Add(craftItemCreationButtonBuiltInPath);
			_craftItemCreationContext.Add(craftItemCreationButton);
			_craftItemInformationList.Add(_craftItemCreationContext);

			VisualElement seperatorLine = new VisualElement();
			seperatorLine.style.backgroundColor = Color.yellow;
			seperatorLine.style.height = 1;
			seperatorLine.style.flexGrow = 1;
			seperatorLine.style.marginBottom = 10;
			seperatorLine.style.marginTop = 10;
			_craftItemInformationList.Add(seperatorLine);

			// Craft Item Modification.
			UpdateSelectedCraftItemUI();

			// CraftRecipe section.
			var craftRecipeTab = new Tab("Craft Recipes");
			// Info Label.
			Label craftReicpeLabel = new Label("This section is for CraftRecipe tooling.")
			{
				style =
				{
					marginTop = 10
				}
			};

			TextField craftRecipeSearchFilter = new TextField("Craft Recipe Search");
			craftRecipeSearchFilter.textEdition.placeholder = "...";
			craftRecipeSearchFilter.textEdition.hidePlaceholderOnFocus = true;
			craftRecipeSearchFilter.RegisterValueChangedCallback(evt =>
			{
				var search = craftRecipeSearchFilter.text.ToLowerInvariant();
				RepopulateCraftRecipeScrollView(search);
			});

			_mainCraftRecipeContent = new VisualElement();
			_mainCraftRecipeContent.style.flexGrow = 1;
			_mainCraftRecipeContent.style.flexDirection = FlexDirection.Row;

			craftRecipeTab.Add(craftReicpeLabel);
			craftRecipeTab.Add(craftRecipeSearchFilter);
			craftRecipeTab.Add(_mainCraftRecipeContent);


			// The left side of the screen showing craft item list.
			_craftRecipeSelectableList = new ScrollView()
			{
				mode = ScrollViewMode.Vertical,
				style =
				{
					flexDirection = FlexDirection.Column,
					backgroundColor = new Color(70/255f,92/255f,120/255f),
					width = 150,

					marginTop = 10,
					marginRight = 10,
					marginBottom = 30,
					marginLeft = 10,

					paddingLeft = 10,
					paddingBottom = 10,
					paddingTop = 10,
					paddingRight = 10,

				}
			};

			var searchRecipe = craftItemSearchFilter.text.ToLowerInvariant();
			RepopulateCraftRecipeScrollView(searchRecipe);
			_mainCraftRecipeContent.Add(_craftRecipeSelectableList);


			// Right side context.
			_craftRecipeInformationList = new ScrollView();
			_craftRecipeInformationList.mode = ScrollViewMode.Vertical;
			_craftRecipeInformationList.style.flexGrow = 1;
			_craftRecipeInformationList.style.flexDirection = FlexDirection.Column;
			_mainCraftRecipeContent.Add(_craftRecipeInformationList);


			// Creation Context.
			_craftRecipeCreationContext = new Foldout()
			{
				text = "Craft Recipe Creation"
			};


			_newCraftRecipeNameField = new TextField("New Craft Recipe Name");
			_newCraftRecipeNameField.textEdition.placeholder = "NewCraftRecipe";
			_newCraftRecipeNameField.RegisterValueChangedCallback(evt =>
			{
				var nameValue = _newCraftRecipeNameField.text;
				if (_creatableCraftRecipeData != null)
				{
					_creatableCraftRecipeData.Data.SetId(nameValue);
				}
			});

			_craftRecipeCreationContext.Add(_newCraftRecipeNameField);

			_creatableCraftRecipeData = CreateInstance<CraftRecipeScriptableData>();
			_serializedCreatableCraftRecipeData = new SerializedObject(_creatableCraftRecipeData);

			_creatableCraftRecipeInspector = new InspectorElement();
			_creatableCraftRecipeInspector.Bind(_serializedCreatableCraftRecipeData);
			_creatableCraftRecipeInspector.style.flexGrow = 0;
			_craftRecipeCreationContext.Add(_creatableCraftRecipeInspector);

			Button craftRecipeCreationButton = new Button(() => CreateNewRecipeInstance(_creatableCraftRecipeInspector, false))
			{
				text = "Create New Instance At Selected Folder"
			};

			Button craftRecipeCreationButtonBuiltInPath = new Button(() => CreateNewRecipeInstance(_creatableCraftRecipeInspector, true))
			{
				text = "Create New Instance At Default Folder"
			};

			craftRecipeCreationButton.style.marginLeft = 18;
			craftRecipeCreationButtonBuiltInPath.style.marginLeft = 18;
			_craftRecipeCreationContext.Add(craftRecipeCreationButtonBuiltInPath);
			_craftRecipeCreationContext.Add(craftRecipeCreationButton);

			_craftRecipeInformationList.Add(_craftRecipeCreationContext);

			VisualElement seperatorLineRecipes = new VisualElement();
			seperatorLineRecipes.style.backgroundColor = Color.yellow;
			seperatorLineRecipes.style.height = 1;
			seperatorLineRecipes.style.flexGrow = 1;
			seperatorLineRecipes.style.marginBottom = 10;
			seperatorLineRecipes.style.marginTop = 10;
			_craftRecipeInformationList.Add(seperatorLineRecipes);

			UpdateSelectedCraftRecipeUI();

			// Add the tabs.
			mainCategoryLabels.Add(craftItemTab);
			mainCategoryLabels.Add(craftRecipeTab);
			boxedContainer.Add(mainCategoryLabels);


			// Reload Button.
			Button reloadButton = new Button(() => { ReloadWindow(); })
			{
				text = "Reload",
				style =
				{
					flexGrow = 1,
					height = 25,
					width = 150,
					marginTop = 10,
					marginLeft = 5,
					position = Position.Absolute,
					bottom = 0,
					left = 0,
					right = 0,
				}
			};
			boxedContainer.Add(reloadButton);
			root.Add(boxedContainer);
		}

		private void ReloadWindow()
		{
			rootVisualElement.Clear();
			OnEnable();
			CreateGUI();
		}

		private void CreateNewItemInstance(InspectorElement inspectorElement, bool useDefaultPath = false)
		{
			string assetName = (_newCraftItemNameField != null) ? _newCraftItemNameField.text : "NewCraftItem";
			if (assetName.IsNullOrEmpty() == true)
			{
				assetName = "NewCraftItem";
			}
			string assetPath = string.Empty;
			if (useDefaultPath == false)
			{
				assetPath = EditorUtility.SaveFilePanelInProject(
					"New CraftItem",
					assetName,
					"asset",
					"Specify new asset name"
					);
			}
			else
			{
				assetPath = $"{GameConstants.CraftItemObjectAssetPath}{assetName}.asset";
			}

			if (assetPath != "")
			{
				if (_creatableCraftItemData.Data.ModelData == null)
				{
					string modelName = _creatableCraftItemData.Data.ModelId.IsNullOrEmpty() == false ? _creatableCraftItemData.Data.ModelId : assetName;
					var model = _creatableCraftItemData.CreateModelDataAtBuiltInPath(modelName);
					_creatableCraftItemData.Data.ModelData = model;
				}
				AssetDatabase.CreateAsset(_creatableCraftItemData, assetPath);


				// Import any changed assets in the project folder.
				AssetDatabase.Refresh();
				// Write all unsaved assets to disk.
				AssetDatabase.SaveAssets();

				if (inspectorElement != null)
				{
					inspectorElement.Unbind();
					_creatableCraftItemData = CreateInstance<CraftItemScriptableData>();
					_serializedCreatableCraftItemData = new SerializedObject(_creatableCraftItemData);
					inspectorElement.Bind(_serializedCreatableCraftItemData);
				}

				//var search = searchFilter.text.ToLowerInvariant();
				// TODO : Just add the new element.
				LoadCraftItems();
				RepopulateCraftItemScrollView(string.Empty);
			}
		}

		private void CreateNewRecipeInstance(InspectorElement inspectorElement, bool useDefaultPath = false)
		{
			string assetName = (_newCraftRecipeNameField != null) ? _newCraftRecipeNameField.text : "NewCraftRecipe";
			if (assetName.IsNullOrEmpty() == true)
			{
				assetName = "NewCraftRecipe";
			}
			string assetPath = string.Empty;
			if (useDefaultPath == false)
			{
				assetPath = EditorUtility.SaveFilePanelInProject(
					"New CraftRecipe",
					assetName,
					"asset",
					"Specify new asset name"
					);
			}
			else
			{
				assetPath = $"{GameConstants.CraftRecipeObjectAssetPath}{assetName}.asset";
			}

			if (assetPath != "")
			{
				
				if (_creatableCraftRecipeData.Data.ID.IsNullOrEmpty())
				{
					_creatableCraftItemData.Data.SetId("");
				}
				
				AssetDatabase.CreateAsset(_creatableCraftRecipeData, assetPath);


				// Import any changed assets in the project folder.
				AssetDatabase.Refresh();
				// Write all unsaved assets to disk.
				AssetDatabase.SaveAssets();

				if (inspectorElement != null)
				{
					inspectorElement.Unbind();
					_creatableCraftRecipeData = CreateInstance<CraftRecipeScriptableData>();
					_serializedCreatableCraftRecipeData = new SerializedObject(_creatableCraftRecipeData);
					inspectorElement.Bind(_serializedCreatableCraftRecipeData);
				}

				//var search = searchFilter.text.ToLowerInvariant();
				// TODO : Just add the new element.
				LoadCraftRecipes();
				RepopulateCraftRecipeScrollView(string.Empty);
			}
		}
		#endregion

		#region -- Public Methods --
		public static void OpenWindow()
		{
			var window = GetWindow<CraftingEditorWindow>();

			window.titleContent = new GUIContent("Crafting Editor");
			window.minSize = new Vector2(475, 250);
		}
		#endregion
	}
}
