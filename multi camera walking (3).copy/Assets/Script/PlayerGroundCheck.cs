

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;//Player Controller ��ũ��Ʈ�� �޼���� ����ϱ� ���� ����
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;//�ش� ��ü�� player�� ����
        playerController.SetGroundedState(true);
        //������ true
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;//�ش� ��ü�� player�� ����
        playerController.SetGroundedState(false);
        //�������� true
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;//�ش� ��ü�� player�� ����
        playerController.SetGroundedState(true);
        //��� ������ true
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;//�ش� ��ü�� player�� ����
        playerController.SetGroundedState(true);
        //������ true
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;//�ش� ��ü�� player�� ����
        playerController.SetGroundedState(false);
        //�������� true
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;//�ش� ��ü�� player�� ����
        playerController.SetGroundedState(true);
        //��� ������ true
    }
}
