using UnityEngine;
using Mirror;
using TMPro;
using Untitled.UI;
using Untitled.ChatSystem;
using System.Collections.Generic;

namespace Untitled.PlayerSystem
{
    public class PlayerSetup : NetworkBehaviour
    {
        public List<Item> playerInventory = new List<Item>();
        public List<Spell> playerSpells = new List<Spell>();

        [SyncVar(hook = nameof(OnUsernameChanged))]
        public string username = "Guest";

        [SerializeField]
        private TextMeshPro usernameText;

        [SyncVar(hook = nameof(OnLifeChanged))]
        public int life = 0;
        public int baseLife;

        public int mana = 4;
        public int baseMana = 4;

        private Character selectedCharacter;

        public override void OnStartClient()
        {
            base.OnStartClient();

            // Check if is local player before LocalStart or RemoteStart
            if (isLocalPlayer)
                LocalStart();
            else
                RemoteStart();

            usernameText.text = username;
        }

        /// <summary>
        /// Perform a local start (executed only on local player)
        /// </summary>
        private void LocalStart()
        {
            Kama.player = this;
            // Set player transform to target of the camera
            PlayerCamera.instance.SetTransformTarget(transform);

            UserInterface.instance.SetLife(life);
            UserInterface.instance.SetUsernameText(username);
            UserInterface.instance.InitPlayer(this);
        }

        /// <summary>
        /// Perform a remote start (executed on remote clients)
        /// </summary>
        private void RemoteStart()
        {

        }

        void OnLifeChanged(int _, int value)
        {
            if(isLocalPlayer)
            {
                UserInterface.instance.SetLife(value);
            }
        }

        /// <summary>
        /// Callback when synced username variable is changed
        /// </summary>
        /// <param name="_">Old username</param>
        /// <param name="value">New username</param>
        void OnUsernameChanged(string _, string value)
        {
            usernameText.text = value;
            UserInterface.instance.SetUsernameText(value);
        }

        /// <summary>
        /// Select character (sended to the server)
        /// </summary>
        /// <param name="selectedCharacter"></param>
        [Command]
        public void CmdSelectCharacter(int selectedCharacter)
        {
            Character[] characters = Resources.LoadAll<Character>("Characters");

            this.selectedCharacter = characters[selectedCharacter];

            life = this.selectedCharacter.life;
            baseLife = this.selectedCharacter.life;
            username = this.selectedCharacter.characterName;

            playerInventory = this.selectedCharacter.baseItems;
            playerSpells = this.selectedCharacter.baseSpells;

            TargetSendText("<color=#52baff>Bienvenue sur UntitledKama, vous êtes actuellement dans le village d'Alcania.</color>");
            TargetSendText("<color=#f5334d>Appuyez sur <color=white>Entrée</color> pour ouvrir le tchat.</color>");
            TargetSendText("<color=orange>Pour démarrer un combat, faites <color=white>Clic droit</color> sur un joueur ou un Paysan Agressif.</color>");
        }

        /// <summary>
        /// Send text to the chat
        /// </summary>
        /// <param name="text"></param>
        [Command]
        public void CmdSendText(string text)
        {
            if (ServerChat.instance.OnInputText(this, text))
                return;

            PlayerSetup[] players = FindObjectsOfType<PlayerSetup>();

            for(int i = 0; i < players.Length; i++)
                players[i].TargetSendText($"<b>{username}</b> : {text}");
        }

        /// <summary>
        /// Display a message in the user chat
        /// </summary>
        /// <param name="text"></param>
        [TargetRpc]
        public void TargetSendText(string text)
        {
            Chat.instance.SendText(text);
        }
    }
}