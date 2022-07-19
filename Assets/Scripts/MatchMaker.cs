using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Diagnostics;
using UnityEditor;
using UnityEngine.UI;
public class MatchMaker : NetworkBehaviour
{    
    List<ulong> waitingPlayers = new();
    string myID;
    public Button button;
    int port = 1000;
    public void PlayerReadyButton()
    {
        myID = NetworkManager.Singleton.LocalClientId.ToString("D16");
        UnityEngine.Debug.Log("my client ID: " + myID);
        PlayerReadyServerRpc(myID);

        button.enabled = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerReadyServerRpc(string ID)
    {
        UnityEngine.Debug.Log("player added to waitlist");
        waitingPlayers.Add(ulong.Parse(ID));

        TryMakeMatch();
    }

    public void TryMakeMatch()
    {
        List<ulong> readyPlayers = new(waitingPlayers);                         // how to make copy of list if it's initialized as member
        if (readyPlayers.Count % 2 != 0)
        {
            readyPlayers.Remove(readyPlayers[readyPlayers.Count - 1]);

            ulong lastPlayer = waitingPlayers[waitingPlayers.Count - 1];
            waitingPlayers.Clear();
            waitingPlayers.Add(lastPlayer);                                     // clean waiting list
        }
        else
        {
            waitingPlayers.Clear();
        }

        for (int i = 0; i < readyPlayers.Count; i = i + 2)
        {
            Process process = new();
            process.StartInfo.FileName = Application.dataPath;
            Process.Start("DefenseGame.x86_64", port.ToString() + " -launch-as-server");             // start server

            ClientRpcParams crp1 = new();
            crp1.Send.TargetClientIds = new ulong[] { readyPlayers[i], readyPlayers[i+1] };
            StartGameClientRpc(crp1);                                                             // start clients 
            port++;
        }

                                                        

    }
    [ClientRpc]
    public void StartGameClientRpc(ClientRpcParams clientRpcParams)
    {
        string commmand = port.ToString() + " -launch-as-client";
        Process process = Process.Start("DefenseGame.exe", commmand);
        //process.WaitForExit();
        //process.Close();
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }
}
