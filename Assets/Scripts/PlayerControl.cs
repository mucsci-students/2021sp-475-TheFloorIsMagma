using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{
    [Header("Game Manager")]
    public GameManager gameManager;

    [Header("Movement Settings")]
    public float walkSpeed = 5;
    public float runSpeed = 10;
    public float jumpAmount = 10;
    public float superJumpAmount = 40;
    public float glideFallSpeed = -1.5f;
    public int numJumps = 2;

    [Header("Look Settings")]
    public float horizontalLookSensitivity = 2;
    public float verticalLookSensitivity = 2;

    [Header("Debug")]
    public Text debugText;

    // Rigid body instance for movement
    private Rigidbody rb;

    // Keep track of input movement
    private Vector3 movement;
    private bool needsToJump;
    private bool needsToSuperJump;
    private int jumpsRemaining;
    private float justJumpedTimer;
    private bool grounded;
    private bool canGlide;
    private bool levelWon;
    private float respawnTimer = -1;
    private bool isGliding;

    // Camera Instance for looking up and down
    private GameObject cam;

    // For respawn
    private Vector3 respawnPos;
    private Quaternion respawnRot;

    private bool sjEnabled = false;

    AudioSource[] audio;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cam = GameObject.Find("Main Camera");
        respawnPos = rb.position;
        respawnRot = rb.rotation;
        grounded = true;
        audio = GetComponents<AudioSource>();
    }

    void Update()
    {
        processMovement();
        processLook();
    }

    private void processMovement() {
        // Fallback respawn
        if (transform.position.y < -100)
        {
            rb.position = respawnPos;
            rb.rotation = respawnRot;
            rb.velocity = new Vector3(0,0,0);
        }

        // Reset movement vector
        movement.Set(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        if (respawnTimer >= 0)
        {
            respawnTimer += Time.deltaTime;
            if (respawnTimer > 0.75f)
            {
                gameManager.FadeIn(4);
                respawnTimer = -1;
            }
            else if (respawnTimer > 0.5f)
            {
                rb.position = respawnPos;
                rb.rotation = respawnRot;
                rb.velocity = new Vector3(0,0,0);
            }
        }

        if (levelWon)
        {
            movement.x = movement.z = 0;
            movement.y = 10;
            return;
        }

        // Deplete jumps if walked off a cliff
        if (grounded && !IsGrounded())
        {
            if (justJumpedTimer == 0)
            {
                jumpsRemaining = 0;
            }
            grounded = false;
        }

        // Reset jumps if on ground
        if (IsGrounded() && justJumpedTimer == 0)
        {
            jumpsRemaining = 2;
            grounded = true;
        }

        if (justJumpedTimer > 0)
        {
            justJumpedTimer -= Time.deltaTime;
        }
        else if (justJumpedTimer < 0)
        {
            justJumpedTimer = 0;
        }

        // Check for key input for forward/back and left/right movement
        float zMove = 0;
        float xMove = 0;
        float speed = walkSpeed;
        if(Input.GetKey(KeyCode.LeftShift)) {
            speed = runSpeed;
        }
        if(Input.GetKey(KeyCode.W)) {
            zMove = speed;
        }
        if(Input.GetKey(KeyCode.S)) {
            zMove = -speed;
        }
        if(Input.GetKey(KeyCode.A)) {
            xMove = -speed;
        }
        if(Input.GetKey(KeyCode.D)) {
            xMove = speed;
        }

        // Check if player jumped
        if(jumpsRemaining > 0 && Input.GetKeyDown(KeyCode.Space)) {
            needsToJump = true;
        }

        if (sjEnabled && !needsToJump && jumpsRemaining == 2 && Input.GetKeyDown(KeyCode.E))
        {
            needsToSuperJump = true;
        }

        movement.x = xMove;
        movement.z = zMove;

        if (xMove == 0 && zMove == 0 && justJumpedTimer == 0)
        {
            RaycastHit[] hits = Physics.RaycastAll(transform.position, -Vector3.up, 2.1f);
            if (hits.Length > 0 && hits[0].transform.gameObject.tag == "Platform")
            {
                transform.parent = hits[0].transform;
            }
            else
            {
                transform.parent = null;
            }
        }
        else
        {
            transform.parent = null;
        }
    }

    void FixedUpdate() { 
        // If gliding, don't let y speed fall below glideFallSpeed
        if (canGlide && Input.GetKey(KeyCode.Space) && rb.velocity.y < glideFallSpeed)
        {
            movement = movement.x * Vector3.right + glideFallSpeed * Vector3.up + movement.z * Vector3.forward;
        }

        if(needsToJump) {
            needsToJump = false;
            jumpsRemaining -= 1;
            justJumpedTimer = 0.1f;
            movement.y = jumpAmount;
        }
        else if (needsToSuperJump)
        {
            needsToSuperJump = false;
            jumpsRemaining = 0;
            justJumpedTimer = 0.1f;
            movement.y = superJumpAmount;
            sjEnabled = false;
            gameManager.StopSJDisplay();
            audio[0].Play();
        }

        rb.velocity = transform.TransformDirection(movement);
    }

    private void processLook() {
        float horizLook = Input.GetAxis("Mouse X") * horizontalLookSensitivity;
        float vertLook = Input.GetAxis("Mouse Y") * verticalLookSensitivity;

        // Rotate player by horiz movement
        transform.Rotate(0, horizLook, 0);

        // Rotate camera by vert movement caping
        float newX = cam.transform.eulerAngles.x-vertLook;
        cam.transform.eulerAngles = new Vector3(
            cameraClamp(newX),
            cam.transform.eulerAngles.y,
            cam.transform.eulerAngles.z
        );
    }

    // Keep camera rotation value between 270 and 90. 
    private float cameraClamp(float val) {
        if(val <= 270.0f && val > 180.0f)
            return 270.0f;
        if(val >= 90.0f && val < 180.0f)
            return 90.0f;
        return val;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Hazard")
        {
            gameManager.FadeOut(4);
            respawnTimer = 0;
            audio[1].Play();
        }
        else if (other.tag == "Checkpoint")
        {
            respawnPos = rb.position;
            respawnRot = rb.rotation;
            gameManager.DisplayCheckpoint();
        }
        else if (other.tag == "TimeUp")
        {
            Destroy(other.gameObject);
            gameManager.IncreaseTimer();
        }
        else if (other.tag == "ScoreUp")
        {
            Destroy(other.gameObject);
            gameManager.IncreaseScore();
        }
        else if (other.tag == "SuperJump")
        {
            Destroy(other.gameObject);
            if (!sjEnabled)
            {
                gameManager.StartSJDisplay();
                sjEnabled = true;
            }
        }
        else if (other.tag == "CoolMagma")
        {
            Destroy(other.gameObject);
            gameManager.StartLavaCool();
        }
        else if (other.tag == "Glide")
        {
            Destroy(other.gameObject);
            gameManager.StartGlide();
        }
        else if (other.tag == "Goal")
        {
            gameManager.Win();
            levelWon = true;
        }
    }

    public void SetCanGlide(bool canGlide)
    {
        this.canGlide = canGlide;
    }
    
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 2.1f);
    }
}
