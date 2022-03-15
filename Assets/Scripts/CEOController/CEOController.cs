using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events = System.ValueTuple<CEOController.Event, CEOController.Event, CEOController.Event, CEOController.Event>;
// (Event,   Event, Event, Event)
//  Default, Fixed, Late,  Cleanup

public class CEOController : MonoBehaviour
{
    public Transform player, boss;
    public BossPath[] paths;
    public float stunTime = 6f;

    BossPath path;
    BossTarget target;

    BossAnimation animate;
    Animator animator;
    GameObject stunLightning;
    [SerializeField] RemovableObject[] removableItems;
    GameObject[] removedParticles;

    public enum BossState { idle, entry, conversation, stage1, stage2, stage3, defeat, LEN };
    public enum RemovableItemType { nozzle_back, nozzle_l, nozzle_r }; // in this order
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
        animate = GetComponent<BossAnimation>();
        animator = GetComponent<Animator>();
        GetPaths();
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
        path = paths[(int)BossState.idle];
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
        path = paths[(int)BossState.entry];
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
        path = paths[(int)BossState.conversation];
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
        path = paths[(int)BossState.stage1];
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
        path = paths[(int)BossState.stage2];
        target = path.NextTarget();
        return (
            new Event(() => // Update
            {
                nextTarget = NavToWaypoint(target);
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
        path = paths[(int)BossState.stage3];
        target = path.NextTarget();
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
        path = paths[(int)BossState.defeat];
        target = path.NextTarget();
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
        StopAllCoroutines();
        UpdateBossState(BossState.idle); //idle
        vulnerable = false;
        nextPath = false;
        nextTarget = false;
        UpdateState = FixedUpdateState = LateUpdateState = CleanupState = null;
        StopLaser();
        stunLightning.SetActive(false);
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
        if (state >= BossState.stage1)
        {
            animate.openFlaps();
            vulnerable = true;
            StopLaser();
            stunLightning.SetActive(true);
            yield return new WaitForSeconds(stunTime);
            if (vulnerable)
            {
                animate.closeFlaps();
                vulnerable = false;
                stunLightning.SetActive(false);
            }
        }
    }



    public Animator GetAnimator()
    {
        return animator;
    }

    public void ItemRemoved(int index)
    {
        print("Item " + index + " removed!");
        removedParticles[index].SetActive(true);
        animate.closeFlaps();
        vulnerable = false;
        stunLightning.SetActive(false);
        nextPath = true;
    }
}