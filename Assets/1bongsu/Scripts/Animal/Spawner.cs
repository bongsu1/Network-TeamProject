using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum AnimalType { chiken, AddMore }
    // 닭 리스폰 기능
    [SerializeField] GameObject chickenPrefab;

    [SerializeField] private int count;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private void Spawn(AnimalType animal)
    {
        switch (animal)
        {
            case AnimalType.chiken:
                GameObject chicken =
                    PhotonNetwork.InstantiateRoomObject(chickenPrefab.name, transform.position, Quaternion.identity);
                Animal newAnimal = chicken.GetComponent<Animal>();
                newAnimal?.OnDie.AddListener(CountDown);
                count++;
                break;
        }
    }

    IEnumerator SpawnRoutine()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsMasterClient);
        while (true)
        {
            yield return new WaitUntil(() => count < 2);
            yield return new WaitForSeconds(5f);
            Spawn(AnimalType.chiken);
        }
    }

    private void CountDown()
    {
        count--;
        if (count <= 0)
            count = 0;
    }
}
