using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_aim : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        //Handle Aim
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 aimDirection = (mousePosition - transform.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y,aimDirection.x) * Mathf.Rad2Deg;

        transform.eulerAngles = new Vector3(0,0,angle);

        //Handle shoot
    }
}
