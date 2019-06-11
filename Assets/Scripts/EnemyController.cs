using System.Collections;
using System.Collections.Generic;
using UnityEngine; 
//using Saga; 
using FluentBehaviourTree;

public class EnemyController : MonoBehaviour
{

    public GameObject player;
    public float Speed = 2;
    public Vector2 HitboxCenter
    {
        get
        {
            return boxCollide.bounds.center;
        }
    }
    private Coord CurrTile
    {
        get
        {
            return new Coord(HitboxCenter);
        }
    }

    private Health health;
    private Rigidbody2D rb2d;
    private MovementAnimator moveAnim;
    private BoxCollider2D boxCollide;
    private GunController gunController;
    private PathFinder finder;
    private Path path;
    private float lookAngle;

    // AI
    private IBehaviourTreeNode tree;

    // Patrol
    private bool IsWalking;


    public void Init()
    {
        health = GetComponent<Health>();
        moveAnim = GetComponent<MovementAnimator>();
        finder = GetComponent<PathFinder>();
        boxCollide = GetComponent<BoxCollider2D>();
        gunController = GetComponentInChildren<GunController>();
        //gunController.HandlePivot = new Vector2(-8, -1);
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.freezeRotation = true;
        health.Amount = 100;
        
        BuildTree();
    }


    private void BuildTree()
    {
        var builder = new BehaviourTreeBuilder();
        tree = builder
            .Selector("Main")
                .Sequence("Attack")
                    .Condition("LOS", t => LOS())
                    .Condition("Range", t => Range(4))
                    .Do("Stop", t => StopMoving())
                    .Do("Attack", t =>
                    {
                        gunController.Shoot();
                        return BehaviourTreeStatus.Success;
                    })
                .End()
                .Sequence("Follow")
                    .Condition("LOS", t => LOS())
                    .Condition("Get Path", t => GetPath())
                    .Do("Move On Path", t => MoveOnPath())
                .End()
                .Sequence("Follow Trail")
                    .Condition("Has Path", t => HasPath())
                    .Do("Move On Path", t => MoveOnPath())
                .End()
                .Do("Patrol", t => Patrol())
            .End()
            .Build();
    }

    // Returns true if the player is in LOS
    private bool LOS()
    {
        Vector2 diff = player.transform.position - transform.position;
        RaycastHit2D ray = Physics2D.Raycast(transform.position, diff, 100, LayerMask.GetMask("Walls", "Friendly"));

        Debug.DrawLine(transform.position, ray.point, Color.red, 0.1f);

        return ray.rigidbody != null && ray.rigidbody.gameObject.tag == "Player";
    }

    private bool Range(float range)
    {
        Vector2 diff = player.transform.position - transform.position;
        return diff.SqrMagnitude() <= range * range;
    }

    private bool HasPath()
    {
        return path != null;
    }

    BehaviourTreeStatus StopMoving()
    {
        rb2d.velocity = Vector2.zero;
        return BehaviourTreeStatus.Success;
    }

    private bool OnScreen(float viewRange)
    {
        Camera main = Camera.main;
        Vector3 viewPoint = main.WorldToViewportPoint(transform.position);
        return viewPoint.x + viewRange >= 0 && viewPoint.x - viewRange <= 1 && viewPoint.y + viewRange >= 0 && viewPoint.y - viewRange <= 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (health.Amount < 0)
        {
            Destroy(gameObject);
        }

        if (!OnScreen(0.1f)) return;

        tree.Tick(new TimeData(Time.deltaTime));
        //FollowPlayer();
        gunController.SetAimAngle(lookAngle);
        moveAnim.LookAngle = lookAngle;
    }

    // Returns true if a path can be created between the enemy and the player
    bool GetPath()
    {
        Transform pTransform = player.transform;
        Transform myTransform = transform;
        Vector2 diff = pTransform.position - myTransform.position;
        lookAngle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);

        // Both lines can be deleted when debug drawing is no longer needed
        RaycastHit2D ray = Physics2D.Raycast(myTransform.position, diff, 100, LayerMask.GetMask("Walls", "Friendly"));
        Debug.DrawLine(myTransform.position, ray.point, Color.red, 0.1f);

        if (path == null || !path.OnPath(pTransform.position) || !path.OnPath(HitboxCenter))
        {
            path = finder.GetPath((int)HitboxCenter.x, (int)HitboxCenter.y, (int)pTransform.position.x, (int)pTransform.position.y);
        }

        return path != null && path.path.Count > 1;
    }

    // Moves the enemy along the path. Fails if the enemy is already at the goal
    BehaviourTreeStatus MoveOnPath()
    {
        Coord tile = new Coord((int)HitboxCenter.x, (int)HitboxCenter.y);
        Vector2 tileCenter = new Vector2(tile.tx + 0.5f, tile.ty + 0.5f);

        if (path.AtGoal(tileCenter) || !path.OnPath(tileCenter)) return BehaviourTreeStatus.Failure;

        Coord goal = path.GetNext(HitboxCenter);

        // Try to get close to the center of the tile
        if ((tileCenter - HitboxCenter).SqrMagnitude() > 1f)
        {
            goal = tile;
        }

        // Calculate angle between enemy and goal
        float angle = Mathf.Atan2(goal.ty - HitboxCenter.y + 0.5f, goal.tx - HitboxCenter.x + 0.5f);

        rb2d.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        rb2d.velocity *= Speed;

        moveAnim.LookAngle = angle * Mathf.Rad2Deg;
        return BehaviourTreeStatus.Success;
    }

    BehaviourTreeStatus Patrol()
    {

        List<Coord> spots = Level.FindCoords(CurrTile, (coord) => true, GameManager.instance.level.map, 25);
        Coord goal = spots[Random.Range(0, spots.Count)];
        return BehaviourTreeStatus.Success;
    }

    private void LateUpdate()
    {
        if (path != null)
        {
            for (int i = 0; i < path.path.Count - 1; i++)
            {
                Coord p1 = path.path[i];
                Coord p2 = path.path[i + 1];
                Debug.DrawLine(new Vector3(p1.tx + 0.5f, p1.ty + 0.5f, 0), new Vector3(p2.tx + 0.5f, p2.ty + 0.5f, 0), Color.green);
            }
        }
    }
}
