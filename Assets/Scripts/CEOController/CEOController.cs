using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events = System.ValueTuple<CEOController.Event, CEOController.Event, CEOController.Event, CEOController.Event>;
// (Event,   Event, Event, Event)
//  Default, Fixed, Late,  Cleanup

public class CEOController : MonoBehaviour
{
    public Transform player, boss;
    public ButtonControls startFightButton;
    public BossPath[] paths;
    public float stunTime = 6f;
    [SerializeField, Range(0.001f, 6.0f)] float stage1AttackRange = 3f;
    [SerializeField, Range(0f, 6.0f)] float stage1AttackCooldown = 2f;

    public MessageController.MessageDesc[] messages;
    public MessageController.MessageDesc[] StartFightMessages;
    public MessageController.MessageDesc[] defeatMessages;

    BossPath path;
    BossTarget target;
    Disable empDisable;
    Grapple grapple;

    DroneAttack stage1Attack;
    BossAnimation animate;
    Animator animator;
    GameObject stunLightning;
    [SerializeField] RemovableObject[] removableItems;
    GameObject[] removedParticles;
    public ParticleSystem killExplosion;

    [SerializeField] GameObject blueprint;

    public enum BossState { idle, entry, conversation, startFight, stage1, stage2, stage3, defeat, LEN };
    public enum RemovableItemType { nozzle_back, nozzle_l, nozzle_r }; // in this order
    public delegate void Event(); // run on Unity events

    BossState state;
    bool initState;
    Events stateEvents;
    Event UpdateState, FixedUpdateState, LateUpdateState, CleanupState;

    bool vulnerable;
    bool stunned;
    bool queuedMessages;
    public bool nextTarget;
    public bool nextPath;

    float rotationSpeed = 10f;
    float speedMultiplier = 10f;

    float stage1CooldownTimer = 0f;

    // Lasers
    float laserSpeed = -10f;
    float laserAngle;
    Quaternion laserAngleOffset;
    Transform laser;


    private void Start()
    {
        RespawnController.instance.CustomActionsOnRespawnReset += ResetFight;
        laser = boss.parent.Find("Laser");
        stunLightning = boss.Find("StunLightning").gameObject;
        for (int i = 0; i < removableItems.Length; i++)
        {
            removableItems[i].SetIndex(i);
        }
        removedParticles = new[]{
            boss.Find("body/RemovedParticles").GetChild(0).gameObject,
            boss.Find("body/RemovedParticles").GetChild(1).gameObject,
            boss.Find("body/RemovedParticles").GetChild(2).gameObject
        };
        stage1Attack = GetComponent<DroneAttack>();
        animate = GetComponent<BossAnimation>();
        animator = GetComponent<Animator>();
        empDisable = FindObjectOfType<Disable>();
        grapple = FindObjectOfType<Grapple>();
        GetPaths();
        ResetFight();

        UpdateBossState(BossState.idle);
    }

    private void FixedUpdate()
    {
        if (nextPath)
        {
            UpdateBossState();
            nextPath = false;
        }

        if (nextTarget)
        {
            NextTarget();
            nextTarget = false;
        }

        if (_init())
        {
            CleanupState?.Invoke();

            switch (state)
            {
                case BossState.idle:
                    stateEvents = _idle();
                    break;
                case BossState.entry:
                    stateEvents = _entry();
                    break;
                case BossState.conversation:
                    stateEvents = _conversation();
                    break;
                case BossState.startFight:
                    stateEvents = _startFight();
                    break;
                case BossState.stage1:
                    stateEvents = _stage1();
                    break;
                case BossState.stage2:
                    stateEvents = _stage2();
                    break;
                case BossState.stage3:
                    stateEvents = _stage3();
                    break;
                case BossState.defeat:
                    stateEvents = _defeat();
                    break;
                default:
                    Debug.LogError("BossState out of bounds");
                    break;
            }

            UpdateState = stateEvents.Item1;
            FixedUpdateState = stateEvents.Item2;
            LateUpdateState = stateEvents.Item3;
            CleanupState = stateEvents.Item4;
        }

        FixedUpdateState?.Invoke();
    }

    private void Update()
    {
        UpdateState?.Invoke();
    }

    private void LateUpdate()
    {
        LateUpdateState?.Invoke();
    }



    bool _init()
    {
        bool tmp = initState;
        initState = false;
        return tmp;
    }

    Events _idle()
    {
        // Initialize here
        path = paths[(int)BossState.idle];
        target = path.CurrentTarget();
        // set animator state
        return (
            new Event(() => // Update
            {
                nextTarget = nextTarget || NavToWaypoint(target);
                nextPath = nextTarget && path.IsLastTarget();
            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {

            })
        );
    }

    Events _entry()
    {
        // Initialize here
        path = paths[(int)BossState.entry];
        target = path.CurrentTarget();
        return (
            new Event(() => // Update
            {
                nextTarget = nextTarget || NavToWaypoint(target);
                nextPath = nextTarget && path.IsLastTarget();
            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {

            })
        );
    }

    Events _conversation()
    {
        // Initialize here
        path = paths[(int)BossState.conversation];
        target = path.CurrentTarget();
        for (int i = 0; i < messages.Length; i++)
        {
            GameController.messageController.QueueMessage(messages[i]);
        }
        GameController.messageController.QueueClearedEvent += ConversationComplete;
        return (
            new Event(() => // Update
            {
                nextTarget = nextTarget || NavToWaypoint(target);
                
            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {
                GameController.messageController.QueueClearedEvent -= ConversationComplete;
            })
        );
    }

    Events _startFight()
    {
        // Initialize here
        path = paths[(int)BossState.startFight];
        target = path.CurrentTarget();
        TeleportToWaypoint(target);
        for (int i = 0; i < StartFightMessages.Length; i++)
        {
            GameController.messageController.QueueMessage(StartFightMessages[i]);
        }
        GameController.messageController.QueueClearedEvent += StartFightMessagesComplete;
        return (
            new Event(() => // Update
            {
                nextTarget = nextTarget || NavToWaypoint(target);

            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {
                GameController.messageController.QueueClearedEvent -= StartFightMessagesComplete;
            })
        );
    }

    Events _stage1()
    {
        // Initialize here
        bool start = true;
        nextTarget = false;
        path = paths[(int)BossState.stage1];
        target = path.CurrentTarget();
        stage1CooldownTimer = stage1AttackCooldown;
        return (
            new Event(() => // Update
            {
                bool tmpNextTarget = NavToWaypoint(target);
                start = start && !tmpNextTarget;
                nextTarget = nextTarget || ((path.CurrentTargetIndex() == 1) ? false : tmpNextTarget);
                if (!start && !stunned && stage1CooldownTimer <= 0.01f && Vector3.Distance(boss.position, player.position) <= stage1AttackRange)
                {
                    stage1Attack.Attack(player.gameObject);
                    nextTarget = true;
                    stage1CooldownTimer = stage1AttackCooldown;
                }

                stage1CooldownTimer = Mathf.Clamp(stage1CooldownTimer - Time.deltaTime, 0, stage1AttackCooldown);
            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {

            })
        );
    }

    Events _stage2()
    {
        // Initialize here
        bool start = true;
        path = paths[(int)BossState.stage2];
        target = path.CurrentTarget();
        return (
            new Event(() => // Update
            {
                nextTarget = nextTarget || NavToWaypoint(target);
                if (!start) UpdateLaser();

                if (start && nextTarget)
                {
                    StartLaser();
                    start = false;
                }
            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {
                StopLaser();
            })
        );
    }

    Events _stage3()
    {
        // Initialize here
        path = paths[(int)BossState.stage3];
        target = path.CurrentTarget();
        return (
            new Event(() => // Update
            {
                nextPath = true;
            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {

            })
        );
    }

    Events _defeat()
    {
        // Initialize here
        path = paths[(int)BossState.defeat];
        target = path.CurrentTarget();
        queuedMessages = false;
        for (int i = 0; i < removableItems.Length; i++)
        {
            if (removableItems[i].transform.parent && removableItems[i].transform.parent.name.Equals("body"))
            {
                ItemRemoved(i, false);
            }
        }
        return (
            new Event(() => // Update
            {
                if (!queuedMessages) nextTarget = nextTarget || NavToWaypoint(target);
                if (!queuedMessages && nextTarget && path.IsLastTarget())
                {
                    queuedMessages = true;
                    TeleportToWaypoint(target);
                    for (int i = 0; i < defeatMessages.Length; i++)
                    {
                        GameController.messageController.QueueMessage(defeatMessages[i]);
                    }
                    GameController.messageController.QueueClearedEvent += Defeated;
                }
            }),
            new Event(() => // FixedUpdate
            {

            }),
            new Event(() => // LateUpdate
            {

            }),
            new Event(() => // Cleanup
            {
                GameController.messageController.QueueClearedEvent -= Defeated;
            })
        );
    }



    public void UpdateBossState(BossState newState)
    {
        if (newState >= BossState.LEN)
        {
            Debug.LogError("BossState out of bounds");
            return;
        }
        initState = true;
        state = newState;
    }

    public void UpdateBossState(int newState)
    {
        if (newState >= (int)BossState.LEN)
        {
            Debug.LogError("BossState out of bounds");
            return;
        }
        initState = true;
        state = (BossState)newState;
    }

    public void UpdateBossState()
    {
        if (state + 1 >= BossState.LEN)
        {
            Debug.LogError("BossState out of bounds");
            return;
        }
        initState = true;
        state++;
    }

    public BossState GetBossState()
    {
        return state;
    }

    public bool GetVulnerable()
    {
        return vulnerable;
    }



    bool NavToWaypoint(Vector3 position, float speed)
    {
        return NavToWaypoint(position, position, speed);
    }

    bool NavToWaypoint(BossTarget target)
    {
        if (target)
            return NavToWaypoint(target.location.position, target.speed);
        else
        {
            RotateToPoint(player.position);
            return true;
        }

    }

    bool NavToWaypoint(Vector3 position, Vector3 lookAt, float speed)
    {
        if (stunned) return false;
        speed *= speedMultiplier * Time.deltaTime;
        if (Vector3.Distance(boss.position, position) < speed + 1)
        {
            RotateToPoint(player.position);
            return true;
        }
        boss.position = Vector3.MoveTowards(boss.position, position, speed);
        RotateToPoint(lookAt);
        return Vector3.Distance(boss.position, position) < speed;
    }

    void TeleportToWaypoint(BossTarget target)
    {
        Vector3 relativePos = player.position - boss.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        boss.position = target.location.position;
        boss.rotation = toRotation;
    }

    void RotateToPoint(Vector3 position)
    {
        if (stunned) return;
        Vector3 relativePos = position - boss.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        boss.rotation = Quaternion.Lerp(boss.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }



    void ConversationComplete()
    {
        nextPath = true;
    }

    void StartFightMessagesComplete()
    {
        startFightButton.OnButtonActivate += StartFightNow;
    }

    void RemoveStartFightEvents()
    {
        startFightButton.OnButtonActivate -= StartFightNow;
    }

    void StartFightNow()
    {
        RemoveStartFightEvents();
        nextPath = true;
        grapple.StopGrapple();
        empDisable.DisableNow();
    }



    void NextTarget()
    {
        target = path.NextTarget();
    }

    void GetPaths()
    {
        paths = boss.parent.Find("BossPaths").GetComponentsInChildren<BossPath>();
    }

    void ResetFight()
    {
        StopAllCoroutines();
        UpdateBossState(BossState.startFight); //idle
        vulnerable = false;
        stunned = false;
        nextPath = false;
        nextTarget = false;
        queuedMessages = false;
        UpdateState = FixedUpdateState = LateUpdateState = CleanupState = null;
        StopLaser();
        stunLightning.SetActive(false);
        boss.gameObject.SetActive(true);

        Rigidbody rb = boss.gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        foreach (BossPath path in paths)
        {
            path.ResetTargetNum();
        }
        foreach (RemovableObject obj in removableItems)
        {
            obj.ResetObj();
        }
        foreach (GameObject obj in removedParticles)
        {
            obj.SetActive(false);
        }
        if (animate.isFlapsOpen())
        {
            // If the flaps are currently open they will close
            // This will NOT detect flaps open and close if the trigger was just set.
            animate.closeFlaps();
        }
    }

    private void OnDrawGizmosSelected()
    {
        GetPaths();
    }



    // Lasers
    void StartLaser(bool resetRotation = true)
    {
        if (resetRotation) laser.rotation = laserAngleOffset;
        laser.gameObject.SetActive(true);
    }

    void UpdateLaser()
    {
        laser.Rotate(Vector3.up, laserSpeed * Time.deltaTime);
    }

    void StopLaser()
    {
        laser.gameObject.SetActive(false);
    }



    // Stun
    public void Stun()
    {
        StartCoroutine(StartStun());
    }

    IEnumerator StartStun()
    {
        if (state >= BossState.stage1 && state <= BossState.stage3)
        {
            animate.openFlaps();
            vulnerable = true;
            stunned = true;
            StopLaser();
            stunLightning.SetActive(true);
            yield return new WaitForSeconds(stunTime);
            animate.closeFlaps();
            vulnerable = false;
            stunned = false;
            stunLightning.SetActive(false);
        }
    }



    void Defeated()
    {
        Rigidbody rb = boss.gameObject.GetComponent<Rigidbody>();
        stunned = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.angularVelocity = new Vector3(1f, 1f, 1f);
        rb.velocity = new Vector3(1f, 1f, 1f);
        StartCoroutine(KillBoss());
    }

    // Defeat
    IEnumerator KillBoss()
    {
        yield return new WaitForSeconds(stunTime);
        killExplosion.Play();
        boss.gameObject.SetActive(false);
        GameObject _blueprint = Instantiate(blueprint);
        Rigidbody bprb = _blueprint.GetComponent<Rigidbody>();
        bprb.isKinematic = true;
        _blueprint.transform.position = boss.position;
        bprb.isKinematic = false;
        bprb.angularVelocity = bprb.velocity = Vector3.zero;

        // spawn particles
    }



    public Animator GetAnimator()
    {
        return animator;
    }

    public void ItemRemoved(int index, bool np = true)
    {
        removedParticles[index].SetActive(true);
        if (!np) removableItems[index].gameObject.SetActive(false);
        //animate.closeFlaps();
        vulnerable = false;
        //stunLightning.SetActive(false);
        nextPath = np;
    }
}
