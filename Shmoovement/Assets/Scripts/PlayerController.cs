using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Camera mainCamera;
    private InputAction movement;
    private InputAction debugAction;
    private InputAction jumpAction;
    public InputActionAsset actions;

    [Header("Speed Controls")]
    [SerializeField] float accelerationFactor;
    [SerializeField] float deccelerationFactor;
    [SerializeField] float maxSpeed;
    [SerializeField] float deadzone;
    [SerializeField] float deadzoneMinSpeed;

    [Space(10)]

    [Header("Jump Controls")]
    [SerializeField] float jumpForce;
    [SerializeField] float gravityForce;
    [SerializeField] float maxSlope;
    [SerializeField] float coyoteFrames;

    [SerializeField] float coyoteTimer;
    [SerializeField] bool coyoteCountStarted = false;

    [Header("Raycasting / Grounded Checks")]
    [SerializeField] bool grounded;
    [SerializeField] float raycastDistance;
    [SerializeField] Vector3 latestNormal;
    [SerializeField] CapsuleCollider playerCollider;

    Rigidbody rb;

    

    Vector3 topSphere;
    Vector3 bottomSphere;

    public bool Grounded { get { return grounded; } set { grounded = value; } }
    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main;
        actions.Enable();

        movement = actions.FindActionMap("gameplay").FindAction("Move");
        debugAction = actions.FindActionMap("gameplay").FindAction("Debug");
        jumpAction = actions.FindActionMap("gameplay").FindAction("Jump");

        //hasJump = false;
        debugAction.performed += OnDebugKey;
        jumpAction.performed += OnJump;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdateRaycast();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateMovement();
        UpdateRotation();
        
        UpdateGravity();

        if (coyoteCountStarted)
        {
            coyoteTimer++;
            if (coyoteTimer >= 20f)
                coyoteTimer = 20f;
        }    
    }
    //void OnCollisionEnter(Collision col)
    //{
    //    if (col.gameObject.CompareTag("Floor"))
    //        foreach (ContactPoint p in col.contacts)
    //        {
    //            //Debug.Log(p.normal);
    //            if (p.normal.y >= maxSlope)
    //            {
    //                grounded = true;
    //                break;
    //            }
    //        }

    //}

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(bottomSphere, playerCollider.radius);
        //Gizmos.DrawWireSphere(topSphere, playerCollider.radius);
    }
    void UpdateRaycast() 
    {
        //bottomSphere = new Vector3(transform.position.x, playerCollider.bounds.min.y + playerCollider.radius, transform.position.z);
        //topSphere = new Vector3(transform.position.x, playerCollider.bounds.max.y - playerCollider.radius, transform.position.z);
        Ray r = new Ray(transform.position, Vector3.down);

        if (Physics.SphereCast(r, playerCollider.radius, out RaycastHit hit, raycastDistance))
        {
            //Debug.Log(hit.normal.y);
            if (hit.normal.y >= maxSlope)
            {
                grounded = true;
                coyoteCountStarted = false;
                coyoteTimer = 0f;
            }
            else
            {
                coyoteCountStarted = true;
                if (coyoteTimer > coyoteFrames)
                    grounded = false;
            }
        }
        else 
        {            
            coyoteCountStarted = true;
            if (coyoteTimer > coyoteFrames)
                grounded = false;
        }
            
    }

    void UpdateGravity() 
    {
        rb.AddForce(Physics.gravity * gravityForce, ForceMode.Acceleration);
    }
    void UpdateMovement() 
    {
        Vector2 movementInput = movement.ReadValue<Vector2>();

        if(Mathf.Abs(movementInput.x) <= deadzone && Mathf.Abs(movementInput.y) <= deadzone)
        {

            //rb.mass = 2f;
            //Apply drag force when input is released, but stop once we are good
            
            if (rb.velocity.sqrMagnitude > deadzoneMinSpeed)
                rb.AddForce(new Vector3(rb.velocity.x * -deccelerationFactor, 0, rb.velocity.z * -deccelerationFactor), ForceMode.Impulse);
        }
        else 
        {
            rb.mass = 1f;
            Vector3 targetDirection = Vector3.zero;
            targetDirection += transform.forward * movementInput.y;
            targetDirection += transform.right * movementInput.x;

            float test = Vector3.ProjectOnPlane(rb.velocity, Vector3.up).magnitude;

            //Point velocity in direction of input
            rb.velocity = new Vector3(targetDirection.x * test, rb.velocity.y, targetDirection.z * test);

            //Apply force if under max speed threshold
            if (rb.velocity.sqrMagnitude < maxSpeed * maxSpeed)
            {
                rb.AddForce(targetDirection * accelerationFactor, ForceMode.Impulse);
            }
            //Debug.Log(targetDirection);
        }
        
    }

    void UpdateRotation() 
    {
        Vector3 cameraRotation = mainCamera.transform.rotation.eulerAngles;
        cameraRotation = new Vector3(0, cameraRotation.y, 0);

        transform.rotation = Quaternion.Euler(cameraRotation);

    }

    void OnDebugKey(InputAction.CallbackContext ctx) 
    {
        //grounded = !grounded;
    }

    void OnJump(InputAction.CallbackContext ctx) 
    {
        
        if (grounded && coyoteTimer <= coyoteFrames) 
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            grounded = false;
            //Debug.Log(coyoteTimer);
        }
    }
}
