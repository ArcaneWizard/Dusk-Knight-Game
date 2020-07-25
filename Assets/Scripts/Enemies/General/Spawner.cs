using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Space(10)]
    [Header("Enemy types")]
    public GameObject E1_group;
    public GameObject E2_group;
    public GameObject E3_group;
    public GameObject E4_group;
    public GameObject E5_group;

    [Space(10)]
    [Header("Spawn points")]
    public Transform E1_sp;
    public Transform E2_sp;
    public Transform E3_sp;
    public Transform E4_sp;
    public Transform E5_sp;
    public Transform FlyingUp_sp;
    public Transform FlyingDown_sp;
    public Transform Hill;

    private int cE1 = 0;
    private int cE2 = 0;
    private int cE3 = 0;
    private int cE4 = 0;
    private int cE5 = 0;

    [Space(10)]
    [Header("Spawn settings")]
    public float initialReloadTime;
    private float reloadTime = 1.5f;
    private float enemiesSpawned;
    private float enemyLimit;

    void Start()
    {
        //Set enemy wave settings
        enemiesSpawned = 0;
        reloadTime = initialReloadTime;
        enemyLimit = UnityEngine.Random.Range(7, 15);

        //Start enemy spawning and enemy spawn rate methods
        StartCoroutine(spawnOver());
        StartCoroutine(quickenSpawn());

        //Ensures enemies won't face overlap problems
        assignEnemyOrder();
    }

    //spawn enemies every reloadTime seconds
    private IEnumerator spawnOver()
    {
        //spawn a random enemy
        deployRandomEnemy();
        enemiesSpawned++;

        //once many enemies have been spawned, temporarily stop spawning
        if (enemiesSpawned == enemyLimit) {
            enemiesSpawned = 0;
            reloadTime = initialReloadTime;
            enemyLimit = UnityEngine.Random.Range(7, 15);
            yield return new WaitForSeconds(5f);
        } 
        //otherwise just take a few seconds before spawning a new enemy
        else 
            yield return new WaitForSeconds(reloadTime);  
        
        StartCoroutine(spawnOver());
    }   

    //spawn Enemies faster over the course of the game
    private IEnumerator  quickenSpawn()
    {
        yield return new WaitForSeconds(1);
        if (reloadTime >= 0.75)
            reloadTime -= 0.005f;

        StartCoroutine(quickenSpawn());
    }

    //Deploy a random enemy
    void deployRandomEnemy()
    {
        int r = UnityEngine.Random.Range(1, 26);
        if (r >= 1 && r <= 5)
            deployEnemy("Enemy 1");
        if (r >= 6 && r <= 10)
            deployEnemy("Enemy 2");
        if (r >= 11 && r <= 15)
            deployEnemy("Enemy 3");
        if (r >= 16 && r <= 20)
            deployEnemy("Enemy 4");
        if (r >= 21 && r <= 25)
            deployEnemy("Enemy 5");
    }

    //Deploying enemy shortened to one method/line
    void deployEnemy(string enemyName)
    {
        //get enemy
        GameObject enemy = findAndCycleEnemy(enemyName);
        Vector3 deployPos = new Vector3(1, 1, 1);

        //set enemy spawn points
        if (enemyName == "Enemy 1")
            deployPos = E1_sp.transform.position;

        if (enemyName == "Enemy 2") 
            deployPos = E2_sp.transform.position;

        if (enemyName == "Enemy 5") 
            deployPos = E5_sp.transform.position;

        //choose exploding reaper spawn point  
        if (enemyName == "Enemy 4") {
            Vector3 r = Hill.GetChild(Random.Range(1, 11)).transform.position;
            deployPos = new Vector3(r.x, r.y+0.6f, 0);  
        }

        //choose flying reaper spawn point 
        if (enemyName == "Enemy 3") {
            deployPos = new Vector3(FlyingUp_sp.transform.position.x, 
            Random.Range(FlyingDown_sp.transform.position.y, FlyingUp_sp.transform.position.y), 0);
        }

        //spawn enemy
        enemy.transform.position = deployPos;
        enemy.SetActive(true);   

        //reset enemy settings
        if (enemy.transform.GetComponent<Enemy_Health>() != null) {
            enemy.transform.GetComponent<Enemy_Health>().setStats();
            enemy.transform.GetComponent<Enemy_Health>().deploy = true;
        }     
    }

    //Find avaliable enemy + update its array cycle
    GameObject findAndCycleEnemy(string enemyName)
    {        
        if (enemyName == "Enemy 1")
        {
            cE1 += 1;
            cE1 %= E1_group.transform.childCount;
            return spawnEnemyIfAvailable(E1_group, cE1);
        }

        else if (enemyName == "Enemy 2")
        {
            cE2 += 1;
            cE2 %= E2_group.transform.childCount;
            return spawnEnemyIfAvailable(E2_group, cE2);
        }

        else if (enemyName == "Enemy 3")
        {
            cE3 += 1;
            cE3 %= E3_group.transform.childCount;
            return spawnEnemyIfAvailable(E3_group, cE3);
        }

        else if(enemyName == "Enemy 4")
        {
            cE4 += 1;
            cE4 %= E4_group.transform.childCount;
            return spawnEnemyIfAvailable(E4_group, cE4);
        }

        else if(enemyName == "Enemy 5")
        {
            cE5 += 1;
            cE5 %= E5_group.transform.childCount;
            return spawnEnemyIfAvailable(E5_group, cE5);
        }

        else
            return GameObject.Find("Fake Enemy");
    }

    //Spawn enemy if it hasn't already been spawned
    GameObject spawnEnemyIfAvailable(GameObject enemyGroup, int cycle)
    {        
        if (enemyGroup.transform.GetChild(cycle).gameObject.activeSelf == false)
            return enemyGroup.transform.GetChild(cycle).gameObject;

        else
            return GameObject.Find("Fake Enemy");
    }

    //Set every enemy at a different order to avoid overlap problems
    void assignEnemyOrder() {
        enemyOrderFormat(E1_group, 10);
        enemyOrderFormat(E2_group, 20);
        enemyOrderFormat(E3_group, 30);
        enemyOrderFormat(E4_group, 40);
        enemyOrderFormat(E5_group, 50);
    }

    //Correctly order the enemies (which one comes on top)
    void enemyOrderFormat(GameObject enemyGroup, int layerOffset) {
        for (int i = 1; i <= enemyGroup.transform.childCount; i++)
        {
            //set diff enemies' order in terms of which sprites come on top
            enemyGroup.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
            enemyGroup.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i - 1 + layerOffset;
            
            //set order of enemies' fire sprites when they are lit on fire
            enemyGroup.transform.GetChild(i - 1).transform.GetChild(2).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
            enemyGroup.transform.GetChild(i - 1).transform.GetChild(2).transform.GetComponent<SpriteRenderer>().sortingOrder = i + 99 + layerOffset;
        }
    }

    void Update() {
        //print the frame rate in the console
        //Debug.Log(1/Time.deltaTime);
    }
}
