using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class GameManager : NetworkBehaviour
{
    public ImprovisedPlayerScript playerPrefab;
    private Vector3 spawnPoint1 = new Vector3(-13, 3 , 12);
    private Vector3 spawnPoint2 = new Vector3(-13, 3, -8);
    public static GameManager instance { get; private set; }

    public int noOfPlayers { get; private set; }

    private ImprovisedPlayerScript player1;
    private ImprovisedPlayerScript player2;

    public List<ImprovisedPlayerScript> playerList { get; private set; }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += NewPlayerConnected;
        }
        playerList = new List<ImprovisedPlayerScript>();

        base.OnNetworkSpawn();
    }

    void NewPlayerConnected(ulong playerID)
    {
        if (IsServer)
        {
            noOfPlayers++;

            if (noOfPlayers == 1)
            {
                player1 = Instantiate(playerPrefab, spawnPoint1, Quaternion.identity);
                player1.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
                playerList.Add(player1);
            }
            else if (noOfPlayers == 2)
            {
                player2 = Instantiate(playerPrefab, spawnPoint2, Quaternion.identity);
                player2.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);
                playerList.Add(player2);
            }
        }
    }

    public ImprovisedPlayerScript GetPlayer1()
    {
        return player1;
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
