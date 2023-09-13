using FPS.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

namespace FPS.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float animationBlendSpeed = 8.9f;

        [SerializeField] Transform _cameraRoot;
        [SerializeField] Transform _camera;
        [SerializeField] Transform spine;

        [SerializeField] float upLimit = -40f;
        [SerializeField] float downLimit = 70f;
        [SerializeField] float mouseSensitive = 21f;

        [SerializeField] GameObject cameraArm;
        [SerializeField] GameObject weaponRoot;
        [SerializeField] Animator cameraArmAnimator;



        private Rigidbody rb;
        private InputManager inputManager;
        private Animator animator;
        private bool _hasAnimator;

        // Pistol Script funtion
        private PistolScript pistolScript;


        private int _Xvelocity, _Yvelocity;
        private int _EquipWeapon;

        private const float crouchSpeed = 2f;
        private const float walkSpeed = 3f;
        private const float runSpeed = 6f;

        private float _xRotation;

        private Vector2 currentVelocity;
        private Vector2 animVelocity;

        public bool canControl = true;
        public bool canControlCamera = true;
        public bool equipWeaon = false;
        private bool buttonPressed = false;

        private int PlayerAnimatorLayer;
        private float LayerWeightVelocity;

        private bool isCrouching;
        float speed;

        [Range(0, 1)]
        public float pistolArmWeight;
        public PistolScript pistol;


        void Start()
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            UnityEngine.Cursor.visible = false;

            _hasAnimator = TryGetComponent<Animator>(out animator);
            rb = GetComponent<Rigidbody>();
            inputManager = GetComponent<InputManager>();

            _Xvelocity = Animator.StringToHash("X_Velocity");
            _Yvelocity = Animator.StringToHash("Y_Velocity");
            _EquipWeapon = Animator.StringToHash("Equip_Weapon");

            PlayerAnimatorLayer = animator.GetLayerIndex("Pistol_Layer");

            // 
            pistolScript = GetComponent<PistolScript>();
        }

        private void FixedUpdate()
        {
            if (!canControl) return;
            Move();
            Crouch();
            EquipWeapon();
            UseWeapon();
        }

        private void LateUpdate()
        {
            CamMovements();
        }

        private void Move()
        {
            if (!_hasAnimator) return;
            speed = inputManager.Run ? runSpeed : walkSpeed;
            /*if (inputManager.Run && inputManager.Move.y==1)
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }*/

            if (inputManager.Move == Vector2.zero) speed = 0.1f;

            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputManager.Move.x * speed, animationBlendSpeed * Time.fixedDeltaTime);
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, inputManager.Move.y * speed, animationBlendSpeed * Time.fixedDeltaTime);

            var xVelocityDif = currentVelocity.x - rb.velocity.x;
            var yVelocityDif = currentVelocity.y - rb.velocity.z;
            rb.AddForce(transform.TransformVector(new Vector3(xVelocityDif, 0, yVelocityDif)), ForceMode.VelocityChange);

            animator.SetFloat(_Xvelocity, currentVelocity.x);
            cameraArmAnimator.SetFloat("Speed", Mathf.Abs(currentVelocity.y));
            animator.SetFloat(_Yvelocity, currentVelocity.y);

        }


        private void CamMovements()
        {
            if (!canControlCamera) return;
            if (!_hasAnimator) return;

            var Mouse_X = inputManager.Look.x;
            var Mouse_Y = inputManager.Look.y;


            _xRotation -= Mouse_Y * mouseSensitive * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, upLimit, downLimit);

            _camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            if (equipWeaon)
            {
                spine.localRotation = Quaternion.Euler(Mathf.Lerp(spine.localRotation.x+6,_xRotation,2f), 0, 0);
            }
            _camera.position = _cameraRoot.position;


            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mouse_X * mouseSensitive * Time.smoothDeltaTime, 0));
        }

        private void Crouch()
        {
            animator.SetBool("isCrouching", inputManager.Crouch);
        }

        private void SetArm()
        {
            if (equipWeaon)
            {
                cameraArm.SetActive(equipWeaon);
                animator.SetLayerWeight(PlayerAnimatorLayer, 1f);
                animator.SetLayerWeight(PlayerAnimatorLayer, 1f);
                weaponRoot.SetActive(true);



            }
            else
            {
                StartCoroutine(Armanim());
            }


        }

        private IEnumerator Armanim()
        {
            yield return new WaitForSeconds(1);
            weaponRoot.SetActive(false);
            cameraArm.SetActive(false);
            animator.SetLayerWeight(PlayerAnimatorLayer, 0);
        }


        private void EquipWeapon()
        {
            if (!_hasAnimator) return;
            if (!inputManager.Equip) return;
            if (buttonPressed) return;
            buttonPressed = true;

            equipWeaon = !equipWeaon;
            SetArm();

            animator.SetBool(_EquipWeapon, equipWeaon);
            cameraArmAnimator.SetBool(_EquipWeapon, equipWeaon);
            weaponRoot.SetActive(equipWeaon);
            StartCoroutine(WaitTilNextPress(2f));
        }

        IEnumerator WaitTilNextPress(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            buttonPressed = false;
        }

        private void UseWeapon()
        {
            if (!equipWeaon) return;
            if (inputManager.Shoot)
            {
                StartCoroutine(Weapon("Shoot"));
            }

            else if (inputManager.Knife)
            {
                StartCoroutine(Weapon("Knife"));
            }

            if (inputManager.Reload)
            {
                if (!pistol.canReload()) return;
                StartCoroutine(Weapon("Reload"));
            }

            else return;
        }

        private IEnumerator Weapon(string weapon)
        {
            //cameraArmAnimator.Play(weapon);
            cameraArmAnimator.SetTrigger(weapon);
            animator.SetTrigger(weapon);
            yield return new WaitForSeconds(0.2f);
            cameraArmAnimator.ResetTrigger(weapon);
            animator.ResetTrigger(weapon);

        }

        public void setControl(bool canControl)
        {
            this.canControl = canControl;
            GetComponent<Collider>().enabled = canControl;
        }
    }
}
