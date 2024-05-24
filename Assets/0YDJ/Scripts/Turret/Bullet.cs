using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] PooledObject poolObject;
    [SerializeField] string poolItemName = "Bullet";//오브젝트 풀에 저장된 bullet 오브젝트의 이름
    [SerializeField] float moveSpeed = 10f;//총알의 이동 속도
    [SerializeField] LayerMask BulletTarget;

    private Rigidbody rigid;
    private ObjectPool pool;
    public ObjectPool Pool { get { return pool; } set { pool = value; } }
    public Vector3 Velocity { get { return rigid.velocity; } set { rigid.velocity = value; } }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        rigid.velocity = (transform.forward * moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (BulletTarget.Contain(other.gameObject.layer)) // 나무, 플레이어 , 닭
        {
            IDamageble damageble = other.gameObject.GetComponent<IDamageble>();
            if (damageble != null) // 데미지 인터페이스가 있다면 데미지 함수 실행
            {
                Debug.Log($" 총알에 {other.name} 맞음");
                damageble.Damaged(3);
            }
            //damageble?.Damaged(1); // 이렇게 쓸 수도 있다
            poolObject.Release();
        }
    }
}
