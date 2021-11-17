using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MP_ChatUIScript : NetworkBehaviour
{
    public Text chatText = null;
    public InputField chatInput = null;

    NetworkVariableString messages = new NetworkVariableString("Temp");

    public NetworkList<MP_PlayerInfo> chatPlayers;
    private string playerName = "N/A";

    private bool showScore = false;
    public GameObject scoreboardPanel;
    public Text scorePlayerName;
    public Text scoreKills;
    public Text scoreDeaths;

    // Start is called before the first frame update
    void Start()
    {
        messages.OnValueChanged += updateUIClientRpc;
        foreach(MP_PlayerInfo player in chatPlayers)
        {
            if(NetworkManager.LocalClientId == player.networkClientID) 
            {
                playerName = player.networkPlayerName;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            //show scoreboard
            showScore = true;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            //hide scoreboard            
            showScore = false;
        }
        if (showScore)
        {
            scoreboardPanel.SetActive(showScore);
            if (IsOwner)
            {
                updateScoreServerRpc();
            }
        }
        else
        {
            scoreboardPanel.SetActive(showScore);
        }
    }

    

    public void handleSend()
    {
        if (!IsServer)
        {
            SendMessageServerRpc(chatInput.text);
        }
        else
        {
            messages.Value += "\n" + playerName + ": " + chatInput.text;
        }
        
    }

    [ClientRpc]
    private void updateUIClientRpc(string previousValue, string newValue)
    {
        chatText.text += newValue.Substring(previousValue.Length, newValue.Length - previousValue.Length);
    }

    [ServerRpc]
    private void SendMessageServerRpc(string text, ServerRpcParams svrParam = default)
    {
        foreach (MP_PlayerInfo player in chatPlayers)
        {
            if (svrParam.Receive.SenderClientId == player.networkClientID)
            {
                playerName = player.networkPlayerName;
            }
        }
        messages.Value += "\n" + playerName + ": " + text;
    }
    
    [ServerRpc]
    private void updateScoreServerRpc(ServerRpcParams svrParam = default)
    {
        //clear out old scores
        clearScoreClientRpc();

        //get each player's info
        GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerObj in currentPlayers)
        {
            foreach(MP_PlayerInfo playerInfo in chatPlayers)
            {
                if(playerObj.GetComponent<NetworkObject>().OwnerClientId == playerInfo.networkClientID)
                {
                    updateScoreClientRpc(playerInfo.networkPlayerName, playerObj.GetComponent<MP_PlayerAttribs>().kills.Value, playerObj.GetComponent<MP_PlayerAttribs>().deaths.Value);
                }
            }
        }
    }

    [ClientRpc]
    private void updateScoreClientRpc(string networkPlayerName, int kills, int deaths)
    {
        if (IsOwner)
        {
            scorePlayerName.text += networkPlayerName + "\n";
            scoreKills.text += kills + "\n";
            scoreDeaths.text += deaths + "\n";
        }
    }

    [ClientRpc]
    private void clearScoreClientRpc()
    {
        
    }
}

