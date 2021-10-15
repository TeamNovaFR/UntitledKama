using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Untitled.PlayerSystem;

namespace Untitled.UI
{
    public class UserInterface : MonoBehaviour
    {
        public static UserInterface instance;

        [SerializeField]
        private CharacterSelection selection;

        [SerializeField]
        private GameObject mainMenu;

        [SerializeField]
        private GameObject gameUI;

        [Header("Player Infos")]
        [SerializeField]
        private TextMeshProUGUI lifeText;

        [SerializeField]
        private TextMeshProUGUI username;

        [SerializeField]
        private GameObject contextualMenu;

        [SerializeField]
        private GameObject requestMenu;

        private PlayerCombat combat;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            selection.Init(this);
        }

        /// <summary>
        /// Hide the main menu
        /// </summary>
        public void HideMainMenu()
        {
            mainMenu.SetActive(false);
        }

        /// <summary>
        /// Show game user interface (life, inventory, character infos...)
        /// </summary>
        public void ShowGameUI()
        {
            gameUI.SetActive(true);
        }

        /// <summary>
        /// Set the username text on player ui
        /// </summary>
        /// <param name="text"></param>
        public void SetUsernameText(string text)
        {
            username.text = text;
        }

        /// <summary>
        /// Set character life on player ui
        /// </summary>
        /// <param name="life"></param>
        public void SetLife(int life)
        {
            lifeText.text = life.ToString();
        }

        /// <summary>
        /// Show contextual menu (combat, etc.)
        /// </summary>
        /// <param name="position">Position of </param>
        public void ShowContextualMenu(Vector2 position)
        {
            contextualMenu.transform.position = position;
            contextualMenu.SetActive(true);
        }

        /// <summary>
        /// Hide contextual menu (combat, etc.)
        /// </summary>
        public void HideContextualMenu()
        {
            contextualMenu.SetActive(false);
        }

        /// <summary>
        /// Send a fighting request to targeted player
        /// </summary>
        public void StartCombat()
        {
            HideContextualMenu();
            combat.SendFightRequest();
        }

        public void InitPlayer(PlayerSetup setup)
        {
            combat = setup.GetComponent<PlayerCombat>();
        }

        public void ShowRequestMenu()
        {
            requestMenu.SetActive(true);
        }

        void HideRequestMenu()
        {
            requestMenu.SetActive(false);
        }

        public void AcceptRequest()
        {
            HideRequestMenu();
            combat.CmdAcceptRequest();
        }

        public void DeclineRequest()
        {
            HideRequestMenu();
            combat.CmdDeclineRequest();
        }
    }
}