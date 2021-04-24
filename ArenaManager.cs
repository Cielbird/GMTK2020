using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    public bool spawnEnemies;
    public float arenaRadius;
    public int currentLevel;
    //0: rolling ball
    public int[] levelEnemies;

    public GameObject[] enemyPrefabs;
    public UIManager ui;
    

    public IEnumerator SpawnEnemies()
    {
        while (true)
        {
            ui.updateLevel(currentLevel + 1);
            yield return new WaitForSeconds(10);

            levelEnemies = new int[currentLevel / 5 + 1];
            if (currentLevel > enemyPrefabs.Length-1)
            {
                for (int i = 0; i < levelEnemies.Length; i++)
                {
                    levelEnemies[i] = Random.Range(0, enemyPrefabs.Length);
                }
            }
            else
            {
                levelEnemies[0] = currentLevel;
            }

            float angle;
            float angleOffset = Random.Range(-Mathf.PI, Mathf.PI);
            int enemyIndex;
            for (int i = 0; i < levelEnemies.Length; i++)
            {
                angle = Mathf.PI * 2 / levelEnemies.Length * i + angleOffset;
                enemyIndex = levelEnemies[i];
                Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * arenaRadius;
                RaycastHit spawnRayHit;
                if (!Physics.Raycast(spawnPos, Vector3.down, out spawnRayHit))
                    throw new System.Exception("Spawn not possible! bruh bigh bruh problemo!");

                Instantiate(enemyPrefabs[enemyIndex], spawnRayHit.point, Quaternion.identity);
            }
            while (!isEveryoneDead())
            {
                yield return new WaitForSeconds(1);
            }
            currentLevel++;
        }
    }
    
    bool isEveryoneDead()
    {
        int enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        return enemyCount == 0;
    }

    public void endGame()
    {
        StopAllCoroutines();
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript.hp > 0)
            {
                enemyScript.agent.enabled = false;
                enemyScript.StopAllCoroutines();
            }
        }
    }
}
