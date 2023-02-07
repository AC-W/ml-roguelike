using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
public class ShootOnce : Agent, IDamageable, MyAgents
{

    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform projectile_launcher;
    [SerializeField] private Transform projectile;
    private Transform projectile_endpoint;

    private Transform weaponParentTransform;
    private Transform equippedWeapon;

    [SerializeField] private float health;
    [SerializeField] private float cooldown;

    private Vector3 self_start_loc;
    private Vector3 target_start_loc;

    private bool can_fire;

    private int num_shots;
    private float rw;

    public override void OnEpisodeBegin() 
    {
        for (int i = 0;i < transform.childCount;i++)
        {
            if (transform.GetChild(i).gameObject.name != "Weapon Parent")
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        num_shots = 0;
        rw = 0f;

        float [] choices = {-4f,4f};

        int index = Random.Range(0,choices.Length);
        float x_offset = choices[index];

        index = Random.Range(0,choices.Length);
        float y_offset = choices[index];

        // transform.position = self_start_loc + new Vector3(Random.Range(-1f,1f),Random.Range(-1f,1f),0);

        float angle = Random.Range(0, Mathf.PI*2);
        float r = 4f;
        Vector2 pos2d = new Vector2(Mathf.Sin(angle)*r,Mathf.Cos(angle)*r);
        // targetTransform.position = target_start_loc + new Vector3(pos2d.x,pos2d.y,0);
        // targetTransform.GetComponent<ObjectBox>().SetHealth(100000f);

        health = 100f;
        weaponParentTransform = transform.Find("Weapon Parent");
        can_fire = true;
    }

    void Awake()
    {
        target_start_loc = targetTransform.position;
        self_start_loc = gameObject.transform.position;

        num_shots = 0;
        rw = 0f;
        weaponParentTransform = transform.Find("Weapon Parent");
        can_fire = true;

        if (equippedWeapon == null){
            Transform weapon = Instantiate(projectile_launcher, Vector3.zero, projectile_launcher.transform.rotation);
            weapon.transform.parent = weaponParentTransform;
            weapon.transform.localPosition = Vector3.right * 0.35f;
            equippedWeapon = weapon;
        }
    }

    public void TakeHit(float damage)
    {
        health -= damage;

        if (this.health <= 0)
        {
            // EndEpisode();
            health = 100f;
            // Destroy(gameObject);
        }
    }
    
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(targetTransform.position);
    }
    
    public override void OnActionReceived(ActionBuffers actions)
    {
        float aim_angle = actions.ContinuousActions[0];
        // int shoot = actions.DiscreteActions[0];

        weaponParentTransform.eulerAngles = new Vector3(0, 0, aim_angle*360);
        
        if (can_fire) 
        {
            projectile_endpoint = equippedWeapon.Find("BowEndPoint");
            Transform arrow = Instantiate(projectile, projectile_endpoint.position, Quaternion.identity);
            Vector3 shootDir = (projectile_endpoint.position - weaponParentTransform.position).normalized;
            arrow.GetComponent<ProjectileArrow>().SetUp(shootDir);
            arrow.parent = transform;
            can_fire = false;
            Invoke("Reset_shot",cooldown);
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

        // ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        // if (Input.GetMouseButtonDown(0))    discreteActions[0] = 1;
        // else                                discreteActions[0] = 0;
    }

    public void GetResult(Collider2D col,Transform projectile)
    {
        Vector3 hit_pos = projectile.position;
        Vector3 target_pos = targetTransform.position;
        float dis = Vector3.Distance(target_pos,hit_pos);
        
        dis = Mathf.Abs(dis);
        // if (col != null & col.gameObject == targetTransform.gameObject)
        // {
        //     Debug.Log(1f);
        //     SetReward(1f);
        // }
        // else 
        // {
        //     Debug.Log(-1f);
        //     SetReward(-1f);
        // }
        rw = -dis;
        Debug.Log(rw/2);
        SetReward(rw/2);
        // EndEpisode();
    }

    public void Reset_shot()
    {
        can_fire = true;
    }
}
