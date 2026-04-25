using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    private static MainManager instance;
    public static MainManager GetInstance()
    {
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadColor();
    }

    public Color TeamColor = Color.white;

    [System.Serializable]
    class SaveData
    {
        // 3.2
        public Color TeamColor;

        // 5.1
        public string LastTimePlayed;
    }

    public void SaveColor()
    {

        // 2.3
        // PlayerPrefs.SetFloat("TeamColor.R", TeamColor.r);
        // PlayerPrefs.SetFloat("TeamColor.G", TeamColor.g);
        // PlayerPrefs.SetFloat("TeamColor.B", TeamColor.b);
        // PlayerPrefs.SetFloat("TeamColor.A", TeamColor.a);

        // 3.3
        // SaveData data = new SaveData();
        // data.TeamColor = TeamColor;
        // string json = JsonUtility.ToJson(data);
        // Debug.Log($"Saving color {TeamColor} as json: {json}");
        // PlayerPrefs.SetString("SaveData", json);

        // 4.2
        // SaveData data = new SaveData();
        // data.TeamColor = TeamColor;

        // string json = JsonUtility.ToJson(data);
        // string path = "C:\\Users\\chany\\Desktop\\savefile.json";
        // File.WriteAllText(path, json);

        // 4.3
        // SaveData data = new SaveData();
        // data.TeamColor = TeamColor;

        // string json = JsonUtility.ToJson(data);
        // string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        // File.WriteAllText(path, json);
        // Debug.Log("Save file path: " + path);

        // 5.1
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;
        data.LastTimePlayed = DateTime.Now.ToString();

        string json = JsonUtility.ToJson(data);
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        File.WriteAllText(path, json);
        Debug.Log("Save file path: " + path);
    }

    public void LoadColor()
    {
        // 2.3
        // TeamColor.r = PlayerPrefs.GetFloat("TeamColor.R", 1f);
        // TeamColor.g = PlayerPrefs.GetFloat("TeamColor.G", 1f);
        // TeamColor.b = PlayerPrefs.GetFloat("TeamColor.B", 1f);
        // TeamColor.a = PlayerPrefs.GetFloat("TeamColor.A", 1f);

        // 3.3
        // string json = PlayerPrefs.GetString("SaveData", "");
        // if (!string.IsNullOrEmpty(json))
        // {
        //     SaveData data = JsonUtility.FromJson<SaveData>(json);
        //     TeamColor = data.TeamColor;
        // }

        // 4.2
        // string path = "C:\\Users\\chany\\Desktop\\savefile.json";
        // if (File.Exists(path))
        // {
        //     string json = File.ReadAllText(path);
        //     SaveData data = JsonUtility.FromJson<SaveData>(json);
        //     TeamColor = data.TeamColor;
        // }

        // 4.3
        string path = Path.Combine(Application.persistentDataPath, "savefile.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            TeamColor = data.TeamColor;
        }
    }

    public void SaveColorWithEncryption()
    {
        // 5.2
        SaveData data = new SaveData();
        data.TeamColor = TeamColor;
        data.LastTimePlayed = DateTime.Now.ToString();

        string json = JsonUtility.ToJson(data);
        string encryptedJson = encrypt(json, "mysecretkey");
        string path = Path.Combine(Application.persistentDataPath, "savefile_encrypted.json");
        File.WriteAllText(path, encryptedJson);
        Debug.Log("Encrypted save file path: " + path);
    }

    public void LoadColorWithEncryption()
    {
        string path = Path.Combine(Application.persistentDataPath, "savefile_encrypted.json");
        if (File.Exists(path))
        {
            string encryptedJson = File.ReadAllText(path);
            string decryptedJson = decrypt(encryptedJson, "mysecretkey");
            SaveData data = JsonUtility.FromJson<SaveData>(decryptedJson);
            TeamColor = data.TeamColor;
            Debug.Log("Last time played: " + data.LastTimePlayed);
        }
    }

    private string encrypt(string input, string aesKey)
    {
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        byte[] keyBytes = new byte[16];
        Array.Copy(System.Text.Encoding.UTF8.GetBytes(aesKey), keyBytes, Math.Min(keyBytes.Length, aesKey.Length));

        using (var aes = new System.Security.Cryptography.AesManaged())
        {
            aes.Key = keyBytes;
            aes.Mode = System.Security.Cryptography.CipherMode.ECB;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            using (var encryptor = aes.CreateEncryptor())
            {
                byte[] encryptedBytes = encryptor.TransformFinalBlock(inputBytes, 0, inputBytes.Length);
                return Convert.ToBase64String(encryptedBytes);
            }
        }
    }

    private string decrypt(string input, string aesKey)
    {
        byte[] encryptedBytes = Convert.FromBase64String(input);
        byte[] keyBytes = new byte[16];
        Array.Copy(System.Text.Encoding.UTF8.GetBytes(aesKey), keyBytes, Math.Min(keyBytes.Length, aesKey.Length));

        using (var aes = new System.Security.Cryptography.AesManaged())
        {
            aes.Key = keyBytes;
            aes.Mode = System.Security.Cryptography.CipherMode.ECB;
            aes.Padding = System.Security.Cryptography.PaddingMode.PKCS7;

            using (var decryptor = aes.CreateDecryptor())
            {
                byte[] decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return System.Text.Encoding.UTF8.GetString(decryptedBytes);
            }
        }
    }
}
