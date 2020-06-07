﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<GameObject> R1;
    public List<GameObject> R2;
    public List<GameObject> R3;
    public List<GameObject> Orc;
    public List<GameObject> Goblin;
    public List<GameObject> Ogre;
    public Transform GRSpawn;
    public Transform GLSpawn;

    private int cR1 = 0;
    private int cR2 = 0;
    private int cR3 = 0;
    private int cOgre = 0;
    private int cOrc = 0;
    private int cGoblin = 0;

    private float reloadTime = 1.5f;

    //spawn enemies every reloadTime seconds
    private IEnumerator spawnOver()
    {
        deployEnemy("Orc");
        deployEnemy("R1");

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
            deployEnemy("R2");
        if (r == 2)
            deployEnemy("R3");
        if (r == 3)
            deployEnemy("Orc");
        if (r == 4)
            deployEnemy("Goblin");
        if (r == 5)
            deployEnemy("Ogre");
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
            if (r == 0)
                deployPos = GRSpawn.position;
            else
                deployPos = GLSpawn.position;  
        }

        //reset enemy settings
        if (enemy.transform.GetComponent<Enemy_Health>() != null) {
            enemy.transform.GetComponent<Enemy_Health>().deploy = true;
            enemy.transform.GetComponent<Enemy_Health>().setHP();
        }

        //spawn enemy
        enemy.transform.position = deployPos;
        enemy.SetActive(true);        
    }

    //Find avaliable enemy + update its array cycle
    GameObject findEnemy(string enemyName)
    {        
        if (enemyName == "R1")
        {
            cR1 += 1;
            cR1 %= GameObject.Find(enemyName + " Group").transform.childCount;
            return findEnemy2(enemyName, cR1);
        }

        else if (enemyName == "R2")
        {
            cR2 += 1;
            cR2 %= GameObject.Find(enemyName + " Group").transform.childCount;
            return findEnemy2(enemyName, cR2);
        }

        else if (enemyName == "R3")
        {
            cR3 += 1;
            cR3 %= GameObject.Find(enemyName + " Group").transform.childCount;
            return findEnemy2(enemyName, cR3);
        }

        else if(enemyName == "Orc")
        {
            cOrc += 1;
            cOrc %= GameObject.Find(enemyName + " Group").transform.childCount;
            return findEnemy2(enemyName, cOrc);
        }

        else if(enemyName == "Ogre")
        {
            cOgre += 1;
            cOgre %= GameObject.Find(enemyName + " Group").transform.childCount;
            return findEnemy2(enemyName, cOgre);
        }

        else if (enemyName == "Goblin")
        {
            cGoblin += 1;
            cGoblin %= GameObject.Find(enemyName + " Group").transform.childCount;
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

    void Start()
    {
        StartCoroutine(spawnOver());
        StartCoroutine(quickenSpawn());

        //Use to reset enemy lists (add more of each type) 
        /*prepList(GameObject.Find("R1 Group"), R1);
        prepList(GameObject.Find("R2 Group"), R2);
        prepList(GameObject.Find("R3 Group"), R3);
        prepList(GameObject.Find("Orc Group"), Orc);
        prepList(GameObject.Find("Goblin Group"), Goblin);
        prepList(GameObject.Find("Ogre Group"), Ogre);

          for (int i = 1; i <= GameObject.Find("R1 Group").gameObject.transform.childCount; i++)
          {
              GameObject.Find("R1 Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
              GameObject.Find("R1 Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i-1 + 10;
          }

          for (int i = 1; i <= GameObject.Find("R2 Group").gameObject.transform.childCount; i++)
          {
              GameObject.Find("R2 Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
              GameObject.Find("R2 Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i - 1 + 20;
          }

          for (int i = 1; i <= GameObject.Find("R3 Group").gameObject.transform.childCount; i++)
          {
              GameObject.Find("R3 Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
              GameObject.Find("R3 Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i - 1 + 30;
          }

          for (int i = 1; i <= GameObject.Find("Orc Group").gameObject.transform.childCount; i++)
          {
              GameObject.Find("Orc Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
              GameObject.Find("Orc Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i - 1 + 40;
          }

          for (int i = 1; i <= GameObject.Find("Ogre Group").gameObject.transform.childCount; i++)
          {
              GameObject.Find("Ogre Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
              GameObject.Find("Ogre Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i - 1 + 50;
          }

          for (int i = 1; i <= GameObject.Find("Goblin Group").gameObject.transform.childCount; i++)
          {
              GameObject.Find("Goblin Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingLayerName = "Enemies";
              GameObject.Find("Goblin Group").gameObject.transform.GetChild(i - 1).transform.GetComponent<SpriteRenderer>().sortingOrder = i - 1 + 60;
          }*/

    }

    void prepList(GameObject enemyType, List<GameObject> enemyList)
    {
        enemyList.Clear();
        for (int i = 1; i <= enemyType.transform.childCount; i++)
        {
            enemyList.Add(enemyType.transform.GetChild(i - 1).gameObject);
        }
    }
}
