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

    //ī�޶� �׽�Ʈ
    //[SerializeField] GameObject cameraHolder;

    //���콺���� �ٴ¼ӵ� �ȴ¼ӵ� ������ �ٱ�ȱ�ٲܶ� ���ӽð�
    float verticalLookRotation;
    bool grounded;//������ ���� �ٴ�üũ
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;//���� �̵��Ÿ�
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

    Vector3 EnterRoomDoor1; //�� ��ǥ
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

        EnterRoomDoor1 = (GameObject.Find("EnterRoomDoor1")).transform.position; //�� ������ �ش� �̸��� �� ������Ʈ�� ã�Ƽ� ��ǥ�� ������
        EnterRoomDoor1.z -= 2; //�̵��� �� ������ �� ���� ������
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
            //���� �ƴϸ� ī�޶� ���ֱ�
            Destroy(rb);
            //���žƴϸ� ������ �ٵ� �����ֱ�

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
            return;//�����ƴϸ� �۵�����
        if (PV.IsMine && component.GetComponent<ChatManager>().move_on_chat == true) //ä�ý� ���콺 Ȱ��ȭ �� �̵� ����
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
        // �¿�� ������ ���콺�� �̵��� * �ӵ��� ���� ī�޶� �¿�� ȸ���� �� ���
        float yRotateSize = Input.GetAxis("Mouse X");
        // ���� y�� ȸ������ ���� ���ο� ȸ������ ���
        float yRotate = transform.eulerAngles.y + yRotateSize;

        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivity);
        //���콺 �����̴� ����*�ΰ�����ŭ ���� �����̱�
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivity;
        //���콺 �����̴� ����*�ΰ�����ŭ ���� �� �ޱ�
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);
        //y�� -90������ 90���� ������ ����
        /*
        //ī�޶� �׽�Ʈ
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
        //���������� �������� ũ��� 1�� �븻������
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
        //���� ����Ʈ�� ������ �ٴ¼ӵ�, �������� �ȴ¼ӵ����ϱ�
        //smoothTime��ŭ�� ���ļ� �̵����ֱ�. 
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)//�������� �����̽��� ������
        {
            rb.AddForce(transform.up * jumpForce);//�����¸�ŭ���� ������
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

    public void SetGroundedState(bool _grounded) //���鿡 �ִ� �� Ȯ��
    {
        grounded = _grounded;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;//�����ƴϸ� �۵�����
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        //�̵��ϴ°Ŵ� ��� ���� moveAmount��ŭ�� �����Ƚð�(0.2��)���ٿ� ���缭
    }

    private void OnTriggerEnter(Collider other) //�� ������Ʈ�� ������ �ݴ��� �� ��ó�� �ڷ���Ʈ
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


