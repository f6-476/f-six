using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class NetworkUI : MonoBehaviour
{
    [SerializeField] private Button clientButton;
    [SerializeField] private Button serverButton;

    private void Start()
    {
        clientButton.onClick.AddListener(ClientButton);
        serverButton.onClick.AddListener(ServerButton);
    }

    private void ClientButton() 
    {
        NetworkManager.Singleton.StartClient();
    }

    private void ServerButton()
    {
        NetworkManager.Singleton.StartHost();
    }
}
