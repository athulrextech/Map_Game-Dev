using System.Collections.Generic;

public class MapUtility 
{

    public static string GetPieceId(List<int> ids, int inx, int childInx)
    {
        List<int> idINx = new List<int>();
        for (int i = 0; i <= inx; i++)
        {
            idINx.Add(ids[i]);
        }

        idINx[idINx.Count - 1] = childInx;

        return GetPieceId(idINx);

    }


    public static string GetPieceId(List<int> ids, int inx)
    {
        List<int> idINx = new List<int>();
        for (int i = 0; i <= inx; i++)
        {
            idINx.Add(ids[i]);
        }

        return GetPieceId(idINx);

    }


    public static MapDataScriptableNew GetPiece(string id, MapDataScriptableNew rootData)
    {
        List<int> idList = GetIdIndex(id);
        if (idList == null)
            return null;

        //if(idList.Count == 1 && idList[0] == 0)
        //    return rootData;

        return GetPiece(idList, rootData);
    }

    public static MapDataScriptableNew GetPiece(List<int> idList, MapDataScriptableNew rootData)
    {
        

        MapDataScriptableNew piece = rootData;
        
        if(idList == null) return piece;

        for (int i = 0; i < idList.Count; i++)
        {
            if (piece.ChildData.Count <= idList[i])
                throw new System.Exception("Piece Dose Not Exist");

            piece = piece.ChildData[idList[i]];

        }

        return piece;
    }


    public static  List<int> GetIdIndex(string id)
    {
        List<int> idList = new List<int>();

        string s = "";

        if (string.IsNullOrEmpty(id))
            return null;

        for (int i = 0; i < id.Length; i++)
        {
            if (char.IsNumber(id[i]) && id[i] != '.')
            {
                s += id[i];
            }
            else if (id[i] == '.')
            {
                idList.Add(int.Parse(s));
                s = "";
            }
            else
            {
                throw new System.Exception("Wrong Id Input");
            }

            if (i == id.Length - 1)
                idList.Add(int.Parse(s));

        }
        return idList;
    }


    public static  string GetPieceId(List<int> idList)
    {
        string id = "";
        for (int i = 0; i < idList.Count; i++)
        {
            id += idList[i];
            if (i != idList.Count - 1)
                id += ".";
        }
        return id;
    }
}
