using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Untitled.UI
{
    public class CharacterSelectionItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI characterName;
        [SerializeField]
        private TextMeshProUGUI characterClass;

        private CharacterSelection selection;
        private Character character;

        public void Init(CharacterSelection selection, Character character)
        {
            this.selection = selection;

            characterName.text = character.characterName;
            characterClass.text = character.characterClass.className;
            this.character = character;
        }

        public void SelectCharacter()
        {
            selection.SelectCharacter(character);
        }
    }
}