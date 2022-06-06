using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomPoint : MonoBehaviour
{
    public SVector3 position { get { return this.transform.position; } }
    public locationTrait[] myTraits = new locationTrait[3];
    [HideInInspector]
    public bool isSpawnPoint = false;
    [HideInInspector]
    public double weightedScore;
}