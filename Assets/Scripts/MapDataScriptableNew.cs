using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Map Data New", menuName = "Map Assets/Map Data New")]
public class MapDataScriptableNew : ScriptableObject
{
    public MapDataScriptableNew ParentData;
    public string Id;
    public string Name;
    public Vector2 Position;
    public float YRotation;
    public float Scale;
    public Texture2D ParentTexture;
    public Texture2D ChildTexture;
    public List<MapDataScriptableNew> ChildData;
    public bool notPlceable;
}

