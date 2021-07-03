using System;
using System.Collections;
using Cinemachine;
using Joystick_Pack.Scripts.Joysticks;
using Logic;
using Mechanics;
using Mechanics.Character;
using Tools;
using UnityEngine;

namespace Scripts
{
    [RequireComponent(typeof(CharacterController))]
    public class MovementInput : MonoBehaviour
    {
        public static event Action<bool> OnSetColliderRadius;

        public float Velocity;

        [Space] public float InputX;
        public float InputZ;

        public Vector3 desiredMoveDirection;
        public bool blockRotationPlayer;
        public float desiredRotationSpeed = 0.1f;
        public Animator anim;
        public float Speed;
        public float allowPlayerRotation = 0.1f;
        public Camera cam;
        public CharacterController controller;
        public bool isGrounded;
        public VariableJoystick variableJoystick;

        [Header("Animation Smoothing")] [Range(0, 1f)]
        public float HorizontalAnimSmoothTime = 0.2f;

        [Range(0, 1f)] public float VerticalAnimTime = 0.2f;
        [Range(0, 1f)] public float StartAnimTime = 0.3f;
        [Range(0, 1f)] public float StopAnimTime = 0.15f;

        public float verticalVel;
        [SerializeField] private bool isJoystick;
        private Vector3 moveVector;

        [Header("Jump Stuff")] [SerializeField]
        private Transform groundCheck;

        [SerializeField] private float jumpForce = 2f;
        [SerializeField] private float groundDistance = 1f;
        [SerializeField] private float stickToGroundForce = 1f;
        [SerializeField] private float characterGravity = 9.8f;

        [Header("Effects")] [SerializeField] private ParticleSystem boomParticle;


        private bool _canMove = true;
        private Transform _effectsBox;
        private Audio _audio;
        private static readonly int Blend = Animator.StringToHash("Blend");
        private static readonly int JumpTrigger = Animator.StringToHash("JumpTrigger");
        private static readonly int Jump = Animator.StringToHash("Jump");

        private float _danceTimer;
        [SerializeField] private float delayBeforeDancing = 3f;
        private static readonly int DanceB = Animator.StringToHash("DanceB");

        private bool _isMoving;


        // Use this for initialization
        private void Start()
        {
            _audio = GetComponent<Audio>();
            anim = GetComponent<Animator>();
            cam = Camera.main;
            controller = GetComponent<CharacterController>();
            _effectsBox = GameObject.FindWithTag(Tags.EffectsBox).transform;
        }

        private void FixedUpdate()
        {
            if (!_isMoving && _danceTimer < delayBeforeDancing)
            {
                _danceTimer += Time.deltaTime;
            }
            else if(_isMoving && _danceTimer != 0)
            {
                _danceTimer = 0;
            }
        
            if (_danceTimer >= delayBeforeDancing && !anim.GetBool(DanceB))
            {
                anim.SetBool(DanceB, true);
            }
            else if (_danceTimer < delayBeforeDancing && anim.GetBool(DanceB))
            {
                anim.SetBool(DanceB, false);
            }
        }

        // Update is called once per frame
        private void Update()
        {
            InputMagnitude();
            JumpSector();
        }

        private void PlayerMoveAndRotation()
        {
            if (!_canMove || !PlayerLogic.CanMove) return;
            var forward = cam.transform.forward;
            var right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            desiredMoveDirection = forward * InputZ + right * InputX;

            if (blockRotationPlayer == false)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection),
                    desiredRotationSpeed);
                controller.Move(desiredMoveDirection * (Time.deltaTime * Velocity));
            }
        }

        public void LookAt(Vector3 pos) =>
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);

        public void RotateToCamera(Transform t)
        {
            var camera = Camera.main;
            var forward = cam.transform.forward;
            var right = cam.transform.right;

            desiredMoveDirection = forward;

            t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection),
                desiredRotationSpeed);
        }

        private void InputMagnitude()
        {
            //Calculate Input Vectors
            if (!isJoystick)
            {
                InputX = Input.GetAxis("Horizontal");
                InputZ = Input.GetAxis("Vertical");
            }
            else
            {
                InputX = variableJoystick.Horizontal;
                InputZ = variableJoystick.Vertical;
            }

            //Calculate the Input Magnitude
            Speed = new Vector2(InputX, InputZ).sqrMagnitude;

            _isMoving = Speed != 0f;

            //Physically move player

            if (Speed > allowPlayerRotation)
            {
                anim.SetFloat(Blend, Speed, StartAnimTime, Time.deltaTime);
                PlayerMoveAndRotation();
                _audio.MoveAudio(true);
            }
            else if (Speed < allowPlayerRotation)
            {
                anim.SetFloat(Blend, Speed, StopAnimTime, Time.deltaTime);
                _audio.MoveAudio(false);
            }

            // Debug.Log($"x {InputX}");
            // Debug.Log($"z {InputZ}");
            // if (InputX != 0 && InputZ != 0)
            // else
        }

        private void JumpSector()
        {
            int groundMask = LayerMask.GetMask("Ground");
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            //If The Player Is On The Ground Stick To Ground And Reset Vertical Velocity
            if (isGrounded && verticalVel < 0)
                verticalVel = -stickToGroundForce;

            //Jump If The Player Is Grounded
            if (isGrounded && JoystickPointer.JumpState == PlayerJumpStates.StartJump)
            {
                verticalVel += jumpForce * stickToGroundForce;
                anim.SetTrigger(JumpTrigger);
                anim.SetBool(Jump, true);
                JoystickPointer.JumpState = PlayerJumpStates.Check;
                _canMove = false;
            }

            if (JoystickPointer.JumpState == PlayerJumpStates.Boom)
            {
                JoystickPointer.JumpState = PlayerJumpStates.EndBoom;
                // Debug.Log($"here");
                anim.SetBool(Jump, false);
                // anim.SetTrigger("Mutant");
                StartCoroutine(WaitBeforeCharacterJumpEnded());
            }

            if (JoystickPointer.JumpState == PlayerJumpStates.None && !_canMove)
            {
                _canMove = true;
            }

            //Gravity
            verticalVel -= characterGravity * Time.deltaTime;

            //Apply These Calculations To The Actual Player Controller
            Vector3 fallVector = new Vector3(0, verticalVel, 0);
            controller.Move(fallVector * Time.deltaTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitBeforeCharacterJumpEnded()
        {
            yield return new WaitForSeconds(.7f);
            ParticleSystem newBoom =
                Instantiate(boomParticle, transform.position + Vector3.up * 1f,
                    Quaternion.Euler(90, 0, 0));
            newBoom.transform.SetParent(_effectsBox);

            _audio.PlaySfx(_audio.jumpClip);

            CinemachineEffects.Instance.MutantEffect();
            OnSetColliderRadius?.Invoke(true);
            yield return new WaitForSeconds(1.3f);
            JoystickPointer.JumpState = PlayerJumpStates.None;
            OnSetColliderRadius?.Invoke(false);
        }
    }
}