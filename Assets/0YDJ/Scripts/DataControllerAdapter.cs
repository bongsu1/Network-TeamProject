using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataControllerAdapter : MonoBehaviour,IDamageble
{
    [SerializeField] PlayerDataController playerDataController;

    public void Damaged(int Damage)
    {
        playerDataController.TakeDamage(Damage);
    }

}
