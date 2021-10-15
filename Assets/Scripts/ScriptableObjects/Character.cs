using UnityEngine;
using System.Collections.Generic;

namespace Untitled
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Kama/Character")]
    public class Character : ScriptableObject
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int id;
        /// <summary>
        /// Character's name (will be displayed in game)
        /// </summary>
        public string characterName;

        /// <summary>
        /// Character's life points
        /// </summary>
        public int life = 10;

        /// <summary>
        /// Character's class
        /// </summary>
        public CharacterClass characterClass;

        public List<Item> baseItems = new List<Item>();
        public List<Spell> baseSpells = new List<Spell>();

        public string jsonRepresentation;

        public string GetJsonData()
        {
            return JsonUtility.ToJson(this);
        }
    }
}