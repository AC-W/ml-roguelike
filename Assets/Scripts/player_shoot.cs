using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_shoot : MonoBehaviour
{
    public PlayerAgent player;
    public Animator m_Animator;
    public Transform BowEndPointTransform;

    // public event EventHandler<OnBowShootEventArgs> OnShoot;
    // public class OnBowShootEventArgs : EventArgs 
    // {
    //     public Vector3 BowEndPointPosition;
    //     public Vector3 shootPosition;
    // }

    private bool fired;

    private Vector3 mousePosition;

    [SerializeField] private Transform pfArrow;

    void start()
    {
        m_Animator = gameObject.GetComponent<Animator>();
        fired = false;
        
    }

    void Step(bool fireDown)
    {
        if (fireDown)
        {
            fired = true;
            m_Animator.SetBool("shoot", true);
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;
            // OnShoot?.Invoke(this,new OnBowShootEventArgs
            // {
            //     BowEndPointPosition = BowEndPointTransform.position,
            //     shootPosition = mousePosition,
            // });
        }
        if (m_Animator.GetCurrentAnimatorStateInfo(0).IsName("shoot"))
        {
            if (fired)
            {

                Transform arrow = Instantiate(pfArrow, BowEndPointTransform.position, Quaternion.identity);
                Vector3 shootDir = (mousePosition - BowEndPointTransform.position).normalized;
                arrow.GetComponent<ProjectileArrow>().SetUp(shootDir);
                fired = false;
            }

            m_Animator.SetBool("shoot", false);
        }
    }
}
