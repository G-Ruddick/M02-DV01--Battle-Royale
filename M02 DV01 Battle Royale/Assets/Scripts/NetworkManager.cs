using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks {
    public int maxPlayers = 10;

    public static NetworkManager instance;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {
        Debug.Log("Connected to master server.");
    }

    public void CreateRoom(string roomName) {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)maxPlayers;

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void JoinRoom(string roomName) {
        PhotonNetwork.JoinRoom(roomName);
    }

    public void ChangeScene(string sceneName) {
        PhotonNetwork.LoadLevel(sceneName);
    }
}
