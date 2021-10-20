using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable.Collections;
using System;
using MLAPI.Connection;
using MLAPI.SceneManagement;
using MLAPI.Messaging;

public class MP_Lobby : NetworkBehaviour
{
    [SerializeField] private LobbyPanel[] lobbyPlayers;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Button startGameButton;
    //holds a list of network players
    private NetworkList<MP_PlayerInfo> nwPlayers = new NetworkList<MP_PlayerInfo>();


    // Start is called before the first frame update
    void Start()
    {
        if (IsOwner)
        {
            UpdateConnListServerRpc(NetworkManager.LocalClientId);
        }
        
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
    }

    public override void NetworkStart()
    {
        Debug.Log("StartingServer");
        if (IsClient)
        {
            nwPlayers.OnListChanged += PlayersInfoChanged;
        }
        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += ClientConnectedHandle;
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconnectedHandle;
            //handle for players connected
            foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                ClientConnectedHandle(client.ClientId);
            }
        }
    }

    private void OnDestroy()
    {
        nwPlayers.OnListChanged -= PlayersInfoChanged;
        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= ClientConnectedHandle;
            NetworkManager.Singleton.OnClientDisconnectCallback -= ClientDisconnectedHandle;
        }
    }

    private void PlayersInfoChanged(NetworkListEvent<MP_PlayerInfo> changeEvent)
    {
        //update the UI lobby
        int index = 0;
        foreach(MP_PlayerInfo connectedplayer in nwPlayers)
        {
            lobbyPlayers[index].playerName.text = connectedplayer.networkPlayerName;
            lobbyPlayers[index].readyIcon.SetIsOnWithoutNotify(connectedplayer.networkPlayerReady);
            index++;
        }
        for(;index < 4; index++)
        {
            lobbyPlayers[index].playerName.text = "Player Name";
            lobbyPlayers[index].readyIcon.SetIsOnWithoutNotify(false);
            index++;
        }
       
        if (IsHost)
        {
            startGameButton.gameObject.SetActive(true);
            startGameButton.interactable = CheckEveryoneReady();
        }
    }

    public void StartGame()
    {
        if (IsServer)
        {
            //spawn a playerprefab for each connected player
            NetworkSceneManager.OnSceneSwitched += SceneSwitched;
            NetworkSceneManager.SwitchScene("Main Game");
        }
        else
        {
            Debug.Log("You are not the host");
        }
    }


    /**************
     * HANDLES
     * ***********/

    private void HandleClientConnected(ulong clientID)
    {
        if (IsOwner)
        {
            UpdateConnListServerRpc(clientID);
        }
        
        Debug.Log("A Player has connected ID: " + clientID);
    }

    [ServerRpc]

    private void UpdateConnListServerRpc(ulong clientID)
    {
        nwPlayers.Add(new MP_PlayerInfo(clientID, PlayerPrefs.GetString("PName"), false));
    }

    private void ClientDisconnectedHandle(ulong clientID)
    {
        for(int indx = 0; indx < nwPlayers.Count; indx++)
        {
            if(clientID == nwPlayers[indx].networkClientID)
            {
                nwPlayers.RemoveAt(indx);
                Debug.Log("A Player has left ID: " + clientID);

                break;
            }
        }
    }

    private void ClientConnectedHandle(ulong clientID)
    {
        Debug.Log("TODO: Player Connected");
    }

    //Ready Functions
    [ServerRpc(RequireOwnership = false)]
    
    private void ReadyUpServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int indx = 0; indx <nwPlayers.Count; indx++)
        {
            if(nwPlayers[indx].networkClientID == serverRpcParams.Receive.SenderClientId)
            {
                Debug.Log("Update with new");
                nwPlayers[indx] = new MP_PlayerInfo(nwPlayers[indx].networkClientID, nwPlayers[indx].networkPlayerName, !nwPlayers[indx].networkPlayerReady);
            }
        }
    }

    public void ReadyButtonPressed()
    {
        ReadyUpServerRpc();
        if (IsLocalPlayer)
        {
            Debug.Log("Ready Pressed");
        }
    }

    private void SceneSwitched()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        //spawn a playerprefab for each connected client
        foreach (MP_PlayerInfo tmpClient in nwPlayers)
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            int index = UnityEngine.Random.Range(0, spawnPoints.Length);
            GameObject currentPoint = spawnPoints[index];
            GameObject playerSpawn = Instantiate(playerPrefab, currentPoint.transform.position, Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(tmpClient.networkClientID);
            Debug.Log("Player spawned for: " + tmpClient.networkPlayerName);
        }
    }

    private bool CheckEveryoneReady()
    {
        foreach(MP_PlayerInfo players in nwPlayers)
        {
            if (!players.networkPlayerReady)
            {
                return false;
            }
        }
        return true;
    }
}
