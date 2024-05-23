using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ax : MonoBehaviour
{
    [SerializeField] LayerMask Target;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 28 || other.gameObject.layer == 3 || other.gameObject.layer == 6) //나무, 플레이어, 닭
        {
            IDamageble damageble = other.gameObject.GetComponent<IDamageble>();  
            if(damageble != null)
            {
                Debug.Log("도끼에 닿음 HP 깍기");
                damageble.Damaged(1);
            }

        }

    }
}
