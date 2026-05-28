using System.Collections.Generic;
using UnityEngine;

public class S_PoolManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private S_GridManager2 gridManager;

    private Queue<GameObject> objectPool;
    private Transform poolContainer;

    private void Awake()
    {
        objectPool = new Queue<GameObject>();

        // Crear contenedor para objetos en el pool
        poolContainer = new GameObject("PoolContainer").transform;
        poolContainer.SetParent(transform, false);

        int gridSize = gridManager.GridSize;
        int poolSize = gridSize * gridSize;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            obj.transform.SetParent(poolContainer, false);
            objectPool.Enqueue(obj);
        }
    }

    public S_Tile SpawnFromPool(Transform parent)
    {
        GameObject objectToSpawn = objectPool.Dequeue();
        objectToSpawn.SetActive(true);
        objectToSpawn.transform.SetParent(parent, false);
        return objectToSpawn.GetComponent<S_Tile>();
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolContainer, false);
        objectPool.Enqueue(obj);
    }
}
