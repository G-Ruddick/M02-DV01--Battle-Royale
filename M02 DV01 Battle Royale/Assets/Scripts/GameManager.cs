using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPun {
    [Header("Players")]
    public string playerPrefabLocation;
    public PlayerController[] players;
    public Transform[] spawnPoints;
    public int alivePlayers;

    public float postGameTime;
    private int playersInGame;

    public static GameManager instance;

    private void Awake() {
        instance = this;
    }

    private void Start() {
        players = new PlayerController[PhotonNetwork.PlayerList.Length];
        alivePlayers = players.Length;

        photonView.RPC("ImInGame", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void ImInGame() {
        playersInGame++;

        if (PhotonNetwork.IsMasterClient && playersInGame == PhotonNetwork.PlayerList.Length) {
            photonView.RPC("SpawnPlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    private void SpawnPlayer() {
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLocation, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
        playerObj.GetComponent<PlayerController>().photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerController GetPlayer (int playerId) {
        return players.First(x => x.id == playerId);
    }

    public PlayerController GetPlayer (GameObject playerObj) {
        return players.First(x => x.gameObject == playerObj);
    }

    public void CheckWinCondition () {
        if(alivePlayers == 1)
            photonView.RPC("WinGame", RpcTarget.All, players.First(x => !x.dead).id);
    }

    [PunRPC]
    void WinGame (int winningPlayer) {
        Invoke("GoBackToMenu", postGameTime);
    }

    void GoBackToMenu (){
        NetworkManager.instance.ChangeScene("Menu");
    }
}
