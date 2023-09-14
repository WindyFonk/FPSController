
﻿using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    public LayerMask hitLayer;

    private Animator animator;

    public Transform shellPoint;
    public GameObject shell;

    public Transform magPoint;
    public GameObject mag;

    public Camera _camera;
    public float fov = 68;


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
        maxRange = 100;
        damage = 10f;
        maxBullet = 15;

        bulletSpreadAngle = 0.15f;
        currentBullet = maxBullet;


    }

    private void Update()
    {
        animator.SetInteger("Bullet", currentBullet);
        _camera.fieldOfView = fov;

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
        Destroy(magEject, 10f);
    }

    public void Fire()
    {
        if (currentBullet > 0)
        {

            muzzleFlash.Play();
            currentBullet--;

            // random x and y from z
            //float randomSpreadX = Random.Range(-2.0f, 2.0f);
            //float randomSpreadY = Random.Range(-2.0f, 2.0f);

            //Quaternion spreadRotation = Quaternion.Euler(randomSpreadX, randomSpreadY, 0);

            //Vector3 rayDirection = spreadRotation * gunPivot.transform.forward;

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
        Destroy(bulletShell, 10f);
    }
}
