using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
public class ChatManager : MonoBehaviourPunCallbacks
{
    public Text chatLog; //채팅로그
    public Text chattingList; //접속자 목록
    public InputField input; //채팅 치는 칸
    public Text Broadcast; //공지 텍스트
    ScrollRect scroll_rect = null; //채팅창 스크롤
    string chatters; //접속자 목록 갱신용 문자열
    string sentence; //공지 출력용 문자열
    public string msg;


    public bool move_on_chat ; //채팅시 마우스 커서 토글용

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.EnableCloseConnection=true; //강퇴기능 사용을 위함
        move_on_chat = false; //커서 비활성화
        PhotonNetwork.IsMessageQueueRunning = true;
        scroll_rect = GameObject.FindObjectOfType<ScrollRect>();
    }
    void Update()
    {
        chatterUpdate();
        if (Input.GetKeyDown(KeyCode.Return) && !input.isFocused) //채팅 입력중이 아닐 때 엔터
        {
            move_on_chat = true; //커서 활성화
            input.ActivateInputField(); //채팅 입력 활성화
        }

        if (Input.GetKeyDown(KeyCode.Escape)) //esc키
        {
            move_on_chat = false; //커서 비활성화
            input.Select();//채팅 입력 비활성화
        }

    }
    public void toggle_move_on_chat() //외부 연결용 커서 비활성화 함수
    {
        move_on_chat = false;
    }

    public void SendButtonOnClicked() //채팅 입/출력 및 명령어 사용
    {
        move_on_chat = true;
        if (input.text.Equals(""))
        {
            return; //비어있으면 출력x
        }
        if (input.text.StartsWith("/")) //   명령어는 /로 시작함
        {
            if (input.text[1].Equals('l') && input.text[2].Equals(' ')) //   로그인(권한 획득) 명령어 /l asdf
            {
                string pswd = input.text.Split(' ')[1];
                if (pswd.Equals("asdf")) //임시 비밀번호 asdf를 맞추면
                {
                    PhotonNetwork.SetMasterClient(PhotonNetwork.LocalPlayer); //해당 플레이어를 마스터클라이언트로 만듦
                    ReceiveMsg("마스터클라이언트로 설정됨");
                }
                else
                    ReceiveMsg("비밀번호가 틀립니다");
            }
            if (input.text[1].Equals('k') && input.text[2].Equals(' '))//     강퇴용 명령어 /k 닉네임
                                                                       //     강퇴는 권한이 있어야만 사용가능
            {
                string k_nick = input.text.Split(' ')[1];
                foreach (Player p in PhotonNetwork.PlayerList) //플레이어 목록 검색 후
                {
                    if (p.NickName.Equals(k_nick)) //명령어의 닉네임과 같은게 있으면
                    {
                        PhotonNetwork.CloseConnection(p); //연결해제
                        if (PhotonNetwork.LocalPlayer.IsMasterClient) //기본적으로 권한이 없으면 강퇴가 안되지만 채팅 출력을 위해 확인
                        {
                            string kicked_msg = string.Format("[{0} {1}]", k_nick, "가 강퇴당함");
                            photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, kicked_msg); //모두에게 강퇴되었음을 알림
                            ReceiveMsg(kicked_msg); //나에게도 출력
                        }
                        else
                            ReceiveMsg("권한이 없습니다.");
                    }
                }
            }

            else if (input.text[1].Equals('b') && input.text[2].Equals(' ')) // 공지용 명령어 /b 말
            {
                if (PhotonNetwork.LocalPlayer.IsMasterClient) //권한 확인
                {
                    sentence = input.text.Substring(3); //명령어에서 할 말을 분리
                    string notice = string.Format("*[공지]{0}*", sentence); //공지용 문자열 포맷
                    photonView.RPC("ReceiveBC", RpcTarget.All, notice); //공지 출력
                    photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, notice); //채팅에도 표시
                    ReceiveMsg(notice);
                    ReceiveBC(notice);
                }
                else
                    ReceiveMsg("권한이 없습니다.");
            }
        }
        else
        {
            msg = string.Format("[{0}] {1}", PhotonNetwork.LocalPlayer.NickName, input.text); //일반 메세지
            photonView.RPC("ReceiveMsg", RpcTarget.OthersBuffered, msg); //출력
            ReceiveMsg(msg);
        }
        input.Select(); //입력창 비활성화
        input.text = "";
        move_on_chat = false; //마우스 비활성화
    }
    void chatterUpdate() //접속목록 갱신
    {
        chatters = "Player List\n";
        foreach (Player p in PhotonNetwork.PlayerList)//플레이어 확인후 출력
        {
            chatters += p.NickName + "\n";
        }
        chattingList.text = chatters;
    }

    [PunRPC]
    public void ReceiveBC(string notice) //공지 출력용
    {
        Broadcast.text = notice;
        Invoke("clearBC", 3f); //3초간 표시
    }

    void clearBC()
    {
        Broadcast.text = ""; //3초후 내용 초기화
    }


    [PunRPC]
    public void ReceiveMsg(string msg) //일반 채팅 출력용
    {
        chatLog.text += "\n" + msg;
        scroll_rect.verticalNormalizedPosition = 0.0f; //스크롤을 맨 아래로
    }

}
