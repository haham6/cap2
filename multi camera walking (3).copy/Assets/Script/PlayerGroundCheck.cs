

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;//Player Controller 스크립트를 메서드로 사용하기 위해 선언
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;//해당 물체가 player면 무시
        playerController.SetGroundedState(true);
        //닿으면 true
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;//해당 물체가 player면 무시
        playerController.SetGroundedState(false);
        //떨어지면 true
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playerController.gameObject)
            return;//해당 물체가 player면 무시
        playerController.SetGroundedState(true);
        //닿고 있으면 true
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;//해당 물체가 player면 무시
        playerController.SetGroundedState(true);
        //닿으면 true
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;//해당 물체가 player면 무시
        playerController.SetGroundedState(false);
        //떨어지면 true
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playerController.gameObject)
            return;//해당 물체가 player면 무시
        playerController.SetGroundedState(true);
        //닿고 있으면 true
    }
}
