using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Fire : MonoBehaviour
{
    public Transform firePos;
    public GameObject bulletPrefab;
    private ParticleSystem muzzleFlash;
    private PhotonView pv;
    // ���� ���콺 ��ư Ŭ�� �̺�Ʈ ����
    private bool isMouseClick => Input.GetMouseButtonDown(0);
    void Start()
    {
        // ����� ������Ʈ ����
        pv = GetComponent<PhotonView>();
        // FirePos ������ �ִ� �ѱ� ȭ�� ȿ�� ����
        muzzleFlash = firePos.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }

    void Update()
    {
        // ���� �������ο� ���콺 ���� ��ư�� Ŭ������ �� �Ѿ��� �߻�
        if (pv.IsMine && isMouseClick)
        {
            FireBullet(pv.Owner.ActorNumber);
            //RPC�� �������� �ִ� �Լ��� ȣ��
            pv.RPC("FireBullet", RpcTarget.Others, pv.Owner.ActorNumber);
        }
    }
    [PunRPC]
    void FireBullet(int actorNo)
    {
        // �ѱ�ȭ�� ȿ���� ���� ���� �ƴ� ��쿡 �ѱ� ȭ��ȿ�� ����
        if (!muzzleFlash.isPlaying) muzzleFlash.Play(true);
        GameObject bullet = Instantiate(bulletPrefab,
        firePos.position,
        firePos.rotation);
        bullet.GetComponent<Bullet>().actorNumber = actorNo;
    }
}