using UnityEngine;

public class MovementController : MonoBehaviour
{


    [Header("Rotation"), Space, SerializeField ,Range(1, 30)]
    public float rotationSpeed;
    
    [Header("Floor Movement"), Space, SerializeField]
    public float speed;
   
    [SerializeField, Range(0.1f, 3)]
    private float isGroundedRange;

    //----------------------------------------------------

    [Header("Air Movement"), Space, SerializeField]
    private float airSpeed;

    //----------------------------------------------------

    [Header("Jump"), Space, SerializeField]
    public float jumpForce;
    [SerializeField]
    private float coyoteTime;
    private float coyoteWaited = 0;


    //----------------------------------------------------

    [HideInInspector]
    public bool isGrounded = false;
    [HideInInspector]
    public bool canCoyote = false;

    [HideInInspector]
    public Vector2 recibedInputs;
    [HideInInspector]
    public Vector3 movementInput;

    [SerializeField]
    public LayerMask walkableMask;

    //----------------------------------------------------

    private PlayerController playerController;
    [HideInInspector]
    private Rigidbody rb;
    //private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        //animator = GetComponent<Animator>();

    }

    public void CheckMovementInput()
    {
        if (recibedInputs.x != 0 || recibedInputs.y != 0)
        {
            movementInput = Quaternion.Euler(0, playerController.playerCamera.transform.eulerAngles.y, 0) * new Vector3(recibedInputs.x, 0, recibedInputs.y);
        }
        else
        {
            movementInput = Vector3.zero;
        }

        // Se coge la direccion en la que va a moverse el personaje en base a los inputs pulsados y los 
        movementInput = movementInput.normalized;

    }
    public void CheckIfGrounded()
    {
        bool actuallyGrounded = false;

        if (isGrounded)
        {
            foreach (var item in playerController.raycastCont.groundRay)
            {
                if (Physics.Raycast(item.position, -Vector3.up, isGroundedRange, walkableMask))
                {
                    actuallyGrounded = true;
                    break;
                }
            }
        }
        else
        {
            foreach (var item in playerController.raycastCont.groundRay)
            {
                if (Physics.Raycast(item.position, -Vector3.up, isGroundedRange, walkableMask))
                {
                    actuallyGrounded = true;
                    canCoyote = true;
                    coyoteWaited = 0;
                    break;
                }
            }

        }

        //animator.SetBool("OnFloor", actuallyGrounded);
        isGrounded = actuallyGrounded;


    }

    #region Rotation

    public void RotatePlayer()
    {
        // Mira hacia la direccion donde se esta moviendo utilizando un lerp esferico
        Quaternion desiredRotation = Quaternion.LookRotation(movementInput, Vector3.up);

        if (movementInput != Vector3.zero)
        {
            //Velocidad de rotacion
            transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSpeed * Time.deltaTime);
        }
    }
    #endregion

    #region Floor Movement Functions
    public void FloorMovement()
    {
        if (movementInput != Vector3.zero)
            rb.velocity = new Vector3(movementInput.x, 0, movementInput.z) * speed + new Vector3(0, rb.velocity.y, 0);
    }

    #endregion

    #region Air Movement Functions

    public void AirMovement()
    {

        if (movementInput != Vector3.zero)
            rb.AddForce(new Vector3(movementInput.x, 0, movementInput.z) * airSpeed * Time.fixedDeltaTime, ForceMode.Force);
    }

    

    #endregion

    #region Jump
    public void Jump()
    {
        if (isGrounded && rb.velocity.y < 0.5f || canCoyote && rb.velocity.y < 0.5f)
        {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            canCoyote = false;
            //animator.SetTrigger("Jump");
        }
    }

    public void WaitCoyoteTime()
    {
        coyoteWaited += Time.deltaTime;

        if (coyoteWaited >= coyoteTime)
        {
            coyoteWaited = 0;
            canCoyote = false;
        }
    }

    #endregion

}

