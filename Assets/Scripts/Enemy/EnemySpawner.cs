using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{

  [System.Serializable]
  public class Wave
  {
    public string waveName;
    public int waveQuota; // Total number of enemies to spawn in this wave
    public List<EnemyGroup> enemyGroups;
    public float spawnInterval; // Interval at which to spawn enemies
    public int spawnCount; // Number of enemies already spawned in this wave 
  }

  [System.Serializable]
  public class EnemyGroup
  {
    public string enemyName;
    public int enemyCount; // number of enemies to spawn in this wave
    public int spawnCount; // The number of enemies of this type already spawned in this wave
    public GameObject enemyPrefab;
  }

  public List<Wave> waves;
  public int currentWaveCount; // index of current wave
  [Header("Spawner Attributes")]
  float spawnTimer;
  public int enemiesAlive;
  public int maxEnemiesAllowed;
  public bool maxEnemiesReached = false;
  public float waveInterval;
  bool isWaveActive = false;

  [Header("Span Positions")]
  public List<Transform> relativeSpawnPoints;

  Transform player;

  // Start is called before the first frame update
  void Start()
  {
    player = FindObjectOfType<PlayerStats>().transform;
    CalculateWaveQuota();
  }

  // Update is called once per frame
  void Update()
  {
    if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
    {
      StartCoroutine(BeginNextWave());
    }

    spawnTimer += Time.deltaTime;
    if (spawnTimer >= waves[currentWaveCount].spawnInterval)
    {
      spawnTimer = 0f;
      SpawnEnemies();
    }
  }

  IEnumerator BeginNextWave()
  {
    isWaveActive = true;

    yield return new WaitForSeconds(waveInterval);

    if (currentWaveCount < waves.Count - 1)
    {
          isWaveActive = false;
      currentWaveCount++;
      CalculateWaveQuota();
    }
  }

  void CalculateWaveQuota()
  {
    int currentWaveQuota = 0;
    foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
    {
      currentWaveQuota += enemyGroup.enemyCount;
    }

    waves[currentWaveCount].waveQuota = currentWaveQuota;
    Debug.LogWarning(currentWaveQuota);
  }

  void SpawnEnemies()
  {
    if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
    {
      foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
      {
        if (enemyGroup.spawnCount < enemyGroup.enemyCount)
        {
          // Spawn the enemy at a random position close to the player
          Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

          enemyGroup.spawnCount++;
          waves[currentWaveCount].spawnCount++;
          enemiesAlive++;

          if (enemiesAlive >= maxEnemiesAllowed)
          {
            maxEnemiesReached = true;
            return;
          }
        }
      }
    }


  }

  public void OnEnemyKilled()
  {
    enemiesAlive--;

    if (enemiesAlive < maxEnemiesAllowed)
    {
      maxEnemiesReached = false;
    }
  }
}
