using System.IO;
using UnityEngine;

public class LocalDataHandler : MonoBehaviour
{
    public static LocalDataHandler Instance { get; private set; }

    public PlayerData PlayerData {  get; private set; }

    private const string playerDataPath = "/PlayerData.json";

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerData = new PlayerData();
    }

    public void LoadData()
    {
        string savePath = Application.persistentDataPath + playerDataPath;
        if(File.Exists(savePath))
        {
            string loadedData = File.ReadAllText(savePath);
            PlayerData = JsonUtility.FromJson<PlayerData>(loadedData);
            //Debug.Log("Loaded player data");
        }
    }

    public void SaveData()
    {
        string savePath = Application.persistentDataPath + playerDataPath;
        string dataToSave = JsonUtility.ToJson(PlayerData);

        File.WriteAllText(savePath, dataToSave);
        //Debug.Log("Saved player data");
    }
}
