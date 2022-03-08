using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events = System.ValueTuple<CEOController.Event, CEOController.Event, CEOController.Event, CEOController.Event>;

public class CEOController : MonoBehaviour
{
    public Transform player, boss;
    public RemovableObject[] removables;
    public BossPath[] paths;

    BossPath path;
    BossTarget target;

    Animator animator;

    public enum BossState { idle, entry, conversation, stage1, stage2, stage3, defeat, LEN };
    public delegate void Event(); // run on Unity events
    public delegate void Activity(); // call a function once the waypoint is reached
    public delegate bool DoneWaiting(); // check if boss is done with its activity
    //(Event,   Event, Event, Event)
    // Default, Fixed, Late,  Cleanup

    BossState state;
    bool initState;
    Events stateEvents;
    Event UpdateState, FixedUpdateState, LateUpdateState, CleanupState;

    bool vulnerable;

    float rotationSpeed = 10f;
    float speedMultiplier = 10f;

    private void Start()
    {
        GetPaths();
        ResetFight();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
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
        path = paths[0];
        target = path.NextTarget();
        // set animator state
        return (
            new Event(() => // Update
            {
                NavToWaypoint(target);
            }),
            null,
            null,
            null
        );
    }

    Events _entry()
    {
        // Initialize here
        path = paths[1];
        target = path.NextTarget();
        return (
            new Event(() => // Update
            {
                if (NavToWaypoint(target)) target = path.NextTarget();
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

        return (
            new Event(() => // Update
            {

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

    Events _stage1()
    {
        // Initialize here

        return (
            new Event(() => // Update
            {

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

        return (
            new Event(() => // Update
            {

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

    Events _stage3()
    {
        // Initialize here

        return (
            new Event(() => // Update
            {

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

        return (
            new Event(() => // Update
            {

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

    void RotateToPoint(Vector3 position)
    {
        //boss.rotation = Quaternion.Lerp(boss.rotation, Quaternion.Euler(position - boss.position), rotationSpeed * Time.deltaTime);

        //Vector3 direction = position - boss.position;
        //Quaternion toRotation = Quaternion.FromToRotation(boss.forward, direction);
        //boss.rotation = Quaternion.Lerp(boss.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //boss.LookAt(position);

        Vector3 relativePos = position - boss.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        boss.rotation = Quaternion.Lerp(boss.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }



    void GetPaths()
    {
        paths = boss.parent.Find("BossPaths").GetComponentsInChildren<BossPath>();
    }

    void ResetFight()
    {
        UpdateBossState(BossState.idle); //idle
        vulnerable = false;
        UpdateState = FixedUpdateState = LateUpdateState = CleanupState = null;
        foreach (BossPath path in paths)
        {
            path.ResetTargetNum();
        }
    }

    private void OnDrawGizmosSelected()
    {
        GetPaths();
    }
}
