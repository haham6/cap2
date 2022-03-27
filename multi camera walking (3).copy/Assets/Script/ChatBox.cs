using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ChatBox : MonoBehaviourPunCallbacks
{
    //public InputField input; //ä�� ġ�� ĭ
    public Text ChatSentences; //���� �ؽ�Ʈ
    //string sentence; //���� ��¿� ���ڿ�
    public GameObject component;
    public GameObject chatBox;

    PhotonView PV;
    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    private void Start()
    {
        if (PV.IsMine)
        {
            component = GameObject.Find("ChatManager");
        }
        Debug.Log(component);
    }

    private void Update()
    {
        if(component.GetComponent<ChatManager>().msg != null)
        {
            chatBox.SetActive(true);
            ChatSentences.text += component.GetComponent<ChatManager>().msg;
        }
    }

}
