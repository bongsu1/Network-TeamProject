using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Terat : MonoBehaviour
{
    [SerializeField] List<Collider> TargetList = new List<Collider>(); // 범위 안에 들어온 플레이어들을 저장할 리스트

    [SerializeField] float range;

    // Update is called once per frame
    void Update()
    {
        

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;    
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void RangeIn()
    {

        Collider[] Targets = Physics.OverlapSphere(transform.position, range);
        foreach (Collider Target in Targets)
        {
            //Debug.Log(Target.Length);
            if (Target.gameObject.layer == 3) //플레이어가 범위 내에 들어왔다면 리스트에 추가하기
            {
                if (!TargetList.Contains(Target))
                {
                    Debug.Log("플레이어 들어옴");
                    TargetList.Add(Target);
                }
                
            }

            if (!Targets.Contains(Target))            //플레이어가 범위 내에 나가면 리스트에서 지우기
            {
                if (TargetList.Contains(Target))
                {
                    Debug.Log("플레이어 나감");
                    TargetList.Remove(Target);
                }
            }
            
        }
    }

    
}
