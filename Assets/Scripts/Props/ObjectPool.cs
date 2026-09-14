using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private int amountPerPrefab = 5;

    private List<List<GameObject>> pools =
        new List<List<GameObject>>();

    public int PrefabCount => prefabs.Length;

    private void Awake()
    {
        CreatePool();
    }

    private void CreatePool()
    {
        for (int prefabIndex = 0;
             prefabIndex < prefabs.Length;
             prefabIndex++)
        {
            List<GameObject> prefabPool =
                new List<GameObject>();

            for (int i = 0; i < amountPerPrefab; i++)
            {
                GameObject obj =
                    Instantiate(prefabs[prefabIndex]);

                obj.SetActive(false);

                prefabPool.Add(obj);
            }

            pools.Add(prefabPool);
        }
    }

    public GameObject GetObject(int prefabIndex)
    {
        if (prefabIndex < 0 ||
            prefabIndex >= pools.Count)
        {
            Debug.LogError(
                "Índice de prefab inválido: " +
                prefabIndex
            );

            return null;
        }

        foreach (GameObject obj in pools[prefabIndex])
        {
            if (!obj.activeInHierarchy)
            {
                return obj;
            }
        }

        GameObject newObj =
            Instantiate(prefabs[prefabIndex]);

        newObj.SetActive(false);

        pools[prefabIndex].Add(newObj);

        return newObj;
    }
}