using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using MLAPI;
using MLAPI.SceneManagement;
using UnityEngine.UI;

public class MP_TestStart : NetworkBehaviour
{
    [SerializeField] private InputField playerName;
    public void HostButtonClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartHost();
        NetworkSceneManager.SwitchScene("Lobby");
    }

    public void ClientButtonClicked()
    {
        PlayerPrefs.SetString("PName", playerName.text);
        NetworkManager.Singleton.StartClient();
        Debug.Log("Client Started");
    }
}
