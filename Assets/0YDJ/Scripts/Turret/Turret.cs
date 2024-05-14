using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] Transform GunBody = null;
    [SerializeField] float range = 0f;
    [SerializeField] LayerMask layerMask = 0;
    [SerializeField] float spinSpeed;
    [SerializeField] float fireRate = 0f;
    [SerializeField] GameObject bulletPre;

    [SerializeField] ObjectPool pool;
    float currentFireRate;

    Transform Target = null;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    void SearchEnemy()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, range, layerMask);
        Transform shortestTarget = null;

        if (targets.Length > 0)
        {
            float shortestDistance = Mathf.Infinity;
            foreach (Collider target in targets)
            {
                float distance = Vector3.SqrMagnitude(transform.position - target.transform.position);
                if (shortestDistance > distance)
                {
                    shortestDistance = distance;
                    shortestTarget = target.transform;
                }
            }
        }

        Target = shortestTarget;
    }


    private void Start()
    {
        currentFireRate = fireRate;
        InvokeRepeating("SearchEnemy", 0f, 0.5f);
    }

    private void Update()
    {

        if (Target == null)
            GunBody.Rotate(new Vector3(0, 45, 0) * Time.deltaTime);//평상 시 천천히 돌기
        else //플레이어가 범위 내에 들어왔을 때
        {

            //총구 세팅
            Vector3 dir = Target.position - transform.position; //타겟과 터렛의 위치를 뺀 값
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 euler = Quaternion.RotateTowards(GunBody.rotation, lookRotation, spinSpeed * Time.deltaTime).eulerAngles;
            GunBody.rotation = Quaternion.Euler(0, euler.y, 0);


            //발사 세팅
            Quaternion fireRotation = Quaternion.Euler(0, lookRotation.eulerAngles.y, 0); //총구가 플레이어를 가리킬 때 쏘기
            if (Quaternion.Angle(GunBody.rotation, fireRotation) < 5f)
            {
                currentFireRate -= Time.deltaTime;
                if (currentFireRate <= 0)
                {
                    currentFireRate = fireRate;
                    //Instantiate(bulletPre, transform.position, lookRotation);
                    pool.GetPool(transform.position, lookRotation);
                    Debug.Log("발사");
                }
            }
        }
    }
}
