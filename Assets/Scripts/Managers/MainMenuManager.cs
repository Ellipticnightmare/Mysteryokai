using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("ScriptableObjects")]
    public ErrorManager errorDatabase;
    [Header("UI Objects")]
    public GameObject mainPanel;
    public GameObject newPlayerPanel, playSelectPanel, modPanel, saveSelectPanel, optionsPanel, saveFileObj, saveHolder;
    public InputField newNameInput;
    public Dropdown modDropdown;
    public Toggle useMods;
    string sceneLoadName;
    [Header("New Player Data")]
    public Vector3 defaultStartingPos;
    public Quaternion defaultStartingRot;
    public List<KeyBindData> defaultKeyBindings = new List<KeyBindData>();
    public JournalChapter startingChapter;
    List<string> saveNames = new List<string>();
    int startMaxHealth, startMaxStamina, startHealth, startStamina, startMoney;
    float startSpeed;
    string selectedMod;
    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/saves/player/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/player/");
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
        if (!Directory.Exists(Application.persistentDataPath + "/saves/world/"))
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/world/");
    }
    private void Start()
    {
        if (PlayerPrefs.GetString("ErrorCode") != "NoError" && PlayerPrefs.HasKey("ErrorCode"))
        {
            string error = "";
            string errorTitle = "";
            errorTitle += System.DateTime.UtcNow.ToShortDateString();
            errorTitle += System.DateTime.UtcNow.ToLongTimeString();
            errorTitle = errorTitle.Replace(@":", "");
            errorTitle = errorTitle.Replace(@"/", "");
            errorTitle = errorTitle.Replace(@" ", "");
            errorTitle = errorTitle.Replace(@"AM", "");
            errorTitle = errorTitle.Replace(@"PM", "");
            foreach (var item in errorDatabase.errorCodes)
            {
                if (item.errorShortName == PlayerPrefs.GetString("ErrorCode"))
                    error = item.errorFullData;
            }
            string directory = Application.persistentDataPath + "/data/errors/" + errorTitle + ".txt";
            File.WriteAllText(directory, "ERROR " + errorTitle + "\n\n\n");
            File.AppendAllText(directory, PlayerPrefs.GetString("ErrorCode") + "\n\n" + error);
            Application.OpenURL(directory);
        }
        PlayerPrefs.SetString("ErrorCode", "NoError");
        PopulateDropdown();
    }
    private void Update()
    {
        if (useMods.isOn)
        {
            if (!modPanel.activeInHierarchy)
            {
                modPanel.SetActive(true);
                PopulateDropdown();
            }
            string modCheck = "";
            foreach (var item in Directory.GetFiles(Application.persistentDataPath + "/mods/" + "*.txt"))
            {
                if (File.ReadAllLines(Application.persistentDataPath + "/mods/" + item + ".txt")[0] == selectedMod)
                    modCheck = item;
            }
            string mod = Application.persistentDataPath + "/mods/" + modCheck + ".txt";
            string[] fileLines = File.ReadAllLines(mod);
            Debug.Log(fileLines[1]);
            if (int.TryParse(fileLines[1], out int number))
            {
                startSpeed = float.Parse(fileLines[1]);
                startMoney = int.Parse(fileLines[2]);
                startMaxHealth = int.Parse(fileLines[3]);
                startMaxStamina = int.Parse(fileLines[4]);
                startHealth = int.Parse(fileLines[5]);
                startStamina = int.Parse(fileLines[6]);
            }
            else
            {
                startSpeed = 3;
                startMoney = 50;
                startMaxHealth = 100;
                startMaxStamina = 100;
                startStamina = 100;
                startHealth = 100;
            }
        }
        else
        {
            modPanel.SetActive(false);
            startSpeed = 3;
            startMoney = 50;
            startMaxHealth = 100;
            startMaxStamina = 100;
            startStamina = 100;
            startHealth = 100;
        }
    }
    public void CreateSampleMod()
    {
        string directory = Application.persistentDataPath + "/mods/SAMPLE_MOD.txt";
        File.WriteAllText(directory, "SAMPLE MOD TITLE\n" +
                                     "AND ONLY ENTER INTEGER VALUES ABOVE ZERO\n" +
                                     "THIS LINE IS FOR PLAYER MOVEMENT SPEED\n" +
                                     "THIS LINE IS FOR PLAYER STARTING MONEY\n" +
                                     "THIS LINE IS FOR PLAYER STARTING MAX HEALTH\n" +
                                     "THIS LINE IS FOR PLAYER STARTING MAX STAMINA\n" +
                                     "THIS LINE IS FOR PLAYER STARTING HEALTH\n" +
                                     "THIS LINE IS FOR PLAYER STARTING STAMINA|\n\n" +
                                     "THIS MOD WILL NOT PARSE ANY USABLE DATA AND WILL FAIL.  WHEN CREATING " +
                                     "YOUR OWN MODS, MAKE SURE TO FOLLOW THE OUTLINED LINE SEQUENCE, AND ONLY ENTER " +
                                     "POSITIVE INTEGER (WHOLE NUMBERS) VALUES.  YOU MAY USE A POSITIVE DECIMAL VALUE " +
                                     "WHEN ENTERING A CUSTOMIZED STARTING MOVEMENT SPEED");
    }
    public void HitQuitGame() => Application.Quit();
    public void HitPlay()
    {
        foreach (var item in saveHolder.GetComponentsInChildren<Transform>())
        {
            if (item != saveHolder.transform)
                Destroy(item.gameObject);
        }
        saveNames.Clear();
        var fileInfo = Directory.GetFiles(Application.persistentDataPath + "/saves/" + "*.sd");
        if (fileInfo.Length > 0)
        {
            foreach (var item in fileInfo)
            {
                string saveData = saveMetadata(item);
                saveNames.Add(saveData);
            }
            foreach (var item in saveNames)
            {
                GameObject newFile = Instantiate(saveFileObj, saveHolder.transform);
                newFile.GetComponentInChildren<Text>().text = item;
                newFile.GetComponent<Button>().onClick.AddListener(() => UpdateFileSelection(newFile.GetComponentInChildren<Text>().text));
            }
            UpdateFileSelection(saveNames[0]);
            playSelectPanel.SetActive(true);
        }
        else
            PromptNewCharacter();
    }
    public void CreateNewCharacter() => PromptNewCharacter();
    public void ContinueWithSaves()
    {
        playSelectPanel.SetActive(false);
        saveSelectPanel.SetActive(true);
    }
    public void PromptNewCharacter()
    {
        mainPanel.SetActive(false);
        playSelectPanel.SetActive(false);
        saveSelectPanel.SetActive(false);
        newPlayerPanel.SetActive(true);
    }
    public void SaveNewCharacter() => CheckNewNameInput();
    void CheckNewNameInput()
    {
        if (ValidUsername(newNameInput.text))
        {
            byte[] ba = System.Text.Encoding.Default.GetBytes(newNameInput.text);
            var hexString = System.BitConverter.ToString(ba);
            hexString = hexString.Replace("-", "");
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(Application.persistentDataPath + "/saves/" + hexString + ".sd");
            PlayerData newSave = new PlayerData();
            newSave.userName = newNameInput.text;
            newSave.timePlayed = 0;
            newSave.maxHealth = startMaxHealth;
            newSave.health = startHealth;
            newSave.maxStamina = startMaxStamina;
            newSave.stamina = startStamina;
            newSave.money = startMoney;
            newSave.playerPosition = defaultStartingPos;
            newSave.playerRotation = defaultStartingRot;
            newSave.chapter = "Intro";
            bf.Serialize(file, newSave);
            file.Close();

            BinaryFormatter kbf = new BinaryFormatter();
            file = File.Create(Application.persistentDataPath + "/data/keys/" + hexString + "kd");
            KeyBindSave newCheck = new KeyBindSave();
            newCheck.savedBindings = defaultKeyBindings.ToArray();
            kbf.Serialize(file, newCheck);
        }
    }
    public void DeleteCharacter()
    {
        byte[] ba = System.Text.Encoding.Default.GetBytes(PlayerPrefs.GetString("curUser"));
        var hexString = System.BitConverter.ToString(ba);
        hexString = hexString.Replace("-", "");
        File.Delete(Application.persistentDataPath + "/saves/" + hexString + ".sd");
        File.Delete(Application.persistentDataPath + "/data/keys/" + hexString + "kd");
        HitPlay();
    }
    public void UpdateFileSelection(string username)
    {
        string newUsername = username;
        newUsername = newUsername.Substring(0, newUsername.IndexOf(","));
        sceneLoadName = newUsername.Substring(newUsername.IndexOf(","));
        sceneLoadName.Remove(0);
        Debug.Log(username);
        Debug.Log(sceneLoadName);
        PlayerPrefs.SetString("curUser", newUsername);
    }
    public void PopulateDropdown()
    {
        List<string> options = new List<string>();
        foreach (var item in Directory.GetFiles(Application.persistentDataPath + "/mods/" + "*.txt"))
        {
            string[] fileLines = File.ReadAllLines(Application.persistentDataPath + "/mods/" + item + "*.txt");
            options.Add(fileLines[0]);
        }
        options.Sort();
        modDropdown.ClearOptions();
        modDropdown.AddOptions(options);
    }
    public void UpdateModSelection(string modName) => selectedMod = modName;
    public void ConfirmSelectedSave() => SceneManager.LoadScene(sceneLoadName);
    string saveMetadata(string item)
    {
        string outString = "";
        string playerName = "";
        string chapterName = "";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(item, FileMode.Open);
        PlayerData check = (PlayerData)bf.Deserialize(file);
        file.Close();
        chapterName = check.chapter;
        playerName = check.userName;
        outString = playerName + ", " + chapterName;
        return outString;
    }
    bool ValidUsername(string check)
    {
        bool isValid = false;
        if (check.Length > 0)
        {
            byte[] ba = System.Text.Encoding.Default.GetBytes(check);
            var hexString = System.BitConverter.ToString(ba);
            hexString = hexString.Replace("-", "");
            if (!File.Exists(Application.persistentDataPath + "/saves/" + hexString + ".sd"))
                isValid = true;
        }
        return isValid;
    }
    public void CloseUIWindow(GameObject closeObj)
    {
        closeObj.SetActive(false);
    }
    public void CloseAllUI()
    {
        mainPanel.SetActive(true);
        newPlayerPanel.SetActive(false);
        modPanel.SetActive(false);
        saveSelectPanel.SetActive(false);
        optionsPanel.SetActive(false);
    }
}