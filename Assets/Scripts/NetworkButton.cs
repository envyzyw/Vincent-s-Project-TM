using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net.NetworkInformation;

public class NetworkButton : MonoBehaviour
{
    public void StartServer()
    {
        NetworkManager.Singleton.StartServer();
        Destroy(gameObject);
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
        Destroy(gameObject);
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        Destroy(gameObject);
    }
}
