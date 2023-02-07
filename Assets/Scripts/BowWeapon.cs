using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowWeapon : Weapon
{
    //public PlayerAgent player;
    private Animator m_Animator;
    private Transform bowEndPointTransform;

    // public event EventHandler<OnBowShootEventArgs> OnShoot;
    // public class OnBowShootEventArgs : EventArgs 
    // {
    //     public Vector3 BowEndPointPosition;
    //     public Vector3 shootPosition;
    // }

    private bool fired;
    private bool can_fire;
    [SerializeField] private float cooldown;

    private Vector3 mousePosition;

    [SerializeField] private Transform arrowPrefab;

    public string Type;

    void Awake()
    {
        bowEndPointTransform = transform.Find("BowEndPoint");
        m_Animator = gameObject.GetComponent<Animator>();
        fired = false;
        can_fire = true;
        Type = "weapons";
    }

    public override void Step(bool fireDown)
    {
        // TODO:
        // Animation timing

        
        // m_Animator.speed = 0.20f/cooldown;
        firing = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("shoot");
        if (fireDown && !fired && can_fire)
        {
            // m_Animator.SetBool("shoot", true);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            fired = true;
            can_fire = false;
        }
        if (fired)
        {
            Transform arrow = Instantiate(arrowPrefab, bowEndPointTransform.position, Quaternion.identity);
            Vector3 shootDir = (mousePosition - bowEndPointTransform.position).normalized;
            arrow.GetComponent<ProjectileArrow>().SetUp(shootDir);
            fired = false;
            Invoke("Reset_Bow",cooldown);
        }
    }

    public void Reset_Bow()
    {
        // m_Animator.SetBool("shoot", false);
        can_fire = true;
    }
}
