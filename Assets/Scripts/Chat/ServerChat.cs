using UnityEngine;
using Mirror;
using Untitled.PlayerSystem;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Untitled.ChatSystem
{
    public class ServerChat : NetworkBehaviour
    {
        public static ServerChat instance;

        public List<ServerChatCommand> commands = new List<ServerChatCommand>();

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            RegisterCommands();
        }

        /// <summary>
        /// Register commons commands
        /// </summary>
        void RegisterCommands()
        {
            ServerChatCommand helpCommand = new ServerChatCommand("/help", "List all commands", "/help (page)", (player, args) =>
            {
                int page = 0;

                if (args.Length > 0)
                {
                    if (int.TryParse(args[0], out int n))
                    {
                        page = n - 1;
                    }
                }

                List<ServerChatCommand> commands = ServerChat.instance.commands;

                player.TargetSendText($"<color=#db9b32>====== Liste des commandes ({page + 1}/{Math.Ceiling(commands.Count / 5.0)}) ======</color>");

                for (int i = 5 * page; i < (5 * page) + 5; i++)
                {
                    if (commands.Count <= i) break;

                    ServerChatCommand command = commands[i];
                    player.TargetSendText($"<color=#db9b32>{command.fullCommandName} : {command.description} | usage : {command.usage}");
                }
            });

            helpCommand.Register();

            ServerChatCommand giveCommand = new ServerChatCommand("/give", "Give an item", "/give <itemId>", (player, args) =>
            {
                if(args.Length == 0)
                {
                    player.TargetSendText("<color=red>USAGE: /give <itemId></color>");
                    return;
                }

                Item item = Database.GetItem(int.Parse(args[0]));

                if(item == null)
                {
                    player.TargetSendText("<color=red>L'id de cet item n'existe pas.</color>");
                    return;
                }

                player.playerInventory.Add(item);
                player.TargetSendText($"Vous avez reçu 1x {item.itemName}.");
            });

            giveCommand.Register();

            ServerChatCommand removeCommand = new ServerChatCommand("/remove", "Remove an item from inventory", "/remove <itemId>", (player, args) =>
            {
                if (args.Length == 0)
                {
                    player.TargetSendText("<color=red>USAGE: /remove <itemId></color>");
                    return;
                }

                Item item = Database.GetItem(int.Parse(args[0]));

                if (item == null)
                {
                    player.TargetSendText("<color=red>L'id de cet item n'existe pas.</color>");
                    return;
                }

                if (player.playerInventory.Contains(item))
                {
                    player.playerInventory.Remove(item);
                    player.TargetSendText($"1x {item.itemName} vous a été retiré de votre inventaire.");
                }
                else
                {
                    player.TargetSendText("<color=red>Vous n'avez pas cet objet dans votre inventaire.</color>");
                }

            });

            removeCommand.Register();

            ServerChatCommand inventoryCommand = new ServerChatCommand("/inventory", "See your inventory", "/inventory", (player, args) =>
            {
                string objects = "";

                foreach(Item item in player.playerInventory)
                {
                    objects += $"x1 {item.itemName}, ";
                }

                player.TargetSendText($"Votre inventaire: {objects}");
            });

            inventoryCommand.Register();

            ServerChatCommand spellsCommand = new ServerChatCommand("/spells", "See your spells", "/spells", (player, args) =>
            {
                string spells = "";

                foreach (Spell spell in player.playerSpells)
                {
                    spells += $"[{spell.id}] - {spell.spellName} (req. <color=orange>{spell.mana} mana</color>)";
                }

                player.TargetSendText($"Vos sorts: {spells}");
            });

            spellsCommand.Register();

            ServerChatCommand itemCommand = new ServerChatCommand("/item", "See item info by id", "/item <itemId>", (player, args) =>
            {
                if(args.Length == 0)
                {
                    player.TargetSendText("<color=red>USAGE: /item <itemId></color>");
                    return;
                }

                Item item = Database.GetItem(int.Parse(args[0]));

                if (item == null)
                {
                    player.TargetSendText("<color=red>L'id de cet item n'existe pas.</color>");
                    return;
                }

                string type = "aucun";

                if (item.type == Item.ItemType.Food)
                    type = "Consommable";
                else if (item.type == Item.ItemType.Resource)
                    type = "Ressource";
                else
                    type = "Arme";

                player.TargetSendText($"<color=orange>{item.itemName}</color> — Type: {type} — Description: {item.description}");
            });

            itemCommand.Register();

            ServerChatCommand manaCommand = new ServerChatCommand("/mana", "See your mana", "/mana", (player, args) =>
            {
                player.TargetSendText($"<color=orange>Vous avez <color=white>{player.mana} mana</color>.</color>");
            });

            manaCommand.Register();

            ServerChatCommand spellCommand = new ServerChatCommand("/spell", "See spell info by id", "/spell <spellId>", (player, args) =>
            {
                if (args.Length == 0)
                {
                    player.TargetSendText("<color=red>USAGE: /spell <spellId></color>");
                    return;
                }

                Spell spell = Database.GetSpell(int.Parse(args[0]));

                if (spell == null)
                {
                    player.TargetSendText("<color=red>L'id de ce sort n'existe pas.</color>");
                    return;
                }

                string type = spell.type == Spell.SpellType.Attack ? "Attaque" : "Soin";

                player.TargetSendText($"<color=orange>{spell.spellName}</color> — Type: {type} — Description: {spell.description}");
            });

            spellCommand.Register();

            ServerChatCommand useCommand = new ServerChatCommand("/use", "Use a spell in a combat", "/use <spellId>", (player, args) =>
            {
                if (args.Length == 0)
                {
                    player.TargetSendText("<color=red>USAGE: /use <spellId></color>");
                    return;
                }

                Spell spell = Database.GetSpell(int.Parse(args[0]));

                if (spell == null)
                {
                    player.TargetSendText("<color=red>L'id de ce sort n'existe pas.</color>");
                    return;
                }

                if(player.mana < spell.mana)
                {
                    player.TargetSendText($"<color=red>Vous n'avez pas assez de mana pour lancer ce sort (vous avez <color=white>{player.mana} mana</color>).</color>");
                    return;
                }

                if(player.playerSpells.Contains(spell))
                {
                    PlayerCombat combat = player.GetComponent<PlayerCombat>();

                    if(combat.IsInCombatWithBot())
                    {
                        Bot targetBot = combat.CurrentCombatBot();

                        if (spell.type == Spell.SpellType.Attack)
                        {
                            player.TargetSendText($"<color=orange>Vous avez lancé le sort {spell.spellName} et vous infligez {spell.spellValue} de dégâts à votre ennemi.</color>");

                            if (targetBot.life - spell.spellValue <= 0)
                            {
                                NetworkServer.Destroy(targetBot.gameObject);

                                player.life = player.baseLife;
                                player.mana = player.baseMana;

                                player.TargetSendText($"<color=white>Vous avez éliminé votre adversaire gagné le combat.</color>");
                                player.TargetSendText("<color=#83C165>Le combat est dès à présent terminé.</color>");

                                combat.ClearCombat();
                                return;
                            }
                            else
                            {
                                targetBot.life -= spell.spellValue;
                                player.mana = Mathf.Clamp(player.mana - spell.mana + 2, 0, player.baseMana);

                                player.TargetSendText($"<color=orange>Votre adversaire a maintenant <color=white>{targetBot.life} points de vie</color>.</color>");
                            }
                        }
                        else
                        {
                            player.TargetSendText($"<color=orange>Vous avez lancé le sort {spell.spellName} et vous gagnez {spell.spellValue} points de vie.</color>");
                            player.mana = Mathf.Clamp(player.mana - spell.mana + 2, 0, player.baseMana);

                            player.life = Mathf.Clamp(player.life + spell.spellValue, 0, player.baseLife);
                        }

                        if(targetBot != null && targetBot.life > 0)
                        {
                            player.TargetSendText("<color=#83C165>C'est le tour de votre adversaire.</color>");

                            Spell usedSpell = targetBot.botSpells[UnityEngine.Random.Range(0, targetBot.botSpells.Length)];

                            if (usedSpell.type == Spell.SpellType.Attack)
                            {
                                if (player.life - usedSpell.spellValue <= 0)
                                {
                                    player.life = player.baseLife;
                                    player.mana = player.baseMana;
                                    targetBot.life = targetBot.baseLife;
                                    targetBot.inCombat = false;
                                    player.TargetSendText($"<color=orange>Votre adversaire a lancé le sort {usedSpell.spellName}, vous avez perdu le combat.</color>");
                                    player.TargetSendText("<color=#83C165>Le combat est dès à présent terminé.</color>");

                                    combat.ClearCombat();

                                    return;
                                }
                                else
                                {
                                    player.life -= usedSpell.spellValue;
                                    player.mana = Mathf.Clamp(player.mana - spell.mana + 2, 0, player.baseMana);

                                    player.TargetSendText($"<color=orange>Votre adversaire a lancé le sort {usedSpell.spellName} contre vous, vous avez maintenant <color=white>{player.life} points de vie</color>.</color>");
                                    player.TargetSendText("<color=#83C165>C'est votre tour à présent. Utilisez <color=white>/use <spellId></color> pour lancer un sort et <color=white>/spells</color> pour consulter vos sorts.</color>");
                                }
                            }
                            else
                            {
                                targetBot.life = Mathf.Clamp(targetBot.life + usedSpell.spellValue, 0, targetBot.baseLife);

                                player.TargetSendText($"<color=orange>Votre adversaire a lancé le sort {usedSpell.spellName} et a maintenant <color=white>{targetBot.life} points de vie</color>.</color>");
                                player.TargetSendText("<color=#83C165>C'est votre tour à présent. Utilisez <color=white>/use <spellId></color> pour lancer un sort et <color=white>/spells</color> pour consulter vos sorts.</color>");
                            }
                        }
                    }
                    else
                    {
                        if (!combat.IsInCombat())
                        {
                            player.TargetSendText("<color=red>Vous devez être en combat pour utiliser ce sort.</color>");
                            return;
                        }

                        if (!combat.IsYourTurn())
                        {
                            player.TargetSendText("<color=red>Ce n'est pas votre tour.</color>");
                            return;
                        }

                        PlayerSetup targetPlayer = combat.CurrentCombatPlayer().GetComponent<PlayerSetup>();

                        if (spell.type == Spell.SpellType.Attack)
                        {
                            player.TargetSendText($"<color=orange>Vous avez lancé le sort {spell.spellName} et vous infligez {spell.spellValue} de dégâts à votre ennemi.</color>");

                            if (targetPlayer.life - spell.spellValue <= 0)
                            {
                                targetPlayer.life = targetPlayer.baseLife;
                                player.life = player.baseLife;
                                player.mana = player.baseMana;

                                targetPlayer.TargetSendText($"<color=orange>{player.username} a lancé le sort {spell.spellName}, vous avez perdu le combat.</color>");
                                player.TargetSendText($"<color=orange>Vous avez lancé le sort {spell.spellName} et gagné le combat.</color>");

                                targetPlayer.TargetSendText("<color=#83C165>Le combat est dès à présent terminé.</color>");
                                player.TargetSendText("<color=#83C165>Le combat est dès à présent terminé.</color>");

                                combat.CurrentCombatPlayer().ClearCombat();
                                combat.ClearCombat();

                                return;
                            }
                            else
                            {
                                targetPlayer.life -= spell.spellValue;
                                player.mana = Mathf.Clamp(player.mana - spell.mana + 2, 0, player.baseMana);

                                targetPlayer.TargetSendText($"<color=orange>{player.username} a lancé le sort {spell.spellName} contre vous, vous avez maintenant <color=white>{targetPlayer.life} points de vie</color>.</color>");
                                player.TargetSendText($"<color=orange>{targetPlayer.username} a maintenant <color=white>{targetPlayer.life} points de vie</color>.</color>");
                            }
                        }
                        else
                        {
                            player.TargetSendText($"<color=orange>Vous avez lancé le sort {spell.spellName} et vous gagnez {spell.spellValue} points de vie.</color>");

                            player.life = Mathf.Clamp(player.life + spell.spellValue, 0, player.baseLife);
                            player.mana = Mathf.Clamp(player.mana - spell.mana + 2, 0, player.baseMana);

                            targetPlayer.TargetSendText($"<color=orange>{player.username} a lancé le sort {spell.spellName} et a maintenant <color=white>{player.life} points de vie</color>.</color>");
                        }

                        if (combat.IsInCombat())
                        {
                            combat.SetTurn(false);
                            combat.CurrentCombatPlayer().SetTurn(true);

                            player.TargetSendText("<color=#83C165>C'est le tour de votre adversaire.</color>");
                            targetPlayer.TargetSendText("<color=#83C165>C'est votre tour à présent. Utilisez <color=white>/use <spellId></color> pour lancer un sort et <color=white>/spells</color> pour consulter vos sorts.</color>");
                        }
                    }
                }else
                {
                    player.TargetSendText("<color=red>Vous n'avez pas ce sort dans votre inventaire.</color>");
                }
            });

            useCommand.Register();
        }

        public bool OnInputText(PlayerSetup player, string message)
        {
            string[] _args = message.Split(char.Parse(" "));

            if (_args?.Length > 0)
            {
                string command = _args[0];

                string[] args = new string[_args.Length - 1];

                if (_args.Length > 1)
                    for (int i = 1; i < _args.Length; i++)
                        args[i - 1] = _args[i];

                return RunCommands(player, command, args);
            }

            return false;
        }

        public bool RunCommands(PlayerSetup player, string command, string[] args)
        {
            for (int i = 0; i < commands.Count; i++)
                for (int x = 0; x < commands[i].aliases.Length; x++)
                    if (commands[i].aliases[x].ToLower() == command.ToLower())
                    {
                        commands[i].Execute(player, args);
                        return true;
                    }

            return false;
        }
    }
}