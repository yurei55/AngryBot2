using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Player = Photon.Realtime.Player;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Damage : MonoBehaviourPunCallbacks
{
    // GameManager ������ ���� ����
    private GameManager gameManager;
    // ��� �� ���� ó���� ���� MeshRenderer ������Ʈ�� �迭
    private Renderer[] renderers;
    // ĳ������ �ʱ� ����ġ
    private int initHp = 100;
    // ĳ������ ���� ����ġ
    public int currHp = 100;
    private Animator anim;
    private CharacterController cc;
    // �ִϸ����� �信 ������ �Ķ������ ��ð� ����
    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");
    public bool blood = false;
    public GameObject Target;
    public Image img;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        // ĳ���� ���� ��� Renderer ������Ʈ�� ������ �� �迭�� �Ҵ�
        renderers = GetComponentsInChildren<Renderer>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        //���� ����ġ�� �ʱ� ����ġ�� �ʱ갪 ����
        currHp = initHp;
        Target = GameObject.FindWithTag("BLOOD");
        img = Target.GetComponent<Image>();
    }

    void OnCollisionEnter(Collision coll)
    {
        // ���� ��ġ�� 0���� ũ�� �浹ü�� �±װ� BULLET�� ��쿡 ���� ��ġ�� ����
        if (currHp > 0 && coll.collider.CompareTag("BULLET"))
        {
            currHp -= 20;
            if (currHp <= 20)
            {
                if (photonView.IsMine)
                {
                    blood = true;
                }
            }
            if (currHp <= 0)
            {
                // �ڽ��� PhotonView �� ���� �޽����� ���
                if (photonView.IsMine)
                {
                    gameManager.totScore--;
                    // �Ѿ��� ActorNumber�� ����
                    var actorNo = coll.collider.GetComponent<Bullet>().actorNumber;
                    // ActorNumber�� ���� �뿡 ������ �÷��̾ ����
                    Player lastShootPlayer = PhotonNetwork.CurrentRoom.GetPlayer(actorNo);
                    // �޽��� ����� ���� ���ڿ� ����
                    string msg = string.Format("\n<color=#00ff00>{0}</color> is killed by <color=#ff0000>{1}</color>",

                    photonView.Owner.NickName,
                    lastShootPlayer.NickName);

                    photonView.RPC("KillMessage", RpcTarget.AllBufferedViaServer, msg);
                    gameManager.DisplayScore();
                }
                StartCoroutine(PlayerDie());
            }
        }
    }

    private void Update()
    {
        if (photonView.IsMine && blood == true)
        {
            img.enabled = true;
            //Debug.Log("on");
        }
        else if (photonView.IsMine && blood == false)
        {
            img.enabled = false;
            //Debug.Log("off");
        }
    }
    [PunRPC]
    void KillMessage(string msg)
    {
        // �޽��� ���
        gameManager.msgList.text += msg;
    }
    IEnumerator PlayerDie()
    {
        blood = false;
        // CharacterController ������Ʈ ��Ȱ��ȭ
        cc.enabled = false;
        // ������ ��Ȱ��ȭ
        anim.SetBool(hashRespawn, false);
        // ĳ���� ��� �ִϸ��̼� ����
        anim.SetTrigger(hashDie);
        yield return new WaitForSeconds(3.0f);
        if (gameManager.totScore >= 0)
        {
            // ������ Ȱ��ȭ
            anim.SetBool(hashRespawn, true);
            SetPlayerVisible(false);
            yield return new WaitForSeconds(1.5f);
            // ���� ��ġ�� ������
            Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
            int idx = Random.Range(1, points.Length);
            transform.position = points[idx].position;
            // ������ �� ���� �ʱ갪 ����
            currHp = 100;
            // ĳ���͸� �ٽ� ���̰� ó��
            SetPlayerVisible(true);
            // CharacterController ������Ʈ Ȱ��ȭ
            cc.enabled = true;
        }
        if (gameManager.totScore < 0)
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");
        }
    }
    //Renderer ������Ʈ�� Ȱ��/��Ȱ��ȭ�ϴ� �Լ�
    void SetPlayerVisible(bool isVisible)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = isVisible;
        }
    }
}