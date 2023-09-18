using FPS.Manager;
using FPS.Player;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Windows;

public class PistolScript : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    public Transform gunPivot;
    public int maxRange;
    public float damage;
    public float impactForce;
    public float fireRate;
    private float lastFireTime;

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
        if (Time.time - lastFireTime >= fireRate && currentBullet > 0)
        {
            muzzleFlash.Play();
            currentBullet--;

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

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }
                   
                Destroy(Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-hit.normal)), 5f);

                lastFireTime = Time.time;
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

    //public void EjectShell()
    //{
    //    GameObject bulletShell = PoolForShell.Instance.GetShell();

    //    if (bulletShell != null)
    //    {
    //        bulletShell.transform.position = shellPoint.position;
    //        bulletShell.transform.rotation = shellPoint.rotation;
    //        bulletShell.SetActive(true);

    //        Rigidbody rb = bulletShell.GetComponent<Rigidbody>();
    //        rb.velocity = Vector3.zero;
    //        rb.angularVelocity = Vector3.zero;

    //        Vector3 force = transform.up + transform.right * 25f * UnityEngine.Random.Range(2f, 5f);
    //        rb.AddForce(force, ForceMode.Impulse);
    //        StartCoroutine(DeactivateShellAfterTime(bulletShell, 10f));

    //    }
    //}

    //private IEnumerator DeactivateShellAfterTime(GameObject bulletShell, float time)
    //{
    //    yield return new WaitForSeconds(time);
    //    PoolForShell.Instance.ReturnShellToPool(bulletShell);
    //}
}
