using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{

    public List<Transform> spawnLocations;
    private int currentSpawnerIndex = 0;

    private void Awake()    
    {
        currentSpawnerIndex = Random.Range(0, spawnLocations.Count);

        DontDestroyOnLoad(this.gameObject);
    }

    public Transform NextSpawnTransform()
    {
        return spawnLocations[currentSpawnerIndex < spawnLocations.Count ? currentSpawnerIndex++ : 0];
    }
}
