using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : MonoBehaviour
{
    public GameObject Player;
    NetworkClient myClient;
    public string IP = "127.0.0.1";
    public int port = 4444;
    

    // Create a client and connect to the server port
    public void ClientConnect()
    {
        ClientScene.RegisterPrefab(Player);
        myClient = new NetworkClient();
        myClient.RegisterHandler(MsgType.Connect, OnClientConnect);
        myClient.Connect(IP, port);
    }

    void OnClientConnect(NetworkMessage msg)
    {
        Debug.Log("Connected to server: " + msg.conn);
    }

    public void ServerListen()
    {
        NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
        NetworkServer.RegisterHandler(MsgType.Ready, OnClientReady);
        if (NetworkServer.Listen(4444))
            Debug.Log("Server started listening on port 4444");
    }

    // When client is ready spawn a few trees
    void OnClientReady(NetworkMessage msg)
    {
        Debug.Log("Client is ready to start: " + msg.conn);
        NetworkServer.SetClientReady(msg.conn);
        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        var playerInstance = Instantiate(Player, new Vector3(0, 0, 0), Quaternion.identity);
        NetworkServer.Spawn(playerInstance);
    }

    void OnServerConnect(NetworkMessage msg)
    {
        Debug.Log("New client connected: " + msg.conn);
    }

}
