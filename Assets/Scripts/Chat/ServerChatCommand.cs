using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Untitled.PlayerSystem;

namespace Untitled.ChatSystem
{
    public class ServerChatCommand
    {
        public string fullCommandName = "New Command";
        // alias also contain fullCommandName
        public string[] aliases;
        public string description;
        public string usage;
        public Action<PlayerSetup, string[]> action;

        public ServerChatCommand(string fullCommandName, string[] aliases, string description, string usage, Action<PlayerSetup, string[]> action)
        {
            this.fullCommandName = fullCommandName;

            List<string> aliasesList = new List<string>();

            aliasesList.Add(fullCommandName);

            for (int i = 0; i < aliases.Length; i++)
                aliasesList.Add(aliases[i]);

            this.aliases = aliasesList.ToArray();

            this.action = action;
            this.description = description;
            this.usage = usage;
        }

        public ServerChatCommand(string fullCommandName, string description, string usage, Action<PlayerSetup, string[]> action)
        {
            this.fullCommandName = fullCommandName;
            this.aliases = new string[] { this.fullCommandName };
            this.action = action;
            this.description = description;
            this.usage = usage;
        }

        public void Register()
        {
            if (ServerChat.instance.commands.Where(c => c.fullCommandName == fullCommandName).Count() > 0)
            {
                Debug.LogError($"[KAMA SERVER] Console Command Error: Name collision with command {fullCommandName} ({description}). Command unregistred.");
                return;
            }

            ServerChat.instance.commands.Add(this);
        }

        public void Execute(PlayerSetup player, string[] args)
        {
            action(player, args);
        }
    }
}