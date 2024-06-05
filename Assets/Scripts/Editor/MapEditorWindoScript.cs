using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MapEditorWindoScript : EditorWindow
{

    private string _dataLocation = "MapData";
    private MapDataScriptableNew _root;
    private Texture2D _parenTexture;
    private string _parentID = "";
    private List<Texture2D> _childTextures = new List<Texture2D>() ;
    private Vector2 _scrollPosition;


    private GameObject _piecePrefab = null;


    private PieceObject _parentObj;
    private PieceObject[] _childObjs;

    private MapDataScriptableNew _parentPiece;



    [MenuItem("Custom/MapEditor")]
    public static void ShowWindow()
    {
        GetWindow<MapEditorWindoScript>("Map Editor");
    }


    private void OnEnable()
    {
        if(_parenTexture == null)
            _parenTexture = Texture2D.normalTexture;
    }


    private void CreateOrLoadRoot()
    {
        string resourcesFolderPath = "Assets/Resources";
        if (!AssetDatabase.IsValidFolder(resourcesFolderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        
        string path = System.IO.Path.Combine(resourcesFolderPath, _dataLocation, "Root.Asset");


        if(System.IO.File.Exists(path))
        {
            string resourcePath = Path.Combine(_dataLocation, "Root");
            _root = Resources.Load<MapDataScriptableNew>(resourcePath);
            if (_root != null)
            {
                // Access and use the loaded ScriptableObject
                // Example: loadedScriptableObject.SomeFunction();
                Debug.Log("ScriptableObject Loaded: " + _root.name);
            }
            else
            {
                Debug.LogError("Failed to load ScriptableObject from Resources: " + resourcePath);
            }
        }
        else
        {
            _root = CreateInstance<MapDataScriptableNew>();
            _root.Name = "Root";
            _root.Id = "";
            _root.Scale = 1;
            // Save the ScriptableObject asset to the Resources folder
            AssetDatabase.CreateAsset(_root, path);
            EditorUtility.SetDirty(_root);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Select the newly created asset in the Project window
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = _root;
        }

        

    }


    private MapDataScriptableNew CreateAsset(MapDataScriptableNew mapDataAsset, string assetName)
    {
        string resourcesFolderPath = "Assets/Resources";
        string directory = Path.Combine(resourcesFolderPath, _dataLocation);
        List<int> idIndex = MapUtility.GetIdIndex(mapDataAsset.Id);
        if (idIndex != null)
        {
            for (int i = 0; i < idIndex.Count - 1; i++)
            {
                directory += "/" + idIndex[i];
            }
        }
        if(!Directory.Exists(directory)) 
        {
            Directory.CreateDirectory(directory);
        }

        string path = Path.Combine(directory, assetName);
        mapDataAsset.Name = assetName;
        MapDataScriptableNew loadedAsset = Resources.Load<MapDataScriptableNew>(path);

        if (loadedAsset != null)
        {
            return loadedAsset; 
        }
        // Save the ScriptableObject asset to the Resources folder
        path += ".asset";

        Debug.Log(mapDataAsset.Name +"   "+ path);
        AssetDatabase.CreateAsset(mapDataAsset, path);
        EditorUtility.SetDirty(mapDataAsset);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Select the newly created asset in the Project window
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = mapDataAsset;
        AssetDatabase.SaveAssetIfDirty(mapDataAsset);
        return mapDataAsset;
    }


    private void OnGUI()
    {
        _dataLocation = EditorGUILayout.TextField("Data Location", _dataLocation);
        
        if(_root == null) 
            CreateOrLoadRoot();

        _parenTexture = EditorGUILayout.ObjectField("Parent Texture",_parenTexture, _parenTexture.GetType(), false) as Texture2D;
        _parentID = EditorGUILayout.TextField("Parent ID", _parentID);
        _piecePrefab = EditorGUILayout.ObjectField("Piece Prefab", _piecePrefab, typeof(GameObject), false) as GameObject;

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Child Textures", EditorStyles.boldLabel);

        if(GUILayout.Button("Refresh"))
        {
            if(string.IsNullOrEmpty(_parentID)) 
            {
                _parentID = 0.ToString();
                _parentPiece = _root;
            }
            else
            {
                _parentPiece = MapUtility.GetPiece(_parentID, _root);
                Debug.Log(_parentPiece.Name);
                _parenTexture = _parentPiece.ParentTexture;
                _childTextures.Clear();
                for(int i = 0;  i < _parentPiece.ChildData.Count; i++)
                {
           
                    _childTextures.Add(_parentPiece.ChildData[i].ChildTexture);
                    
                }
            }


        }
        

        if (GUILayout.Button("Clear"))
        {
            _childTextures.Clear();
        }

        CreateChildDropArea();


        if (GUILayout.Button("Save Data"))
        {
            CreateOrLoadData();
        }

        GUILayout.Space(20);

        if (GUILayout.Button("Load Map"))
        {
            LoadMapObjects(_parentID);
        }

        if(GUILayout.Button("Update Child Positions"))
        {
            SaveChildPositions();
        }

    }

    private void CreateOrLoadData()
    {
        MapDataScriptableNew parentPiece = _root;
        List<int> parentIdIndex;
        List<int> childId = new List<int>();
        if (_parentID != "")
        {

            parentIdIndex = MapUtility.GetIdIndex(_parentID);
            List<int> currentParentID = new List<int>();
            for (int idInx = 0; idInx < parentIdIndex.Count; idInx++)
            {

                currentParentID.Add(parentIdIndex[idInx]);

                if (parentPiece.ChildData == null)
                    parentPiece.ChildData = new List<MapDataScriptableNew>();

                if (parentPiece.ChildData.Count <= parentIdIndex[idInx])
                {
                    for (int i = parentPiece.ChildData.Count; i < parentIdIndex[idInx] + 1; i++)
                    {
                        string pieceID = MapUtility.GetPieceId(currentParentID, idInx, i);
                        MapDataScriptableNew childPiece = CreateInstance<MapDataScriptableNew>();
                        string pieceName = "DefaultDataObject";
                        childPiece.Name = pieceID;
                        childPiece.ParentData = parentPiece;
                        childPiece.Scale = 1;
                        parentPiece.ChildData.Add(CreateOrUpdateAsset(childPiece, pieceName));
                        EditorUtility.SetDirty(parentPiece);
                        EditorUtility.SetDirty(childPiece);
                    }

                }


                MapDataScriptableNew newParentPiece = MapUtility.GetPiece(currentParentID, _root);

                Debug.Log("Root Name : " + _root.Name);
                string parentId = "";
                for (int i = 0; i < currentParentID.Count; i++)
                {
                    parentId += currentParentID[i];
                    if (i != currentParentID.Count - 1)
                        parentId += ".";
                }

                Debug.Log("Parent Piece Name : " + newParentPiece.Name +"  Id: "+ parentId);
                newParentPiece = CreateOrUpdateAsset(newParentPiece, _parenTexture.name);
                parentPiece.ChildData[idInx] = newParentPiece;
                parentPiece = newParentPiece;
                EditorUtility.SetDirty(newParentPiece);
               
                // if Child count exeeds the sprite cound add child to populate Data
            }

            if(_parenTexture != null) 
            {
                parentPiece.Name = _parenTexture.name;
                parentPiece.Id = _parentID;
                parentPiece.ParentTexture = _parenTexture;
                parentPiece.Scale = 1;
            }
            
            if(parentPiece.ChildData != null)
            {
                foreach(var childData in parentPiece.ChildData)
                {
                    AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(childData));
                }  
            }

            parentPiece.ChildData = new List<MapDataScriptableNew>();

            childId.AddRange(parentIdIndex);
            childId.Add(0);

            for (int childInx = 0; childInx < _childTextures.Count; childInx++)
            {
                childId[childId.Count - 1] = childInx;
                // initilize childe Pieces according to the sprite provided

                MapDataScriptableNew childPiece = CreateInstance<MapDataScriptableNew>();
                childPiece.Id = MapUtility.GetPieceId(childId);
                parentPiece.ChildData.Add(CreateOrUpdateAsset(childPiece, _childTextures[childInx].name));
                // Create ID for each child and populate sprites
                parentPiece.ChildData[childInx].ChildTexture = _childTextures[childInx];
                parentPiece.ChildData[childInx].Name = _childTextures[childInx].name;
                parentPiece.ChildData[childInx].Scale = 1;
                EditorUtility.SetDirty(childPiece);
            }

            if (parentPiece == null)
                parentPiece = _root;
            EditorUtility.SetDirty(parentPiece);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
        }
    }


    private MapDataScriptableNew CreateOrUpdateAsset(MapDataScriptableNew asset, string assetName)
    {
        string assetLocation = AssetDatabase.GetAssetPath(asset);
        Debug.Log(assetName + "  " + assetLocation);

        if(!string.IsNullOrEmpty( assetLocation)) 
        {
            return RenameAsset(asset, assetName);
            //return asset;
            // AssetDatabase.DeleteAsset(assetLocation);

            //var preAsset = Resources.Load<MapDataScriptableNew>(assetLocation); 


            //EditorUtility.SetDirty(asset);
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
            //return asset;
        }


        return CreateAsset(asset, assetName);
    }



    private MapDataScriptableNew RenameAsset(MapDataScriptableNew asset, string newName)
    {
        if(asset.name ==  newName) return asset;

        string path = AssetDatabase.GetAssetPath(asset);
        string fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(path);

        if (string.IsNullOrEmpty(path))
            return null;

        Debug.Log($"Asset Path : {path}, new file name: {newName} ");
        //string newFilePath = path.Replace(fileNameWithoutExtension, newName);


        string newFilePath = AssetDatabase.RenameAsset(path, newName);

        ////MapDataScriptableNew sc = Resources.Load<MapDataScriptableNew>(path);

        //// Create a copy of the ScriptableObject with the new file path
        //MapDataScriptableNew newAsset = CreateInstance<MapDataScriptableNew>();

        //AssetDatabase.CreateAsset(newAsset, newFilePath);
        //newAsset.ParentData = asset.ParentData;
        //newAsset.ChildData = asset.ChildData;
        //newAsset.Id = asset.Id;
        //newAsset.Name = newName;
        //newAsset.ChildTexture = asset.ChildTexture;
        //newAsset.Position = asset.Position;
        //newAsset.YRotation = asset.YRotation;
        //newAsset.Scale = asset.Scale;

        //EditorUtility.SetDirty(newAsset);

        //// Delete the original ScriptableObject
        //if (fileNameWithoutExtension != "DefaultDataObject")
        //    AssetDatabase.DeleteAsset(path);
        //AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(asset));


        //// Save and refresh the AssetDatabase
        //AssetDatabase.SaveAssetIfDirty(newAsset);

        AssetDatabase.SaveAssetIfDirty(asset);

        Debug.Log("ScriptableObject renamed successfully.");


        return asset;
    }


    private void CreateChildDropArea()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
        for (int i = 0; i < _childTextures.Count; i++)
        {

            EditorGUILayout.BeginHorizontal();

            GUILayout.Label(_childTextures[i], GUILayout.Width(50), GUILayout.Height(50));
            GUILayout.Label(_childTextures[i].name);

            if (GUILayout.Button("Remove"))
            {
                _childTextures.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space(10);

        Event currentEvent = Event.current;

        if (currentEvent.type == EventType.DragUpdated || currentEvent.type == EventType.DragPerform)
        {
            if (DragAndDrop.objectReferences.Length > 0)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (currentEvent.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (UnityEngine.Object draggedObject in DragAndDrop.objectReferences)
                    {
                        Texture2D draggedSprite = draggedObject as Texture2D;

                        if (draggedSprite != null && !_childTextures.Contains(draggedSprite))
                        {
                            _childTextures.Add(draggedSprite);
                        }
                    }
                }

                Event.current.Use();
            }
        }
    }


    public void LoadMapObjects(string parentId)
    {
        if (_parentObj != null)
            Destroy(_parentObj.gameObject);
        if (_childObjs != null)
        {
            for (int chInx = 0; chInx < _childObjs.Length; chInx++)
            {
                if (_childObjs[chInx] != null)
                    Destroy(_childObjs[chInx].gameObject);
            }
        }

        _parentPiece = MapUtility.GetPiece(parentId, _root);



        GameObject obj = Instantiate(_piecePrefab, Vector3.zero, Quaternion.identity);
        _parentObj = obj.GetComponent<PieceObject>();
        _parentObj.SetPieceData(_parentPiece, true);
        _childObjs = new PieceObject[_parentPiece.ChildData.Count];
        for (int chInx = 0; chInx < _childObjs.Length; chInx++)
        {
            GameObject childObj = Instantiate(_piecePrefab, Vector3.zero, Quaternion.identity);
            _childObjs[chInx] = childObj.GetComponent<PieceObject>();
            _childObjs[chInx].SetPieceData(_parentPiece.ChildData[chInx], false);
            _childObjs[chInx].transform.parent = _parentObj.transform;
            _childObjs[chInx].UpdateLocalPosition();
        }

        

    }


    [ContextMenu("Save Child Positions")]
    public void SaveChildPositions()
    {
        if (_childObjs != null)
        {
            for (int chInx = 0; chInx < _childObjs.Length; chInx++)
            {
                _parentPiece.ChildData[chInx].Position = _childObjs[chInx].transform.localPosition;
                EditorUtility.SetDirty(_parentPiece.ChildData[chInx]);
            }
        }
        else
        {
            //PieceObject parentObject = transform.GetComponentInChildren<PieceObject>();
            if (_parentObj != null)
            {
                PieceObject[] childObjects = _parentObj.GetComponentsInChildren<PieceObject>();
                for (int i = 0; i < childObjects.Length; i++)
                {
                    MapDataScriptableNew piece = MapUtility.GetPiece(childObjects[i].Id, _root);
                    piece.Position = childObjects[i].transform.localPosition;
                    EditorUtility.SetDirty(piece);
                }
            }
        }
        AssetDatabase.SaveAssets();
    }



}


