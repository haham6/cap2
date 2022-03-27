using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityStandardAssets.Utility;
using UnityEngine.SceneManagement;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime, BackDistance;

    //카메라 테스트
    //[SerializeField] GameObject cameraHolder;

    //마우스감도 뛰는속도 걷는속도 점프힘 뛰기걷기바꿀때 가속시간
    float verticalLookRotation;
    bool grounded;//점프를 위한 바닥체크
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;//실제 이동거리
    public Transform see;

    public Text NicknameText;

    public GameObject component;

    RaycastHit hitinfo;
    bool IsContact;
    GameObject MyCanvas;
    Text MyText;

    int CameraMode = 1;

    Rigidbody rb;
    Transform tr;
    PhotonView PV;

    Vector3 EnterRoomDoor1; //문 좌표
    Vector3 ExitRoomDoor1;
    Vector3 EnterRoomDoor2;
    Vector3 ExitRoomDoor2;
    Vector3 EnterRoomDoor3;
    Vector3 ExitRoomDoor3;
    void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        PV = GetComponent<PhotonView>();

        EnterRoomDoor1 = (GameObject.Find("EnterRoomDoor1")).transform.position; //씬 내에서 해당 이름의 문 오브젝트를 찾아서 좌표를 저장함
        EnterRoomDoor1.z -= 2; //이동할 땐 문보다 좀 옆에 가야함
        ExitRoomDoor1 = (GameObject.Find("ExitRoomDoor1")).transform.position;
        ExitRoomDoor1.z += 2;
        EnterRoomDoor2 = (GameObject.Find("EnterRoomDoor2")).transform.position;
        EnterRoomDoor2.z -= 2;
        ExitRoomDoor2 = (GameObject.Find("ExitRoomDoor2")).transform.position;
        ExitRoomDoor2.z += 2;
        EnterRoomDoor3 = (GameObject.Find("EnterRoomDoor3")).transform.position;
        EnterRoomDoor3.z -= 2;
        ExitRoomDoor3 = (GameObject.Find("ExitRoomDoor3")).transform.position;
        ExitRoomDoor3.z += 2;
    }

    void Start()
    {
        if (!PV.IsMine)
        {
            //Destroy(GetComponentInChildren<Camera>().gameObject);
            //내꺼 아니면 카메라 없애기
            Destroy(rb);
            //내거아니면 리지드 바디 없애주기

        }
        if (PV.IsMine)
        {
            component = GameObject.Find("ChatManager");
            Camera.main.GetComponent<SmoothFollow>().target = tr.Find("CamPivot").transform;
        }
        NicknameText.text = PV.Owner.NickName;
        //MyCanvas = GameObject.Find("Canvas");
        MyText = GameObject.Find("ReadTag").GetComponent<Text>();
    }

    void Update()
    {
        if (!PV.IsMine)
            return;//내꺼아니면 작동안함
        if (PV.IsMine && component.GetComponent<ChatManager>().move_on_chat == true) //채팅시 마우스 활성화 및 이동 제한
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            return;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        Look();
        //Camera.main.GetComponent<SmoothFollow>().target = verticalLookRotation;
        Move();
        Jump();
        CheckObject();
        if (Input.GetMouseButtonDown(2) && CameraMode == 1)
        {
            CameraMode = 2;
        }
        else if (Input.GetMouseButtonDown(2) && CameraMode == 2)
        {
            CameraMode = 1;
        }
    }

    void Look()
    {
        // 좌우로 움직인 마우스의 이동량 * 속도에 따라 카메라가 좌우로 회전할 양 계산
        float yRotateSize = Input.GetAxis("Mouse X");
        // 현재 y축 회전값에 더한 새로운 회전각도 계산
        float yRotate = transform.eulerAngles.y + yRotateSize;

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);
        //마우스 움직이는 정도*민감도만큼 각도 움직이기
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivity;
        //마우스 움직이는 정도*민감도만큼 각도 값 받기
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        //y축 -90도에서 90도만 값으로 받음
        /*
        //카메라 테스트
        if (CameraMode == 1)
        {
            Camera.main.transform.localEulerAngles = new Vector3(-verticalLookRotation, yRotate, 0);
            Camera.main.transform.position = tr.position + Vector3.up * 1;
        }
        else if (CameraMode == 2)
        {
            Camera.main.transform.localEulerAngles = new Vector3(-verticalLookRotation, yRotate, 0);
            Camera.main.transform.position = tr.position + Vector3.up * 1 + Vector3.back * BackDistance;
        }*/
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        //벡더방향을 가지지만 크기는 1로 노말라이즈
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
        //왼쪽 쉬프트가 누르면 뛰는속도, 나머지는 걷는속도로하기
        //smoothTime만큼에 걸쳐서 이동해주기. 
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)//땅위에서 스페이스바 누르면
        {
            rb.AddForce(transform.up * jumpForce);//점프력만큼위로 힘받음
        }
    }

    void CheckObject()
    {
        Debug.DrawRay(see.position, see.forward * 2, Color.blue, 0.3f);
        Vector3 t_MousePos = new Vector3(see.position.x, see.position.y, 0);

        if (Physics.Raycast(see.position, see.forward, out hitinfo, 2))
        {
            Contact();
        }
        else
            NotContact();
    }

    void Contact()
    {
        if (hitinfo.transform.CompareTag("Door"))
        {
            if (!IsContact)
            {
                IsContact = true;
                MyText.text = hitinfo.transform.gameObject.name;
                if (Input.GetKeyDown(KeyCode.E))
                {

                }
            }
        }
        else
            NotContact();
    }

    void NotContact()
    {
        if (IsContact)
        {
            IsContact = false;
            MyText.text = "";
        }
    }

    public void SetGroundedState(bool _grounded) //지면에 있는 지 확인
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;//내꺼아니면 작동안함
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        //이동하는거는 계산 끝난 moveAmount만큼만 고정된시간(0.2초)마다에 맞춰서
    }

    private void OnTriggerEnter(Collider other) //문 오브젝트에 닿으면 반대편 문 근처로 텔레포트
    {
        if(other.gameObject.name=="EnterRoomDoor1")
        {
            transform.position =ExitRoomDoor1;
        }
        if (other.gameObject.name == "EnterRoomDoor2")
        {
            transform.position = ExitRoomDoor2;
        }
        if (other.gameObject.name == "EnterRoomDoor3")
        {
            transform.position = ExitRoomDoor3;
        }
        if (other.gameObject.name == "ExitRoomDoor1")
        {
            transform.position = EnterRoomDoor1;
        }
        if (other.gameObject.name == "ExitRoomDoor2")
        {
            transform.position = EnterRoomDoor2;
        }
        if (other.gameObject.name == "ExitRoomDoor3")
        {
            transform.position = EnterRoomDoor3;
        }
    }


}


