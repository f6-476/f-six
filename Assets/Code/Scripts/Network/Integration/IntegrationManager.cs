using UnityEngine;
using System.Collections.Generic;
using System;

public class IntegrationManager : AbstractManager<IntegrationManager>
{
    [SerializeField] private Integration[] integrations;

    public struct CommandHandler
    {
        public string key;
        public string args;
        public string instructions;
        public System.Func<Integration.OnCommandArgs, string> respond;

        public string HelpMessage => $"!{this.key} {this.args} - {this.instructions}";
    }

    private void Start()
    {
        bank = new Bank();
        house = new BettingHouse(50);
        house.Lock = true;

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

        RaceManager.OnRaceBegin += OnRaceBegin;
        RaceManager.OnNewLap += OnNewLap;
        RaceManager.OnRaceWon += OnRaceWon;
        RaceManager.OnRaceOver += OnRaceOver;
    }

    public void Connect()
    {
        Integration.OnConnected += OnConnected;
        Integration.OnDisconnected += OnDisconnected;
        Integration.OnCommand += OnCommand;

        foreach (Integration integration in integrations)
        {
            integration.Connect();
        }
    }

    public void Disconnect()
    {
        Integration.OnConnected -= OnConnected;
        Integration.OnDisconnected -= OnDisconnected;
        Integration.OnCommand -= OnCommand;

        SendMessageAll("Game is over. Goodbye!");

        foreach (Integration integration in integrations)
        {
            integration.Disconnect();
        }
    }

    private void OnConnected(Integration.OnConnectedArgs args)
    {
        args.integration.SendMessage("Hello! I'm Thimble and I'll be your assistant for this amazing race. Get started with !help.");
    }

    private void OnDisconnected(Integration.OnDisconnectedArgs args)
    {}

    private void OnCommand(Integration.OnCommandArgs args)
    {
        if (commands.TryGetValue(args.command, out CommandHandler handler))
        {
            args.integration.SendReply(args.messageId, handler.respond(args));
        }
        else 
        {
            args.integration.SendReply(args.messageId, $"Unknown command {args.command} - use !help to list available commands.");
        }
    }

    public void SendMessageAll(string message)
    {
        foreach (Integration integration in integrations)
        {
            integration.SendMessage(message);
        }
    }

    CommandHandler help = new CommandHandler {
        key = "help",
        args = "<command>",
        instructions = "displays more info on a specific command.",
        respond = (Integration.OnCommandArgs command) => {
            if(command.arguments.Count > 0 && commands.ContainsKey(command.arguments[0]))
            {
                CommandHandler handler = commands[command.arguments[0]];

                return handler.HelpMessage;
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
        args = "",
        instructions = "lists all players in the game.",
        respond = (Integration.OnCommandArgs command) => {
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
        args = "",
        instructions = "displays the amount of credits you have",
        respond = (Integration.OnCommandArgs command) => "You have C" + bank.GetBalance(command.username)
    };

    CommandHandler bet = new CommandHandler {
        key = "bet",
        args = "<player-name> <amount>",
        instructions = "if you guess the winner correctly you will double your stake!",
        respond = (Integration.OnCommandArgs command) => {
            if (command.arguments.Count < 2)
            {
                return $"Argument(s) missing. Use !help <command> in doubt.";
            }

            string outcome = command.arguments[0];
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
                stake = UInt32.Parse(command.arguments[1]);
            }
            catch (OverflowException)
            {
                return $"C{command.arguments[1]} is too big.";
            }
            catch (FormatException)
            {
                return $"Cannot parse {command.arguments[1]}. Make sure it is a positive integer number.";
            }

            if (stake < house.Minimum)
            {
                return $"Minimum stake is C{house.Minimum}";
            }

            if (bank.Withdraw(command.username, stake))
            {
                if (!house.RegisterBet(command.username, outcome, stake))
                {
                    bank.Deposit(command.username, stake);
                    return $"Bets are currently locked. Wait for a new race to start, or for this one to finish.";
                }

                return $"Bet registered successfully.";
            }

            return $"You don't have enough credits.";
        }
    };

    CommandHandler powerups = new CommandHandler {
        key = "powerups",
        args = "",
        instructions = "lists available powerups.",
        respond = (Integration.OnCommandArgs command) =>
        {
            PowerUpConfig[] configs = RaceManager.Singleton.PowerUpConfigs;

            List<string> powerupsName = new List<string>();

            foreach (var config in configs)
            {
                powerupsName.Add(config.name.ToLower().Replace(" ", "-"));
            }

            return $"Available powerups: {String.Join(", ", powerupsName)}.";
        }
    };

    CommandHandler sponsor = new CommandHandler {
        key = "sponsor",
        args = "<player-name> <powerup>",
        instructions = "award give a player a power-up in exchange for C50.",
        respond = (Integration.OnCommandArgs command) => {
            if (command.arguments.Count < 2)
            {
                return $"Argument(s) missing. Use !help <command> in doubt.";
            }

            string beneficiary = command.arguments[0];
            Ship ship = null;

            foreach (Ship someShip in RaceManager.Singleton.Ships)
            {
                if (beneficiary == someShip.Multiplayer.Lobby.Value.Username)
                {
                    ship = someShip;
                    break;
                }
            }

            if (ship == null)
            {
                return $"Cannot find player {beneficiary}.";
            }

            string powerup = command.arguments[1];

            PowerUpConfig[] configs = RaceManager.Singleton.PowerUpConfigs;

            int i = 0;
            foreach (var config in configs)
            {
                if (powerup == config.name.ToLower().Replace(" ", "-"))
                {
                    break;
                }
                
                i++;
            }

            if (i >= configs.Length)
            {
                return $"Invalid powerup {powerup}";
            }

            if (!bank.Withdraw(command.username, 50))
            {
                return "You need at least C50 to sponsor a player";
            }

            ship.PowerUp.PickUpPowerUp(i);

            return $"A fresh {powerup} was given to {beneficiary}.";
        }
    };    

    CommandHandler maps = new CommandHandler {
        key = "maps",
        args = "",
        instructions = "lists available maps.",
        respond = (Integration.OnCommandArgs command) => {
            MapConfig[] maps = LobbyManager.Singleton.GetMaps();

            List<String> mapHandles = new List<string>();

            foreach(MapConfig map in maps)
            {
                mapHandles.Add(map.displayName.ToLower().Replace(" ", "-"));
            }

            return $"Available maps: {String.Join(", ", mapHandles)}."; 
        }
    };

    CommandHandler vote = new CommandHandler {
        key = "vote",
        args = "<map-name>",
        instructions = "vote for the next map.",
        respond = (Integration.OnCommandArgs command) => {
            if (command.arguments.Count < 1)
            {
                return $"Argument missing. Use !help <command> in doubt.";
            }

            string mapArgument = command.arguments[0];

            List<String> mapHandles = new List<string>();

            MapConfig[] maps = LobbyManager.Singleton.GetMaps();
            
            bool mapExists = false;

            string handle = "";

            foreach(MapConfig map in maps)
            {
                handle = map.displayName.ToLower().Replace(" ", "-");

                if (handle == mapArgument)
                {
                    mapExists = true;
                    break;
                }
            }

            if (!mapExists)
            {
                return $"Invalid map name {mapArgument}. Use !maps to list available maps.";
            }

            voting.RegisterVote(command.username, handle);

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

        public void DepositAll(uint amount)
        {
            var keys = new List<string>(_bank.Keys);

            foreach(var key in keys)
            {
                _bank[key] += amount;
            }
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

        public bool Lock {get; set;}

        public BettingHouse(uint minimum)
        {
            _bets = new List<(string, string, uint)>();
            Minimum = minimum;
        }

        public bool RegisterBet(string username, string outcome, uint stake)
        {
            if (Lock)
            {
                return false;
            }

            _bets.Add((username, outcome, stake));

            return true;
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

        public void RegisterVote(string username, string map)
        {
            _votes[username] = map;
        }

        public (Dictionary<string, int>, string) GetResults()
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

    private void OnRaceOver()
    {
        bank.DepositAll(100);

        (Dictionary<string, int> results, string nextMapHandle) = voting.GetResults();

        if (results.Count > 0)
        {
            SendMessageAll($"Race Over! The next map is {nextMapHandle}, with {results[nextMapHandle]} vote(s).");

            MapConfig[] maps = LobbyManager.Singleton.GetMaps();
            int i = 0;

            foreach(MapConfig map in maps)
            {
                if (nextMapHandle == map.displayName.ToLower().Replace(" ", "-"))
                {
                    LobbyManager.Singleton.MapIndex = i;
                }
                
                i++;
            }
        }
        else
        {
            SendMessageAll($"Race Over!");
        }
    }

    public void OnRaceBegin()
    {
        house.Lock = false;

        SendMessageAll("Race Begins! Bets are unlocked!");
    }

    public void OnNewLap(int lap)
    {
        if (lap >= 2 && !house.Lock)
        {
            house.Lock = true;

            SendMessageAll("Bets are now locked.");
        }
    }

    public void OnRaceWon(string username)
    {
        house.ResolveBets(username, bank);

        SendMessageAll($"Race Won by {username}!");
    }
}
