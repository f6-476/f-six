using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

public class ServerManager : AbstractManager<ServerManager>
{
    private static readonly string[] SERVERS = {
        "http://localhost:13337"
    };

    private bool foundServer = false;
    private string mainServer;
    public IEnumerator RefreshMainServer()
    {
        foreach (string server in SERVERS)
        {
            UnityWebRequest request = UnityWebRequest.Get($"{server}/ping");
            yield return request.SendWebRequest();
            if (request.responseCode == 200)
            {
                foundServer = true;
                mainServer = server;

                yield return RefreshServers();

                break;
            }
        }
    }

    public delegate void RefreshMainServerCallback();
    public IEnumerator RefreshMainServer(RefreshMainServerCallback callback)
    {
        yield return RefreshMainServer();
        callback();
    }

    [System.Serializable]
    private struct ServersResponse
    {
        public Server[] result;
    }
    [System.Serializable]
    public struct Server
    {
        public string id;
        public string name;
        public int count;
    }
    private List<Server> servers = new List<Server>();
    public List<Server> Servers
    {
        get => servers;
    }
    public IEnumerator RefreshServers()
    {
        servers = new List<Server>();
        if (foundServer)
        {
            UnityWebRequest request = UnityWebRequest.Get($"{mainServer}/servers/");
            yield return request.SendWebRequest();
            if (request.responseCode == 200)
            {
                string response = request.downloadHandler.text;
                ServersResponse serversResponse = JsonUtility.FromJson<ServersResponse>("{\"result\":" + response + "}");
                servers = new List<Server>(serversResponse.result);
            }
        }
    }

    public delegate void RefreshServerCallback();
    public IEnumerator RefreshServers(RefreshServerCallback callback)
    {
        yield return RefreshServers();
        callback();
    }

    [System.Serializable]
    private struct GetServerResponse
    {
        public string id;
        public string name;
        public string host;
        public int port;
    }

    [System.Serializable]
    public struct Config 
    {
        public string name;
        public string host;
        public int port;
        public string password;
    }

    public IEnumerator JoinServer(string id, string password)
    {
        if (foundServer)
        {
            string json = $"{{\"password\":\"{password}\"}}";

            UnityWebRequest request = new UnityWebRequest($"{mainServer}/servers/{id}", "POST");
            byte[] requestBody = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(requestBody);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                GetServerResponse response = JsonUtility.FromJson<GetServerResponse>(request.downloadHandler.text);

                LobbyManager.Config lobbyConfig = new LobbyManager.Config {
                    id = response.id,
                    name = response.name,
                    host = response.host,
                    port = response.port,
                    password = password
                };

                LobbyManager.Singleton.JoinLobby(lobbyConfig);
            }
        }
    }

    public Config GetConfig(string password)
    {
        return new Config
        {
            name = AuthManager.Singleton.Username,
            host = "127.0.0.1",
            port = 7777,
            password = password.Trim()
        };
    }

    [System.Serializable]
    private struct CreateServerResponse
    {
        public string id;
        public string name;
        public string host;
        public int port;
        public string token;
    }
    public IEnumerator HostServer(string password)
    {
        if (foundServer)
        {
            Config config = GetConfig(password);

            string json = JsonUtility.ToJson(config);

            UnityWebRequest request = new UnityWebRequest($"{mainServer}/servers/", "POST");
            byte[] requestBody = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(requestBody);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                CreateServerResponse response = JsonUtility.FromJson<CreateServerResponse>(request.downloadHandler.text);
                AuthManager.Singleton.ServerToken = response.token;

                LobbyManager.Config lobbyConfig = new LobbyManager.Config {
                    id = response.id,
                    name = response.name,
                    host = response.host,
                    port = response.port,
                    password = password
                };

                LobbyManager.Singleton.HostLobby(lobbyConfig);
            }
        }
    }

    public IEnumerator UpdatePlayerCount(string id, int count)
    {
        if (foundServer && !id.Equals("0"))
        {
            string json = $"{{\"count\":\"{count}\"}}";

            UnityWebRequest request = new UnityWebRequest($"{mainServer}/servers/{id}", "PUT");
            byte[] requestBody = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(requestBody);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {AuthManager.Singleton.ServerToken}");

            yield return request.SendWebRequest();
        }
    }
}
