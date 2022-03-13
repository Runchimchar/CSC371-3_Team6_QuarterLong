using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events = System.ValueTuple<CEOController.Event, CEOController.Event, CEOController.Event, CEOController.Event>;
// (Event,   Event, Event, Event)
//  Default, Fixed, Late,  Cleanup

public class CEOController : MonoBehaviour
{
    public Transform player, boss;
    public RemovableObject[] removables;
    public BossPath[] paths;
    public float stunTime = 6f;

    BossPath path;
    BossTarget target;

    BossAnimation animate;

    public enum BossState { idle, entry, conversation, stage1, stage2, stage3, defeat, LEN };
    public delegate void Event(); // run on Unity events

    BossState state;
    bool initState;
    Events stateEvents;
    Event UpdateState, FixedUpdateState, LateUpdateState, CleanupState;

    bool vulnerable;
    public bool nextTarget;
    public bool nextPath;

    float rotationSpeed = 10f;
    float speedMultiplier = 10f;

    // Lasers
    float laserSpeed = -10f;
    float laserAngle;
    Quaternion laserAngleOffset;
    Transform laser;

    private void Start()
    {
        laser = boss.parent.Find("Laser");
        GetPaths();
        animate = GetComponent<BossAnimation>();
        ResetFight();

        laserAngleOffset = laser.rotation;
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
                nextTarget = NavToWaypoint(target);
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
        path = paths[1];
        target = path.NextTarget();
        return (
            new Event(() => // Update
            {
                nextTarget = NavToWaypoint(target);
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
        path = paths[2];
        target = path.NextTarget();
        return (
            new Event(() => // Update
            {
                nextTarget = NavToWaypoint(target);
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

    Events _stage1()
    {
        // Initialize here
        path = paths[3];
        target = path.NextTarget();
        StartLaser();
        return (
            new Event(() => // Update
            {
                nextTarget = NavToWaypoint(target);
                UpdateLaser();
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
        if (vulnerable) return false;
        speed *= speedMultiplier * Time.deltaTime;
        if (Vector3.Distance(boss.position, position) < speed + 1)
        {
            RotateToPoint(player.position);
            return true;
        }
        if (!vulnerable) boss.position = Vector3.MoveTowards(boss.position, position, speed);
        RotateToPoint(lookAt);
        return Vector3.Distance(boss.position, position) < speed;
    }

    void RotateToPoint(Vector3 position)
    {
        //boss.rotation = Quaternion.Lerp(boss.rotation, Quaternion.Euler(position - boss.position), rotationSpeed * Time.deltaTime);

        //Vector3 direction = position - boss.position;
        //Quaternion toRotation = Quaternion.FromToRotation(body.forward, direction);
        //body.rotation = Quaternion.Lerp(body.rotation, toRotation, rotationSpeed * Time.deltaTime);

        //body.LookAt(position);

        if (vulnerable) return;
        Vector3 relativePos = position - boss.position;
        Quaternion toRotation = Quaternion.LookRotation(relativePos);
        boss.rotation = Quaternion.Lerp(boss.rotation, toRotation, rotationSpeed * Time.deltaTime);
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
        UpdateBossState(BossState.idle); //idle
        vulnerable = false;
        nextPath = false;
        nextTarget = false;
        UpdateState = FixedUpdateState = LateUpdateState = CleanupState = null;
        StopLaser();
        animate.closeFlaps();
        foreach (BossPath path in paths)
        {
            path.ResetTargetNum();
        }
    }

    private void OnDrawGizmosSelected()
    {
        GetPaths();
    }



    // Lasers
    void StartLaser()
    {
        laser.rotation = laserAngleOffset;
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
        animate.openFlaps();
        vulnerable = true;
        StopLaser();
        yield return new WaitForSeconds(stunTime);
        animate.closeFlaps();
        vulnerable = false;
        StartLaser();
    }
}
