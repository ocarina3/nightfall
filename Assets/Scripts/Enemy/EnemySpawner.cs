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
    if (currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0)
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
    yield return new WaitForSeconds(waveInterval);

    if (currentWaveCount < waves.Count - 1)
    {
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
          if (enemiesAlive >= maxEnemiesAllowed)
          {
            maxEnemiesReached = true;
            return;
          }
          Vector2 spawnPosition = new Vector2(player.transform.position.x + Random.Range(-10f, 10f), player.transform.position.y + Random.Range(-10f, 10f));
          Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

          enemyGroup.spawnCount++;
          waves[currentWaveCount].spawnCount++;
          enemiesAlive++;
        }
      }
    }

    if (enemiesAlive < maxEnemiesAllowed)
    {
      maxEnemiesReached = false;
    }

  }

  public void OnEnemyKilled()
  {
    enemiesAlive--;
  }
}
