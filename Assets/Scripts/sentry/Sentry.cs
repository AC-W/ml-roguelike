using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class Sentry : Agent, IDamageable
{

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform projectile_launcher;
    [SerializeField] private Transform projectile;
    private Transform projectile_endpoint;

    private Transform weaponParentTransform;
    private Transform equippedWeapon;

    [SerializeField] private float health;
    [SerializeField] private float cooldown;
    private bool can_fire;

    private int num_shots;
    private float rw;

    public override void OnEpisodeBegin() 
    {
        Debug.Log(rw);
        for (int i = 0;i < transform.childCount;i++)
        {
            if (transform.GetChild(i).gameObject.name != "Weapon Parent")
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        num_shots = 0;
        rw = 0f;
        transform.position = new Vector3(4f,0f,0f);
        transform.localPosition = new Vector3(Random.Range(3f,7f),Random.Range(-4f,4f),0);
        targetTransform.position = new Vector3(-4f,0,0);
        targetTransform.localPosition = new Vector3(Random.Range(-7f,-3f),Random.Range(-4f,4f),0);
        targetTransform.GetComponent<ObjectBox>().SetHealth(100f);
        health = 100;
        weaponParentTransform = transform.Find("Weapon Parent");
        can_fire = true;
    }

    void Awake()
    {
        num_shots = 0;
        rw = 0f;
        weaponParentTransform = transform.Find("Weapon Parent");
        can_fire = true;
    }

    public void TakeHit(float damage)
    {
        // health -= damage;

        if (this.health <= 0)
        {
            // EndEpisode();
            // Destroy(gameObject);
        }
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        if (can_fire)
        {
            sensor.AddObservation(transform.position);
            sensor.AddObservation(targetTransform.position);
        }
        else
        {
            sensor.AddObservation(new Vector3(0,0,0));
            sensor.AddObservation(new Vector3(0,0,0));
        }
        
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (can_fire)
        {
            float aim_angle = actions.ContinuousActions[0];
            int shoot = actions.DiscreteActions[0];

            if (equippedWeapon == null){
                Transform weapon = Instantiate(projectile_launcher, Vector3.zero, projectile_launcher.transform.rotation);
                weapon.transform.parent = weaponParentTransform;
                weapon.transform.localPosition = Vector3.right * 0.35f;
                equippedWeapon = weapon;
            }

            weaponParentTransform.eulerAngles = new Vector3(0, 0, aim_angle*360);
            
            if (can_fire & shoot == 1) 
            {
                projectile_endpoint = equippedWeapon.Find("BowEndPoint");
                Transform arrow = Instantiate(projectile, projectile_endpoint.position, Quaternion.identity);
                Vector3 shootDir = (projectile_endpoint.position - weaponParentTransform.position).normalized;
                num_shots += 1;
                // Debug.Log(shootDir);
                arrow.GetComponent<ProjectileArrow>().SetUp(shootDir);
                arrow.parent = transform;
                can_fire = false;
                // rw += 1f;
                // AddReward(1f);
                Invoke("Reset_shot",cooldown);
            }
            // if (num_shots > 10)
            // {
            //     EndEpisode();
            // }
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;
        Vector3 aimDirection = (mousePosition - weaponParentTransform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        continuousActions[0] = angle/360;

        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        if (Input.GetMouseButtonDown(0))    discreteActions[0] = 1;
        else                                discreteActions[0] = 0;
    }

    public void GetResult(Collider2D col)
    {
        if (col != null & col.gameObject.name == targetTransform.gameObject.name)
        {
            // Debug.Log("HIT");
            rw += 10f;
            AddReward(10f);
        }
        // else
        // {
        //     rw -= 1f;
        //     AddReward(-1f);
        // }
    }

    public void Reset_shot()
    {
        can_fire = true;
    }
}
