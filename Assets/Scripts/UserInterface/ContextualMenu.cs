using UnityEngine;
using UnityEngine.UI;

namespace Untitled.UI
{
    public class ContextualMenu : MonoBehaviour
    {
        [SerializeField]
        private UserInterface ui;

        private void Update()
        {
            if (Input.GetMouseButton(0) && !RectTransformUtility.RectangleContainsScreenPoint(GetComponent<RectTransform>(), Input.mousePosition, null))
                gameObject.SetActive(false);
        }

        public void Combat()
        {
            ui.StartCombat();
        }
    }
}