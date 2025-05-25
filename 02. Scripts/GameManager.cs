using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text roomName;
    public TMP_Text connectInfo;
    public Button exitBtn;
    public TMP_Text msgList;
    public TMP_Text scoreText;
    public int totScore = 3;

    void Awake()
    {
        CreatePlayer();
        // ���� ���� ���� �� ǥ��
        SetRoomInfo();
        // Exit ��ư �̺�Ʈ ����
        exitBtn.onClick.AddListener(() => OnExitClick());
    }
    private void Start()
    {
        DisplayScore();
    }
    public void DisplayScore()
    {
        //totScore--;
        scoreText.text = $"{totScore}";
    }

    void CreatePlayer()
    {
        // ���� ��ġ ������ �迭�� ����
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        // ��Ʈ��ũ�� ĳ���� ����
        PhotonNetwork.Instantiate("Player",

        points[idx].position,
        points[idx].rotation,
        0);

    }

    // �� ���� ������ ���
    void SetRoomInfo()
    {
        Room room = PhotonNetwork.CurrentRoom;
        roomName.text = room.Name;
        connectInfo.text = $"({room.PlayerCount}/{room.MaxPlayers})";
    }
    // Exit ��ư�� OnClick�� ������ �Լ�
    private void OnExitClick()
    {
        PhotonNetwork.LeaveRoom();
    }
    // ���� �뿡�� �������� �� ȣ��Ǵ� �ݹ��Լ�
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }
    // ������ ���ο� ��Ʈ��ũ ������ �����߶� ȣ��Ǵ� �ݹ��Լ�
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#00ff00>{newPlayer.NickName}</color> is joined room" + connectInfo.text;
        msgList.text += msg;
    }
    // �뿡�� ��Ʈ��ũ ������ �����߶� ȣ��Ǵ� �ݹ��Լ�
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomInfo();
        string msg = $"\n<color=#ff0000>{otherPlayer.NickName}</color> is left room" + connectInfo.text;
        msgList.text += msg;
    }
}