﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public int totalLevelCount;
    private bool isMaxLevelReached;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        totalLevelCount = getLevelCount();    
    }



    public void loadNextLevelCall()
    {
      
        int buildIndex = SceneManager.GetActiveScene().buildIndex;

        if (buildIndex < SceneManager.sceneCountInBuildSettings -1)
        {
            StartCoroutine(loadLevel(buildIndex + 1));
        }
     
    }

    public void restartLevelCall()
    {
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        StartCoroutine(loadLevel(buildIndex));

    }

    public void loadLevelCall(int buildIndex)
    {
        StartCoroutine(loadLevel(buildIndex));
    }

    public IEnumerator loadLevel(int buildIndexToLoad){

        GameController.instance.audioSource.Stop();
        GameController.instance.loadingScreen.SetActive(true);


        GameController.instance.allLevelsPassedPanel.SetActive(false);
        GameController.instance.levelPassedPanel.SetActive(false);
        GameController.instance.levelFailedPanel.SetActive(false);

        /*
        if(buildIndexToLoad == 0)
        {
            GameController.instance.deactivateEverything();
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(buildIndexToLoad);
            while (!asyncLoadLevel.isDone)
            {
                yield return null;
            }
            GameController.instance.loadingScreen.SetActive(false);
            GameController.instance.resumeGame();
        }
        else
        {
            AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(buildIndexToLoad);
            while (!asyncLoadLevel.isDone)
            {
                yield return null;
            }
            GameController.instance.loadingScreen.SetActive(false);
            GameController.instance.startGame();
        }
        */


        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(buildIndexToLoad);
        while (!asyncLoadLevel.isDone)
        {
            yield return null;
        }

        GameController.instance.loadingScreen.SetActive(false);
        
        if (buildIndexToLoad != 0)
        {
            GameController.instance.startGame();
        }
        else
        {

            GameController.instance.deactivateEverything();
            GameController.instance.resumeGame();
        }
       
    }

    public void checkIfNewLevelUnlocked()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;

        if (currentLevel > LevelManager.instance.totalLevelCount)
        {
            return;
        }

        int maxPassedLevel = getPassedMaxLevel();

        if (currentLevel > maxPassedLevel)
        {
            setPassedMaxLevel(currentLevel); 

        }
    }

    public void goToMenu()
    {
        StartCoroutine(loadLevel(0));
    }


    public int getLevelCount()
    {
        int count = 0;

        string keyword = "Level";

       

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {

            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);

     
          if (sceneName.Contains(keyword))
          {
               
            count++;
          }
        }
        return count;
    }

    public int getPassedMaxLevel()
    {
        return PlayerPrefs.GetInt("PassedMaxLevel");
    }

    public void setPassedMaxLevel(int levelIndex)
    {
        PlayerPrefs.SetInt("PassedMaxLevel", levelIndex);
    }

    public bool checkIfMaxLevelReached()
    {
        bool flag = false;

        int passedMaxLevel = SceneManager.GetActiveScene().buildIndex;

        if (passedMaxLevel >= totalLevelCount)
        {
            flag = true;
        }

        return flag;
    }



  








}
