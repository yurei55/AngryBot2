using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject effect;
    public int actorNumber;
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 1000.0f);
        // �����ð��� ���� �� �Ѿ��� ����
        Destroy(this.gameObject, 3.0f);
    }
    void OnCollisionEnter(Collision coll)
    {
        // �浹 ���� ����
        var contact = coll.GetContact(0);
        // �浹 ������ ����ũ ����Ʈ ����
        var obj = Instantiate(effect,
        contact.point,
        Quaternion.LookRotation(-contact.normal));

        Destroy(obj, 2.0f);
        Destroy(this.gameObject);
    }
}
