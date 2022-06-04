using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "errorDatabase", menuName = "createErrorDatabase", order = 0)]
public class ErrorManager : ScriptableObject
{
    public ErrorData[] errorCodes;
}