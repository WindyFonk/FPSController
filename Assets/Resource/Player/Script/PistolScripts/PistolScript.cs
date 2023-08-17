using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolScript : MonoBehaviour
{
    public Transform gunPivot;
    public int maxRange;
    public float damage;

    public int maxBullet;
    public int currentBullet;

    public float bulletSpreadAngle;

    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        maxRange = 100;
        damage = 10f;
        maxBullet = 15;

        bulletSpreadAngle = 0.15f;
        currentBullet = maxBullet;
    }

    private void Update()
    {
        animator.SetInteger("Bullet", currentBullet);
    }

    public void Reload()
    {
        currentBullet = maxBullet;
        Debug.Log(">>> Reload mag !");
        Debug.Log(">>> currentBullet after reload: " + currentBullet);
    }

    public void Fire()
    {
        if (currentBullet > 0)
        {
            muzzleFlash.Play();
            currentBullet--;

            // Bắn một Raycast từ vị trí camera
            RaycastHit hit;
            if (Physics.Raycast(gunPivot.transform.position, gunPivot.transform.forward, out hit, maxRange))
            {
                Debug.DrawRay(gunPivot.transform.position, gunPivot.transform.forward * hit.distance, Color.green);
                Debug.Log(hit.transform.name);

                TargetScript target = hit.transform.GetComponent<TargetScript>();

                if (target != null)
                {
                    target.TakeDamage(damage);
                    Debug.Log(">>> damage: " + damage);
                }

                Destroy(Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-hit.normal)), 2f);
            }
            
        }
    }
}
