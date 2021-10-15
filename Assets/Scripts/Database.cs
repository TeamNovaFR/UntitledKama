using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Untitled
{
    public static class Database
    {
        public static Item[] items;
        public static Spell[] spells;
        public static Character[] characters;

        /// <summary>
        /// Init Item Manager system
        /// </summary>
        /// <param name="man">LifeManager reference</param>
        public static void Init()
        {
            items = Resources.LoadAll<Item>("Items");
            spells = Resources.LoadAll<Spell>("Spells");
            characters = Resources.LoadAll<Character>("Characters");

            Debug.Log(items.Length + " items loaded.");
            Debug.Log(spells.Length + " spells loaded.");
            Debug.Log(characters.Length + " characters loaded.");
        }

        /// <summary>
        /// Get an item by id
        /// </summary>
        /// <param name="id">Id of wanted item</param>
        /// <returns>Item Scriptable Object</returns>
        public static Item GetItem(int id)
        {
            for (int i = 0; i < items.Length; i++)
                if (items[i].id == id)
                    return items[i];

            return null;
        }

        /// <summary>
        /// Get a spell by id
        /// </summary>
        /// <param name="id">Id of wanted spell</param>
        /// <returns>Spell Scriptable Object</returns>
        public static Spell GetSpell(int id)
        {
            for (int i = 0; i < spells.Length; i++)
                if (spells[i].id == id)
                    return spells[i];

            return null;
        }

        /// <summary>
        /// Get a character by id
        /// </summary>
        /// <param name="id">Id of wanted character</param>
        /// <returns>Character Scriptable Object</returns>
        public static Character GetCharacter(int id)
        {
            for (int i = 0; i < characters.Length; i++)
                if (characters[i].id == id)
                    return characters[i];

            return null;
        }
    }
}