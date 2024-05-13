using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public string poolItemName = "Bullet";//오브젝트 풀에 저장된 bullet 오브젝트의 이름
    public float moveSpeed = 10f;//총알의 이동 속도
    public float lifeTime = 3f; //총알의 수명
    public float elapsedTime;//총알이 활성화된 뒤 경과시간을 계산하기 위한 변수




    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
        if(GetTimer() > lifeTime)
        {
            SetTimer();

            //ObjectPool.Instantiate.PushToPool(poolItemName, gameObject);

        }
    }

    float GetTimer()
    {
        return (elapsedTime += Time.deltaTime);
    }

    void SetTimer()
    {
        elapsedTime = 0f;
    }
}
