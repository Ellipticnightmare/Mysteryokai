using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Variables
    #region General
    #region Public/Exposed
    [Header("General")]
    public Camera mainCam;
    #endregion
    #region Private/Hidden
    bool RanStart;
    public int tMod;
    #endregion
    #endregion
    #region Data Management
    #region Public/Exposed
    [Header("Data Management")]
    public int pageStringCount = 20;
    public List<JournalPage> pages = new List<JournalPage>();
    public GameObject keybindUI;
    public Button[] keyBindButtons;
    #endregion
    #region Private/Hidden
    int pageShow;
    string saveDirectory, keyDirectory;
    PlayerManager player;
    PlayerData curPlayer;
    KeyBindSave curBinding;
    List<KeyBindData> keybindsMain = new List<KeyBindData>();
    bool isReadingForKey = false;
    string curKeyToBind;
    Button curKeyBind;
    #endregion
    #endregion
    #region UI
    #region Public/Exposed
    [Header("UI")]
    public InputField leftPage, rightPage;
    public Text leftPageNum, rightPageNum;
    public GameObject pauseMenu, saveUI;
    #endregion
    #region Private/Hidden
    #endregion
    #endregion
    #endregion
    #region Functions
    #region Base/Generic
    private void Awake()
    {
        leftPage.characterLimit = pageStringCount;
        rightPage.characterLimit = pageStringCount;
        mainCam.enabled = false;
        tMod = 0;
        ReadData();
    }
    private void RunStart()
    {
        player = FindObjectOfType<PlayerManager>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        mainCam.enabled = true;
        RanStart = true;
        tMod = 1;
    }
    private void Update()
    {
        if (RanStart)
        {
            leftPage.text = "";
            rightPage.text = "";
            leftPageNum.text = "";
            rightPageNum.text = "";
            if (pages.Count > 1)
            {
                leftPage.text = pages[pageShow].myPage;
                rightPage.text = pages[pageShow + 1].myPage;
                leftPageNum.text = pageShow.ToString();
                rightPageNum.text = (pageShow + 1).ToString();
            }
            if (Input.GetKeyDown(PlayerManager.pause))
                togglePause();
        }
    }
    private void OnGUI()
    {
        #region KeyBinding
        Event e = Event.current;
        if (isReadingForKey)
        {
            if (e.isKey || e.isMouse)
            {
                isReadingForKey = false;
                string outputcheck = e.isKey ? e.keyCode.ToString() : e.button.ToString();
                foreach (var item in keybindsMain)
                {
                    if (item.keyBindName == curKeyToBind)
                        item.keyBindData = outputcheck;
                }
                UpdateKeyBindUI();
                SaveKeyData();
            }
            else if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                isReadingForKey = false;
                string outputCheck = Input.GetKey(KeyCode.LeftShift) ? "LeftShift" : "RightShift";
                foreach (var item in keybindsMain)
                {
                    if (item.keyBindName == curKeyToBind)
                        item.keyBindData = outputCheck;
                }
                UpdateKeyBindUI();
                SaveKeyData();
            }
        }
        #endregion
    }
    #endregion
    #region GamePlay
    #region JournalManagement
    public void PageUp()
    {
        if (pageShow + 2 < pages.Count)
            pageShow += 2;
    }
    public void PageDown()
    {
        if (pageShow >= 2)
            pageShow -= 2;
    }
    public void EnterNewPageJump(string value)
    {
        int x = int.Parse(value);
        if (x % 2 == 0)
            pageShow = x;
        else
            pageShow = x - 1;
    }
    public void UpdatePageLeft(string value)
    {
        pages[pageShow].myPage = value;
    }
    public void UpdatePageRight(string value)
    {
        pages[pageShow + 1].myPage = value;
        if (pageShow + 1 >= pages.Count)
        {
            JournalPage newPage = new JournalPage();
            JournalPage newPage2 = new JournalPage();
            newPage.myPage = "";
            newPage2.myPage = "";
            pages.Add(newPage);
            pages.Add(newPage2);
        }
    }
    #endregion
    #region Quality Of Life
    public void togglePause()
    {
        pauseMenu.SetActive(pauseMenu.activeInHierarchy);
        tMod = pauseMenu.activeInHierarchy ? 0 : 1;
        Cursor.visible = pauseMenu.activeInHierarchy ? true : false;
        Cursor.lockState = pauseMenu.activeInHierarchy ? CursorLockMode.None : CursorLockMode.Locked;
    }
    #endregion
    #endregion
    #region DataManagement
    #region Saving/Loading
    public void ReadData()
    {
        byte[] ba = System.Text.Encoding.Default.GetBytes(PlayerPrefs.GetString("curUser"));
        var hexString = System.BitConverter.ToString(ba);
        hexString = hexString.Replace("-", "");
        saveDirectory = Application.persistentDataPath + "/saves/" + hexString + ".sd";
        keyDirectory = Application.persistentDataPath + "/data/keys/" + hexString + ".kd";
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(saveDirectory, FileMode.Open);
        PlayerData check = (PlayerData)bf.Deserialize(file);
        file.Close();
        if (check.userName == PlayerPrefs.GetString("curUser"))
        {
            curPlayer = check;
            player.money = check.money;
            player.maxHealth = check.maxHealth;
            player.maxStamina = check.maxStamina;
            player.health = check.health;
            player.stamina = check.stamina / check.maxStamina >= .9f ? player.maxStamina : check.stamina;
            pages = check.savePages;
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
            PlayerPrefs.SetString("ErrorCode", "corruptUser");
            return;
        }

        BinaryFormatter kF = new BinaryFormatter();
        file = File.Open(keyDirectory, FileMode.Open);
        KeyBindSave checkKey = (KeyBindSave)kF.Deserialize(file);
        keybindsMain.AddRange(checkKey.savedBindings);
        UpdateKeyBindUI();
    }
    public void SaveData(bool returnToMenu)
    {
        byte[] ba = System.Text.Encoding.Default.GetBytes(curPlayer.userName);
        var hexString = System.BitConverter.ToString(ba);
        hexString = hexString.Replace("-", "");
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(saveDirectory);
        curPlayer.maxHealth = player.maxHealth;
        curPlayer.maxStamina = player.maxStamina;
        curPlayer.money = player.money;
        curPlayer.health = player.health;
        curPlayer.stamina = player.stamina;
        curPlayer.timePlayed = curPlayer.timePlayed + Time.timeSinceLevelLoad;
        curPlayer.savePages = pages;
        curPlayer.playerPosition = player.transform.position;
        curPlayer.playerRotation = player.transform.rotation;
        bf.Serialize(file, curPlayer);
        file.Close();

        BinaryFormatter kF = new BinaryFormatter();
        file = File.Create(keyDirectory);
        curBinding.savedBindings = keybindsMain.ToArray();
        kF.Serialize(file, curBinding);
        file.Close();

        if (returnToMenu)
            SceneManager.LoadScene("MainMenu");
        else
        {
            saveUI.SetActive(false);
            pauseMenu.SetActive(false);
            tMod = 1;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    #endregion
    #region KeyBinding
    public void StartReadKey(Button curButton)
    {
        isReadingForKey = true;
        curKeyBind = curButton;
        curKeyToBind = curButton.name.ToString();
    }
    string keyName(string inButton)
    {
        string output = null;
        if (int.TryParse(inButton, out int number))
        {
            switch (inButton)
            {
                case "0":
                    output = "Left Mouse";
                    break;
                case "1":
                    output = "Right Mouse";
                    break;
            }
        }
        else
            output = (inButton == "LeftShift") ? "Left Shift" : ((inButton == "RightShift")) ? "Right Shift" : inButton;
        return output;
    }
    public void toggleKeyBinds() => keybindUI.SetActive(!keybindUI.activeInHierarchy);
    void UpdateKeyBindUI()
    {
        foreach (var item in keybindsMain)
        {
            foreach (var item2 in keyBindButtons)
            {
                if (item.keyBindName == item2.name)
                {
                    item2.GetComponentInChildren<Text>().text = item2.name + ": " + keyName(item.keyBindData);
                    PlayerPrefs.SetString(item2.name, item.keyBindData);
                }
            }
        }
    }
    void SaveKeyData()
    {
        curBinding.savedBindings = keybindsMain.ToArray();
    }
    #endregion
    #endregion
    #endregion
}
#region Externals
#region Classes
[System.Serializable]
public class JournalPage
{
    public string myPage;
}
[System.Serializable]
public class PlayerData
{
    public SVector3 playerPosition;
    public SQuat playerRotation;
    public double timePlayed;
    public string userName;
    public int money, maxHealth, maxStamina, health, stamina;
    public List<JournalPage> savePages = new List<JournalPage>();
}
[System.Serializable]
public class KeyBindSave
{
    public KeyBindData[] savedBindings;
}
[System.Serializable]
public class KeyBindData
{
    public string keyBindName, keyBindData;
}
#endregion
#region Structs
[System.Serializable]
public struct SVector3
{
    public float x, y, z;
    public SVector3(float X, float Y, float Z)
    {
        x = X;
        y = Y;
        z = Z;
    }
    public override string ToString()
    {
        return System.String.Format("[{0},{1},{2}]", x, y, z);
    }
    public static implicit operator Vector3(SVector3 iValue)
    {
        return new Vector3(iValue.x, iValue.y, iValue.z);
    }
    public static implicit operator SVector3(Vector3 value)
    {
        return new SVector3(value.x, value.y, value.z);
    }
}
[System.Serializable]
public struct SQuat
{
    public float x, y, z, w;
    public SQuat(float X, float Y, float Z, float W)
    {
        x = X;
        y = Y;
        z = Z;
        w = W;
    }
    public override string ToString()
    {
        return System.String.Format("[{0},{1},{2},{3}]", x, y, z, w);
    }
    public static implicit operator Quaternion(SQuat iValue)
    {
        return new Quaternion(iValue.x, iValue.y, iValue.z, iValue.w);
    }
    public static implicit operator SQuat(Quaternion iVale)
    {
        return new SQuat(iVale.x, iVale.y, iVale.z, iVale.w);
    }
}
[System.Serializable]
public struct ErrorData
{
    public string errorShortName;
    [TextArea(3, 6)]
    public string errorFullData;
}
#endregion
#endregion