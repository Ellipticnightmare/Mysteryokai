using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildingManager : MonoBehaviour
{
    public BuildingRegion[] rooms;
    public locationTrait[] locationTraits;
    [HideInInspector]
    public bool hasCase;
    [HideInInspector]
    public personalityTrait[] bonusTraitsSave;
    List<personalityTrait> myYokaiTraits = new List<personalityTrait>();
    YokaiData myYokai;
    NavMeshPath path;
    RoomPoint initialSpawnPoint;
    PlayerManager player;
    GameObject yokaiSpawn;
    List<YokaiData> yokaiOptions = new List<YokaiData>();
    List<YokaiData> yokaiWeightedList = new List<YokaiData>();
    List<locationTrait> resTraits = new List<locationTrait>();
    List<locationTrait> prefTraits = new List<locationTrait>();
    float AggressionIndex;
    bool isActive;
    private void Start()
    {
        player = FindObjectOfType<PlayerManager>();
    }
    private void Update()
    {
        if (hasCase && isActive)
        {

        }
    }

    public void UpdateRegionData(BuildingSaveData data)
    {
        foreach (var item in rooms)
        {
            foreach (var item2 in data.savedRegions)
            {
                if(item.roomName == item2.roomName)
                    item.roomPoints = item2.roomPoints;
            }
        }
        hasCase = data.hasCase;
        myYokai = data.savedData;
        bonusTraitsSave = data.savedBonusTraits;
        AggressionIndex = data.savedAggression;
        startCase();
    }
    public void startCase()
    {
        if (hasCase)
        {
            if (myYokai == null)
            {
                FilterYokaiList();
                WeighYokaiList();
                int randSpawn = Random.Range(0, yokaiWeightedList.Count);
                myYokai = yokaiWeightedList[randSpawn];
            }
            if (bonusTraitsSave.Length <= 0)
                GainBonusTraits();
            myYokaiTraits.AddRange(myYokai.myTraits);
            foreach (var item in GameManager.instance.yokai.allYokai)
            {
                if (item.GetComponent<YokaiController>().thisData == myYokai)
                    yokaiSpawn = item;
            }
        }
    }
    public void EnterArea()
    {
        startCase();
        isActive = true;
    }
    public void LeaveArea()
    {
        if (yokaiSpawn != null)
            Destroy(yokaiSpawn);
        isActive = false;
    }
    void FilterYokaiList()
    {
        foreach (var item in GameManager.instance.yokai.allYokai)
        {
            yokaiOptions.Add(item.GetComponent<YokaiController>().thisData);
        }
        foreach (var item in yokaiOptions)
        {
            resTraits.Clear();
            resTraits.AddRange(item.restrictedTraits);
            foreach (var item2 in locationTraits)
            {
                if (resTraits.Contains(item2))
                {
                    yokaiOptions.Remove(item);
                    break;
                }
            }
        }
    }
    void WeighYokaiList()
    {
        foreach (var item in yokaiOptions)
        {
            prefTraits.Clear();
            prefTraits.AddRange(item.preferredTraits);
            foreach (var item2 in locationTraits)
            {
                if (prefTraits.Contains(item2))
                {
                    yokaiWeightedList.Add(item);
                }
            }
        }
    }
    void GainBonusTraits()
    {
        List<personalityTrait> myBonusTraits = new List<personalityTrait>();
        int Random1 = Random.Range(0, (System.Enum.GetValues(typeof(personalityTrait)).Length));
        int Random2 = Random.Range(0, (System.Enum.GetValues(typeof(personalityTrait)).Length));
        if (Random2 == Random1)
            Random2 = Mathf.Abs((Random2 - Random1) + (Random1 - 1));
        if(TryParse((personalityTrait)Random1))
            myBonusTraits.Add((personalityTrait)Random1);
        if (TryParse((personalityTrait)Random2))
            myBonusTraits.Add((personalityTrait)Random2);
        if (myBonusTraits.Count > 0)
            bonusTraitsSave = myBonusTraits.ToArray();
    }
    bool TryParse(personalityTrait check)
    {
        bool isValid = true;
        foreach (var item in myYokai.myTraits)
        {
            if (item == check)
                isValid = false;
        }
        if(isValid)
        {
            switch (check)
            {
                case personalityTrait.Vocal:
                    if (myYokaiTraits.Contains(personalityTrait.Quiet))
                        isValid = false;
                    break;
                case personalityTrait.Quiet:
                    if (myYokaiTraits.Contains(personalityTrait.Vocal))
                        isValid = false;
                    break;
                case personalityTrait.Slow:
                    if (myYokaiTraits.Contains(personalityTrait.Speedy))
                        isValid = false;
                    break;
                case personalityTrait.Speedy:
                    if (myYokaiTraits.Contains(personalityTrait.Slow))
                        isValid = false;
                    break;
                case personalityTrait.Aggressive:
                    if (myYokaiTraits.Contains(personalityTrait.Calm))
                        isValid = false;
                    break;
                case personalityTrait.Calm:
                    if (myYokaiTraits.Contains(personalityTrait.Aggressive))
                        isValid = false;
                    break;
            }
        }
        return isValid;
    }
}