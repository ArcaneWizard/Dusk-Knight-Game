using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    [Space(10)]
    [Header("Spawn settings")]
    public float roughSpawnRate;
    public float spawnRateFall = 0.2f;
    public float wavesBeforeLast = 1;
    public int diffEnemyTypes;

    public List<Vector2> enemiesSpawnedPerWave;
    public List<float> timeBetweenWaves;

    [Space(10)]
    [Header("READ ONLY")]
    public float reloadTime = 1.5f;
    public float btwnReloadTime = 2.4f;
    private bool waveOver;
    public int enemiesSpawned;
    public int enemyLimit;
    public int cycle = 0;

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

    public Text playerText;
    public float textBlinkRate = 0.2f;

    void Start()
    {
        //Set enemy wave settings
        cycle = 0;
        enemiesSpawned = 0;
        reloadTime = roughSpawnRate;
        enemyLimit = Mathf.RoundToInt(UnityEngine.Random.Range(enemiesSpawnedPerWave[0].x, enemiesSpawnedPerWave[0].y));

        //Start enemy spawning and enemy spawn rate methods
        StartCoroutine(spawnEnemies());
        StartCoroutine(spawnEnemiesBetweenWaves());
        StartCoroutine(quickenSpawn());

        //Ensures enemies won't face overlap problems
        assignEnemyOrder();
        
        //In case the spawn settings are set up wrong
        if (enemiesSpawnedPerWave.Count != timeBetweenWaves.Count) 
            Debug.LogError("The two lists in the Spawn settings need to have the same number of elements");
        else if (enemiesSpawnedPerWave.Count == 0)
            Debug.LogError("Please fill out the Spawn settings");
    }

    //spawn enemies every reloadTime seconds
    private IEnumerator spawnEnemies()
    {
        //spawn a random enemy
        deployRandomEnemy();
        enemiesSpawned++;

        //once the enemy limit has been reached, stop spawning and wait till the next "wave"
        if (enemiesSpawned >= enemyLimit) 
        {
            //reset the enemies Spawned and reload rate for the next wave
            enemiesSpawned = 0;
            ++cycle;

            //configure the new number of enemies in the next wave + the waitTime after that wave
            if (cycle < enemiesSpawnedPerWave.Count) {
                enemyLimit = Mathf.RoundToInt(UnityEngine.Random.Range(enemiesSpawnedPerWave[cycle].x, enemiesSpawnedPerWave[cycle].y));
                
                //spawn enemies sparsely while waiting for the next wave
                waveOver = true;
                yield return new WaitForSeconds(timeBetweenWaves[cycle]);
                waveOver = false;
                
                //show the last wave is incoming message when 1 or 2 enemy waves are left
                if (cycle == enemiesSpawnedPerWave.Count - wavesBeforeLast)
                    StartCoroutine(displayText("The battle is nearing an end.", 3.5f, 0.3f));

                //reset the reload Time again
                reloadTime = roughSpawnRate -= Random.Range(0, spawnRateFall);
            }

            else   
                StartCoroutine(endOfLevel());
        } 
        //take a few seconds before spawning the next enemy
        else 
            yield return new WaitForSeconds(reloadTime - Random.Range(0, spawnRateFall));
        
        if (cycle < enemiesSpawnedPerWave.Count) 
            StartCoroutine(spawnEnemies());
    }   

    //spawn some Enemies in between waves
    private IEnumerator spawnEnemiesBetweenWaves() {
       while(1 == 1)
       {
            if (waveOver)
                deployRandomEnemy();
            yield return new WaitForSeconds(btwnReloadTime);
        }
    }

    //spawn Enemies faster over the course of the game
    private IEnumerator  quickenSpawn()
    {
        yield return new WaitForSeconds(1);
        if (reloadTime >= 1.2)
            reloadTime -= Random.Range(0.02f, 0.03f);

        StartCoroutine(quickenSpawn());
    }

    //Deploy a random enemy (based off how many enemy types were specified for this level)
    void deployRandomEnemy()
    {
        int r = Random.Range(1, enemyRange());

        if (r >= 1 && r <= 25)
            deployEnemy("Enemy 1");
        if (r >= 26 && r <= 50)
            deployEnemy("Enemy 2");
        if (r >= 51 && r <= 75)
            deployEnemy("Enemy 3");
        if (r >= 76 && r <= 100)
            deployEnemy("Enemy 4");
        if (r >= 101 && r <= 125)
            deployEnemy("Enemy 5");
    }

    //Deploying enemy shortened to one method/line
    void deployEnemy(string enemyName)
    {
        //get enemy
        GameObject enemy = findAndCycleEnemy(enemyName);
        Vector3 deployPos = new Vector3(1, 1, 1);

        //set the differnt enemies' spawn points
        if (enemyName == "Enemy 1") deployPos = E1_sp.transform.position;

        else if (enemyName == "Enemy 2") deployPos = E2_sp.transform.position;

        else if (enemyName == "Enemy 3")
        {
            deployPos = new Vector3(FlyingUp_sp.transform.position.x,
            Random.Range(FlyingDown_sp.transform.position.y, FlyingUp_sp.transform.position.y), 0);
        }

        else if (enemyName == "Enemy 4")
        {
            Vector3 r = Hill.GetChild(Random.Range(1, Hill.childCount-8)).transform.position;
            deployPos = new Vector3(r.x, r.y + 1.4f, 0);
        }

        else if (enemyName == "Enemy 5") deployPos = E5_sp.transform.position;

        else print ("Wtf do you want me to spawn. " + enemyName + " isn't a valid enemy.");

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
        orderTheEnemies(E1_group, 20);
        orderTheEnemies(E2_group, 40);
        orderTheEnemies(E3_group, 30);
        orderTheEnemies(E4_group, 10);
        orderTheEnemies(E5_group, 50);
    }

    //Correctly order the enemies (which one comes on top)
    void orderTheEnemies(GameObject enemyGroup, int layerOffset) {
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

    //Choose from the specified number of enemy types
    private int enemyRange() {
        if (diffEnemyTypes >= 1 && diffEnemyTypes <= 5)
            return 25 * diffEnemyTypes;
        else {            
            Debug.LogError("Choose a number from 1-5 to represent the number of diff enemies that can be spawned");
            return 0;
        }
    }

    //display text to the player for a specified duration
    private IEnumerator displayText(string description, float duration, float flashRate) {

        playerText.text = description;
        playerText.enabled = true;
        
        float progress = 0;
        while ( progress < duration) {
            playerText.enabled = (playerText.isActiveAndEnabled) ? false : true;
            yield return new WaitForSeconds(flashRate);
            progress += flashRate;
        }

        playerText.enabled = false;
    }

    //complete the level only when all enemies are dead
    private IEnumerator endOfLevel() {
        while (!enemiesAllDead()) {
            yield return new WaitForSeconds(0.5f);
        }

        //fade transition

        //load lvl selection
        SceneManager.LoadScene(0);
    }

    //return a bool regarding whether all enemies are dead
    private bool enemiesAllDead() 
    {
        //check all enemy groups
        foreach (Transform enemyGroup in transform) 
        {
            //check if any enemy in the enemy group is active
            foreach (Transform enemy in enemyGroup) 
            {
                if (enemy.gameObject.activeSelf)
                    return false;
            }
        }
        return true;
    }

}
