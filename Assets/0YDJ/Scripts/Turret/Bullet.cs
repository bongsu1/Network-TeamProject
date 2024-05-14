using Unity.XR.Oculus.Input;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] PooledObject poolObject;
    public string poolItemName = "Bullet";//오브젝트 풀에 저장된 bullet 오브젝트의 이름
    public float moveSpeed = 10f;//총알의 이동 속도
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //AddFo
        //rigid.AddForce(Vector3.forward * moveSpeed * Time.deltaTime);

        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

        //Debug.Log(gameObject.ri)

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3) // 플레이어 레이어에 닿았다
        {
            IDamageble damageble = other.gameObject.GetComponent<IDamageble>(); // 데미지 인터페이스 받아오기
            if (damageble != null ) // 데미지 인터페이스가 있다면 데미지 함수 실행
            {
                damageble.Damaged(1);
            }
            //damageble?.Damaged(1); // 이렇게 쓸 수도 있다
            poolObject.Release();
        }
    }
}
