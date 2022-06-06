using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class YokaiController : MonoBehaviour
{
    public YokaiData thisData;
    public personalityTrait[] randomTraits;
    NavMeshAgent agent;
    public float AggressionMeter;
    private void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }
}