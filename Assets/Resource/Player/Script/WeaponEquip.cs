using FPS.Player;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEquip : MonoBehaviour
{
    [SerializeField]
    private ArmIK m_leftArmIK = default;
    [SerializeField]
    private ArmIK m_rightArmIK = default;
    private PlayerController player;
    // Start is called before the first frame update
    void Start()
    {
        m_rightArmIK.enabled = false;
        m_leftArmIK.enabled = false;
        player = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        m_rightArmIK.enabled = player.equipWeaon;
        m_leftArmIK.enabled = player.equipWeaon;
    }
}
