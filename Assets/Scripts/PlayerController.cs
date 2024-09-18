using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Inputs"), SerializeField]
    private InputActionReference moveAction;
    [SerializeField]
    private InputActionReference jumpAction;


    public enum States { NONE, MOVING, AIR, INTERACTING };

    [Space]
    public States currentState = States.NONE;


    [SerializeField]
    public GameObject playerCamera;


    
    [HideInInspector]
    public Vector3 respawnPos;

    [HideInInspector]
    public MovementController movementCont;
    [HideInInspector]
    public RaycastController raycastCont;

    private CapsuleCollider coll;
    private Animator animator;
    private Rigidbody rb;

    private void Awake()
    {

        movementCont = GetComponent<MovementController>();
        raycastCont = GetComponent<RaycastController>();

        rb = GetComponent<Rigidbody>();
        coll = GetComponentInChildren<CapsuleCollider>();
        animator = GetComponent<Animator>();

        respawnPos = transform.position;
    }

    private void OnEnable()
    {
        Debug.Log("enabled?");
        moveAction.action.started += SetMovmentValues;
        moveAction.action.performed += SetMovmentValues;
        moveAction.action.canceled += SetMovmentValues;

        jumpAction.action.started += Jump;
    }

    private void OnDisable()
    {
        Debug.Log("disabled?");

        moveAction.action.started -= SetMovmentValues;
        moveAction.action.performed -= SetMovmentValues;
        moveAction.action.canceled -= SetMovmentValues;

        jumpAction.action.started -= Jump;
    }


    #region Input
    private void SetMovmentValues(InputAction.CallbackContext obj)
    {
        Debug.Log("EING?");
        //Guardamos el valor de los inputs de movimiento
        movementCont.recibedInputs = obj.ReadValue<Vector2>();
    }
    private void Jump(InputAction.CallbackContext obj)
    {
        Debug.Log("JOMP?");
        movementCont.Jump();
    }

    #endregion


    void Update()
    {
        StatesAction();
        UpdateCheckers();
    }

    #region Update Functions

    private void StatesAction()
    {
        switch (currentState)
        {
            case States.NONE:
            case States.MOVING:
                movementCont.CheckMovementInput();
                SetCurrentState();
                movementCont.RotatePlayer();
                break;
            case States.AIR:
                movementCont.CheckMovementInput();
                SetCurrentState();
                movementCont.RotatePlayer();
                break;
            default:
                movementCont.movementInput = Vector3.zero;
                break;
        }
    }

    private void UpdateCheckers()
    {

        switch (currentState)
        {
            case States.NONE:
            case States.MOVING:
            case States.AIR:
                movementCont.CheckIfGrounded();
                break;
            default:
                break;
        }


        if (!movementCont.isGrounded && movementCont.canCoyote)
        {
            movementCont.WaitCoyoteTime();
        }
    }

    private void SetCurrentState()
    {
        if (movementCont.isGrounded)
        {
            if (movementCont.recibedInputs != Vector2.zero)
            {
                currentState = States.MOVING;
            }
            else
            {
                currentState = States.NONE;
            }

        }
        else
            currentState = States.AIR;


    }

    #endregion


    private void FixedUpdate()
    {
        DoStates();
    }

    #region FixedUpdate Functions
    private void DoStates()
    {

        switch (currentState)
        {
            case States.NONE:
            case States.MOVING:
                movementCont.FloorMovement();
                break;
            case States.AIR:
                movementCont.AirMovement();
                break;
            case States.INTERACTING:
                //Hacer las cosas de interactuar

                break;
            default:
                break;
        }


    }

    #endregion


    #region Extras
    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        System.Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
    #endregion


    private void OnDrawGizmosSelected()
    {

        if (raycastCont != null)
        {
            if (!movementCont.isGrounded)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(raycastCont.groundRay[0].position, -transform.up);
            }
        }
    }

}
