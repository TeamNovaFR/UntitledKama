using UnityEngine;

namespace Untitled
{
    [CreateAssetMenu(fileName = "New Spell", menuName = "Kama/Spell")]
    public class Spell : ScriptableObject
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int id;
        /// <summary>
        /// Spell's name (will be displayed in game)
        /// </summary>
        public string spellName;

        public string description = "An useful description";

        public int spellValue;
        public SpellType type;

        public int mana;
        public string jsonRepresentation;

        public string GetJsonData()
        {
            return JsonUtility.ToJson(this);
        }

        public enum SpellType
        {
            Attack,
            Heal
        }
    }
}