using UnityEngine;

[CreateAssetMenu(fileName = "newYokaiData", menuName = "Yokai Data", order = 0)]
public class YokaiData : ScriptableObject
{
    public string yokaiName;
    public locationTrait[] preferredTraits;
    public locationTrait[] restrictedTraits;
    public personalityTrait[] myTraits;
}