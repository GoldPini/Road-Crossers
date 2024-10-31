using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;


public class OnGameFailureEventArgs : EventArgs
{
    public string failureMessage;
}

public enum PlayerState
{
    RagdollDead, Overruned, Exploded, Alive, Won, Failed
}

[SelectionBase]
public class RigidBodyCharacterController : MonoBehaviour
{
    public static event EventHandler OnPlayerDeathImpact;
    public static event EventHandler<OnGameFailureEventArgs> OnLevelFailure;


    public static void ResetStaticData()
    {
        OnPlayerDeathImpact = null;
        OnLevelFailure = null;
    }

    private readonly string IS_WALKING = "isWalking";
    private readonly string IS_RUNNING = "isRunning";
    private readonly string FORWARD_VELOCITY = "velocityZ";
    private readonly string SIDEWAYS_VELOCITY = "velocityX";
    
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask vheicleLayer;
    [SerializeField] private Rigidbody playerBody;    
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform playerBottom;
    [SerializeField] private Transform projectileFocusPoint;
    [SerializeField] private GameObject characterModel;
    [Space]
    [SerializeField] private Animator animator;    
    [Header("Speed")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float sprintAcceleration;
    [SerializeField] private float deceleration;
    [Range(1f,2f),SerializeField] private float maxRunningVelocity;
    [Range(0f,1f),SerializeField] private float runningTransitionThreshhold = 0.4f;
    [Space]
    [SerializeField] private float sensitivity;
    [SerializeField] private float jumpForce;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private bool onlyFaceForward = true;
    [Space]
    [SerializeField, Range(0, 20)] private float failedScreenTimerAfterFail = 3f;

    public Transform ProjectileFocusPoint { get {return projectileFocusPoint; } }
    private GameInput gameInput;
    private Vector3 playerMovementNormalized;
    private Collider playerCapsuleCollider;
    private GameManager gameManager;

    public event EventHandler OnPlayerOverruned;
    public event EventHandler OnPlayerLevelSuccess;
    public event EventHandler<PlayerRespawnedEventArgs> OnPlayerRespawned;    
    public class PlayerRespawnedEventArgs : EventArgs
    {
        public Vector3 respawnPosition;
    }

    private PlayerState playerState;
    private Vector3 lastMovementDir = Vector3.zero;
    private bool isWalking = false;
    private bool isRunning = false;
    private bool isJumping = false;
    private bool isWalkingForward = false;
    private bool isWalkingBackwards = false;
    private bool isWalkingRight = false;
    private bool isWalkingLeft = false;
    private float velocity = 0f;
    private float forwardVelocity = 0f;
    private float sidewaysVelocity = 0f;
    private List<Collider> ragdollColliderPartsList;
    private float impactForce = 7;
    private float showFailScreenTimer = 0;
    private string failMessage = "";
    private bool hasFailed = false;
    private Vector3 spawnPoint;

    private void Awake()
    {
        playerState = PlayerState.Alive;
        playerCapsuleCollider = GetComponent<Collider>();

        ragdollColliderPartsList = characterModel.GetComponentsInChildren<Collider>().ToList();

        setRagdollExcludeLayers();
        disableRagdollPhysics();
    }

    private void Start()
    {
        gameManager = GameManager.Instance;
        Projectile.OnAnyPojectileCollidesPlayer += Projectile_OnAnyPojectileCollidesPlayer;
        GameManager.OnRestartDecision += GameManager_OnRestartDecision;
        GameStateManager.Instance.OnGameTimeOver += Game_OnTimeOverFailure; ;

        //Physics.gravity = new Vector3(0, -30, 0);
        gameInput = new GameInput();
        gameInput.CharacterControl.Enable();

        spawnPoint = this.transform.position;
    }

    private void Game_OnTimeOverFailure(object sender, EventArgs e)
    {
        setFailureMessage("YOU RUN OUT OF TIME");
        playerState = PlayerState.Failed;
    }

    private void GameManager_OnRestartDecision(object sender, EventArgs e)
    {
        hasFailed = false;
        ressetFailureMessage();
        respawnAtStart();
    }

    private void Projectile_OnAnyPojectileCollidesPlayer(object sender, EventArgs e)
    {
        if (playerState == PlayerState.Won || playerState == PlayerState.Failed)
            return;        
        if(playerState != PlayerState.RagdollDead)
        {
            //a projectile hit the player
            characterModel.transform.position = this.transform.position;
            playerState = PlayerState.Exploded;

            OnPlayerDeathImpact?.Invoke(this, EventArgs.Empty);

            setFailureMessage("YOU GOT BLOWN TO PIECES");
        }
    }

    private void FixedUpdate()
    {
        if (!GameStateManager.Instance.IsGamePlaying())
        {
            resetMovement();
            return;
        }
        switch (playerState)
        {
            case PlayerState.Alive:
                handleMovement();
                handleAnimator();
                break;

            case PlayerState.Overruned:
                enableRagdollPhysics(true);
                playerState= PlayerState.RagdollDead;
                break;

            case PlayerState.Exploded:
                enableRagdollPhysics(true);
                playerState = PlayerState.RagdollDead;
                break;

            case PlayerState.RagdollDead:
                if (showFailScreenTimer >= failedScreenTimerAfterFail)
                {
                    showFailScreenTimer = 0;
                    playerState = PlayerState.Failed;
                }
                else
                    showFailScreenTimer += Time.deltaTime;
                break;
            case PlayerState.Won:
                handleMovement();
                handleAnimator();
                break;
            case PlayerState.Failed:
                if (!hasFailed)
                {
                    hasFailed = true;
                    OnLevelFailure?.Invoke(this, new OnGameFailureEventArgs
                    {
                        failureMessage = failMessage
                    });
                }
                handleMovement();
                handleAnimator();
                break;
        }
    }

    private void Update()
    {
        if (!GameStateManager.Instance.IsGamePlaying())
        {
            resetMovement();
            return;
        }        

        Vector2 moveDirNormalized = gameInput.CharacterControl.Move.ReadValue<Vector2>();

        //disabling movement if player is in the win state
        if (playerState == PlayerState.Won || playerState == PlayerState.Failed)
            moveDirNormalized = Vector2.zero;

        playerMovementNormalized = moveDirNormalized != Vector2.zero ? new Vector3(moveDirNormalized.x, 0, moveDirNormalized.y) : Vector3.zero;

        isWalking = moveDirNormalized != Vector2.zero;
        isWalkingForward = playerMovementNormalized.z >= 0;
        isWalkingBackwards = !isWalkingForward;
        isWalkingRight = playerMovementNormalized.x >= 0;
        isWalkingLeft = !isWalkingRight;
    }  

    private void handleJumping()
    {
        //jumping
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Physics.Raycast(playerBottom.transform.position, Vector3.down, 0.1f))
            {
                isJumping = true;
            }
            else
                isJumping = false;
        }
        else
            isJumping = false;
    }

    private void handleMovement()
    {
        //checking if charachter sprinting
        isRunning = isWalking && gameInput.CharacterControl.Run.ReadValue<float>() == 1;
        //isRunning = true;

        if (isWalking)
        {
            //running
            if(isRunning)
            {
                //velocity is'nt maxed out yet, need to accelerate
                if (velocity < maxRunningVelocity)
                    velocity += sprintAcceleration * Time.deltaTime;
                velocity = Math.Min(maxRunningVelocity, velocity);  
            }
            //walking
            else
            {
                //need to accelerate
                if (velocity < runningTransitionThreshhold)
                {
                    velocity += acceleration * Time.deltaTime;
                    velocity = Math.Min(runningTransitionThreshhold, velocity);
                }
                //too fast, need to decelerate
                else if (velocity > runningTransitionThreshhold)
                    velocity -= deceleration * Time.deltaTime;    
            }
        }
        else //stationary
        {
            //still moving, need to decelerate
            if (velocity > 0f)
            {
                velocity -= deceleration * Time.deltaTime;
                velocity = Math.Max(0, velocity);
            }
        }

        //determining the final movement vector
        lastMovementDir = playerMovementNormalized == Vector3.zero ? lastMovementDir : playerMovementNormalized;
        Vector3 moveVector = lastMovementDir * maxSpeed * velocity;

        //facing direction
        if(!onlyFaceForward)
            playerBody.transform.forward = Vector3.Slerp(transform.forward, moveVector, rotationSpeed * Time.deltaTime);

        //acceleration
        playerBody.velocity = new Vector3(moveVector.x, playerBody.velocity.y, moveVector.z);        
        
        //handling gravity
        if (!Physics.Raycast(playerBottom.transform.position, Vector3.down, 0.1f))
            playerBody.AddForce(Vector3.down * gravity * Time.deltaTime, ForceMode.VelocityChange);

        
        if(isJumping)
        {
            playerBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        //playerBody.AddTorque(moveVector * rotationSpeed * Time.deltaTime, ForceMode.VelocityChange);

        //transform.Rotate(moveVector, rotationSpeed * Time.deltaTime);
    }

    private void resetMovement()
    {
        velocity = 0;
        forwardVelocity = 0;
        sidewaysVelocity = 0;
        playerBody.velocity = Vector3.zero;
        animator.SetFloat(FORWARD_VELOCITY, forwardVelocity);
        animator.SetFloat(SIDEWAYS_VELOCITY, sidewaysVelocity);
    }

    private void handleAnimator()
    {
        animator.SetBool(IS_WALKING, isWalking);
        animator.SetBool(IS_RUNNING, isRunning);            
                
        //calculating the normalized horizontal and vertical velicity 
        float forwardSpeedNormalized = math.abs(playerBody.velocity.z / maxSpeed);
        float sidewaysSpeedNormalized = math.abs(playerBody.velocity.x / maxSpeed);
        float velocityChangeFactor = 12f;

        //walking forward
        if (isWalkingForward)
        {
            forwardVelocity = math.lerp(forwardVelocity, math.min(velocity, forwardSpeedNormalized), 
                velocityChangeFactor * Time.deltaTime);
        }
        //walking backwards
        else if (isWalkingBackwards)
        {
            forwardVelocity = math.lerp(forwardVelocity, math.max(-velocity, -forwardSpeedNormalized),
                velocityChangeFactor * Time.deltaTime);
        }
        //walking right
        if (isWalkingRight)
        {
            sidewaysVelocity = math.lerp(sidewaysVelocity, math.min(velocity, sidewaysSpeedNormalized),
                velocityChangeFactor * Time.deltaTime);
        }
        //walking left
        else if (isWalkingLeft)
        {
            sidewaysVelocity = math.lerp(sidewaysVelocity, math.max(-velocity, -sidewaysSpeedNormalized),
                velocityChangeFactor * Time.deltaTime);
        }

        animator.SetFloat(FORWARD_VELOCITY, forwardVelocity);
        animator.SetFloat(SIDEWAYS_VELOCITY, sidewaysVelocity);
    }  

    private void disableRagdollPhysics()
    {
        animator.enabled = true;
        playerCapsuleCollider.enabled = true;

        foreach (Collider collider in ragdollColliderPartsList)
        {
            collider.enabled = false;
        }
        //foreach(CharacterJoint joint in characterModel.GetComponentsInChildren<CharacterJoint>())
        //{
        //    joint.enableProjection = false;
        //    joint.enablePreprocessing = false;
        //    joint.enableCollision = false;
        //}
    }

    private void enableRagdollPhysics(bool applyImpactForce = false)
    {      
        animator.enabled = false;
        playerBody.velocity = Vector3.zero;

        foreach (Collider collider in ragdollColliderPartsList)
        {            
            collider.enabled = true;
            if(applyImpactForce)
                collider.attachedRigidbody.velocity = new Vector3(impactForce, impactForce /2f, 0);
        }
    } 
    
    private void setRagdollExcludeLayers()
    {
        foreach (Collider collider in ragdollColliderPartsList)
        {
            collider.attachedRigidbody.excludeLayers = LayerMask.GetMask("Vehicles");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if the impacting collider is a vehicle and the player is alive, trigger a hit
        if (other.gameObject.layer == gameManager.VehicleFrontLayerMask
            && playerState == PlayerState.Alive || playerState == PlayerState.Failed)
        {
            characterModel.transform.position = this.transform.position;
            playerState = PlayerState.Overruned;            

            BaseVheicle vheicle = other.GetComponentInParent<BaseVheicle>();
            if (vheicle != null)
            {
                impactForce = vheicle.Speed / 3;
                vheicle.Stop(true);
            }
            if (playerState == PlayerState.Alive)
            {
                OnPlayerOverruned?.Invoke(this, EventArgs.Empty);
                setFailureMessage("YOU GOT SQUISHED");
            }
            OnPlayerDeathImpact?.Invoke(this, EventArgs.Empty);
        }
        if (other.gameObject.layer == gameManager.FinishingSegmentLayerMask
            && playerState == PlayerState.Alive)
        {            
            playerState = PlayerState.Won;
            OnPlayerLevelSuccess?.Invoke(this, EventArgs.Empty);
        }
    }

    private void respawnAtStart()
    {
        playerState = PlayerState.Alive;
        playerBody.velocity = Vector3.zero;
        velocity = 0f;
        playerBody.transform.position = spawnPoint;
        disableRagdollPhysics();

        OnPlayerRespawned?.Invoke(this, new PlayerRespawnedEventArgs { respawnPosition = spawnPoint});

        //SceneLoader.Load(SceneLoader.Scene.GameScene);
    }

    public bool IsPlayerWalking()
    {
        return isWalking;
    }
    
    public bool IsPlayerRunning()
    {
        return isRunning;
    }

    private void setFailureMessage(string message)
    {
        failMessage = failMessage == "" ? message : failMessage;
    }

    private void ressetFailureMessage()
    {
        failMessage = "";
    }

}
