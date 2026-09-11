using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int amountPerPrefab = 5;

    private List<GameObject> pooledObjects = new List<GameObject>();

    private void Start()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        foreach (GameObject prefab in prefabs)
        {
            for (int i = 0; i < amountPerPrefab; i++)
            {
                GameObject obj = Instantiate(prefab);

                obj.SetActive(false);

                pooledObjects.Add(obj);
            }
        }
    }

    public GameObject GetRandomObject()
    {
        List<GameObject> availableObjects = new List<GameObject>();

        foreach (GameObject obj in pooledObjects)
        {
            if (!obj.activeInHierarchy)
            {
                availableObjects.Add(obj);
            }
        }

        if (availableObjects.Count == 0)
        {
            Debug.LogWarning("pool vacia");
            return null;
        }

        int randomIndex = Random.Range(0, availableObjects.Count);

        return availableObjects[randomIndex];
    }
}