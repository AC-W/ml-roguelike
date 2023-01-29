using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public SpriteRenderer m_SpriteRenderer;
    public Animator animator;
    Vector2 movement;
    bool facingLeft;

    void Start()
    {
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        //left: -1, Right: 1
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        //Movement
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);

        if (movement.x != 0 || movement.y != 0) animator.SetBool("running", true);
        else                                    animator.SetBool("running", false);

        if (movement.x != 0)
        {
            if (movement.x < 0) facingLeft = true;
            else facingLeft = false;
        }
        if (facingLeft) m_SpriteRenderer.flipX = true;
        else m_SpriteRenderer.flipX = false;
    }

}
