using System.IO;
using UnityEngine;

public static class SaveSystem
{
    public static void Save(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        string encrypted = EncryptionUtility.Encrypt(json);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", encrypted);
    }

    public static GameSaveData Load()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (!File.Exists(path))
        {
            return new GameSaveData();
        }

        string encrypted = File.ReadAllText(path);

        try
        {
            string decrypted = EncryptionUtility.Decrypt(encrypted);
            return JsonUtility.FromJson<GameSaveData>(decrypted);
        }
        catch
        {
            Debug.LogWarning("Failed to load save data. Returning new save data.");
            return new GameSaveData();
        }
    }
}
