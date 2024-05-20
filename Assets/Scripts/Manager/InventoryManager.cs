using System;
using System.IO;
using System.Text;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [SerializeField] InventoryObject inven;
    [SerializeField] InventoryObject equip;
    //[SerializeField] DynamicInterface inventory;
    //[SerializeField] StaticInterface equipment;

    [ContextMenu("저장 (JSON)")]
    public void SaveToJson()
    {
        Debug.Log("인벤토리 저장 (JSON)");

        // 1. JSON 문자열 준비
        string jsonData = JsonUtility.ToJson(new SaveInven(inven.Container, equip.Container), true); // 개인 필드 포함

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
        }
    }

    [ContextMenu("로드 (JSON)")]
    public void LoadFromJson()
    {
        Debug.Log("인벤토리 로드 (JSON)");
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
                    SaveInven newContainer = JsonUtility.FromJson<SaveInven>(jsonData);
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
        }
    }
}
