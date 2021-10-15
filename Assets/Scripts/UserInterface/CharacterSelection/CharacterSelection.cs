using UnityEngine;

namespace Untitled.UI
{
    public class CharacterSelection : MonoBehaviour
    {
        [SerializeField]
        private Transform content;

        [SerializeField]
        private CharacterSelectionItem characterSelectionItemPrefab;

        private UserInterface ui;

        public void Init(UserInterface ui)
        {
            this.ui = ui;
            LoadCharacters();
        }

        /// <summary>
        /// Load all characters from asset database
        /// </summary>
        void LoadCharacters()
        {
            Clear();

            Character[] loadedCharacters = Resources.LoadAll<Character>("Characters");

            for(int i = 0; i < loadedCharacters.Length; i++)
            {
                CharacterSelectionItem item = Instantiate(characterSelectionItemPrefab, content);

                item.Init(this, loadedCharacters[i]);
            }
        }

        /// <summary>
        /// Clear character content (grid ui group)
        /// </summary>
        void Clear()
        {
            foreach(Transform child in content)
            {
                Destroy(content.gameObject);
            }
        }

        /// <summary>
        /// Select character and spawn with character name
        /// </summary>
        /// <param name="character">Character Scriptable Object</param>
        public void SelectCharacter(Character character)
        {
            Kama.player.CmdSelectCharacter(character.id);
            ui.HideMainMenu();
            ui.ShowGameUI();
        }
    }
}