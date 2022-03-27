using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;//path�������



public class PlayerManager : MonoBehaviourPunCallbacks
{
    PhotonView PV;//����� ����

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PV.IsMine)//�� ���� ��Ʈ��ũ�̸�
        {
            CreateController();//�÷��̾� ��Ʈ�ѷ� �ٿ��ش�. 
        }
    }
    void CreateController()//�÷��̾� ��Ʈ�ѷ� �����
    {
        Debug.Log("Instantiated Player Controller");
        PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), new Vector3(25, 0, -5), Quaternion.identity);
        //���� �����鿡 �ִ� �÷��̾� ��Ʈ�ѷ��� �� ��ġ�� �� ������ ������ֱ�
    }
}