using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KUNAI.Player {
    public class PlayerMovement : MonoBehaviour
    {
        InputManager inputManager;
        [HideInInspector]
        public Rigidbody rb;
        public Transform orientation;
        
        Vector3 direction;

        public enum State {
            Free,
            Wallrunning
        }

        public State state;

        [Header("Movement")]
        public float speed = 10f;
        public float airSpeedMultiplier = 0.5f;
        public float groundDrag;
        public float airDrag;

        [Header("Jumping")]
        public float jumpForce;

        [Header("Ground Check")]
        public float height;
        public float groundCheckLength;
        public LayerMask groundLayers;
        
        public bool onGround;

        [Header("Slopes")]
        public float maxSlopeAngle;
        public float slopeCheckLength;
        RaycastHit slopeHit;

        [Header("Wallrunning")]
        public float wallRunSpeed;
        public float wallSnapForce = 100;
        [Tooltip("X handles side force (away from wall), Y handles vertical force.")]
        public Vector2 wallJumpForce;

        [Header("Wallrunning - Detection")]
        public LayerMask wallLayers;
        public float wallCheckLength;
        RaycastHit leftWallHit;
        RaycastHit rightWallHit;
        public bool wallLeft;
        public bool wallRight;

        void Start()
        {
            inputManager = InputManager.GetInstance();

            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            CheckGround();

            if (onGround) {
                rb.drag = groundDrag;
            } else {
                rb.drag = airDrag;
            }

            if (inputManager.jump.down) {
                if(onGround) {
                    Jump();
                }
                if(state == State.Wallrunning) {
                    WallJump();
                }
                
            }
        }

        void FixedUpdate() {
            rb.useGravity = true;

            CheckWall();

            if (wallLeft || wallRight) {
                StartWallRun();
            } else {
                StopWallRun();
            }

            switch (state) {
                case State.Free:
                default:
                    HandleLocomotion();
                    break;
                case State.Wallrunning:
                    HandleWallRun();
                    break;
            }

            LimitSpeed();
            
        }

        void CheckGround() {
            onGround = Physics.Raycast(transform.position, Vector3.down, height * 0.5f + groundCheckLength, groundLayers);
        }

        void HandleLocomotion() {
            direction = (orientation.forward * inputManager.movement.y) + (orientation.right * inputManager.movement.x);
            Vector3 moveDir = direction.normalized;
            if(OnSlope()) {
                moveDir = SlopeMoveDirection();
            }

            rb.AddForce(direction.normalized * speed * (onGround ? 1 : airSpeedMultiplier), ForceMode.Force);
        }

        void LimitSpeed() {
            // Factors Y velocity out of equation, avoiding weird interactions with jumping and gravity.
            Vector3 flatVel = new Vector3(rb.velocity.x, 0, rb.velocity.y);

            if(flatVel.magnitude > speed) {
                Vector3 limitedVel = flatVel.normalized * speed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }

        bool OnSlope() {
            // Shoots a raycast down, getting the angle of the ground below.
            if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, height * 0.5f + slopeCheckLength)) {
                float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

                // Returns true and exists if the ground isn't steeper than the max angle, and isn't flat.
                return angle < maxSlopeAngle && angle != 0;
            }
            
            // If the code reaches this point, it's either on flat ground, too steep an incline, or literally in the air.
            return false;
        }

        Vector3 SlopeMoveDirection() {
            // Angles the move direction based on a slope, so it moves up inclines instead of against them.
            return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
        }

        void Jump() {
            rb.AddForce(jumpForce * orientation.up, ForceMode.Impulse);
        }

        void CheckWall() {
            wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckLength, wallLayers);
            wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckLength, wallLayers);
        }

        void StartWallRun() {
            state = State.Wallrunning;
        }

        void ResetYVel() {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z); 
        }

        Vector3 GetWallNormal() {
            return wallRight ? rightWallHit.normal : leftWallHit.normal;
        }

        void HandleWallRun() {
            rb.useGravity = false;
            ResetYVel();
            
            Vector3 wallNormal = GetWallNormal();


            // Gets the forward direction based off the wall normal.
            // It actually returns a backwards direction if the wall's on the right, so we multiply it by -1.

            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up)  * (wallRight ? -1 : 1);

            rb.AddForce (wallForward * wallRunSpeed, ForceMode.Force);

            rb.AddForce(-wallNormal * wallSnapForce, ForceMode.Force);

        }

        void StopWallRun() {
            state = State.Free;
        }

        void WallJump() {
            Vector3 wallNormal = GetWallNormal();
            
            // Wall Jump Force Vector3. Wasn't pressed about the name since it's a temporary variable.
            Vector3 wjfv3 = (transform.up * wallJumpForce.y) + (wallNormal * wallJumpForce.x);

            // Reset Y velocity so the wall jump height's consistent.
            ResetYVel();
            rb.AddForce(wjfv3, ForceMode.Impulse);
        }
    }
}