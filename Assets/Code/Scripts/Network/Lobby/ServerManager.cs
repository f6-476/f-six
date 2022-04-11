using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

public class ServerManager : NetworkBehaviour
{
    private static readonly string[] SERVERS = {
        "http://localhost:13337"
    };

    private bool foundServer = false;
    private string mainServer;
    public IEnumerator RefreshMainServer()
    {
        foreach(string server in SERVERS)
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
    private List<Server> servers;
    public List<Server> Servers 
    {
        get => servers;
    }
    public IEnumerator RefreshServers()
    {
        servers = new List<Server>();
        if(foundServer) 
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

    private static ServerManager instance;
    public static ServerManager Singleton 
    {
        get => instance;
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
