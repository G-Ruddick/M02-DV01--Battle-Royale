using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Menu : MonoBehaviourPunCallbacks, ILobbyCallbacks {
    [Header("Screens")]
    public GameObject mainScreen;
    public GameObject createRoomScreen;
    public GameObject lobbyScreen;
    public GameObject lobbyBrowserScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button findRoomButton;

    [Header("Lobby")]
    public TextMeshProUGUI playerListText;
    public TextMeshProUGUI roomInfoText;
    public Button startGameButton;

    [Header("Lobby Browser")]
    public RectTransform roomListContainer;
    public GameObject roomButtonPrefab;

    private List<GameObject> roomButtons = new List<GameObject>();
    private List<RoomInfo> roomList = new List<RoomInfo>();

    private void Start() {
        createRoomButton.interactable = false;
        findRoomButton.interactable = false;

        Cursor.lockState = CursorLockMode.None;

        if (PhotonNetwork.InRoom) {
            PhotonNetwork.CurrentRoom.IsVisible = true;
            PhotonNetwork.CurrentRoom.IsOpen = true;
        }
    }

    public override void OnConnectedToMaster() {
        createRoomButton.interactable = true;
        findRoomButton.interactable = true;
    }

    public override void OnJoinedRoom() {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer) {
        UpdateLobbyUI();
    }

    public override void OnRoomListUpdate(List<RoomInfo> allRooms) {
        roomList = allRooms;
    }

    private void SetScreen(GameObject screen) {
        mainScreen.SetActive(false);
        createRoomScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        lobbyBrowserScreen.SetActive(false);
        screen.SetActive(true);

        if (screen == lobbyBrowserScreen) {
            UpdateLobbyBrowserUI();
        }
    }

    // main screen
    public void OnPlayerNameValueChanged(TMP_InputField playerNameInput) {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    public void OnCreatedRoomButton() {
        SetScreen(createRoomScreen);
    }
   
    public void OnFindRoomButton() {
        SetScreen(lobbyBrowserScreen);
    }

    // create room screen
    public void OnBackButton() {
        SetScreen(mainScreen);
    }

    public void OnCreateButton(TMP_InputField roomNameInput) {
        NetworkManager.instance.CreateRoom(roomNameInput.text);
    }

    // lobby screen
    [PunRPC]
    private void UpdateLobbyUI() {
        startGameButton.interactable = PhotonNetwork.IsMasterClient;

        playerListText.text = "";

        foreach (Player player in PhotonNetwork.PlayerList) {
            playerListText.text += player.NickName + "\n";
        }

        roomInfoText.text = "<b>Room Name</b>\n" + PhotonNetwork.CurrentRoom.Name;
    }

    public void OnStartGameButton() {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "Game");
    }

    public void OnLeaveLobbyButton() {
        PhotonNetwork.LeaveRoom();
        SetScreen(mainScreen);
    }

    // lobby browser screen
    private void UpdateLobbyBrowserUI() {
        foreach (GameObject button in roomButtons) {
            button.SetActive(false);
        }

        for (int i = 0; i < roomList.Count; ++i) {
            GameObject button = (i >= roomButtons.Count) ? CreateRoomButton() : roomButtons[i];
            button.SetActive(true);
            button.transform.Find("RoomNameText").GetComponent<TextMeshProUGUI>().text = roomList[i].PlayerCount + " / " + roomList[i].MaxPlayers;

            Button buttonComp = button.GetComponent<Button>();
            string roomName = roomList[i].Name;
            buttonComp.onClick.RemoveAllListeners();
            buttonComp.onClick.AddListener(() => { OnJoinRoomButton(roomName); });
        }
    }

    private GameObject CreateRoomButton() {
        GameObject buttonObj = Instantiate(roomButtonPrefab, roomListContainer.transform);
        roomButtons.Add(buttonObj);
        return buttonObj;
    }

    public void OnJoinRoomButton(string roomName) {
        NetworkManager.instance.JoinRoom(roomName);
    }

    public void OnRefreshButton() {
        UpdateLobbyBrowserUI();
    }
}
