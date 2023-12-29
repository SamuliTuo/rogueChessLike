using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectAndSavePath
{
    public string savePath;
    public GameObject objectPrefab;
    public Sprite image;
}

public class ObjectSavePaths : MonoBehaviour
{
    [Header("Name has to match the unitPrefab's name")]
    public List<ObjectAndSavePath> objectDatas = new List<ObjectAndSavePath>();

    public string GetSavePath(string objectName)
    {
        foreach (var ud in objectDatas)
        {
            if (ud.objectPrefab.name == objectName)
            {
                return ud.savePath + objectName;
            }
        }
        return null;
    }
    public string GetName(string path)
    {
        foreach (var objectPath in objectDatas)
        {
            if (objectPath.savePath == path)
            {
                return objectPath.objectPrefab.name;
            }
        }
        return null;
    }

    public Sprite GetImg(Unit o)
    {
        //find the correct object from spawnableObjects
        foreach (var ud in objectDatas)
        {
            if (ud.objectPrefab == o.gameObject)
            {
                return ud.image;
            }
        }
        return null;
    }
    public Sprite GetImg(string objectName)
    {
        foreach (var ud in objectDatas)
        {
            if (ud.objectPrefab.name == objectName)
            {
                return ud.image;
            }
        }
        return null;
    }

}
