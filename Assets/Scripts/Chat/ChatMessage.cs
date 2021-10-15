using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Untitled.ChatSystem
{
    public class ChatMessage : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI text;

        public void SetText(string text)
        {
            // Get the size of the text for the given string.
            Vector2 textSize = this.text.GetPreferredValues(text);
            this.text.text = text;
            this.text.autoSizeTextContainer = true;
            this.text.rectTransform.sizeDelta = textSize;
        }
    }
}