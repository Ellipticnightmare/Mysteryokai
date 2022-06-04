using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    public float movementSpeed;
    public GameObject CameraObject;
    CharacterController chara;
    GameManager manager;
    public static KeyCode crouch, run, interact, attack, pause, inventory, quickQuests, quickJournal, left, right, up, down;
    [HideInInspector]
    public int money, health, stamina, maxHealth, maxStamina;
    private void Start()=>chara = GetComponent<CharacterController>();
    private void Update()
    {
        #region KeyBinding
        crouch = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Crouch"));
        run = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Run"));
        interact = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Interact"));
        attack = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Attack"));
        pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause"));
        inventory = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Inventory"));
        quickQuests = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("QuickQuests"));
        quickJournal = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("QuickJournal"));
        left = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Left"));
        right = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Right"));
        up = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Up"));
        down = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Down"));
        #endregion
        float h = float.Parse(Input.GetKey(right).ToString()) - float.Parse(Input.GetKey(left).ToString());
        float v = float.Parse(Input.GetKey(up).ToString()) - float.Parse(Input.GetKey(down).ToString());
        Vector3 moveDirection = this.transform.TransformDirection(h, -9.81f, v);
        chara.Move(moveDirection * movementSpeed * Time.deltaTime);
        transform.Rotate(new Vector3(0, Input.GetAxis("Mouse X"), 0));
        CameraObject.transform.Rotate(new Vector3(-(Input.GetAxisRaw("Mouse Y")), 0, 0));
    }
}