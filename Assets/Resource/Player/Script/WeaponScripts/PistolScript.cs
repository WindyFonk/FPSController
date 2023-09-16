using FPS.Manager;
using FPS.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Windows;

public class PistolScript : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    public Transform gunPivot;
    public int maxRange = 100;
    public float damage = 10;

    public int maxBullet = 8;
    public int currentBullet;

    public float bulletSpreadAngle;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    public LayerMask hitLayer;

    private Animator animator;

    public Transform shellPoint;
    public GameObject shell;

    public Transform magPoint;
    public GameObject mag;

    public Camera _camera;
    public float fov = 68;

    [Header("Recoil system")]
    private RecoilSystem recoilSystem;
    public Animator modelAnimator;

    public PlayerController player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public bool canReload()
    {
        if (currentBullet < maxBullet)
        {
            return true;
        }
        return false;
    }

    private void Start()
    {
        currentBullet = maxBullet;
        Debug.Log(">>> currentBullet: " + currentBullet);

        recoilSystem = GameObject.Find("Main Camera/CameraRot/CameraRecoil").GetComponent<RecoilSystem>();
        Debug.Log(">>> recoilSystem: " + recoilSystem);
    }

    private void Update()
    {
        animator.SetFloat("Speed", Mathf.Abs(player.currentVelocity.y));

        animator.SetInteger("Bullet", currentBullet);
        _camera.fieldOfView = fov;

        UseWeapon();
        AimDownSight();

    }

    private void UseWeapon()
    {
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
            StartCoroutine(Weapon("Reload"));
        }

        else return;
    }

    private IEnumerator Weapon(string weapon)
    {
        //cameraArmAnimator.Play(weapon);
        animator.SetTrigger(weapon);
        modelAnimator.SetTrigger(weapon);
        yield return new WaitForSeconds(0.1f);
        animator.ResetTrigger(weapon);
        modelAnimator.ResetTrigger(weapon);
    }


    private void AimDownSight()
    {
        if (inputManager.Aim)
        {
            Aim();
        }
        else
        {
            StartCoroutine(Unaim());
        }
    }

    private void Aim()
    {
        animator.SetBool("Aim", true);
    }

    private IEnumerator Unaim()
    {
        animator.SetBool("Aim", false);
        yield return new WaitForSeconds(1f);
    }



    public void Reload()
    {
        currentBullet = maxBullet;
    }

    public void EjectMag()
    {
        GameObject magEject = Instantiate(mag, magPoint);
        magEject.transform.SetParent(null);
        magEject.GetComponent<Rigidbody>().velocity = Vector3.zero;
        magEject.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        Destroy(magEject, 5f);
    }

    public void Fire()
    {
        if (currentBullet > 0)
        {
            muzzleFlash.Play();
            currentBullet--;

            recoilSystem.RecoilFire();

            // Bắn một Raycast từ vị trí camera
            RaycastHit hit;
            if (Physics.Raycast(gunPivot.transform.position, gunPivot.transform.forward, out hit, maxRange, hitLayer))
            {
                
                Debug.DrawRay(gunPivot.transform.position, gunPivot.transform.forward * hit.distance, Color.green);
                Debug.Log(hit.transform.name);

                TargetScript target = hit.transform.GetComponent<TargetScript>();

                if (target != null)
                {
                    target.TakeDamage(damage);
                }

                Destroy(Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-hit.normal)), 10f);
            }
        }
    }

    public void EjectShell()
    {
        GameObject bulletShell = Instantiate(shell, shellPoint);
        Vector3 force = transform.up + transform.right * 25f * UnityEngine.Random.Range(2f, 5f);
        bulletShell.GetComponent<Rigidbody>().AddForce(force);
        bulletShell.transform.SetParent(null);
        Destroy(bulletShell, 3f);
    }
}
