using FPS.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        private int _Xvelocity, _Yvelocity;
        private int _EquipWeapon;

        private const float walkSpeed = 2f;
        private const float runSpeed = 4f;

        private float _xRotation;

        private Vector2 currentVelocity;
        private Vector2 animVelocity;

        public bool canControl = true;
        public bool canControlCamera = true;
        public bool equipWeaon = false;
        private bool buttonPressed = false;

        private int PlayerAnimatorLayer;
        private float LayerWeightVelocity;



        void Start()
        {
            Cursor.visible = false;
            _hasAnimator = TryGetComponent<Animator>(out animator);
            rb = GetComponent<Rigidbody>();
            inputManager = GetComponent<InputManager>();

            _Xvelocity = Animator.StringToHash("X_Velocity");
            _Yvelocity = Animator.StringToHash("Y_Velocity");
            _EquipWeapon = Animator.StringToHash("Equip_Weapon");

            PlayerAnimatorLayer = animator.GetLayerIndex("Pistol_Layer");
        }

        private void FixedUpdate()
        {
            if (!canControl) return;
            Move();
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
            float speed = inputManager.Run ? runSpeed : walkSpeed;
            cameraArmAnimator.SetFloat("Speed", speed);
            /*if (inputManager.Run && inputManager.Move.y==1)
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }*/
            
            if (inputManager.Move == Vector2.zero) speed = 0.1f;

            currentVelocity.x = Mathf.Lerp(currentVelocity.x, inputManager.Move.x * speed, animationBlendSpeed*Time.fixedDeltaTime);
            currentVelocity.y = Mathf.Lerp(currentVelocity.y, inputManager.Move.y * speed, animationBlendSpeed* Time.fixedDeltaTime);

            var xVelocityDif = currentVelocity.x - rb.velocity.x;
            var yVelocityDif = currentVelocity.y - rb.velocity.z;
            rb.AddForce(transform.TransformVector(new Vector3(xVelocityDif, 0, yVelocityDif)), ForceMode.VelocityChange);

            animator.SetFloat(_Xvelocity, currentVelocity.x);
            animator.SetFloat(_Yvelocity, currentVelocity.y);

        }

        private void CamMovements()
        {
            if (!canControlCamera) return;
            if (!_hasAnimator) return;
            if (buttonPressed) return;

            var Mouse_X = inputManager.Look.x;
            var Mouse_Y = inputManager.Look.y;


            _xRotation -= Mouse_Y * mouseSensitive * Time.smoothDeltaTime;
            _xRotation = Mathf.Clamp(_xRotation, upLimit, downLimit);

            _camera.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            if (equipWeaon)
            {
                spine.localRotation = Quaternion.Euler(_xRotation, 0, 0);
            }
            else
            {
                _camera.position = _cameraRoot.position;
            }

            rb.MoveRotation(rb.rotation * Quaternion.Euler(0, Mouse_X * mouseSensitive * Time.smoothDeltaTime, 0));
        }

        private void SetArm()
        {
            if (equipWeaon)
            {
                cameraArm.SetActive(equipWeaon);
                animator.SetLayerWeight(PlayerAnimatorLayer, 1);

            }
            else
            {
                StartCoroutine(Armanim());
                animator.SetLayerWeight(PlayerAnimatorLayer, 0);

            }
        }

        private IEnumerator Armanim()
        {
            yield return new WaitForSeconds(1);
            cameraArm.SetActive(false);
            weaponRoot.SetActive(equipWeaon);
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
            StartCoroutine(WaitTilNextPress(0.5f));
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
            else if (inputManager.Knife) { 
                StartCoroutine(Weapon("Knife"));
            }

            if (inputManager.Reload)
            {
                StartCoroutine(Weapon("ReloadFull"));
            }

            else return;
        }

        private IEnumerator Weapon(string weapon)
        {
            cameraArmAnimator.Play(weapon);
            yield return new WaitForSeconds(0.2f);
            //cameraArmAnimator.ResetTrigger(weapon);
        }

        public void setControl(bool canControl)
        {
            this.canControl = canControl;
            GetComponent<Collider>().enabled = canControl;
        }
    }
}
