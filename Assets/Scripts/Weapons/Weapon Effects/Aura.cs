using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{

    Dictionary<EnemyStats, float> affectedTargets = new Dictionary<EnemyStats, float>();
    List<EnemyStats> targetsToUnaffect = new List<EnemyStats>();

    // Update is called once per frame
    void Update()
    {
        Dictionary<EnemyStats, float> affectedTargsCopy = new Dictionary<EnemyStats, float>(affectedTargets);

        // Loop through every target affected by the aura, and reduce the cooldown
        // of the aura for it. If the cooldown reaches 0, deal damage to it.
        foreach (KeyValuePair<EnemyStats, float> pair in affectedTargsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if (pair.Value <= 0)
            {
                if (targetsToUnaffect.Contains(pair.Key))
                {
                    // If the target is marked for removal, remove it.
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    // Reset the cooldown and deal damage.
                    Weapon.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position, stats.knockback);
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
{
    if (other.TryGetComponent(out EnemyStats es))
    {
        Debug.Log("No Hit");
        // If the target is not yet affected by this aura,
        // add it to our list of affected targets
        if (!affectedTargets.ContainsKey(es))
        {
            Weapon.Stats stats = weapon.GetStats();
            es.TakeDamage(GetDamage(), transform.position, stats.knockback);
            Debug.Log("Hit! Number of affected targets: " + affectedTargets.Count);
            // Always starts with an interval of 0,
            // so that'll get damaged in the next Update() tick
            affectedTargets.Add(es, 0);
        } else
        {
            Debug.Log("Refresh hit list");
            if (targetsToUnaffect.Contains(es)) targetsToUnaffect.Remove(es);
        }
    }
}

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out EnemyStats es))
        {
            // Do not directly remove the target upon leaving,
            // because we still have to track their cooldowns.
            if (affectedTargets.ContainsKey(es))
            {
                targetsToUnaffect.Add(es);
            }
        }
    }
}