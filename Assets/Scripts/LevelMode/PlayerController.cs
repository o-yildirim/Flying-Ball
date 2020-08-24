﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public static PlayerController instance;

    private Rigidbody2D playerRb;
    private SpringJoint2D joint;
    private LineRenderer lineRenderer;
    private Vector2 grapplePoint;
    public bool isGrappling = false;
    private SpringJoint2D jointNode;
    private float velocityMultiplier = 10f;
   // private float forceMultiplier;
    private float smoothFactor = 6f;
    private float hookRange;
    private Touch touch;
    private float angle;
    public float zoomOutWhileGrappling = 8f;
    public float zoomTime = 3f;
    Coroutine zoomOut;
    Coroutine zoomIn;



    private Vector2 currentSpeed;
    public float currentSpeedMagnitude;
  

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        calculateHookRange();
    }


    void Update()
    {

        if (GameController.instance.pause) return;


        Vector2 velocity = playerRb.velocity;
        angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
        Quaternion newRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, smoothFactor * Time.deltaTime);
     

        if (Input.touchCount > 0 )
        {
            touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                grapple();
            }
        }


        else if (Input.touchCount == 0)
        {
            if (touch.phase == TouchPhase.Ended)
            {
                releaseGrapple();
            }
        }

        currentSpeedMagnitude = playerRb.velocity.magnitude;

        
    }

    private void FixedUpdate()
    {
        if (isGrappling)
        {
            playerRb.velocity = playerRb.velocity.normalized * velocityMultiplier;

        }
    }


    void LateUpdate()
    {
        drawRope();     
    }

    public void grapple()
    {

        if(jointNode != null)
        {
            Destroy(jointNode);
        }

        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);


        Vector2 playerPosition = transform.position;
        lineRenderer.positionCount = 2;

        RaycastHit2D hit;
        //hit = Physics2D.Raycast(playerPosition, touchPosition - playerPosition, 30f, LayerMask.GetMask("Obstacle"));

        hit = Physics2D.CircleCast(playerPosition, 1.5f ,touchPosition - playerPosition, hookRange, LayerMask.GetMask("Obstacle"));
       

        

        if (hit.collider != null)
        {
            playerRb.gravityScale = 0.5f;
            //grapplePoint = hit.point;
            grapplePoint = hit.transform.position;
            float distance = Vector3.Distance(grapplePoint,transform.position);
            createJoint(distance);
            playerRb.gravityScale = 0.5f;
     
            isGrappling = true;

            if(zoomIn != null)
            {
                StopCoroutine(zoomIn);
            }

            if (zoomOut == null)
            {
                zoomOut = StartCoroutine(CameraController.instance.zoomOut(GameController.instance.originalCamSize + zoomOutWhileGrappling,zoomTime));
                zoomIn = null;
            }
        }
        
 
    }

    public void releaseGrapple()
    {
        lineRenderer.positionCount = 0;
        Destroy(jointNode);
        isGrappling = false;

        if(zoomOut != null)
        {
            StopCoroutine(zoomOut);
        }

        if (zoomIn == null)
        {
            zoomIn = StartCoroutine(CameraController.instance.zoomIn(zoomOut, GameController.instance.originalCamSize, zoomTime));
            zoomOut = null;
        }
            

    }

    public void drawRope()
    {
        if (!isGrappling) return;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);
    
    }

    public void createJoint(float distance)
    {
       jointNode = gameObject.AddComponent<SpringJoint2D>();
       jointNode.autoConfigureConnectedAnchor = false;
       jointNode.connectedAnchor = grapplePoint;
       jointNode.distance = distance;
       jointNode.frequency = 0f;
         
    }

    public void calculateHookRange()
    {
        //float y = Screen.height / 2;
        //float x = Screen.width / 2;

        float x = GameController.instance.originalCamSize + zoomOutWhileGrappling;

        //Debug.Log("Screen height/2 = "+y);


        hookRange = Mathf.Sqrt(Mathf.Pow(x,2) + Mathf.Pow(x,2));
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Obstacle") || collision.transform.CompareTag("Wall") || collision.transform.CompareTag("Ground"))
        {
            GameController.instance.die();
        }
    }

}
