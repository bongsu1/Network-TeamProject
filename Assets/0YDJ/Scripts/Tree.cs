using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviourPun, IDamageble
{
    [SerializeField] Rigidbody rigid;
    [SerializeField] int hp = 10;
    [SerializeField] float rotSpeed = 100f;

    [Header("Sound")]
    [SerializeField] AudioSource TreeDamagedSound;
    [SerializeField] AudioSource TreeDiedSound;

    public void Damaged(int Damage)
    {

        if (hp <= 0) // 죽음
        {
            rigid.isKinematic = false;
            StartCoroutine(Died());
        }
        else // 안죽음 (죽었을 때 못때리게 하기)
        {
            hp -= Damage;
            TreeDamagedSound.Play();
        }
    }

    IEnumerator Died()
    {
        TreeDiedSound.Play();
        yield return new WaitForSeconds(5f);
        PhotonNetwork.Destroy(gameObject);
    }
}
