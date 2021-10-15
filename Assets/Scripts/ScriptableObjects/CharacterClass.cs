using UnityEngine;

namespace Untitled
{
    [CreateAssetMenu(fileName = "New Class", menuName = "Kama/Class")]
    public class CharacterClass : ScriptableObject
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int id;
        /// <summary>
        /// Class's name (will be displayed in game)
        /// </summary>
        public string className;

        public string jsonRepresentation;

        public string GetJsonData()
        {
            return JsonUtility.ToJson(this);
        }
    }
}