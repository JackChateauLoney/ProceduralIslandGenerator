using System;
using UnityEngine;

namespace Script.Movements
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        //Serialized variables
        [Header("Camera")]
        [SerializeField] private Camera mainCam = null;

        [Header("Ground surfaces")]
        [SerializeField] private LayerMask groundMask = ~0;


        [Header("Walk / Run Setting")]
        [Space(25)]
        [SerializeField] private float walkSpeed = 4.0f;
        [SerializeField] private float runSpeed = 6.0f;

        [Header("Jump Settings")]
        [SerializeField] private float playerJumpForce = 20000.0f;
        [SerializeField] private ForceMode appliedForceMode = ForceMode.Force;
        [SerializeField] private float gravity = 20.0f;





        //private variables
        private Rigidbody rb;
        private Animator anim = null;

        //movement varaibles
        private float _xAxis;
        private float _yAxis;
        private float currentSpeed;

        //jumping variables
        private Vector3 groundLocation;
        private bool reachGround = true;
        private bool playerIsJumping;
        private bool canJump = true;
        private float jumpCooldown = 0.0f;


        private void Start()
        {
            //initialise
            rb = GetComponent<Rigidbody>();
            anim = transform.GetChild(0).GetComponent<Animator>();
            rb.useGravity = false;
        }

        private void Update()
        {

            #region Get Input

            //Controller Input
            _xAxis = Input.GetAxisRaw("Horizontal");
            _yAxis = Input.GetAxisRaw("Vertical");


            //sprinting
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;

            //jumping
            playerIsJumping = Input.GetKeyDown(KeyCode.Space);
            
            #endregion




            #region Jumping

            //jump cooldown
            if (!canJump)
            {
                if (jumpCooldown > .5f)
                {
                    jumpCooldown = 0.0f;
                    canJump = true;
                }
                else
                {
                    jumpCooldown += Time.deltaTime;
                }
            }
            
            //ground raycast around players feet
            RaycastHit hit;
            if (
                    Physics.Raycast(transform.position, -Vector3.up, out hit, 1.1f, groundMask) ||
                    Physics.Raycast(transform.position + new Vector3(0.2f, 0, 0),  -Vector3.up, out hit, 1.1f, groundMask) ||
                    Physics.Raycast(transform.position + new Vector3(-0.2f, 0, 0), -Vector3.up, out hit, 1.1f, groundMask) ||
                    Physics.Raycast(transform.position + new Vector3(0, 0, 0.2f),  -Vector3.up, out hit, 1.1f, groundMask) ||
                    Physics.Raycast(transform.position + new Vector3(0, 0, -0.2f), -Vector3.up, out hit, 1.1f, groundMask)
                )
            {
                reachGround = true;
                anim.SetBool("Grounded", true);
            }
            else
            {
                reachGround = false;
                anim.SetBool("Grounded", false);
            }

            //jump force
            if (Input.GetKeyDown(KeyCode.Space) && canJump && reachGround)
            {
                rb.AddForce(playerJumpForce * rb.mass * Vector3.up, appliedForceMode);
                canJump = false;
            }

            #endregion Jumping




            //rotate player to face the direction theyre moving



            //ainimate running
            anim.SetFloat("VelX", _xAxis);
            anim.SetFloat("VelY", _yAxis);

        }


        private void FixedUpdate()
        {
            #region Move Player

            //Move Player
            Vector3 moveDir = Vector3.Normalize(transform.right * _xAxis) + (transform.forward * _yAxis);
            moveDir.y = 0;
            rb.AddForce(moveDir * Time.fixedDeltaTime * currentSpeed, ForceMode.VelocityChange);


            //gravity
            if(!reachGround) rb.AddForce(new Vector3(0, -gravity * rb.mass * Time.fixedDeltaTime, 0));
            else rb.AddForce(new Vector3(0, -4 * rb.mass, 0));
            
            #endregion



        }



        private void OnDrawGizmosSelected()
        {
            Debug.DrawRay(transform.position, -Vector3.up, Color.green);
            Debug.DrawRay(transform.position + new Vector3(0.2f, 0, 0), -Vector3.up, Color.green);
            Debug.DrawRay(transform.position + new Vector3(-0.2f, 0, 0), -Vector3.up, Color.green);
            Debug.DrawRay(transform.position + new Vector3(0, 0, 0.2f), -Vector3.up, Color.green);
            Debug.DrawRay(transform.position + new Vector3(0, 0, -0.2f), -Vector3.up, Color.green);
        }
    }
}

