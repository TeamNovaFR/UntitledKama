using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Untitled.EditorSystem
{
    public class KamaEditor : EditorWindow
    {
        private int tab;
        private Vector2 scrollPosition = Vector2.zero;

        private Character[] loadedCharacters;
        private int selectedCharacter;

        private CharacterClass[] loadedClasses;
        private int selectedClass;

        private Spell[] loadedSpells;
        private int selectedSpell;

        private Item[] loadedItems;
        private int selectedItem;

        [MenuItem("Kama/Editor")]

        public static void ShowWindow()
        {
            GetWindow(typeof(KamaEditor));
        }

        private void OnEnable()
        {
            RefreshCharacters();
            RefreshClasses();
            RefreshItems();
            RefreshSpells();
        }

        void OnGUI()
        {
            if (loadedCharacters.Where(c => c.id == selectedCharacter).FirstOrDefault() == null && loadedCharacters.Length > 0)
                selectedCharacter = loadedCharacters[0].id;

            if (loadedItems.Where(i => i.id == selectedItem).FirstOrDefault() == null && loadedItems.Length > 0)
                selectedItem = loadedItems[0].id;

            if (loadedSpells.Where(s => s.id == selectedSpell).FirstOrDefault() == null && loadedSpells.Length > 0)
                selectedSpell = loadedSpells[0].id;

            if (loadedClasses.Where(c => c.id == selectedClass).FirstOrDefault() == null && loadedClasses.Length > 0)
                selectedClass = loadedClasses[0].id;

            titleContent.text = "An Kama Editor";

            tab = GUILayout.Toolbar(tab, new string[] { "Characters", "Items", "Spells", "Classes" });
            switch (tab)
            {
                case 0:
                    CharactersGUI();
                    break;
                case 1:
                    ItemsGUI();
                    break;
                case 2:
                    SpellsGUI();
                    break;
                case 3:
                    ClassesGUI();
                    break;
            }
        }

        /// <summary>
        /// Show characters tab editor gui
        /// </summary>
        void CharactersGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(GUILayout.Height(254));

            Rect rectPos = EditorGUILayout.GetControlRect();
            Rect rectBox = new Rect(rectPos.x, rectPos.y, rectPos.width, 250f);

            Rect viewRect = new Rect(rectBox.x, rectBox.y, rectBox.width, loadedCharacters.Length * 20f);

            scrollPosition = GUI.BeginScrollView(rectBox, scrollPosition, viewRect, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);

            int viewCount = 15;
            int firstIndex = (int)scrollPosition.y / 22;

            Rect contentPos = new Rect(rectBox.x, firstIndex * 18f, rectBox.width, 20f);

            for (int i = firstIndex; i < Mathf.Min(loadedCharacters.Length, firstIndex + viewCount); i++)
            {
                contentPos.y += 22f;
                if (GUI.Button(contentPos, $"{loadedCharacters[i].characterName}"))
                    SelectCharacter(loadedCharacters[i].id);
            }

            GUI.EndScrollView();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Create new character"))
            {
                Character newCharacter = CreateInstance<Character>();

                newCharacter.id = loadedCharacters.Length == 0 ? 0 : loadedCharacters[loadedCharacters.Length - 1].id + 1;
                newCharacter.characterName = "New Character";

                AssetDatabase.CreateAsset(newCharacter, $"Assets/Resources/Characters/{newCharacter.id}_NEW_CHARACTER.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                RefreshCharacters();

                SelectCharacter(newCharacter.id);
            }

            if(loadedCharacters.Length > 0)
            {
                Character selectedCharacter = loadedCharacters.Where(i => i.id == this.selectedCharacter).FirstOrDefault();

                if (GUILayout.Button($"Duplicate {selectedCharacter.characterName}"))
                {
                    Character newCharacter = ScriptableObject.CreateInstance<Character>();

                    newCharacter.id = loadedCharacters.Length == 0 ? 0 : loadedCharacters[loadedCharacters.Length - 1].id + 1;
                    newCharacter.characterName = selectedCharacter.characterName;
                    newCharacter.characterClass = selectedCharacter.characterClass;

                    AssetDatabase.CreateAsset(newCharacter, $"Assets/Resources/Characters/{newCharacter.id}_NEW_CHARACTER.asset");
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    RefreshCharacters();

                    SelectCharacter(newCharacter.id);
                }

                if (GUILayout.Button($"Delete {selectedCharacter.characterName}"))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedCharacter));
                    AssetDatabase.SaveAssets();

                    if(this.selectedCharacter > 0)
                        this.selectedCharacter--;

                    RefreshCharacters();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if(loadedCharacters.Length == 0)
            {
                GUILayout.Label($"Please create at least one character.");
            }
            else
            {
                Character selectedCharacter = loadedCharacters.Where(i => i.id == this.selectedCharacter).FirstOrDefault();

                GUILayout.Label($"Selected character: {selectedCharacter.characterName}");
                EditorGUILayout.LabelField("ID:", selectedCharacter.id.ToString());

                selectedCharacter.characterName = EditorGUILayout.TextField("Character Name:", selectedCharacter.characterName);
                selectedCharacter.life = EditorGUILayout.IntField("Base Life:", selectedCharacter.life);

                if(loadedClasses.Length > 0)
                {
                    CharacterClass characterClass = loadedClasses.Where(c => c == selectedCharacter.characterClass).FirstOrDefault();

                    int indexClassSelected = characterClass != null ? characterClass.id : 0;

                    List<string> options = new List<string>();

                    for (int i = 0; i < loadedClasses.Length; i++)
                        options.Add(loadedClasses[i].className);

                    indexClassSelected = EditorGUILayout.Popup(indexClassSelected, options.ToArray());
                        
                    selectedCharacter.characterClass = loadedClasses[indexClassSelected];
                }else
                {
                    GUILayout.Label("Please create at least one class.");
                }

                SerializedObject serializedObject = new SerializedObject(selectedCharacter);

                EditorGUILayout.PropertyField(serializedObject.FindProperty("baseItems"), new GUIContent { text = "Base Inventory" });
                EditorGUILayout.PropertyField(serializedObject.FindProperty("baseSpells"), new GUIContent { text = "Base Spells" });

                if (GUILayout.Button("Save"))
                {
                    selectedCharacter.jsonRepresentation = selectedCharacter.GetJsonData();
                    EditorUtility.SetDirty(selectedCharacter);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedCharacter), $"{this.selectedCharacter}_{NormalizeName(selectedCharacter.characterName)}");
                    RefreshCharacters();
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Select character by id
        /// </summary>
        /// <param name="id">unique identifier</param>
        void SelectCharacter(int id)
        {
            selectedCharacter = id;
            EditorUtility.FocusProjectWindow();
        }

        void RefreshCharacters()
        {
            loadedCharacters = Resources.LoadAll<Character>("Characters");
        }

        /// <summary>
        /// Show items tab editor gui
        /// </summary>
        void ItemsGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(GUILayout.Height(254));

            Rect rectPos = EditorGUILayout.GetControlRect();
            Rect rectBox = new Rect(rectPos.x, rectPos.y, rectPos.width, 250f);

            Rect viewRect = new Rect(rectBox.x, rectBox.y, rectBox.width, loadedItems.Length * 20f);

            scrollPosition = GUI.BeginScrollView(rectBox, scrollPosition, viewRect, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);

            int viewCount = 15;
            int firstIndex = (int)scrollPosition.y / 22;

            Rect contentPos = new Rect(rectBox.x, firstIndex * 18f, rectBox.width, 20f);

            for (int i = firstIndex; i < Mathf.Min(loadedItems.Length, firstIndex + viewCount); i++)
            {
                contentPos.y += 22f;
                if (GUI.Button(contentPos, $"{loadedItems[i].itemName}"))
                    SelectItem(loadedItems[i].id);
            }

            GUI.EndScrollView();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Create new item"))
            {
                Item newItem = CreateInstance<Item>();

                newItem.id = loadedItems.Length == 0 ? 0 : loadedItems[loadedItems.Length - 1].id + 1;
                newItem.itemName = "New Item";

                AssetDatabase.CreateAsset(newItem, $"Assets/Resources/Items/{newItem.id}_NEW_ITEM.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                RefreshItems();

                SelectItem(newItem.id);
            }

            if (loadedItems.Length > 0)
            {
                Item selectedItem = loadedItems.Where(i => i.id == this.selectedItem).FirstOrDefault();

                if (GUILayout.Button($"Duplicate {selectedItem.itemName}"))
                {
                    Item newItem = CreateInstance<Item>();

                    newItem.id = loadedItems.Length == 0 ? 0 : loadedItems[loadedItems.Length - 1].id + 1;
                    newItem.itemName = selectedItem.itemName;
                    newItem.itemValue = selectedItem.itemValue;
                    newItem.type = selectedItem.type;

                    AssetDatabase.CreateAsset(newItem, $"Assets/Resources/Items/{newItem.id}_NEW_ITEM.asset");
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    RefreshItems();

                    SelectItem(newItem.id);
                }

                if (GUILayout.Button($"Delete {selectedItem.itemName}"))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedItem));
                    AssetDatabase.SaveAssets();

                    if(this.selectedItem > 0)
                        this.selectedItem--;

                    RefreshItems();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if (loadedItems.Length == 0)
            {
                GUILayout.Label($"Please create at least one item.");
            }
            else
            {
                Item selectedItem = loadedItems.Where(i => i.id == this.selectedItem).FirstOrDefault();

                GUILayout.Label($"Selected item: {selectedItem.id}");
                EditorGUILayout.LabelField("ID:", selectedItem.id.ToString());

                selectedItem.itemName = EditorGUILayout.TextField("Item Name:", selectedItem.itemName);
                selectedItem.description = EditorGUILayout.TextArea(selectedItem.description);

                int indexTypeSelected = (int)selectedItem.type;

                List<string> options = new List<string>();

                foreach(Item.ItemType type in Enum.GetValues(typeof(Item.ItemType)))
                    options.Add(type.ToString());

                indexTypeSelected = EditorGUILayout.Popup(indexTypeSelected, options.ToArray());

                selectedItem.type = (Item.ItemType)indexTypeSelected;

                if (GUILayout.Button("Save"))
                {
                    selectedItem.jsonRepresentation = selectedItem.GetJsonData();
                    EditorUtility.SetDirty(selectedItem);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedItem), $"{this.selectedItem}_{NormalizeName(selectedItem.itemName)}");
                    RefreshItems();
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Select Item by id
        /// </summary>
        /// <param name="id">unique identifier</param>
        void SelectItem(int id)
        {
            selectedItem = id;
            EditorUtility.FocusProjectWindow();
        }

        void RefreshItems()
        {
            loadedItems = Resources.LoadAll<Item>("Items");
        }

        /// <summary>
        /// Show spells tab editor gui
        /// </summary>
        void SpellsGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(GUILayout.Height(254));

            Rect rectPos = EditorGUILayout.GetControlRect();
            Rect rectBox = new Rect(rectPos.x, rectPos.y, rectPos.width, 250f);

            Rect viewRect = new Rect(rectBox.x, rectBox.y, rectBox.width, loadedSpells.Length * 20f);

            scrollPosition = GUI.BeginScrollView(rectBox, scrollPosition, viewRect, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);

            int viewCount = 15;
            int firstIndex = (int)scrollPosition.y / 22;

            Rect contentPos = new Rect(rectBox.x, firstIndex * 18f, rectBox.width, 20f);

            for (int i = firstIndex; i < Mathf.Min(loadedSpells.Length, firstIndex + viewCount); i++)
            {
                contentPos.y += 22f;
                if (GUI.Button(contentPos, $"{loadedSpells[i].spellName}"))
                    SelectSpell(loadedSpells[i].id);
            }

            GUI.EndScrollView();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Create new spell"))
            {
                Spell newSpell = CreateInstance<Spell>();

                newSpell.id = loadedSpells.Length == 0 ? 0 : loadedSpells[loadedSpells.Length - 1].id + 1;
                newSpell.spellName = "New Spell";

                AssetDatabase.CreateAsset(newSpell, $"Assets/Resources/Spells/{newSpell.id}_NEW_SPELL.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                RefreshSpells();

                SelectSpell(newSpell.id);
            }

            if (loadedSpells.Length > 0)
            {
                Spell selectedSpell = loadedSpells.Where(i => i.id == this.selectedSpell).FirstOrDefault();

                if (GUILayout.Button($"Duplicate {selectedSpell.spellName}"))
                {
                    Spell newSpell = CreateInstance<Spell>();

                    newSpell.id = loadedSpells.Length == 0 ? 0 : loadedSpells[loadedSpells.Length - 1].id + 1;
                    newSpell.spellName = selectedSpell.spellName;
                    newSpell.spellValue = selectedSpell.spellValue;
                    newSpell.type = selectedSpell.type;
                    newSpell.mana = selectedSpell.mana;

                    AssetDatabase.CreateAsset(newSpell, $"Assets/Resources/Spells/{newSpell.id}_NEW_SPELL.asset");
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    RefreshSpells();

                    SelectSpell(newSpell.id);
                }

                if (GUILayout.Button($"Delete {selectedSpell.spellName}"))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedSpell));
                    AssetDatabase.SaveAssets();

                    if(this.selectedSpell > 0)
                        this.selectedSpell--;

                    RefreshSpells();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if (loadedSpells.Length == 0)
            {
                GUILayout.Label($"Please create at least one spell.");
            }
            else
            {
                Spell selectedSpell = loadedSpells.Where(i => i.id == this.selectedSpell).FirstOrDefault();

                GUILayout.Label($"Selected class: {selectedSpell.spellName}");
                EditorGUILayout.LabelField("ID:", selectedSpell.id.ToString());

                selectedSpell.spellName = EditorGUILayout.TextField("Spell Name:", selectedSpell.spellName);
                selectedSpell.description = EditorGUILayout.TextArea(selectedSpell.description);

                int indexTypeSelected = (int)selectedSpell.type;

                List<string> options = new List<string>();

                foreach (Spell.SpellType type in Enum.GetValues(typeof(Spell.SpellType)))
                    options.Add(type.ToString());

                indexTypeSelected = EditorGUILayout.Popup(indexTypeSelected, options.ToArray());

                selectedSpell.type = (Spell.SpellType)indexTypeSelected;

                if(selectedSpell.type == Spell.SpellType.Attack)
                {
                    selectedSpell.spellValue = EditorGUILayout.IntField("Attack Damages:", selectedSpell.spellValue);
                }
                else
                {
                    selectedSpell.spellValue = EditorGUILayout.IntField("Heal Given:", selectedSpell.spellValue);
                }

                selectedSpell.mana = EditorGUILayout.IntField("Mana Required:", selectedSpell.mana);

                if (GUILayout.Button("Save"))
                {
                    selectedSpell.jsonRepresentation = selectedSpell.GetJsonData();
                    EditorUtility.SetDirty(selectedSpell);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedSpell), $"{this.selectedSpell}_{NormalizeName(selectedSpell.spellName)}");
                    RefreshSpells();
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Select Spell by id
        /// </summary>
        /// <param name="id">unique identifier</param>
        void SelectSpell(int id)
        {
            selectedSpell = id;
            EditorUtility.FocusProjectWindow();
        }

        void RefreshSpells()
        {
            loadedSpells = Resources.LoadAll<Spell>("Spells");
        }

        /// <summary>
        /// Show classes tab editor gui
        /// </summary>
        void ClassesGUI()
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(GUILayout.Height(254));

            Rect rectPos = EditorGUILayout.GetControlRect();
            Rect rectBox = new Rect(rectPos.x, rectPos.y, rectPos.width, 250f);

            Rect viewRect = new Rect(rectBox.x, rectBox.y, rectBox.width, loadedClasses.Length * 20f);

            scrollPosition = GUI.BeginScrollView(rectBox, scrollPosition, viewRect, false, true, GUIStyle.none, GUI.skin.verticalScrollbar);

            int viewCount = 15;
            int firstIndex = (int)scrollPosition.y / 22;

            Rect contentPos = new Rect(rectBox.x, firstIndex * 18f, rectBox.width, 20f);

            for (int i = firstIndex; i < Mathf.Min(loadedClasses.Length, firstIndex + viewCount); i++)
            {
                contentPos.y += 22f;
                if (GUI.Button(contentPos, $"{loadedClasses[i].className}"))
                    SelectClass(loadedClasses[i].id);
            }

            GUI.EndScrollView();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Create new class"))
            {
                CharacterClass newClass = CreateInstance<CharacterClass>();

                newClass.id = loadedClasses.Length == 0 ? 0 : loadedClasses[loadedClasses.Length - 1].id + 1;
                newClass.className = "New Class";

                AssetDatabase.CreateAsset(newClass, $"Assets/Resources/Classes/{newClass.id}_NEW_CLASS.asset");
                AssetDatabase.SaveAssets();

                EditorUtility.FocusProjectWindow();

                RefreshClasses();

                SelectClass(newClass.id);
            }

            if (loadedClasses.Length > 0)
            {
                CharacterClass selectedClass = loadedClasses.Where(i => i.id == this.selectedClass).FirstOrDefault();

                if (GUILayout.Button($"Duplicate {selectedClass.className}"))
                {
                    CharacterClass newClass = ScriptableObject.CreateInstance<CharacterClass>();

                    newClass.id = loadedClasses.Length == 0 ? 0 : loadedClasses[loadedClasses.Length - 1].id + 1;
                    newClass.className = selectedClass.className;

                    AssetDatabase.CreateAsset(newClass, $"Assets/Resources/Classes/{newClass.id}_NEW_CLASS.asset");
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    RefreshClasses();

                    SelectClass(newClass.id);
                }

                if (GUILayout.Button($"Delete {selectedClass.className}"))
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(selectedClass));
                    AssetDatabase.SaveAssets();

                    if(this.selectedClass > 0)
                        this.selectedClass--;

                    RefreshClasses();
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical();

            if (loadedClasses.Length == 0)
            {
                GUILayout.Label($"Please create at least one class.");
            }
            else
            {
                CharacterClass selectedClass = loadedClasses.Where(i => i.id == this.selectedClass).FirstOrDefault();

                GUILayout.Label($"Selected class: {selectedClass.className}");
                EditorGUILayout.LabelField("ID:", selectedClass.id.ToString());

                selectedClass.className = EditorGUILayout.TextField("Class Name:", selectedClass.className);

                if (GUILayout.Button("Save"))
                {
                    selectedClass.jsonRepresentation = selectedClass.GetJsonData();
                    EditorUtility.SetDirty(selectedClass);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(selectedClass), $"{this.selectedClass}_{NormalizeName(selectedClass.className)}");
                    RefreshClasses();
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Select class by id
        /// </summary>
        /// <param name="id">unique identifier</param>
        void SelectClass(int id)
        {
            selectedClass = id;
            EditorUtility.FocusProjectWindow();
        }

        void RefreshClasses()
        {
            loadedClasses = Resources.LoadAll<CharacterClass>("Classes");
        }

        string NormalizeName(string value)
        {
            return value.Replace(" ", "_").ToUpper();
        }
    }
}
