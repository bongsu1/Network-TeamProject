using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] PooledObject poolObject;
    [SerializeField] string poolItemName = "Bullet";//오브젝트 풀에 저장된 bullet 오브젝트의 이름
    [SerializeField] float moveSpeed = 10f;//총알의 이동 속도
    private Rigidbody rigid;


    //public float MoveSpeed { get { return moveSpeed; } }

    private ObjectPool pool;
    public ObjectPool Pool { get { return pool; } set { pool = value; } }

    public Vector3 Velocity { get { return rigid.velocity; } set { rigid.velocity = value; } }
    //public Rigidbody Rigid { get { return rigid; } set { rigid = value; } }

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();

        //rigid.velocity = (transform.forward * moveSpeed);
    }

    //public void AwakeN(Vector3 SpawnPosition, Vector3 NextPosition)
    //{
    //    transform.position = Vector3.MoveTowards(SpawnPosition, NextPosition, moveSpeed * Time.deltaTime);
    //}

    void Update()
    {
        //transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        rigid.velocity = (transform.forward * moveSpeed);
        //Debug.Log(rigid.velocity);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 3 || other.gameObject.layer == 29) // 플레이어 레이어에 닿았다
        {
            IDamageble damageble = other.gameObject.GetComponent<IDamageble>(); // 데미지 인터페이스 받아오기
            if (damageble != null) // 데미지 인터페이스가 있다면 데미지 함수 실행
            {
                Debug.Log($" 총 {other.name}맞음");
                damageble.Damaged(1);
            }
            //damageble?.Damaged(1); // 이렇게 쓸 수도 있다
            poolObject.Release();
        }
        //else
        //{
        //    poolObject.Release();
        //}
    }


    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(rigid.position);
    //        stream.SendNext(rigid.rotation);
    //        stream.SendNext(rigid.velocity);
    //    }
    //    else
    //    {
    //        rigid.position = (Vector3)stream.ReceiveNext();
    //        rigid.rotation = (Quaternion)stream.ReceiveNext();
    //        rigid.velocity = (Vector3)stream.ReceiveNext();

        //        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        //        rigid.position += rigid.velocity * lag;
        //    }
    //}
}
