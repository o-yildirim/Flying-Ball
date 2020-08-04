﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public List<GameObject> additiveObjects;

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

 

    public void loadNextLevel()
    {
       
      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
      GameController.instance.startGame();


    }

    public void restartLevel()
    {
       
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameController.instance.startGame();
    }
}
