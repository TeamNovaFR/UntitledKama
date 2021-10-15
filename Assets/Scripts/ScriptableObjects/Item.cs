using UnityEngine;

namespace Untitled
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Kama/Item")]
    public class Item : ScriptableObject
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int id;
        /// <summary>
        /// Item's name (will be displayed in game)
        /// </summary>
        public string itemName;

        public string description = "An awesome description";

        /// <summary>
        /// Cases: 
        ///     Food (will add life)
        ///     Weapon
        ///     Resources
        /// </summary>
        public int itemValue;
        public ItemType type;

        public string jsonRepresentation;

        public string GetJsonData()
        {
            return JsonUtility.ToJson(this);
        }

        public enum ItemType
        {
            Food,
            Weapon,
            Resource
        }
    }
}