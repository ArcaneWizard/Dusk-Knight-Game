using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private List<GameObject> R1 = new List<GameObject>();
    private List<GameObject> R2 = new List<GameObject>();
    private List<GameObject> R3 = new List<GameObject>();
    private List<GameObject> Orc = new List<GameObject>();
    private List<GameObject> Goblin = new List<GameObject>();
    private List<GameObject> Ogre = new List<GameObject>();

    [Space(10)]
    [Header("Enemy types")]
    public GameObject R1_group;
    public GameObject R2_group;
    public GameObject R3_group;
    public GameObject Orc_group;
    public GameObject Goblin_group;
    public GameObject Ogre_group;

    [Space(10)]
    [Header("Spawn points")]
    public Transform GRSpawn;
    public Transform GLSpawn;

    private int cR1 = 0;
    private int cR2 = 0;
    private int cR3 = 0;
    private int cOgre = 0;
    private int cOrc = 0;
    private int cGoblin = 0;

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

        //Reset enemy lists based off the hierarchy 
        prepList(R1_group, R1);
        prepList(R2_group, R2);
        prepList(R3_group, R3);
        prepList(Orc_group, Orc);
        prepList(Goblin_group, Goblin);
        prepList(Ogre_group, Ogre);

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
        int r = UnityEngine.Random.Range(0, 6);
        if (r == 0)
            deployEnemy("R1");
        if (r == 1)
            deployEnemy("R3");
        if (r == 2)
            deployEnemy("Goblin");
        if (r == 3)
            deployEnemy("Orc");
        if (r == 4)
            deployEnemy("Goblin");
        if (r == 5)
            deployEnemy("Orc");
    }

    //Deploying enemy shortened to one method/line
    void deployEnemy(string enemyName)
    {
        //get enemy
        GameObject enemy = findEnemy(enemyName);
        Vector3 deployPos;

        //choose enemy spawn point randomly
        int r = UnityEngine.Random.Range(0, 2);
        if (r == 0)
            deployPos = GameObject.Find("BL Spawn Point").transform.position;
        else
            deployPos = GameObject.Find("BR Spawn Point").transform.position;

        //choose reaper 1 spawn point differently 
        if (enemyName == "R1")
        {
            float b = UnityEngine.Random.Range(GRSpawn.position.x, GLSpawn.position.x);
            deployPos = new Vector3(b, GLSpawn.position.y, GLSpawn.position.z);
        }

        //spawn enemy
        enemy.transform.position = deployPos;
        enemy.SetActive(true);   

        //reset enemy settings
        if (enemy.transform.GetComponent<Enemy_Health>() != null) {
            enemy.transform.GetComponent<Enemy_Health>().deploy = true;
            enemy.transform.GetComponent<Enemy_Health>().setHP();
        }     
    }

    //Find avaliable enemy + update its array cycle
    GameObject findEnemy(string enemyName)
    {        
        if (enemyName == "R1")
        {
            cR1 += 1;
            cR1 %= R1_group.transform.childCount;
            return findEnemy2(enemyName, cR1);
        }

        else if (enemyName == "R2")
        {
            cR2 += 1;
            cR2 %= R2_group.transform.childCount;
            return findEnemy2(enemyName, cR2);
        }

        else if (enemyName == "R3")
        {
            cR3 += 1;
            cR3 %= R3_group.transform.childCount;
            return findEnemy2(enemyName, cR3);
        }

        else if(enemyName == "Orc")
        {
            cOrc += 1;
            cOrc %= Orc_group.transform.childCount;
            return findEnemy2(enemyName, cOrc);
        }

        else if(enemyName == "Ogre")
        {
            cOgre += 1;
            cOgre %= Ogre_group.transform.childCount;
            return findEnemy2(enemyName, cOgre);
        }

        else if (enemyName == "Goblin")
        {
            cGoblin += 1;
            cGoblin %= Goblin_group.transform.childCount;
            return findEnemy2(enemyName, cGoblin);
        }

        else
            return GameObject.Find("Fake Enemy");
    }

    //Error check: Can't spawn more of one enemy if all are already active
    GameObject findEnemy2(string enemyName, int cycle)
    {        
        if (GameObject.Find(enemyName + " Group").transform.GetChild(cycle).gameObject.activeSelf == false)
            return GameObject.Find(enemyName + " Group").transform.GetChild(cycle).gameObject;

        else
            return GameObject.Find("Fake Enemy");
    }

    //Add every enemy in the hierarchy to its enemy cycle array
    void prepList(GameObject enemyType, List<GameObject> enemyList)
    {
        for (int i = 1; i <= enemyType.transform.childCount; i++)
            enemyList.Add(enemyType.transform.GetChild(i - 1).gameObject);
    }

    //Set every enemy at a different order to avoid overlap problems
    void assignEnemyOrder() {
        enemyOrderFormat(R1_group, 10);
        enemyOrderFormat(R2_group, 20);
        enemyOrderFormat(R3_group, 30);
        enemyOrderFormat(Orc_group, 40);
        enemyOrderFormat(Ogre_group, 50);
        enemyOrderFormat(Goblin_group, 60);
    }

    void enemyOrderFormat(GameObject group, int layerOffset) {
        for (int i = 1; i <= group.transform.childCount; i++)
        {
            group.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
            group.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i - 1 + layerOffset;
        }
    }

    void Update() {
        //print the frame rate in the console
        Debug.Log(1/Time.deltaTime);
    }
}
