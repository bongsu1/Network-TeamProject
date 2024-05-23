using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour, IDamageble
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] int hp = 10;
    [SerializeField] float rotSpeed = 100f;

    public void Damaged(int Damage)
    {
        hp -= Damage;

        if (hp <= 0)
        {
            rigid.isKinematic = false;
            StartCoroutine(Died());
        }
    }

    IEnumerator Died()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}
