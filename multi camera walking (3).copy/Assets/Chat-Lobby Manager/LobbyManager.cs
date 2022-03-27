using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button loginBtn; //로그인 버튼
    public Text IDtext; //아이디 적는 칸
    public Text ConnectionStatus; //접속 상태 텍스트
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        loginBtn.interactable = false; //버튼 비활성화
        ConnectionStatus.text = "Connecting to Master Server...";
        Cursor.visible = true; //커서 활성화
        Cursor.lockState = CursorLockMode.None; //화면고정해제
    }

    
    public void Connect()
    {
        if (IDtext.text.Equals("")) //아이디가 공백인 걸 막음
        {
            return;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = IDtext.text; //닉네임을 아이디로 설정
            loginBtn.interactable = false; //접속 직전에 버튼 비활성화
            if (PhotonNetwork.IsConnected)
            {
                ConnectionStatus.text = "connecting to room...";
                PhotonNetwork.JoinRandomRoom(); //접속성공
            }
            else
            {
                ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";
                PhotonNetwork.ConnectUsingSettings(); //오프라인 등으로 실패, 재접속 시도
            }

        }

    }
    public override void OnConnectedToMaster()//마스터서버에 접속, 온라인 판정
    {
        loginBtn.interactable = true;//아이디 입력 가능
        ConnectionStatus.text = "Online : connected to master server";
    }
    public override void OnDisconnected(DisconnectCause cause)// 접속 실패
    {
        loginBtn.interactable = false;//아이디 입력 불가능
        ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";
        PhotonNetwork.ConnectUsingSettings(); //재접속시도
    }
   public override void OnJoinRandomFailed(short returnCode, string message) //빈 방이 없으면 새로 만듦 (현 프로젝트에선 방을 하나만 쓰므로 필요없음)
  {
     ConnectionStatus.text = "No empty room. creating new room...";
       PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 10 });
  }
    public override void OnJoinedRoom() //접속 성공 후 메인 씬으로
    {
        ConnectionStatus.text = "Success to join room";
        PhotonNetwork.LoadLevel("Main");
    }
}

