using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] InventoryObject inven;
    [SerializeField] InventoryObject equip;

    [SerializeField] Inventory inventory;
    //[SerializeField] DynamicInterface inventory;
    //[SerializeField] StaticInterface equipment;
    [Header("Drop")]
    [SerializeField] public PracticePlayer player;
    [SerializeField] public Vector3 dropPotision;

    [Header("Auth")]
    [SerializeField] string email;
    [SerializeField] string pass;

    [Header("Firebase")]
    private InvenData invenData;
    public InvenData InvenData { get { return invenData; } }


    [ContextMenu("저장 (JSON)")]
    public void SaveToJson()
    {
        Debug.Log("인벤토리 저장 (JSON)");

        /*//오프라인
        // 1. JSON 문자열 준비
        string jsonData = JsonUtility.ToJson(new InvenData(inven.Container, equip.Container), true); // 개인 필드 포함

        // 2. 저장 경로 가져오기
        string savePath = Path.Combine(Application.persistentDataPath, "inventory.json"); // 경로

        // 3. JSON 데이터를 파일에 쓰기
        try
        {
            using (FileStream fileStream = File.Create(savePath))
            {
                byte[] data = Encoding.UTF8.GetBytes(jsonData);
                fileStream.Write(data, 0, data.Length);
            }
            Debug.Log("인벤토리 JSON으로 성공적으로 저장됨!");
        }
        catch (Exception e)
        {
            Debug.LogError("인벤토리를 JSON으로 저장하는 중 오류 발생: " + e.Message);
        }*/

        string json = JsonUtility.ToJson(new InvenData(inven.Container, equip.Container));
        string userID = FirebaseManager.Auth.CurrentUser.UserId;

        FirebaseManager.DB.GetReference($"InvenData/{userID}").SetRawJsonValueAsync(json);
    }

    [ContextMenu("로드 (JSON)")]
    public void LoadFromJson()
    {
        Debug.Log("인벤토리 로드 (JSON)");
        /*// 오프라인
        // 1. 저장 경로 가져오기
        string savePath = Path.Combine(Application.persistentDataPath, "inventory.json"); // 경로

        // 2. 파일 존재 여부 확인
        if (File.Exists(savePath))
        {
            // 3. JSON 데이터를 파일에 읽기
            try
            {
                using (FileStream fileStream = File.OpenRead(savePath))
                {
                    byte[] data = new byte[(int)fileStream.Length];
                    fileStream.Read(data, 0, data.Length);

                    string jsonData = Encoding.UTF8.GetString(data);

                    // 4. JSON 데이터를 Container 객체로 역직렬화
                    InvenData newContainer = JsonUtility.FromJson<InvenData>(jsonData);
                    //inven.Container.Items = newContainer.invenSave.Items; // 직접 배열 복사
                    //equip.Container.Items = newContainer.equipSave.Items; // 이게 원인이었네~~~

                    for (int i = 0; i < inven.Container.Items.Length; i++) // 배열마다 분배
                    {
                        Debug.Log(newContainer.invenSave.Items[i]);
                        inven.Container.Items[i].UpdateSlot(newContainer.invenSave.Items[i].item, newContainer.invenSave.Items[i].amount);
                    }
                    for (int i = 0; i < equip.Container.Items.Length; i++)
                    {
                        Debug.Log(newContainer.equipSave.Items[i]);
                        equip.Container.Items[i].UpdateSlot(newContainer.equipSave.Items[i].item, newContainer.equipSave.Items[i].amount);
                    }

                    Debug.Log("인벤토리 JSON에서 성공적으로 로드됨!");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("JSON에서 인벤토리를 로드하는 중 오류 발생: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("인벤토리 JSON 파일을 찾을 수 없음: " + savePath);
        }*/
        string userID = FirebaseManager.Auth.CurrentUser.UserId;
        #region invenData
        FirebaseManager.DB.GetReference($"InvenData/{userID}")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log($"Get InvenData canceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log($"Get InvenData failed : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    invenData = JsonUtility.FromJson<InvenData>(json);
                    for (int i = 0; i < inven.Container.Items.Length; i++) // 배열마다 분배
                    {
                        Debug.Log(invenData.invenSave.Items[i]);
                        inven.Container.Items[i].UpdateSlot(invenData.invenSave.Items[i].item, invenData.invenSave.Items[i].amount);
                    }
                    for (int i = 0; i < equip.Container.Items.Length; i++)
                    {
                        Debug.Log(invenData.equipSave.Items[i]);
                        equip.Container.Items[i].UpdateSlot(invenData.equipSave.Items[i].item, invenData.equipSave.Items[i].amount);
                    }
                }
                else
                {
                    invenData = new InvenData(inven.Container, equip.Container);
                }
            });
        #endregion
    }
    // 크아악
    // 이 아래로 정리 필요함
    [ContextMenu("Login")]
    public void Login()
    {
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                return;
            }
            else if (task.IsFaulted)
            {
                return;
            }

            LoadFromJson();
            //StartSaveRoutine(); // 자동저장 루틴
        });
    }
    [ContextMenu("Logout")]
    private void Logout()
    {
        FirebaseManager.Auth.SignOut();
        //StopSaveRoutine();
    }

    public void DropPositioning()
    {
        Debug.Log("Repositioning");
        if(player == null)
        {
            return;
        }
        else
        {
            dropPotision = player.transform.position + new Vector3(0, 0, 2);
        }
    }
    //private void StartSaveRoutine()
    //{
    //    saveRoutine = StartCoroutine(SaveRoutine());
    //}
    //Coroutine saveRoutine;
    //IEnumerator SaveRoutine()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(10f);
    //        SaveRoomData();
    //    }
    //}
    //private void StopSaveRoutine()
    //{
    //    SaveRoomData();
    //    StopCoroutine(saveRoutine);
    //}
}
