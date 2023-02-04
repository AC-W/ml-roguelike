using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public enum WeaponType { BOW, SWORD };
public enum ContinuousActionIndex { AIM };
public enum DiscreteActionIndex { UP, DOWN, RIGHT, LEFT, LIGHT_ATTACK, DASH };
public enum PlayerType { PLAYER1, PLAYER2, STAND, BOT };
public enum Team { RED, BLUE };

public struct PlayerState
{
    public float position;
    public float angle;
    public float velocity;
    public float angularVelocity;
    public PlayerState(float position, float angle, float velocity, float angularVelocity)
    {
        this.position = position;
        this.angle = angle;
        this.velocity = velocity;
        this.angularVelocity = angularVelocity;
    }

    public void AddObservations(VectorSensor sensor)
    {
        sensor.AddObservation(position);
        sensor.AddObservation(angle);
        sensor.AddObservation(velocity);
        sensor.AddObservation(angularVelocity);
    }
}
public class PlayerAgent : Agent, IDamageable
{

    private float m_Existential;

    [Header("Component References")]
    public PlayerAgent opponent;

    [Header("Game Stats")]
    [SerializeField]
    public PlayerType playerType = PlayerType.PLAYER1;
    public Team team = Team.RED;

    [Header("Player Settings")]
    public float moveSpeed = 5f;

    // States for action
    public bool hasActed = false;

    [Header("Prefabs")]
    public BowWeapon bowPrefab;
    public SwordWeapon swordPrefab;

    // Private vars

    ActionSegment<int> frameDiscreteActions;
    ActionSegment<float> frameContinuousActions;
    private float health;

    // References to components
    RogueEnvController envController;
    RogueSettings m_rogueSettings;
    private Transform weaponParentTransform;
    private Rigidbody2D rigidbody;
    private SpriteRenderer m_SpriteRenderer;
    private Animator animator;

    private Weapon equippedWeapon;
    private WeaponType equippedWeaponType;
    Vector2 inputAxes;
    bool facingLeft;

    EnvironmentParameters m_ResetParams;

    void Start()
    {
        health = 100f;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        m_SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        weaponParentTransform = transform.Find("Weapon Parent");
    }

    // Update is called once per frame
    void Update()
    {
        //Input
        //left: -1, Right: 1
        inputAxes.x = Input.GetAxisRaw("Horizontal");
        inputAxes.y = Input.GetAxisRaw("Vertical");

        // Load weapon if needed
        if (equippedWeapon == null){
            Weapon weapon = Instantiate(bowPrefab, Vector3.zero, bowPrefab.transform.rotation);
            weapon.transform.parent = weaponParentTransform;
            weapon.transform.localPosition = Vector3.right * 0.45f;
            equippedWeapon = weapon;
        }

        //Handle Aim
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector3 aimDirection = (mousePosition - weaponParentTransform.position).normalized;

        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        weaponParentTransform.eulerAngles = new Vector3(0, 0, angle);

        // Tick weapon
        equippedWeapon.Step(Input.GetMouseButtonDown(0));
    }

    void FixedUpdate()
    {
        //Movement
        rigidbody.MovePosition(rigidbody.position + inputAxes * moveSpeed * Time.fixedDeltaTime);

        if (inputAxes.x != 0 || inputAxes.y != 0) animator.SetBool("running", true);
        else animator.SetBool("running", false);

        if (inputAxes.x != 0)
        {
            if (inputAxes.x < 0) facingLeft = true;
            else facingLeft = false;
        }
        if (facingLeft) m_SpriteRenderer.flipX = true;
        else m_SpriteRenderer.flipX = false;
    }

    public void TakeHit(float damage)
    {
        health -= damage;

        if (this.health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ResetAgent()
    {
        // Reset positions, velocities, angular velocities
        hasActed = false;
    }

    // Called to start the game
    public override void OnEpisodeBegin()
    {
        ResetAgent();
    }

    // public override void CollectObservations(VectorSensor sensor)
    // {
    //     // My state
    //     PlayerState myState = GetPlayerState();
    //     foreach (PlayerState state in myStates)
    //     {
    //         state.AddObservations(sensor);
    //     }

    //     // Opponent state
    //     PlayerState opponentState = opponent.GetPlayerState();
    //     foreach (PlayerState state in opponentStates)
    //     {
    //         state.AddObservations(sensor);
    //     }

    //     // Global state
    //     sensor.AddObservation(envController.totalSteps / envController.maxSteps);
    // }

    public PlayerState GetPlayerState()
    {
        PlayerState ps = new PlayerState();
        return ps;
    }

    public void Move(ActionSegment<float> continuousActions, ActionSegment<int> discreteActions)
    {
        frameContinuousActions = continuousActions;
        frameDiscreteActions = discreteActions;

        hasActed = true;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        var continuousActionsOut = actionsOut.ContinuousActions;
        /*discreteActionsOut[(int)DiscreteActionKey.GOALIE_HOLD] = 1;
        discreteActionsOut[(int)DiscreteActionKey.DEFENDERS_HOLD] = 1;
        discreteActionsOut[(int)DiscreteActionKey.MIDFIELDERS_HOLD] = 1;
        discreteActionsOut[(int)DiscreteActionKey.OFFENSIVE_HOLD] = 1;*/

        if (playerType == PlayerType.PLAYER1)
        {
            if (Input.GetKey(KeyCode.W))
            {
                discreteActionsOut[(int)DiscreteActionIndex.UP] = 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                discreteActionsOut[(int)DiscreteActionIndex.DOWN] = 1;
            }
            if (Input.GetKey(KeyCode.A))
            {
                discreteActionsOut[(int)DiscreteActionIndex.RIGHT] = 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                discreteActionsOut[(int)DiscreteActionIndex.LEFT] = 1;
            }

            if (Input.GetMouseButton(0))
            {
                discreteActionsOut[(int)DiscreteActionIndex.LIGHT_ATTACK] = 1;
            }

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Vector3 aimDirection = (mousePosition - weaponParentTransform.position).normalized;

            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            continuousActionsOut[(int)ContinuousActionIndex.AIM] = angle;

        }
        else if (playerType == PlayerType.PLAYER2)
        {
            // idk controller support?
        }
        else if (playerType == PlayerType.STAND)
        {
            // do nothing hurrah
        }
        else if (playerType == PlayerType.BOT)
        {

        }
    }
}
