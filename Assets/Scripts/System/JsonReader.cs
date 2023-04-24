using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class JsonReader :MonoBehaviour
{
    public static JsonReader instance = null;
    string miJsonString;

    string path = "Assets/Resources/MiPartida.json";


    public void Awake()
    {
        instance = this;
    }

    public static string LoadJsonAsResource(string path) {
        string jsonFilePath = path.Replace(".json", "");
        TextAsset loadedJsonFile = Resources.Load<TextAsset>(jsonFilePath);
        return loadedJsonFile.text;
    }
    public void SaveJsonData(GameSave miPartida)
    {
        miJsonString = JsonUtility.ToJson(miPartida);
        JsonUtility.FromJsonOverwrite(miJsonString, miPartida);
        string str = miJsonString;
        using (FileStream fs = new FileStream(path, FileMode.Create))
        {
            using (StreamWriter writer = new StreamWriter(fs))
            {
                writer.Write(str);
            }
        }
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public void LoadJsonData(ref GameSave miPartida)
    {
        if (File.Exists(path))
        {
            string loadMyPartidaFromJson = JsonReader.LoadJsonAsResource("MiPartida.json");
            miPartida = JsonUtility.FromJson<GameSave>(loadMyPartidaFromJson);
        }
        else
        {
            SaveJsonData(miPartida);
        }
    }
}

public class GameSave
{

}
