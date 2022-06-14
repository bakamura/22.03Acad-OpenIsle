using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {

    // Saves the progress using a BinaryFormatter into it's N save file
    public static void SaveProgress(int saveFile) {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/progress" + saveFile.ToString() + ".data";
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData data = new SaveData(PlayerData.Instance);

        formatter.Serialize(stream, data);

        stream.Close();
    }

    // Loads the game progress in N save file to the game
    public static SaveData LoadProgress(int saveFile) {
        string path = Application.persistentDataPath + "/progress" + saveFile.ToString() +".data";
        if (File.Exists(path)) {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();

            return data;
        }
        else {
            Debug.Log("Save file could'nt be found in" + path);
            return null;
        }
    }

    // Erases the save file in N
    public static void EraseProgress(int saveFile) {
        string path = Application.persistentDataPath + "/progress" + saveFile.ToString() + ".data";
        if (File.Exists(path)) File.Delete(path);
        else Debug.Log("Couldn't find anything in" + path);
    }
}
