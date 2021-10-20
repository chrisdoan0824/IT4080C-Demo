using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] public Text playerName;
    [SerializeField] public Image playerIcon;
    [SerializeField] public Toggle readyIcon;
    [SerializeField] public Text waitingText;

    internal void UpdatePlayerName(Text playerNameIn)
    {
        playerName = playerNameIn;
    }
}
