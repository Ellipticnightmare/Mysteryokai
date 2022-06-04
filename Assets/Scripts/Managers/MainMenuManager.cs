using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public ErrorManager errorDatabase;
    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        if (!Directory.Exists(Application.persistentDataPath + "/data/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/data/");
        if (!Directory.Exists(Application.persistentDataPath + "/data/keys/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/data/keys/");
        if (!Directory.Exists(Application.persistentDataPath + "/data/errors/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/data/errors");
        if (!Directory.Exists(Application.persistentDataPath + "/data/logs/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/data/logs/");
        if (!Directory.Exists(Application.persistentDataPath + "/mods/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/mods/");
            CreateSampleMod();
        }
    }
    private void Start()
    {
        if(PlayerPrefs.GetString("ErrorCode") != "NoError")
        {
            string error = "";
            string errorTitle = "";
            errorTitle += System.DateTime.UtcNow.ToShortDateString() + "_";
            errorTitle += System.DateTime.UtcNow.ToShortTimeString();
            foreach (var item in errorDatabase.errorCodes)
            {
                if (item.errorShortName == PlayerPrefs.GetString("ErrorCode"))
                    error = item.errorFullData;
            }
            string directory = Application.persistentDataPath + "/data/errors/" + errorTitle + ".txt";
            File.WriteAllText(directory, "ERROR " + errorTitle + "\n\n\n");
            File.AppendAllText(directory, PlayerPrefs.GetString("ErrorCode") + "\n\n" + error);
        }
    }
    public void CreateSampleMod()
    {
        string directory = Application.persistentDataPath + "/mods/SAMPLE_MOD.txt";
        File.WriteAllText(directory, "THIS IS A SAMPLE MOD TITLE.  THIS MOD WILL NOT PARSE ANY USABLE DATA AND WILL FAIL.  WHEN " +
                                     "CREATING YOUR OWN MODS, MAKE SURE TO USE THE FORMATTING SYMBOL AT THE END OF EACH LINE, " +
                                     "AND ONLY ENTER INTEGER VALUES ABOVE ZERO|\n" +
                                     "THIS LINE IS FOR PLAYER MOVEMENT SPEED|\n" +
                                     "THIS LINE IS FOR PLAYER STARTING MONEY|\n" +
                                     "THIS LINE IS FOR PLAYER STARTING MAX HEALTH|\n" +
                                     "THIS LINE IS FOR PLAYER STARTING MAX STAMINA|\n");
    }
    public void HitQuitGame() => Application.Quit();
}