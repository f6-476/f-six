using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RegistryManager : AbstractManager<RegistryManager>
{
    private static readonly string[] REGISTRIES = {
        "http://localhost:13337"
    };

    private string selectedRegistry = null;
    public bool IsConnected
    {
        get => selectedRegistry != null;
    }

    private List<Server> servers = new List<Server>();
    public List<Server> Servers
    {
        get => servers;
    }

    public delegate void RefreshRegisterCallback();
    private IEnumerator RefreshRegistryAsync(RefreshRegisterCallback callback = null)
    {
        foreach (string registry in REGISTRIES)
        {
            UnityWebRequest request = UnityWebRequest.Get($"{registry}/ping");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                this.selectedRegistry = registry;

                yield return RefreshServersAsync();

                break;
            }
        }

        if (callback != null) callback();
    }

    public void RefreshRegistry(RefreshRegisterCallback callback = null)
    {
        StartCoroutine(RefreshRegistryAsync(callback));
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

    public delegate void RefreshServersCallback();
    private IEnumerator RefreshServersAsync(RefreshServersCallback callback = null)
    {
        if (IsConnected)
        {
            UnityWebRequest request = UnityWebRequest.Get($"{selectedRegistry}/servers/");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                string response = request.downloadHandler.text;
                ServersResponse serversResponse = JsonUtility.FromJson<ServersResponse>("{\"result\":" + response + "}");
                servers = new List<Server>(serversResponse.result);
            }
        }

        if (callback != null) callback();
    }

    public void RefreshServers(RefreshServersCallback callback = null)
    {
        StartCoroutine(RefreshServersAsync(callback));
    }

    [SerializeField]
    public struct InsertServerData
    {
        public string name;
        public int port;
        public string password;
    }

    [SerializeField]
    public struct InsertServerResponse
    {
        public string id;
        public string name;
        public string host;
        public int port;
        public string token;
    }

    public delegate void RegistryFailCallback();
    public delegate void InsertServerCallback(InsertServerResponse data);
    private IEnumerator InsertServerAsync(InsertServerData data, InsertServerCallback pass = null, RegistryFailCallback fail = null)
    {
        if (IsConnected)
        {
            string json = JsonUtility.ToJson(data);

            UnityWebRequest request = new UnityWebRequest($"{selectedRegistry}/servers/", "POST");
            byte[] requestBody = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(requestBody);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                InsertServerResponse response = JsonUtility.FromJson<InsertServerResponse>(request.downloadHandler.text);
                if (pass != null) pass(response);
            }
            else
            {
                if (fail != null) fail();
            }
        }
        else
        {
            if (fail != null) fail();
        }
    }
    public void InsertServer(InsertServerData data, InsertServerCallback pass = null, RegistryFailCallback fail = null)
    {
        StartCoroutine(InsertServerAsync(data, pass, fail));
    }

    public struct GetServerData
    {
        public string id;
        public string password;
    }

    public struct GetServerResponse
    {
        public string id;
        public string name;
        public string host;
        public int port;
    }

    public delegate void GetServerCallback(GetServerResponse data);
    private IEnumerator GetServerAsync(GetServerData data, GetServerCallback pass = null, RegistryFailCallback fail = null)
    {
        if (IsConnected)
        {
            string json = $"{{\"password\":\"{data.password}\"}}";

            UnityWebRequest request = new UnityWebRequest($"{selectedRegistry}/servers/{data.id}", "POST");
            byte[] requestBody = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(requestBody);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.responseCode == 200)
            {
                GetServerResponse response = JsonUtility.FromJson<GetServerResponse>(request.downloadHandler.text);

                if (response.host == null || response.host.Trim() == "" || response.port == 0)
                {
                    if (fail != null) fail();
                }
                else
                {
                    if (pass != null) pass(response);
                }
            }
            else
            {
                if (fail != null) fail();
            }
        }
        else
        {
            if (fail != null) fail();
        }
    }

    public void GetServer(GetServerData data, GetServerCallback pass = null, RegistryFailCallback fail = null)
    {
        StartCoroutine(GetServerAsync(data, pass, fail));
    }

    private IEnumerator UpdateServerPlayerCountAsync(string serverId, int count)
    {
        if (IsConnected && serverId != null)
        {
            string json = $"{{\"count\":\"{count}\"}}";

            UnityWebRequest request = new UnityWebRequest($"{selectedRegistry}/servers/{serverId}", "PUT");
            byte[] requestBody = new System.Text.UTF8Encoding().GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(requestBody);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {AuthManager.Singleton.ServerToken}");

            yield return request.SendWebRequest();
        }
    }

    public void UpdateServerPlayerCount(string serverId, int count)
    {
        StartCoroutine(UpdateServerPlayerCountAsync(serverId, count));
    }

    public IEnumerator DeleteServerAsync(string serverId)
    {
        if (IsConnected && serverId != null)
        {
            UnityWebRequest request = new UnityWebRequest($"{selectedRegistry}/servers/{serverId}", "DELETE");
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {AuthManager.Singleton.ServerToken}");

            yield return request.SendWebRequest();
        }
    }

    public void DeleteServer(string serverId)
    {
        StartCoroutine(DeleteServerAsync(serverId));
    }
}