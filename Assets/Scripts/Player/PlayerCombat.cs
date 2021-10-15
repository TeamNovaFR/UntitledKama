using UnityEngine;
using Mirror;
using Untitled.UI;
using Untitled.ChatSystem;

namespace Untitled.PlayerSystem
{
    public class PlayerCombat : NetworkBehaviour
    {
        [SerializeField]
        private LayerMask clickMask;

        /// <summary>
        /// Which player is currently targeted
        /// </summary>
        private PlayerCombat targetedPlayer;
        /// <summary>
        /// Which bot is currently targeted
        /// </summary>
        private NetworkIdentity targetedBot;
        /// <summary>
        /// Which player will you fight if you accept request (server var)
        /// </summary>
        private PlayerCombat requestPlayer;
        /// <summary>
        /// Combat Player (server var) used for spells
        /// </summary>
        private PlayerCombat combatPlayer;
        /// <summary>
        /// Combat bot (server var) used for spells
        /// </summary>
        private Bot combatBot;

        /// <summary>
        /// Determine if it's your turn or not (server var)
        /// </summary>
        private bool yourTurn;

        private PlayerSetup setup;

        private void Awake()
        {
            setup = GetComponent<PlayerSetup>();
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            if(Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, clickMask))
                {
                    PlayerCombat target = hit.collider.GetComponent<PlayerCombat>();
                    Bot targetBot = hit.collider.GetComponent<Bot>();

                    if(targetBot != null)
                        RightClickBot(targetBot.netIdentity);
                    else if (target != this && target != null)
                        RightClick(target);
                }
            }
        }

        /// <summary>
        /// Handle right click on other players
        /// </summary>
        /// <param name="target">Player Target</param>
        void RightClick(PlayerCombat target)
        {
            targetedPlayer = target;

            UserInterface.instance.ShowContextualMenu(Input.mousePosition);
        }

        /// <summary>
        /// Handle right click on other bots
        /// </summary>
        /// <param name="target">Bot Target</param>
        void RightClickBot(NetworkIdentity target)
        {
            targetedBot = target;

            UserInterface.instance.ShowContextualMenu(Input.mousePosition);
        }

        public void SendFightRequest()
        {
            if (targetedPlayer != null)
                CmdSendFightRequest(targetedPlayer.netId);
            else if (targetedBot != null)
                CmdSendFightRequestBot(targetedBot.netId);
        }

        [Command]
        void CmdSendFightRequestBot(uint botId)
        {
            combatBot = NetworkClient.spawned[botId].GetComponent<Bot>();

            if (combatBot.inCombat)
                return;

            yourTurn = true;

            combatBot.inCombat = true;

            setup.TargetSendText("<color=#83C165>C'est votre tour. Utilisez <color=white>/use <spellId></color> pour lancer un sort et <color=white>/spells</color> pour consulter vos sorts.</color>");
        }

        [Command]
        void CmdSendFightRequest(uint playerId)
        {
            PlayerCombat targetedCombat = NetworkClient.spawned[playerId].GetComponent<PlayerCombat>();

            if (combatPlayer || combatBot || targetedCombat.combatPlayer)
                return;

            targetedCombat.requestPlayer = this;
            targetedCombat.TargetFightRequest(netId);
        }

        [TargetRpc]
        void TargetFightRequest(uint playerId)
        {
            PlayerCombat combat = NetworkClient.spawned[playerId].GetComponent<PlayerCombat>();

            UserInterface.instance.ShowRequestMenu();
        }

        [Command]
        public void CmdAcceptRequest()
        {
            if(requestPlayer == null)
            {
                setup.TargetSendText("<color=red>Impossible d'accepter la requête.</color>");
                return;
            }

            requestPlayer.setup.TargetSendText($"<color=#66CDAA>{setup.username} a accepté votre demande de combat</color>");
            setup.TargetSendText($"<color=#66CDAA>Vous avez accepté sa demande de combat</color>");

            setup.TargetSendText("<color=#83C165>C'est votre tour. Utilisez <color=white>/use <spellId></color> pour lancer un sort et <color=white>/spells</color> pour consulter vos sorts.</color>");
            requestPlayer.setup.TargetSendText($"<color=#83C165>C'est le tour de votre adversaire.</color>");

            yourTurn = true;
            requestPlayer.yourTurn = false;

            combatPlayer = requestPlayer;
            requestPlayer.combatPlayer = this;
        }

        [Command]
        public void CmdDeclineRequest()
        {
            if(requestPlayer == null)
            {
                setup.TargetSendText("<color=red>Impossible de refuser la requête.</color>");
                return;
            }

            requestPlayer.setup.TargetSendText($"<color=red>{setup.username} a refusé votre demande de combat</color>");
            setup.TargetSendText($"<color=red>Vous avez refusé sa demande de combat</color>");

            requestPlayer.requestPlayer = null;
            requestPlayer.combatPlayer = null;

            requestPlayer = null;
            combatPlayer = null;
        }

        public bool IsInCombat()
        {
            return combatPlayer != null;
        }

        public bool IsInCombatWithBot()
        {
            return combatBot != null;
        }

        public bool IsYourTurn()
        {
            return yourTurn;
        }

        public PlayerCombat CurrentCombatPlayer()
        {
            return combatPlayer;
        }

        public Bot CurrentCombatBot()
        {
            return combatBot;
        }

        public void SetTurn(bool turn)
        {
            yourTurn = turn;
        }

        public void ClearCombat()
        {
            combatPlayer = null;
            targetedPlayer = null;
            requestPlayer = null;
            combatBot = null;
        }
    }
}