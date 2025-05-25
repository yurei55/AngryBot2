using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Cinemachine;

public class Movement : MonoBehaviour, IPunObservable
{
    private PhotonView pv;
    private Cinemachine.CinemachineVirtualCamera virtualCamera;
    // ������Ʈ ĳ�� ó���� ���� ����
    private CharacterController controller;
    private new Transform transform;
    private Animator animator;
    private new Camera camera;
    // ������ Plane�� ����ĳ�����ϱ� ���� ����
    private Plane plane;
    private Ray ray;
    private Vector3 hitPoint;
    // ���ŵ� ��ġ�� ȸ������ ������ ����
    private Vector3 receivePos;
    private Quaternion receiveRot;
    // ���ŵ� ��ǥ�� �̵� �� ȸ�� �ӵ��� �ΰ���
    public float damping = 10.0f;
    // �̵� �ӵ�
    public float moveSpeed = 10.0f;
    void Start()
    {
        controller = GetComponent<CharacterController>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        camera = Camera.main;
        pv = GetComponent<PhotonView>();
        virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        //PhotonView�� �ڽ��� ���� ��� �ó׸ӽ� ����ī�޶� ����
        if (pv.IsMine)
        {
            virtualCamera.Follow = transform;
            virtualCamera.LookAt = transform;
        }
        // ������ �ٴ��� ���ΰ��� ��ġ�� �������� ����
        plane = new Plane(transform.up, transform.position);
    }
    void Update()
    {
        // �ڽ��� ������ ��Ʈ��ũ ��ü�� ��Ʈ��
        if (pv.IsMine)
        {
            Move();
            Turn();
        }
        else
        {
            // ���ŵ� ��ǥ�� ������ �̵�ó��
            transform.position = Vector3.Lerp(transform.position,
            receivePos,
            Time.deltaTime * damping);

            // ���ŵ� ȸ�������� ������ ȸ��ó��
            transform.rotation = Quaternion.Slerp(transform.rotation,
            receiveRot,
            Time.deltaTime * damping);

        }
    }

    // Ű���� �Է°� ����
    float h => Input.GetAxis("Horizontal");
    float v => Input.GetAxis("Vertical");
    // �̵� ó���ϴ� �Լ�
    void Move()
    {
        Vector3 cameraForward = camera.transform.forward;
        Vector3 cameraRight = camera.transform.right;
        cameraForward.y = 0.0f;
        cameraRight.y = 0.0f;
        // �̵��� ���� ���� ���
        Vector3 moveDir = (cameraForward * v) + (cameraRight * h);
        moveDir.Set(moveDir.x, 0.0f, moveDir.z);
        // ���ΰ� ĳ���� �̵�ó��
        controller.SimpleMove(moveDir * moveSpeed);
        // ���ΰ� ĳ������ �ִϸ��̼� ó��
        float forward = Vector3.Dot(moveDir, transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);
        animator.SetFloat("Forward", forward);
        animator.SetFloat("Strafe", strafe);
    }

    // ȸ�� ó���ϴ� �Լ�
    void Turn()
    {
        // ���콺�� 2���� ��ǩ���� �̿��� 3���� ����(����)�� ����
        ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0.0f;
        // ������ �ٴڿ� ���̸� �߻��� �浹�� ������ �Ÿ��� enter
        // ������ ��ȯ
        plane.Raycast(ray, out enter);
        // ������ �ٴڿ� ���̰� �浹�� ��ǩ�� ����
        hitPoint = ray.GetPoint(enter);
        // ȸ���ؾ� �� ������ ���͸� ���
        Vector3 lookDir = hitPoint - transform.position;
        lookDir.y = 0;
        // ���ΰ� ĳ������ ȸ���� ����
        transform.localRotation = Quaternion.LookRotation(lookDir);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // �ڽ��� ���� ĳ������ ��� �ڽ��� �����͸� �ٸ� ��Ʈ��ũ �������� �۽�
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            receivePos = (Vector3)stream.ReceiveNext();
            receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
