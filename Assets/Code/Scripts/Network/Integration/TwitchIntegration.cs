using UnityEngine;
using Unity.Netcode;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using System.Collections.Generic;
using System;

public class TwitchIntegration : Integration
{

    public struct CommandHandler
    {
        public string key;
        public string instructions;
        public System.Func<ChatCommand, string> respond;
    }

    private Client client;

    private string channel;

    CommandHandler help = new CommandHandler {
        key = "help",
        instructions = "!help <command> - displays more info on a specific command.",
        respond = (ChatCommand command) => {
            if(command.ArgumentsAsList.Count > 0)
            {
                return commands[command.ArgumentsAsList[0]].instructions;
            }
            else
            {
                List<string> commandKeys = new List<string>();

                foreach(string key in commands.Keys)
                {
                    commandKeys.Add("!" + key);
                }

                return "Available commands: " + String.Join(", ", commandKeys) + ".";
            }
        }
    };

    CommandHandler players = new CommandHandler {
        key = "players",
        instructions = "!players - lists all players in the game.",
        respond = (ChatCommand command) => {
            List<string> players = new List<string>();

            foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
            {
                players.Add($"{player.Username} ({player.ClientMode})");
            }

            return $"The List of players:\n\n{String.Join(",\n\n", players)}";
        }
    };

    CommandHandler credits = new CommandHandler {
        key = "credits",
        instructions = "!credits - displays the amount of credits you have",
        respond = (ChatCommand command) => "You have C" + bank.GetBalance(command.ChatMessage.Username)
    };

    CommandHandler bet = new CommandHandler {
        key = "bet",
        instructions = "!bet <player-name> <amount> - if you guess the winner correctly you will double your stake!",
        respond = (ChatCommand command) => {
            if (command.ArgumentsAsList.Count < 2)
            {
                return $"Argument(s) missing. Use !help <command> in doubt.";
            }

            string outcome = command.ArgumentsAsList[0];
            bool playerExists = false;

            foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
            {
                if (outcome == player.Username && player.ClientMode != ClientMode.SPECTATOR)
                {
                    playerExists = true;
                    break;
                }
            }

            if (!playerExists)
            {
                return $"Cannot find player {outcome}.";
            }
            uint stake = 0;
            try {
                stake = UInt32.Parse(command.ArgumentsAsList[1]);
            }
            catch (OverflowException)
            {
                return $"C{command.ArgumentsAsList[1]} is too big.";
            }
            catch (FormatException)
            {
                return $"Cannot parse {command.ArgumentsAsList[1]}. Make sure it is a positive integer number.";
            }

            if (stake < house.Minimum)
            {
                return $"Minimum stake is C{house.Minimum}";
            }

            if (bank.Withdraw(command.ChatMessage.Username, stake))
            {
                house.RegisterBet(command.ChatMessage.Username, outcome, stake);

                return $"Bet registered successfully.";
            }

            return $"You don't have enough credits.";
        }
    };

    CommandHandler powerups = new CommandHandler {
        key = "powerups",
        instructions = "!powerups - lists available powerups.",
        respond = (ChatCommand command) =>
        {
            return "Available powerups: shield, missiles, boost.";
        }
    };

    CommandHandler sponsor = new CommandHandler {
        key = "sponsor",
        instructions = "!sponsor <player-name> <powerup> - award give a player a power-up in exchange for C50.",
        respond = (ChatCommand command) => {
            if (command.ArgumentsAsList.Count < 2)
            {
                return $"Argument(s) missing. Use !help <command> in doubt.";
            }

            string beneficiary = command.ArgumentsAsList[0];

            bool playerExists = false;

            foreach (LobbyPlayer player in LobbyManager.Singleton.Players)
            {
                if (beneficiary == player.Username && player.ClientMode != ClientMode.SPECTATOR)
                {
                    playerExists = true;
                    break;
                }
            }

            if (!playerExists)
            {
                return $"Cannot find player {beneficiary}.";
            }

            string powerup = command.ArgumentsAsList[1];

            if (!(powerup == "shield" || powerup == "missiles" || powerup == "boost"))
            {
                return $"Invalid powerup {powerup}";
            }

            if (!bank.Withdraw(command.ChatMessage.Username, 50))
            {
                return "You need at least C50 to sponsor a player";
            }

            return $"TODO: give {beneficiary} {powerup}";
        }
    };    

    CommandHandler maps = new CommandHandler {
        key = "maps",
        instructions = "!maps - lists available maps.",
        respond = (ChatCommand command) => {
            return "Available maps: rusty-giant, space-donut. TODO: Hook this up with actual maps."; 
        }
    };

    CommandHandler vote = new CommandHandler {
        key = "vote",
        instructions = "!vote <map-name> - vote for the next map.",
        respond = (ChatCommand command) => {
            if (command.ArgumentsAsList.Count < 1)
            {
                return $"Argument missing. Use !help <command> in doubt.";
            }

            string map = command.ArgumentsAsList[0];

            if (!voting.RegisterVote(command.ChatMessage.Username, map)) {
                return $"Invalid map name. Use !maps to list available maps.";
            }

            return $"Vote registered successfully.";
        }
    };

    public class Bank
    {
        private Dictionary<string, uint> _bank;

        public Bank()
        {
            _bank = new Dictionary<string, uint>();
        }

        public uint GetBalance(string username)
        {
            uint moneys;

            if (!_bank.TryGetValue(username, out moneys))
            {
                _bank.Add(username, 100);
                moneys = 100;
            }

            return moneys;
        }

        public void Deposit(string username, uint amount)
        {
            uint balance = GetBalance(username);

            _bank[username] += amount;
        }

        public bool Withdraw(string username, uint amount)
        {
            uint balance = GetBalance(username);

            if (balance < amount) return false;

            _bank[username] -= amount;

            return true;
        }
    }

    public class BettingHouse
    {
        private List<(string, string, uint)> _bets;

        private uint _minimum;

        public uint Minimum {
            get => _minimum;
            private set => _minimum = value;
        }

        public BettingHouse(uint minimum)
        {
            _bets = new List<(string, string, uint)>();
            Minimum = minimum;
        }

        public void RegisterBet(string username, string outcome, uint stake)
        {
            _bets.Add((username, outcome, stake));
        }

        public void ResolveBets(string victor, Bank bank)
        {
            foreach ((string username, string outcome, uint stake) in _bets)
            {
                if (outcome == victor)
                {
                    bank.Deposit(username, stake * 2);
                }
            }

            _bets.Clear();
        }
    }

    public class MapVoting
    {
        private Dictionary<string, string> _votes;

        public MapVoting()
        {
            _votes = new Dictionary<string, string>();
        }

        public bool RegisterVote(string username, string map)
        {
            // Validate map-name
            if (!(map == "rusty-giant" || map == "space-donut"))
            {
                return false;
            }

            _votes[username] = map;

            return true;
        }

        public (Dictionary<string, int>, string) GetResult()
        {
            Dictionary<string, int> results = new Dictionary<string, int>();

            string max = "";
            int maxCount = -1;

            foreach (string map in _votes.Values)
            {
                int count = 0;

                results.TryGetValue(map, out count);

                results[map] = ++count;

                if (count > maxCount)
                {
                    maxCount = count;
                    max = map;
                }
            }

            _votes.Clear();

            return (results, max);
        }
    }

    public static Bank bank;
    public static BettingHouse house;
    public static MapVoting voting;
    public static Dictionary<string, CommandHandler> commands;

    public override void Connect()
    {
        bank = new Bank();
        house = new BettingHouse(50);
        voting = new MapVoting();

        commands = new Dictionary<string, CommandHandler>();
        commands.Add(help.key, help);
        commands.Add(players.key, players);
        commands.Add(credits.key, credits);
        commands.Add(bet.key, bet);
        commands.Add(powerups.key, powerups);
        commands.Add(sponsor.key, sponsor);
        commands.Add(maps.key, maps);
        commands.Add(vote.key, vote);

        Debug.Log("Connection to Twitch...");

        ConnectionCredentials credentials = new ConnectionCredentials("thimblebot", Secrets.Twitch.BOT_ACCESS_TOKEN);

        client = new Client();

        client.Initialize(credentials, AuthManager.Singleton.TwitchChannel);

        client.OnError += OnError;
        client.OnJoinedChannel += OnJoinedChannel;
        client.OnMessageReceived += OnMessageReceived;
        client.OnChatCommandReceived += OnChatCommandReceived;
        client.OnConnected += OnConnected;

        client.Connect();
    }

    public override void Disconnect()
    {
        if(client != null) client.Disconnect();
    }

    private void OnError(object sender, TwitchLib.Communication.Events.OnErrorEventArgs e)
    {
        Debug.Log($"Error: {e}");
    }

    private void OnJoinedChannel(object sender, TwitchLib.Client.Events.OnJoinedChannelArgs e)
    {
        Debug.Log("Joined Channel");

        OnConnectedInner(new OnConnectedArgs {
            name="Twitch"
        });

        client.SendMessage(e.Channel, "Hello! I'm Thimble and I'll be your assistant for this amazing race. Get started with !help.");

        channel = e.Channel;
    }

    private void OnMessageReceived(object sender, TwitchLib.Client.Events.OnMessageReceivedArgs e)
    {
        Debug.Log($"Message {e.ChatMessage.Username}: {e.ChatMessage.Message}");
    }

    private void OnChatCommandReceived(object sender, TwitchLib.Client.Events.OnChatCommandReceivedArgs e)
	{
        Debug.Log($"Chat Command {e.Command.ChatMessage.Username} {e.Command.CommandText} {e.Command.ArgumentsAsString}");

        if (commands.TryGetValue(e.Command.CommandText, out CommandHandler handler))
        {
            client.SendReply(channel, e.Command.ChatMessage.Id, handler.respond(e.Command));
        }

        client.SendReply(channel, e.Command.ChatMessage.Id, $"Unknown command {e.Command.CommandText} - use !help to list available commands.");
    }

    private void OnConnected(object sender, TwitchLib.Client.Events.OnConnectedArgs e)
    {
        Debug.Log($"OnConnected: {e}");

        // client.JoinChannel("antoinepaulinb7");
    }
}