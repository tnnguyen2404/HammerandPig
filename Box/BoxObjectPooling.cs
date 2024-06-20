using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxObjectPooling : MonoBehaviour
{
    public static BoxObjectPooling SharedInstance;
    public List<GameObject> pooledObject;
    public GameObject objectToPool;
    public int amountToPool;

    private void Awake() 
    {
        SharedInstance = this;
    }

    void Start() 
    {
        pooledObject = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++) {
            tmp = Instantiate(objectToPool);
            tmp.SetActive(false);
            pooledObject.Add(tmp);
        }
    }

    public GameObject GetPooledObject() 
    {
        for (int i = 0; i < amountToPool; i++) {
            if(!pooledObject[i].activeInHierarchy) {
                pooledObject[i].SetActive(true);
                return pooledObject[i];
            }
        }
        return null;
    }
}
