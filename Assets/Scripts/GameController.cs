﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public bool pause = true;
    public Text countDownText;
    public GameObject levelPassedPanel;
    public GameObject levelFailedPanel;
    public GameObject player;
    public Camera mainCam;
    public float differenceBetweenSizes = 12f;
    public float countDownTime = 3f;
    public float originalCamSize;
    public float zoomOutSize;
    public float zoomTime = 3.5f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        originalCamSize = mainCam.orthographicSize;


    }

    private void Start()
    {
        
   //     startGame();
    }



    public void startGame()
    {
        Debug.Log("Start game called");
        prepareToStart();

       

        Time.timeScale = 1f;
        CameraController.instance.startZoomOutInSequence(originalCamSize,zoomOutSize,zoomTime);
       
 
    }

    public void die()
    {
       
        PlayerController.instance.releaseGrapple();
        pauseGame();
        levelFailedPanel.SetActive(true);
    }

    public void levelPassed()
    {
        PlayerController.instance.releaseGrapple();
        pause = true;
        levelPassedPanel.SetActive(true);
    }

    public void pauseGame()
    {
        pause = true;
        Time.timeScale = 0f;
    }
    public void resumeGame()
    {
        pause = false;
        Time.timeScale = 1f;
    }

    public void prepareToStart()
    {
        pause = true;
        //Time.timeScale = 0f;

        levelFailedPanel.SetActive(false);
        levelPassedPanel.SetActive(false);

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

        playerRb.velocity = Vector2.zero;
        playerRb.gravityScale = 0f;

        player.transform.position = Vector3.zero;
        player.transform.rotation = Quaternion.Euler(0f, 0f, 0f);


        mainCam.orthographicSize = originalCamSize;
        zoomOutSize = originalCamSize + differenceBetweenSizes;
        mainCam.transform.position = new Vector3(0f, 0f, -10f);
    }

    public void resetEverything()
    {
        DontDestroy[] objects = GameObject.FindObjectsOfType<DontDestroy>();
        for(int i = 0; i<objects.Length; i++)
        {
            Destroy(objects[i].gameObject);
        }
    }



}