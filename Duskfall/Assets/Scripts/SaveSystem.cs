using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveSettings(SettingsMenu settings)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settingsdata.chungus";
        FileStream stream = new FileStream(path, FileMode.Create);

        SettingsData data = new SettingsData(settings);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static SettingsData LoadSettings()
    {
        string path = Application.persistentDataPath + "/settingsdata.chungus";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SettingsData data = formatter.Deserialize(stream) as SettingsData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
