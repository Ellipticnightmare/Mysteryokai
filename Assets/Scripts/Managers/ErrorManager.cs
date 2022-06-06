using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "errorDatabase", menuName = "databases/createErrorDatabase", order = 0)]
public class ErrorManager : ScriptableObject
{
    public ErrorData[] errorCodes;
}