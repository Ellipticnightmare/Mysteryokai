using UnityEngine;

[CreateAssetMenu(fileName = "yokaiDatabase",menuName = "databases/yokaiDatabase", order = 0)]
public class YokaiDatabase : ScriptableObject
{
    public GameObject[] allYokai;
}