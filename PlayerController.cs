using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    [Tooltip("Insert character controller")]
    private CharacterController controller;

    [SerializeField]
    [Tooltip("Insert main camera")]
    private Camera mainCamera;
    
    [SerializeField]
    [Tooltip("Insert Animator Controller")]
    private Animator playerAnimator;
    
    [SerializeField]
    [Tooltip("Insert Nestball Prefab")]
    private GameObject nestBallPF;
    
    [SerializeField]
    [Tooltip("Insert Animator Controller")]
    private Transform nestBallBone;
    
    private Vector3 velocity;
    private float gravity = -9.8f;
    private bool grounded;
    public float groundCastDist = 0.05f;
    
    public float speed = 2f;
    public float runSpeed = 6f;
    public float jumpHeight = 20f;

    private bool throwing = false;
    public float throwStrength = 4f;
    private GameObject instantiatedNestBall;
    public LayerMask playerMask;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Transforms of game objects/camera
        Transform playerTransform = controller.transform;
        Transform cameraTransform = mainCamera.transform;

        
        // Grounded
        Vector3 adjustment = new Vector3(0f, 0.5f, 0f);
        
        grounded = Physics.Raycast(playerTransform.position + adjustment, Vector3.down, groundCastDist + adjustment.y);

        // DEBUG - Raycast visualizing
        if (grounded) // DEBUG
        {
            Debug.DrawRay(playerTransform.position, Vector3.down, Color.blue);
        }
        else
        {
            Debug.DrawRay(playerTransform.position,Vector3.down, Color.red);
        }
        
        //Ground movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 movement = (playerTransform.right * x) + (playerTransform.forward * z);

        float moveDir = Vector3.Dot(movement.normalized, cameraTransform.forward);
        

        
        
        // Throw
        if (Input.GetButtonDown("Fire1") && grounded)
        {
            throwing = true;
            SpawnNestballToBone();
            playerAnimator.SetBool("IsThrowing", true);
        }
        
        // Apply Movement

        if (!throwing)
        {
            if (movement.magnitude > 0)
            {
                if (moveDir > 0)
                {
                    playerAnimator.SetBool("IsWalkF", true);
                    playerAnimator.SetBool("IsWalkB", false);

                }
                else
                {
                    playerAnimator.SetBool("IsWalkB", true);
                    playerAnimator.SetBool("IsWalkF", false);

                }
            }
            else
            {
                playerAnimator.SetBool("IsWalkF", false);
                playerAnimator.SetBool("IsWalkB", false);

            }

            if (Input.GetKey(KeyCode.LeftShift) && moveDir > 0)
            {
                controller.Move(movement * runSpeed * Time.deltaTime);
                playerAnimator.SetBool("IsRunning", true);

            }
            else
            {
                controller.Move(movement * speed * Time.deltaTime);
                playerAnimator.SetBool("IsRunning", false);

            } 
            //gravity and jumping
            velocity.y += Time.deltaTime * gravity;
            if (Input.GetButtonDown("Jump") && grounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight);
            }
            controller.Move(Time.deltaTime * velocity);
            playerAnimator.SetBool("IsJumping", !grounded);
        }
        
        
        //Link trainer rotation and camera rotation
        playerTransform.rotation = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up);
        
        
       
    }

    public void ThrowEnded()
    {
        throwing = false;
        playerAnimator.SetBool("IsThrowing", false);
    }

    private void SpawnNestballToBone()
    {
        if (instantiatedNestBall == null)
        {
            instantiatedNestBall = Instantiate(nestBallPF, nestBallBone, false);

        }
    }

    public void ReleasePokeball()
    {
        if (instantiatedNestBall != null)
        {
            instantiatedNestBall.transform.parent = null;
            instantiatedNestBall.GetComponent<SphereCollider>().enabled = true;
            instantiatedNestBall.GetComponent<Rigidbody>().useGravity = true;
            Transform cameraTransform = mainCamera.transform;
            Vector3 throwAdjustment = new Vector3(0f, 0.5f, 0f); 
            Vector3 throwVector = (cameraTransform.forward + throwAdjustment) * throwStrength;
            instantiatedNestBall.GetComponent<Rigidbody>().AddForce(throwVector, ForceMode.Impulse);
            instantiatedNestBall = null;
        }
    }
    
    //Hide the mouse
    private void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
