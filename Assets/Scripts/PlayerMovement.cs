// Rigidbody based movement written by Dani and modified by An3
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMovement : MonoBehaviour
{

    //Assignables
    public Transform playerCam;
    public Transform orientation;

    //Other
    private Rigidbody rb;
    private float lastMovementTimer;
    private float timeBeforeRigidbodySleep = 1;

    
    // Arms
    public Transform armsParent;
    public Animator armsAnimator;
    private Vector3 armsOffset;

    // Camera
    Vector3 cameraOffset;

    //Rotation and look
    private float xRotation;
    public float sensitivity = 50f;
    private float sensMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public float startingMaxSpeed;
    public float maximumMaxSpeed = 30;

    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float x, y;
    bool jumping, sprinting, crouching;

    bool died = false;

    // Audio
    public AudioClip[] stepSounds;
    public AudioSource audioSource;
    public AudioClip backgroundTrack;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;


    public bool usingSpeedBoost = false;
    public LayerMask groundLayerMask;

    public float speedBoostMaxTime = 10;
    float powerUpTimer = 0;
    public float speedBoostSpeed = 80;

    public AudioClip hitSound, speedSound, regularSpeedSound;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Die(float force, Vector3 direction)
    {
        died = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.velocity = Vector3.zero;

        armsAnimator.SetFloat("Speed", 0);
        armsAnimator.SetTrigger("Die");

        rb.AddForce(force * direction, ForceMode.Impulse);
    }

    public void PlayStepSound()
    {
        //if (!grounded)
        //{
        //    armsAnimator.SetFloat("Speed", 0)
        //    return;
        //}
        if (grounded)
        {
            float volume = Random.Range(0.4f, 0.7f);
            audioSource.PlayOneShot(stepSounds[Random.Range(0, stepSounds.Length - 1)], volume);
        }
    }

    public void PlayBoulderHitSound()
    {
        audioSource.PlayOneShot(hitSound);
    }

    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraOffset = playerCam.position - transform.position;
        armsOffset = armsParent.position - playerCam.position;
        startingMaxSpeed = maxSpeed;
    }


    private void FixedUpdate()
    {
        if (!died)
            Movement();
    }

    private void Update()
    {
        if (died)
            return;

        if (usingSpeedBoost)
        {
            powerUpTimer += Time.unscaledDeltaTime;
            if (powerUpTimer > speedBoostMaxTime)
            {
                powerUpTimer = 0;
                maxSpeed = maximumMaxSpeed;
                usingSpeedBoost = false;
                audioSource.PlayOneShot(regularSpeedSound);
            }
        }

        MyInput();
        Look();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput()
    {
        //x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        y = Mathf.Clamp(y, 0, 1);
        //jumping = Input.GetButton("Jump");
        //crouching = Input.GetKey(KeyCode.LeftControl);

        //Crouching
        //if (Input.GetKeyDown(KeyCode.LeftControl))
        //    StartCrouch();
        //if (Input.GetKeyUp(KeyCode.LeftControl))
        //    StopCrouch();
    }

    private void StartCrouch()
    {
        transform.localScale = crouchScale;
        // make sure player stayed in same position even though center of mass changed
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        
        // if the player is moving faster than 0.5 meters per second, add a slide force
        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    public void CollectPowerUp(PowerUp.PowerUpType type)
    {
        if (type == PowerUp.PowerUpType.Speed)
        {
            maxSpeed = speedBoostSpeed;
            usingSpeedBoost = true;
            audioSource.PlayOneShot(speedSound);
        } else if (type == PowerUp.PowerUpType.Time)
        {
            Time.timeScale = 0.1f;
        }
    }

    private void StopCrouch()
    {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    void LateUpdate()
    {
        if (died)
        {
            return;
        }
        playerCam.position = transform.position + cameraOffset;
        armsParent.position = playerCam.position + armsOffset;
        //armsParent.rotation = Quaternion.Euler(armsParent.rotation.eulerAngles.x, playerCam.rotation.eulerAngles.y, 0);

        float speedValue = rb.velocity.magnitude / startingMaxSpeed;
        //speedValue = Mathf.Clamp(speedValue, 0, 10);
        armsAnimator.SetFloat("Speed", speedValue);
    }

    private void Movement()
    {
        // TODO: make rigidbody sleep when there's no movement for a second
        //if (x == 0 && y == 0 && !jumping)
        //{
        //    if (lastMovementTimer >= timeBeforeRigidbodySleep)
        //    {
        //        rb.Sleep();
        //        return;
        //    }
        //    lastMovementTimer += Time.deltaTime;
        //} else
        //{
        //    lastMovementTimer = 0;
        //}

        //Extra gravity
        if (!grounded)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 1000);
        }

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        //if (!grounded)
        //{
        //    multiplier = 0.5f;
        //    multiplierV = 0.5f;
        //}

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
        //if (y > 0)
        //{
            rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
            rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
        //} else
        //{
        //    //Debug.Log("Test");
        //}


        //rb.velocity = orientation.transform.forward * y * moveSpeed * Time.deltaTime;
    }

    private void Jump()
    {
        if (grounded && readyToJump)
        {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private float desiredX;


    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag)
    {
        if (!grounded || jumping) return;

        //Slow down sliding
        //if (crouching)
        //{
        //    rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
        //    return;
        //}

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Vector3 flatVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        ////Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        //if (flatVelocity.sqrMagnitude > maxSpeed * maxSpeed)
        //{
        //    float fallspeed = rb.velocity.y;
        //    Vector3 n = rb.velocity.normalized * maxSpeed;
        //    rb.velocity = new Vector3(n.x, fallspeed, n.z);
        //}
    }

    public void ResetPlayerPosition()
    {
        transform.position = Vector3.zero + transform.lossyScale / 2;
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook()
    {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v)
    {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    /// <summary>
    /// Handle ground detection
    /// </summary>
    private void OnCollisionStay(Collision other)
    {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;


        grounded = true;
        cancellingGrounded = false;
        Vector3 normal = other.contacts[0].normal;
        normalVector = normal;
        CancelInvoke(nameof(StopGrounded));

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++)
        {
            
            //FLOOR
            //if (IsFloor(normal))
            //{

            //}
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded)
        {
            cancellingGrounded = true;

            if (!grounded)
            {
                RaycastHit hit = new RaycastHit();
                Physics.queriesHitTriggers = false;
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 10, groundLayerMask))
                {
                    grounded = true;
                }
                else
                {
                    armsAnimator.SetFloat("Speed", 0);
                    Invoke(nameof(StopGrounded), Time.deltaTime * delay);

                }
                Physics.queriesHitTriggers = true;
            }

        }
    }

    private void StopGrounded()
    {
        grounded = false;
    }

}