using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class Spawner : MonoBehaviour
{
    [SerializeField] Diction<int, GameObject> prefabs;

    private Dictionary<int, GameObject> dictionaryPrefabs;
    private Dictionary<int, List<GameObject>> allSpawnObjects = new Dictionary<int, List<GameObject>>();


    private void Awake()
    {
        
        dictionaryPrefabs = prefabs.returnDictionary();
        foreach (var item in prefabs.items)
        {
            allSpawnObjects.Add(item.key, new List<GameObject>());
        }

    }

    public GameObject Spawn(int ID, Vector3 position)
    {
        //todo ÍÀéòè ëó÷øå find
        GameObject item = allSpawnObjects[ID].Find((value) => !value.activeSelf);
        if (item != null)
        {
            item.SetActive(true);
            item.transform.position = position;
            return item;
        }
        else
        {
            item = Instantiate(dictionaryPrefabs[ID], position,new Quaternion());
            allSpawnObjects[ID].Add(item);
            return item;
        }
    }

    public GameObject this[int ID]
    {
        get { return dictionaryPrefabs[ID]; }
    }

    public void Destroy(GameObject gameObject)
    {
        gameObject.SetActive(false);
        //Path.Combine();
        
    }

    public void DestroyGameObject(GameObject gameObject)
    {
        foreach (var item in allSpawnObjects)
        {
            item.Value.Remove(gameObject);
            Destroy(gameObject);
        }
    }
}

[Serializable]
public struct Diction<Tkey, Tvalue>
{
    public List<DictionItem<Tkey, Tvalue>> items;
    

    
    public Dictionary<Tkey,Tvalue> returnDictionary()
    {
        Dictionary<Tkey, Tvalue> value = new Dictionary<Tkey,Tvalue>();
        foreach (var item in items)
        {
            value.Add(item.key, item.value);
        }
        return value;
    }
}

[Serializable]
public struct DictionItem<Tkey,Tvalue>
{
    public Tkey key;
    public Tvalue value;
}
