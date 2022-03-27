using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class ChatManager : MonoBehaviourPunCallbacks
{
    public Text chatLog; //ä�÷α�
    public Text chattingList; //������ ���
    public InputField input; //ä�� ġ�� ĭ
    public Text Broadcast; //���� �ؽ�Ʈ
    ScrollRect scroll_rect = null; //ä��â ��ũ��
    string chatters; //������ ��� ���ſ� ���ڿ�
    string sentence; //���� ��¿� ���ڿ�
    public string msg;


    public bool move_on_chat ; //ä�ý� ���콺 Ŀ�� ��ۿ�

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.EnableCloseConnection=true; //������ ����� ����
        move_on_chat = false; //Ŀ�� ��Ȱ��ȭ
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
    }
    void Update()
    {
        chatterUpdate();
        if (Input.GetKeyDown(KeyCode.Return) && !input.isFocused) //ä�� �Է����� �ƴ� �� ����
        {
            move_on_chat = true; //Ŀ�� Ȱ��ȭ
            input.ActivateInputField(); //ä�� �Է� Ȱ��ȭ
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //escŰ
        {
            move_on_chat = false; //Ŀ�� ��Ȱ��ȭ
            input.Select();//ä�� �Է� ��Ȱ��ȭ
        }

    }
    public void toggle_move_on_chat() //�ܺ� ����� Ŀ�� ��Ȱ��ȭ �Լ�
    {
        move_on_chat = false;
    }

    public void SendButtonOnClicked() //ä�� ��/��� �� ��ɾ� ���
    {
        move_on_chat = true;
        if (input.text.Equals(""))
        {
            return; //��������� ���x
        }
        if (input.text.StartsWith("/")) //   ��ɾ�� /�� ������
        {
            if (input.text[1].Equals('l') && input.text[2].Equals(' ')) //   �α���(���� ȹ��) ��ɾ� /l asdf
            {
                string pswd = input.text.Split(' ')[1];
                if (pswd.Equals("asdf")) //�ӽ� ��й�ȣ asdf�� ���߸�
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer); //�ش� �÷��̾ ������Ŭ���̾�Ʈ�� ����
                    ReceiveMsg("������Ŭ���̾�Ʈ�� ������");
                }
                else
                    ReceiveMsg("��й�ȣ�� Ʋ���ϴ�");
            }
            if (input.text[1].Equals('k') && input.text[2].Equals(' '))//     ����� ��ɾ� /k �г���
                                                                       //     ����� ������ �־�߸� ��밡��
            {
                string k_nick = input.text.Split(' ')[1];
                foreach (Player p in PhotonNetwork.PlayerList) //�÷��̾� ��� �˻� ��
                {
                    if (p.NickName.Equals(k_nick)) //��ɾ��� �г��Ӱ� ������ ������
                    {
                        PhotonNetwork.CloseConnection(p); //��������
                        if (PhotonNetwork.LocalPlayer.IsMasterClient) //�⺻������ ������ ������ ���� �ȵ����� ä�� ����� ���� Ȯ��
                        {
                            string kicked_msg = string.Format("[{0} {1}]", k_nick, "�� �������");
                            photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, kicked_msg); //��ο��� ����Ǿ����� �˸�
                            ReceiveMsg(kicked_msg); //�����Ե� ���
                        }
                        else
                            ReceiveMsg("������ �����ϴ�.");
                    }
                }
            }

            else if (input.text[1].Equals('b') && input.text[2].Equals(' ')) // ������ ��ɾ� /b ��
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient) //���� Ȯ��
                {
                    sentence = input.text.Substring(3); //��ɾ�� �� ���� �и�
                    string notice = string.Format("*[����]{0}*", sentence); //������ ���ڿ� ����
                    photonView.RPC("ReceiveBC", RpcTarget.All, notice); //���� ���
                    photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, notice); //ä�ÿ��� ǥ��
                    ReceiveMsg(notice);
                    ReceiveBC(notice);
                }
                else
                    ReceiveMsg("������ �����ϴ�.");
            }
        }
        else
        {
            msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, input.text); //�Ϲ� �޼���
            photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg); //���
            ReceiveMsg(msg);
        }
        input.Select(); //�Է�â ��Ȱ��ȭ
        input.text = "";
        move_on_chat = false; //���콺 ��Ȱ��ȭ
    }
    void chatterUpdate() //���Ӹ�� ����
    {
        chatters = "Player List\n";
        foreach (Player p in PhotonNetwork.PlayerList)//�÷��̾� Ȯ���� ���
        {
            chatters += p.NickName + "\n";
        }
        chattingList.text = chatters;
    }

    [PunRPC]
    public void ReceiveBC(string notice) //���� ��¿�
    {
        Broadcast.text = notice;
        Invoke("clearBC", 3f); //3�ʰ� ǥ��
    }

    void clearBC()
    {
        Broadcast.text = ""; //3���� ���� �ʱ�ȭ
    }


    [PunRPC]
    public void ReceiveMsg(string msg) //�Ϲ� ä�� ��¿�
    {
        chatLog.text += "\n" + msg;
        scroll_rect.verticalNormalizedPosition = 0.0f; //��ũ���� �� �Ʒ���
    }

}
