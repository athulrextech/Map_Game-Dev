using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Data", menuName = "GameData/Data")]
public class GameDataScriptable : ScriptableObject
{
    public string ParentId;
    public int OutOfPlacePieceCont;
}
