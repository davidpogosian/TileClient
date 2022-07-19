using Unity.Netcode;
using UnityEngine;

namespace ClientManagerSpace
{
    public class ClientManager : MonoBehaviour
    {
        public GameObject matchMaker;
        private void Start()
        {
            NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-launch-as-server")
                {
                    NetworkManager.Singleton.StartServer();
                    GameObject mm = Instantiate(matchMaker);
                    mm.GetComponent<NetworkObject>().Spawn();
                }
                if (args[i] == "-launch-as-client")
                {
                    NetworkManager.Singleton.StartClient();
                }
            }
        }
            void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (GUILayout.Button("Host"))
            {
                NetworkManager.Singleton.StartHost();
                GameObject mm = Instantiate(matchMaker);
                mm.GetComponent<NetworkObject>().Spawn();
            }
            if (GUILayout.Button("Client")) NetworkManager.Singleton.StartClient();
            if (GUILayout.Button("Server"))
            {
                NetworkManager.Singleton.StartServer();
                GameObject mm = Instantiate(matchMaker);
                mm.GetComponent<NetworkObject>().Spawn();
            }
        }

        void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }

        private NetworkManager.ConnectionApprovalResponse ApprovalCheck(NetworkManager.ConnectionApprovalRequest request)
        {
            // The client identifier to be authenticated 0,1,2...
            var clientId = request.ClientNetworkId;

            // Additional connection data defined by user code
            var connectionData = request.Payload;

            var response = new NetworkManager.ConnectionApprovalResponse();
            // Your approval logic determines the following values
            response.Approved = true;

            response.CreatePlayerObject = false;
            // The prefab hash value of the NetworkPrefab, if null the default NetworkManager player prefab is used
            response.PlayerPrefabHash = null;

            // Position to spawn the player object (if null it uses default of Vector3.zero)
            response.Position = Vector3.zero;

            // Rotation to spawn the player object (if null it uses the default of Quaternion.identity)
            response.Rotation = Quaternion.identity;

            // If additional approval steps are needed, set this to true until the additional steps are complete
            // once it transitions from true to false the connection approval response will be processed.
            //response.Pending = false;

            return response;
        }
    }
}
