using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.Splines.Interpolators;

public enum VheicleState
{
    Moving, Stopped, Stationed, broken
}
public class RigidBodyVheicleController : MonoBehaviour
{

    public static event EventHandler OnAnyVehicleHonking;
    public static event EventHandler OnAnyVehicleExploded;

    public static void ResetStaticData()
    {
        OnAnyVehicleHonking = null;
        OnAnyVehicleExploded = null;
    }

    [Space]//TODO remove
    [SerializeField, TextArea(2,4)] private string debug = "TEST";

    [Space]
    [SerializeField] private float speed;
    [Tooltip("How quickly the vheicle gains speed upon accelerating")]
    [SerializeField] private float acceleration = 2f;
    [Tooltip("How quickly the vheicle slows down. the lower, the slower the vheicle will deaccelerate")]
    [SerializeField] private float deacceleration = 2f;
    [Tooltip("Time in seconds it takes the vheicle to reaccelerate after stoping")]
    [SerializeField] private float accelerationDelay = 2f;
    [Tooltip("The distance at which this vehicle will look for an obscuring object in its front path")]
    [SerializeField] private float forwardCastingDistance = 3f;
    [Space]
    [SerializeField] private GameObject front;
    [Space]
    [SerializeField] private Transform vfxContainer;
    [SerializeField] private Transform playerHitParticles;
    [SerializeField] private AudioSource audioSource;


    private Rigidbody vehicleRigidBody;

    public float Speed { get { return speed; } set { speed = value; } }
    public VheicleState State { get { return state; }}
    private float currentSpeed = 0;
    private float timeSinceStop;
    private VheicleState state = VheicleState.Stationed;
    private const float DELTA = 0.01f;
    private GameManager difficultyController;

    public event EventHandler OnVheicleStopped;
    public event EventHandler OnVheicleExplode;
    public event EventHandler<OnVehicleSpeedChangedEventArgs> OnVehicleSpeedChanged;
    public class OnVehicleSpeedChangedEventArgs : EventArgs
    {
        public float speedNormalized;
    }

    private void Start()
    {        
        vehicleRigidBody = GetComponent<Rigidbody>();
        timeSinceStop = 0;        
        difficultyController = GameManager.Instance;
        this.OnVheicleStopped += VheicleController_OnVheicleStopped;

        //picking a random engine sound for the vehicle
        //if(audioSource.clip == null)
        //    audioSource.clip = SoundManager.Instance.RandomEngineSound;
        if (audioSource.clip != null)
            audioSource.Play();
    }

    private void VheicleController_OnVheicleStopped(object sender, EventArgs e)
    {
        setPlayerHitAbility(false);
    }

    private void Update()
    {
        //bool isInsideScreen = vehicleRigidBody.position.IsInsideViewScreen(Camera.main);
        //if (isInsideScreen && !audioSource.isPlaying)
        //{
        //    audioSource.Play();
        //}
        //else if(!isInsideScreen && audioSource.isPlaying)
        //{
        //    audioSource.Stop(); 
        //}

        RaycastHit[] hits = Physics.RaycastAll(front.transform.position, front.transform.forward, forwardCastingDistance);
        bool objectInFront = false;
        BaseVheicle nextVheicle = null;
        foreach (var hit in hits)
        {
            if ((hit.collider.gameObject.layer == difficultyController.VehicleLayerMask)
             || (hit.collider.gameObject.layer == difficultyController.VehicleObstacleMask))
            {
                nextVheicle = hit.collider.gameObject.GetComponentInParent<BaseVheicle>();
                objectInFront = true;
                break;
            }
        }

        switch (state)
        {
            case VheicleState.Moving:
                // object in the front, need to stop
                if (objectInFront) 
                {
                    // the front object is a vheicle
                    if (nextVheicle != null)
                    {
                        float nextVheicleSpeed = nextVheicle.Speed;
                        if (nextVheicleSpeed > 0) Speed = nextVheicleSpeed;
                    }                        
                    timeSinceStop = 0;
                    SlowTemporeraly();
                }
                // no object, can keep moving               
                else
                {
                    timeSinceStop += Time.deltaTime;
                    if (timeSinceStop >= accelerationDelay)
                    {
                        MaximizeSpeed();
                    }
                }
                break;
            case VheicleState.Stationed:

                timeSinceStop += Time.deltaTime;
                //no obscuring object, can start accelerating
                if (!objectInFront && timeSinceStop >= accelerationDelay)
                {
                    setPlayerHitAbility(true);
                    MaximizeSpeed();
                }
                break;

            case VheicleState.Stopped:
                if (currentSpeed > DELTA)
                {
                    SlowTemporeraly();
                }
                break;

            case VheicleState.broken:                
                break;
        }
        debug = vehicleRigidBody.velocity.magnitude + "";
    }

    public void SlowTemporeraly()
    {
        Stop(false);
    }

    public void MaximizeSpeed()
    {
        this.state = VheicleState.Moving;
        float t = Time.deltaTime;

        //calculating direction vector
        Vector3 direction = transform.forward * Speed;

        //accelerating if needed
        if (vehicleRigidBody.velocity.magnitude < direction.magnitude)
        {
            vehicleRigidBody.velocity = new Vector3(
                    Mathf.Lerp(vehicleRigidBody.velocity.x, direction.x, t * acceleration),
                    Mathf.Lerp(vehicleRigidBody.velocity.y, direction.y, t * acceleration), 
                    Mathf.Lerp(vehicleRigidBody.velocity.z, direction.z, t * acceleration)
                );

            currentSpeed = Math.Max(0, vehicleRigidBody.velocity.magnitude);

            if (currentSpeed < vehicleRigidBody.velocity.magnitude)
                vehicleRigidBody.velocity = direction;

            float maxSpeed = Speed != 0 ? Speed : float.MaxValue;
            OnVehicleSpeedChanged?.Invoke(this, new OnVehicleSpeedChangedEventArgs 
            { speedNormalized = currentSpeed / maxSpeed});
        }
        setState();
    }

    public void Stop(bool completeStop, float? deacccelerationRate = null)
    {
        Speed = completeStop ? 0f : Speed;
        if (completeStop) state = VheicleState.Stopped;
        deacccelerationRate = deacccelerationRate != null ? deacccelerationRate : deacceleration;
        
        float t = Time.deltaTime;
        vehicleRigidBody.velocity = new Vector3(
                Mathf.Lerp(vehicleRigidBody.velocity.x, 0f, t * deacceleration),
                Mathf.Lerp(vehicleRigidBody.velocity.y, 0f, t * deacceleration), 
                Mathf.Lerp(vehicleRigidBody.velocity.z, 0f, t * deacceleration)
            );

        currentSpeed = Math.Max(0, vehicleRigidBody.velocity.magnitude);

        float maxSpeed = Speed != 0 ? Speed : float.MaxValue;
        OnVehicleSpeedChanged?.Invoke(this, new OnVehicleSpeedChangedEventArgs
        { speedNormalized = currentSpeed / maxSpeed });

        setState();
    }

    private void setState()
    {
        if (state == VheicleState.Moving)
            if(currentSpeed <= DELTA)
            {
                state = VheicleState.Stationed;
                OnAnyVehicleHonking?.Invoke(this, EventArgs.Empty);
            }

        if (currentSpeed <= DELTA)
            OnVheicleStopped?.Invoke(this, EventArgs.Empty);
    }

    private void setPlayerHitAbility(bool ableToHit)
    {
        front.SetActive(ableToHit);
    }

    public void Explode()
    {
        Stop(true);

        OnVheicleExplode?.Invoke(this, EventArgs.Empty);
        OnAnyVehicleExploded?.Invoke(this, EventArgs.Empty);

        state = VheicleState.broken;
        setPlayerHitAbility(false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(front.transform.position, front.transform.forward * forwardCastingDistance);
    }

    private void OnTriggerEnter(Collider other)
    {
        //TODO move logic to the destroyer script
        if (other.gameObject.layer == difficultyController.VehicleDestroyerMask)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == difficultyController.PlyayerLayerMask)
        {
            float hitVelocity = 0;  
            hitVelocity = collision.relativeVelocity.magnitude;

            debug = "Collision: " + collision.contactCount + " contact points | "
                + hitVelocity + " velocity";
            foreach (ContactPoint contact in collision.contacts)
            {
                //spawning particles
                Transform particle = Instantiate(playerHitParticles, contact.point,
                    Quaternion.FromToRotation(Vector3.up, contact.normal), vfxContainer);
            }

            OnAnyVehicleHonking?.Invoke(this, EventArgs.Empty);
        }
    }   
}
