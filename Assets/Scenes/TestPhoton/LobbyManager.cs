﻿using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region Private variables
    [SerializeField, Header("Splash")] Animation slpashAnimation = null;
    [SerializeField, Header("Mode selection")] CanvasGroup modeSelectionUi = null;
    [SerializeField] Button localModeButton = null, onlineModeButton = null;
    [SerializeField, Header("Mode selection")] CanvasGroup lobbyUi = null;
    [SerializeField] Button joinRandomRoomButton = null, createOrJoinCustomRoomButton = null, closeButton = null;
    [SerializeField] TMP_InputField createOrJoinCustomRoomInputField = null;
    [SerializeField] GameObject waitingLabel = null;
    string roomName;
    string gameScene = "TestGame";
    #endregion

    #region Unity methods
    void Awake() 
    {
        Init();
        AddListeners();
    }

    void Start() => PhotonNetwork.ConnectUsingSettings();

    void OnDestroy() 
    {
        RemoveListeners();
    }
    #endregion

    #region Private methods
    void Init()
    {
        waitingLabel.SetActive(false);
        lobbyUi.alpha = modeSelectionUi.alpha = 0;
        modeSelectionUi.interactable = modeSelectionUi.blocksRaycasts = lobbyUi.interactable = lobbyUi.blocksRaycasts = false;
    }

    void AddListeners()
    {
        RemoveListeners();
        onlineModeButton.onClick.AddListener(ShowLobbyUi);
        localModeButton.onClick.AddListener(LocalModeSelected);

        joinRandomRoomButton.onClick.AddListener(JoinRandomRoom);
        createOrJoinCustomRoomButton.onClick.AddListener(CreateCustomRoom);

        closeButton.onClick.AddListener(CancelJoin);
    }

    void RemoveListeners()
    {
        onlineModeButton.onClick.RemoveListener(ShowLobbyUi);
        localModeButton.onClick.RemoveListener(LocalModeSelected);

        joinRandomRoomButton.onClick.RemoveListener(JoinRandomRoom);
        createOrJoinCustomRoomButton.onClick.RemoveListener(CreateCustomRoom);

        closeButton.onClick.RemoveListener(CancelJoin);
    }

    void ShowLobbyUi()
    {
        modeSelectionUi.interactable = modeSelectionUi.blocksRaycasts = false;
        modeSelectionUi.DOFade(0, 0.5f);
        lobbyUi.DOFade(1, 0.5f).OnComplete(() => lobbyUi.interactable = lobbyUi.blocksRaycasts = true );
    }

    void LocalModeSelected()
    {
        Debug.Log("Local mode selected");
    }

    void JoinRandomRoom()
    {
        lobbyUi.interactable = lobbyUi.blocksRaycasts = false;  
        roomName = Guid.NewGuid().ToString();  
        PhotonNetwork.JoinRandomRoom();
    }

    void CreateCustomRoom()
    {
        roomName = createOrJoinCustomRoomInputField.text;   
        CreateRoom(false); 
    }

    void CancelJoin() 
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            StopCoroutine(WaitForOtherPlayer());
            PhotonNetwork.LeaveRoom();
            waitingLabel.SetActive(false);
        }
    }

    void CreateRoom(bool visible = true)
    {
        if(!string.IsNullOrEmpty(roomName))
        {            
            lobbyUi.interactable = lobbyUi.blocksRaycasts = false;  
            RoomOptions roomOptions = new RoomOptions(){ IsVisible = visible, IsOpen = true, MaxPlayers = (byte)2 };
            PhotonNetwork.CreateRoom(roomName, roomOptions);
        }
    }

    IEnumerator WaitForServerConnection()
    {        
        yield return new WaitUntil(() => !slpashAnimation.isPlaying);
        yield return new WaitUntil(() => Keyboard.current.anyKey.wasPressedThisFrame || Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame || Mouse.current.middleButton.wasPressedThisFrame); 
        modeSelectionUi.DOFade(1, 0.5f).OnComplete(() => modeSelectionUi.interactable = modeSelectionUi.blocksRaycasts = true );
    }

    IEnumerator WaitForOtherPlayer()
    {        
        waitingLabel.SetActive(true);
        lobbyUi.interactable = lobbyUi.blocksRaycasts = true;  
        PhotonNetwork.AutomaticallySyncScene = true;
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount == 2);
        waitingLabel.SetActive(false);
        PhotonNetwork.LoadLevel(gameScene);
    }
    #endregion
        
    #region Photon override methods
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();          
        StartCoroutine(WaitForServerConnection());
    }

    public override void OnJoinRandomFailed(short returnCode, string message) => CreateRoom();

    public override void OnCreateRoomFailed(short returnCode, string message) => PhotonNetwork.JoinRoom(roomName);

    public override void OnJoinedRoom() => StartCoroutine(WaitForOtherPlayer());
    #endregion
}
