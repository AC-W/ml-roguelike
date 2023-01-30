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

    private Vector3 mousePosition;

    [SerializeField] private Transform arrowPrefab;

    void Awake()
    {
        bowEndPointTransform = transform.Find("BowEndPoint");
        m_Animator = gameObject.GetComponent<Animator>();
        fired = false;
    }

    public override void Step(bool fireDown)
    {
        if (fireDown)
        {
            m_Animator.SetBool("shoot", true);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            fired = true;
            // OnShoot?.Invoke(this,new OnBowShootEventArgs
            // {
            //     BowEndPointPosition = BowEndPointTransform.position,
            //     shootPosition = mousePosition,
            // });
        }

        firing = m_Animator.GetCurrentAnimatorStateInfo(0).IsName("shoot");
        if (firing)
        {
            if (fired)
            {

                Transform arrow = Instantiate(arrowPrefab, bowEndPointTransform.position, Quaternion.identity);
                Vector3 shootDir = (mousePosition - bowEndPointTransform.position).normalized;
                arrow.GetComponent<ProjectileArrow>().SetUp(shootDir);
                fired = false;
            }

            m_Animator.SetBool("shoot", false);
        }
    }
}
