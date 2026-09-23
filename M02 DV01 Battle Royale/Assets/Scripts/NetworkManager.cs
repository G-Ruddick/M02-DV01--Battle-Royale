using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks {
    public int maxPlayer = 10;

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
}
