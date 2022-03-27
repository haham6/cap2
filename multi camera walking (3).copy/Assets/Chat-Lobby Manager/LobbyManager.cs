using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public Button loginBtn; //�α��� ��ư
    public Text IDtext; //���̵� ���� ĭ
    public Text ConnectionStatus; //���� ���� �ؽ�Ʈ
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        loginBtn.interactable = false; //��ư ��Ȱ��ȭ
        ConnectionStatus.text = "Connecting to Master Server...";
        Cursor.visible = true; //Ŀ�� Ȱ��ȭ
        Cursor.lockState = CursorLockMode.None; //ȭ���������
    }

    
    public void Connect()
    {
        if (IDtext.text.Equals("")) //���̵� ������ �� ����
        {
            return;
        }
        else
        {
            PhotonNetwork.LocalPlayer.NickName = IDtext.text; //�г����� ���̵�� ����
            loginBtn.interactable = false; //���� ������ ��ư ��Ȱ��ȭ
            if (PhotonNetwork.IsConnected)
            {
                ConnectionStatus.text = "connecting to room...";
                PhotonNetwork.JoinRandomRoom(); //���Ӽ���
            }
            else
            {
                ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";
                PhotonNetwork.ConnectUsingSettings(); //�������� ������ ����, ������ �õ�
            }

        }

    }
    public override void OnConnectedToMaster()//�����ͼ����� ����, �¶��� ����
    {
        loginBtn.interactable = true;//���̵� �Է� ����
        ConnectionStatus.text = "Online : connected to master server";
    }
    public override void OnDisconnected(DisconnectCause cause)// ���� ����
    {
        loginBtn.interactable = false;//���̵� �Է� �Ұ���
        ConnectionStatus.text = "Offline : failed to connect.\nreconnecting...";
        PhotonNetwork.ConnectUsingSettings(); //�����ӽõ�
    }
   public override void OnJoinRandomFailed(short returnCode, string message) //�� ���� ������ ���� ���� (�� ������Ʈ���� ���� �ϳ��� ���Ƿ� �ʿ����)
  {
     ConnectionStatus.text = "No empty room. creating new room...";
       PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 10 });
  }
    public override void OnJoinedRoom() //���� ���� �� ���� ������
    {
        ConnectionStatus.text = "Success to join room";
        PhotonNetwork.LoadLevel("Main");
    }
}

