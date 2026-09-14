using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class PropSpawner : MonoBehaviourPun
{
    [SerializeField] private ObjectPool objectPool;

    [SerializeField] private int amountToSpawn = 30;

    [Header("Zona de spawn")]
    [SerializeField] private Vector2 minSpawn = new Vector2(-8f, -4f);
    [SerializeField] private Vector2 maxSpawn = new Vector2(8f, 4f);

    private Dictionary<int, Prop> spawnedProps =
        new Dictionary<int, Prop>();

    public void SpawnProps()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        int[] prefabIndexes =
            new int[amountToSpawn];

        Vector3[] positions =
            new Vector3[amountToSpawn];

        for (int i = 0; i < amountToSpawn; i++)
        {
            prefabIndexes[i] =
                Random.Range(
                    0,
                    objectPool.PrefabCount
                );

            float randomX =
                Random.Range(
                    minSpawn.x,
                    maxSpawn.x
                );

            float randomY =
                Random.Range(
                    minSpawn.y,
                    maxSpawn.y
                );

            positions[i] =
                new Vector3(
                    randomX,
                    randomY,
                    0f
                );
        }

        photonView.RPC(
            nameof(RPC_SpawnProps),
            RpcTarget.All,
            prefabIndexes,
            positions
        );
    }

    [PunRPC]
    private void RPC_SpawnProps(
        int[] prefabIndexes,
        Vector3[] positions)
    {
        spawnedProps.Clear();

        for (int i = 0; i < prefabIndexes.Length; i++)
        {
            GameObject propObject =
                objectPool.GetObject(
                    prefabIndexes[i]
                );

            if (propObject == null)
                continue;

            propObject.transform.position =
                positions[i];

            propObject.transform.rotation =
                Quaternion.identity;

            Prop prop =
                propObject.GetComponent<Prop>();

            if (prop != null)
            {
                prop.Initialize(i, this);

                spawnedProps.Add(i, prop);
            }

            propObject.SetActive(true);
        }
    }

    public void RequestDestroyProp(int propId)
    {
        photonView.RPC(
            nameof(RPC_DestroyProp),
            RpcTarget.All,
            propId
        );
    }

    [PunRPC]
    private void RPC_DestroyProp(int propId)
    {
        if (!spawnedProps.TryGetValue(
                propId,
                out Prop prop))
            return;

        prop.DestroyPropLocal();
    }

    public void ClearAllProps()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        foreach (int propId in spawnedProps.Keys)
        {
            RequestDestroyProp(propId);
        }
    }
}