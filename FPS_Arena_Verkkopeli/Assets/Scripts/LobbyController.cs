using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Relay.Models;

public class LobbyController : NetworkBehaviour
{
    [HideInInspector]
    public static bool isHosting;

    [SerializeField]
    private int m_MinimumPlayerCount = 1;   // Minimum player count required to transition to next level

    [SerializeField]
    private int m_MaximumPlayerCount = 4;   // Maximum player count that can join lobby

    public TMP_Text LobbyText;  // Visible list of players
    public TMP_Text readyButtonText;
    public TMP_Text joinCodeText;

    public Button buttonStart;
    public Button buttonLeave;

    public Button ButtonHost;
    public Button ButtonClient;

    public TMP_Dropdown DropdownRegions;
    public TMP_InputField InputFieldJoinCode;

    public GameObject menuCanvas;
    public GameObject lobbyCanvas;

    private bool m_AllPlayersInLobby;
    private PlayerSpawner playerSpawner;

    private Dictionary<ulong, bool> m_ClientsInLobby;

    private string m_UserLobbyStatusText;

    private UnityTransport transport;

    private void Start()
    {
        playerSpawner = GameObject.FindObjectOfType<PlayerSpawner>();

        m_ClientsInLobby = new Dictionary<ulong, bool>();

        transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport as UnityTransport;

        ButtonHost.onClick.AddListener(() => InitiateHosting(DropdownRegions.options[DropdownRegions.value].text));
        ButtonClient.onClick.AddListener(() => InitiateJoiningAsClient());

        buttonStart.gameObject.SetActive(false);
        buttonLeave.gameObject.SetActive(false);

        menuCanvas.gameObject.SetActive(true);
        lobbyCanvas.gameObject.SetActive(false);

        GetRelayRegions();
    }

    async private void GetRelayRegions()
    {
        Task<List<Region>> regionsTask = RelaySetup.GetRegions();

        try
        {
            await regionsTask;
        }
        catch (AggregateException ae)
        {
            ae.InnerExceptions.ToList().ForEach(e => Debug.LogError(e.Message + "\n Stack trace: " + e.StackTrace));
        }

        //ListRegionsAsync palauttaa listan Region-objekteja:
        List<Region> regions = regionsTask.Result;

        //Regionin Id on string id, joka voidaan antaa parametrina CreateAllocationAsync-metodille:
        regions.ForEach(r => Debug.Log(r.Description + " - region ID string: " + r.Id + "\n\n"));

        //Käyttöliittymänä yksinkertainen Dropdown, johon filtteröidään lista pelkistä Id-arvoista:
        List<string> regionStringIDs = regions.Select(r => r.Id).ToList();
        DropdownRegions.ClearOptions();
        DropdownRegions.AddOptions(regionStringIDs);
        DropdownRegions.onValueChanged.AddListener(HandleDropDownChange);

        // Laitetaan menu näkyviin, kun regionit on haettu
        menuCanvas.gameObject.SetActive(true);
    }

    private void HandleDropDownChange(int i)
    {
        Debug.Log("selected region: " + DropdownRegions.value);
    }

    // Check if lobby is full or not
    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        bool hasRoom = false;

        if (m_ClientsInLobby.Count < m_MaximumPlayerCount)
        {
            hasRoom = true;
        }

        callback(true, null, hasRoom, playerSpawner.NextSpawnTransform().position, playerSpawner.NextSpawnTransform().rotation);
    }

    private void OnGUI()
    {
        LobbyText.text = m_UserLobbyStatusText;
    }

    public void StartLocalGame()
    {
        Debug.Log("Started hosting");

        menuCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);
        buttonStart.gameObject.SetActive(true);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartHost();

        if (NetworkManager.Singleton.IsListening)
        {
            //If we are hosting, then handle the server side for detecting when clients have connected
            //and when their lobby scenes are finished loading.
            if (IsServer)
            {
                // Add host to player list
                Debug.Log("Adding host to player list...");
                m_ClientsInLobby.Add(NetworkManager.Singleton.LocalClientId, false);

                m_AllPlayersInLobby = false;

                // Server will be notified when a client connects/disconnects
                Debug.Log("Listening to connects/disconnects...");
                NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
            }

            // Update lobby
            GenerateUserStatsForLobby();
        }
    }

    public void JoinLocalGame()
    {
        menuCanvas.gameObject.SetActive(false);
        lobbyCanvas.gameObject.SetActive(true);

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.StartClient();
        Debug.Log("Starting client...");
        buttonLeave.gameObject.SetActive(true);
    }

    async private void InitiateHosting(string regionStringID = null)
    {
        //Pass region to host if such exists:
        Task<RelayHostData> hostRelaySetup = RelaySetup.HostGame(4, regionStringID);

        try
        {
            await hostRelaySetup;
        }
        catch (AggregateException ae)
        {
            ae.InnerExceptions.ToList().ForEach(e => Debug.LogError(e.Message + "\n Stack trace: " + e.StackTrace));
        }

        RelayHostData relayHostData = hostRelaySetup.Result;
        transport.SetRelayServerData(
            relayHostData.IPv4Address,
            relayHostData.Port,
            relayHostData.AllocationIDBytes,
            relayHostData.Key,
            relayHostData.ConnectionData
            );

        Debug.Log("Relay IP: " + relayHostData.IPv4Address);

        Debug.Log("Join code: " + relayHostData.JoinCode);
        joinCodeText.gameObject.SetActive(true);
        SetJoinCodeInfoText(relayHostData.JoinCode);

        StartLocalGame();
    }

    async private void InitiateJoiningAsClient()
    {
        Task<RelayJoinData> joinRelaySetup = RelaySetup.JoinGame(InputFieldJoinCode.text);

        try
        {
            await joinRelaySetup;
        }
        catch (AggregateException ae)
        {
            ae.InnerExceptions.ToList().ForEach(e => Debug.LogError(e.Message + "\n Stack trace: " + e.StackTrace));
        }

        RelayJoinData relayJoinData = joinRelaySetup.Result;
        transport.SetRelayServerData(
            relayJoinData.IPv4Address,
            relayJoinData.Port,
            relayJoinData.AllocationIDBytes,
            relayJoinData.Key,
            relayJoinData.ConnectionData,
            relayJoinData.HostConnectionData
            );

        joinCodeText.gameObject.SetActive(false);

        JoinLocalGame();
    }

    // Update visible player list
    private void GenerateUserStatsForLobby()
    {
        m_UserLobbyStatusText = string.Empty;
        foreach (var clientLobbyStatus in m_ClientsInLobby)
        {
            m_UserLobbyStatusText += "Player_" + clientLobbyStatus.Key + " ";
            if (clientLobbyStatus.Value)
                m_UserLobbyStatusText += "(Ready)\n";
            else
                m_UserLobbyStatusText += "(Not Ready)\n";
        }
    }

    // Check player count & status
    private void UpdateAndCheckPlayersInLobby()
    {
        m_AllPlayersInLobby = m_ClientsInLobby.Count >= m_MinimumPlayerCount;

        foreach (var clientLobbyStatus in m_ClientsInLobby)
        {
            SendClientReadyStatusUpdatesClientRpc(clientLobbyStatus.Key, clientLobbyStatus.Value);
            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(clientLobbyStatus.Key))
            {
                // If some clients are still loading into the lobby scene then this is false
                m_AllPlayersInLobby = false;
            }
        }
    }

    // Update join code
    public void SetJoinCodeInfoText(string joinCode)
    {
        joinCodeText.text = "Join Code: " + joinCode;
    }

    private void ClientLoadedScene(ulong clientId)
    {
        if (IsServer)
        {
            if (!m_ClientsInLobby.ContainsKey(clientId))
            {
                m_ClientsInLobby.Add(clientId, false);
                GenerateUserStatsForLobby();
            }

            UpdateAndCheckPlayersInLobby();
        }
    }

    private void OnClientConnectedCallback(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log("Client " + clientId + " has connected...");
            if (!m_ClientsInLobby.ContainsKey(clientId)) m_ClientsInLobby.Add(clientId, false);

            GenerateUserStatsForLobby();
            UpdateAndCheckPlayersInLobby();
        }
    }

    public void OnClientDisconnectedCallback(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log("Client " + clientId + " has disconnected...");
            if (m_ClientsInLobby.ContainsKey(clientId))
            {
                m_ClientsInLobby.Remove(clientId);
                RemoveDisconnectedClientRpc(clientId);
            }

            GenerateUserStatsForLobby();
            UpdateAndCheckPlayersInLobby();
        }
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        lobbyCanvas.SetActive(false);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject go in players)
        {
            if (go.GetComponent<NetworkObject>().IsOwner)
            {
                go.transform.Find("PlayerCamera").gameObject.SetActive(true);
                go.GetComponentInChildren<MouseLook>().enabled = true;
                go.transform.Find("Canvas").gameObject.SetActive(true);
            }
        }
    }

    [ClientRpc]
    private void SendClientReadyStatusUpdatesClientRpc(ulong clientId, bool isReady)
    {
        if (!IsServer)
        {
            if (!m_ClientsInLobby.ContainsKey(clientId))
                m_ClientsInLobby.Add(clientId, isReady);
            else
                m_ClientsInLobby[clientId] = isReady;
            GenerateUserStatsForLobby();
        }
    }

    [ClientRpc]
    private void RemoveDisconnectedClientRpc(ulong clientId)
    {
        if (!IsServer)
        {
            if (m_ClientsInLobby.ContainsKey(clientId))
            {
                m_ClientsInLobby.Remove(clientId);
            }
            GenerateUserStatsForLobby();
        }
    }

    public void StartGame()
    {
        if (m_AllPlayersInLobby)
        {
            var allPlayersAreReady = true;
            foreach (var clientLobbyStatus in m_ClientsInLobby)
                if (!clientLobbyStatus.Value)

                    // If some clients are still loading into the lobby scene then this is false
                    allPlayersAreReady = false;

            // Only if all players are ready
            if (allPlayersAreReady)
            {
                Debug.Log("Starting game!");

                lobbyCanvas.SetActive(false);

                GameObject mainCamera = GameObject.Find("MainCamera");

                if (mainCamera != null)
                {
                    mainCamera.SetActive(false);
                }

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject go in players)
                {
                    if (go.GetComponent<NetworkObject>().IsOwner)
                    {
                        go.transform.Find("PlayerCamera").gameObject.SetActive(true);
                        go.GetComponentInChildren<MouseLook>().enabled = true;
                        go.transform.Find("Canvas").gameObject.SetActive(true);
                    }
                }

                StartGameClientRpc();   // Activate player gameobjects on all clients

                // Remove our client connected/disconnected callback
                NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
                NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectedCallback;
            }
        }
    }

    // Update ready-status
    public void ReadyUp()
    {
        if (IsServer)
        {
            if (!m_ClientsInLobby[NetworkManager.Singleton.ServerClientId])
            {
                m_ClientsInLobby[NetworkManager.Singleton.ServerClientId] = true;
                readyButtonText.text = "Unready";
            }
            else
            {
                m_ClientsInLobby[NetworkManager.Singleton.ServerClientId] = false;
                readyButtonText.text = "Ready";
            }

            UpdateAndCheckPlayersInLobby();
        }
        else
        {
            if (!m_ClientsInLobby[NetworkManager.Singleton.LocalClientId])
            {
                m_ClientsInLobby[NetworkManager.Singleton.LocalClientId] = true;
                readyButtonText.text = "Unready";
            }
            else
            {
                m_ClientsInLobby[NetworkManager.Singleton.LocalClientId] = false;
                readyButtonText.text = "Ready";
            }

            OnClientIsReadyServerRpc(NetworkManager.Singleton.LocalClientId, m_ClientsInLobby[NetworkManager.Singleton.LocalClientId]);
        }

        GenerateUserStatsForLobby();
    }

    // Leave lobby
    public void LeaveLobby()
    {
        if (!IsServer)
        {
            Debug.Log("Leaving lobby...");
            OnClientLeaveServerRpc(NetworkManager.Singleton.LocalClientId);
            menuCanvas.SetActive(true);
        }
    }

    // Send ready-status to server
    [ServerRpc(RequireOwnership = false)]
    private void OnClientIsReadyServerRpc(ulong clientid, bool isReady)
    {
        if (m_ClientsInLobby.ContainsKey(clientid))
        {
            if (isReady)
            {
                m_ClientsInLobby[clientid] = true;
            }
            else
            {
                m_ClientsInLobby[clientid] = false;
            }

            UpdateAndCheckPlayersInLobby();
            GenerateUserStatsForLobby();
        }
    }

    // Disconnect client
    [ServerRpc(RequireOwnership = false)]
    private void OnClientLeaveServerRpc(ulong clientId)
    {
        if (IsServer)
        {
            Debug.Log("Disconnecting client " + clientId + "...");

            //var utpTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
            // utpTransport.DisconnectRemoteClient(clientId);
            NetworkManager.Singleton.DisconnectClient(clientId);

            if (m_ClientsInLobby.ContainsKey(clientId))
            {
                m_ClientsInLobby.Remove(clientId);
                RemoveDisconnectedClientRpc(clientId);
            }

            GenerateUserStatsForLobby();
            UpdateAndCheckPlayersInLobby();
        }
    }
}