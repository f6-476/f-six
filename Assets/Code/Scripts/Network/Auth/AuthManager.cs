using UnityEngine;
using Unity.Netcode;

public class AuthManager : AbstractManager<AuthManager>
{
    private string username;
    public string Username
    {
        get => username;
        set => username = value;
    }

    private string serverToken;
    public string ServerToken
    {
        get => serverToken;
        set => serverToken = value;
    }

    private string twitchChannel = "";
    public string TwitchChannel
    {
        get => twitchChannel;
        set => twitchChannel = value;
    }

    private string discordChannel = "";
    public string DiscordChannel
    {
        get => discordChannel;
        set => discordChannel = value;
    }

    private static readonly string[] ADJECTIVES = new string[]{
        "crazy",
        "speedy",
        "pretty",
        "funny",
        "happy"
    };
    private static readonly string[] NAMES = new string[]{
        "Kiwi",
        "Banana",
        "Orange",
        "Pineapple",
        "Strawberry"
    };
    public static string GenerateUsername()
    {
        string adjective = ADJECTIVES[Random.Range(0, ADJECTIVES.Length)];
        string name = NAMES[Random.Range(0, NAMES.Length)];
        int number = Random.Range(10, 100);

        return $"{adjective}{name}{number}";
    }
    private void Start() {
        this.username = GenerateUsername();
    }
}
