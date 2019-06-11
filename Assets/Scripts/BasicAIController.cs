using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FluentBehaviourTree;

public class BasicAIController : MonoBehaviour
{
    private GameObject player;
    private Level level;
    private PathFinder pathFinder;
    private Path path;
    private float lookAngle;
    private Rigidbody2D rb2d;
    private MovementAnimator moveAnim;

    public Transform enemyTransform;
    public Transform playerTransform;

    public float Speed = 2;
    private Coord CurrTile
    {
        get
        {
            return level.WorldToTile(enemyTransform.position);
        }
    }

    // AI
    private IBehaviourTreeNode tree;

    // Patrol
    private bool IsWalking;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        player = GameManager.instance.player;
        playerTransform = player.GetComponent<Physics>().HitboxCenter;
        level = GameManager.instance.level;
        pathFinder = level.GetPathFinder();
        moveAnim = GetComponent<MovementAnimator>();

        BuildTree();
    }

    private void BuildTree()
    {
        var builder = new BehaviourTreeBuilder();
        tree = builder
            .Selector("Main")
                .Sequence("Attack")
                    .Condition("LOS", t => LOS())
                    .Condition("Range", t => Range(1.5f))
                    .Do("Stop", t => StopMoving())
                    .Do("Attack", t =>
                    {
                        //gunController.Shoot
                        moveAnim.Attack();
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
        Vector2 diff = playerTransform.position - enemyTransform.position;
        RaycastHit2D ray = Physics2D.Raycast(enemyTransform.position, diff, 100, LayerMask.GetMask("Walls", "Player Feet"));

        Debug.DrawLine(enemyTransform.position, ray.point, Color.red, 0.1f);

        return ray.rigidbody != null && ray.rigidbody.gameObject.tag == "Player";
    }

    private bool Range(float range)
    {
        Vector2 diff = playerTransform.position - enemyTransform.position;
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
        Vector3 viewPoint = main.WorldToViewportPoint(enemyTransform.position);
        return viewPoint.x + viewRange >= 0 && viewPoint.x - viewRange <= 1 && viewPoint.y + viewRange >= 0 && viewPoint.y - viewRange <= 1;
    }

    // Update is called once per frame
    void Update()
    {
        // We probably don't want the AI itself to have control over whether or not it updates. This should
        // probably be in a state machine somewhere else.
        //if (!OnScreen(0.1f)) return;

        tree.Tick(new TimeData(Time.deltaTime));
        //FollowPlayer();
        //gunController.SetAimAngle(lookAngle);
        //moveAnim.LookAngle = lookAngle;
    }

    // Returns true if a path can be created between the enemy and the player
    bool GetPath()
    {
        Transform pTransform = playerTransform;
        Transform myTransform = enemyTransform;
        Vector2 diff = pTransform.position - myTransform.position;
        lookAngle = Mathf.Rad2Deg * Mathf.Atan2(diff.y, diff.x);

        // Both lines can be deleted when debug drawing is no longer needed
        RaycastHit2D ray = Physics2D.Raycast(myTransform.position, diff, 100, LayerMask.GetMask("Walls", "Player Feet"));
        Debug.DrawLine(myTransform.position, ray.point, Color.red, 0.1f);

        Coord playerPos = level.WorldToTile(pTransform.position);
        Coord enemyPos = level.WorldToTile(enemyTransform.position);
        if (path == null || !path.OnPath(playerPos) || !path.OnPath(enemyPos))
        {
            path = pathFinder.GetPath(
                enemyPos.tx,
                enemyPos.ty,
                playerPos.tx,
                playerPos.ty
            );
        }

        return path != null && path.path.Count > 1;
    }

    // Moves the enemy along the path. Fails if the enemy is already at the goal
    BehaviourTreeStatus MoveOnPath()
    {
        Coord tile = level.WorldToTile(enemyTransform.position);
        Vector2 enemyPos = level.WorldToLevel(enemyTransform.position);

        // tileCenter in Level Coords
        Vector2 tileCenter = new Vector2(tile.tx + 0.5f, tile.ty + 0.5f);

        if (path.AtGoal(tileCenter) || !path.OnPath(tileCenter)) return BehaviourTreeStatus.Failure;

        Coord goal = path.GetNext(tile);

        // Try to get close to the center of the tile
        if ((level.LevelToWorld(tileCenter) - (Vector2)enemyTransform.position).SqrMagnitude() > 1f)
        {
            goal = tile;
        }

        // Calculate angle between enemy and goal
        var goalVec = new Vector2(goal.tx - enemyPos.x + 0.5f, goal.ty - enemyPos.y + 0.5f).normalized;
        var dir = Vector2.Lerp(rb2d.velocity.normalized, goalVec, Time.deltaTime * 2);

        rb2d.velocity = dir * Speed;

        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle + 360) % 360;
        moveAnim.LookAngle = angle;

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
            //for (int i = 0; i < path.path.Count - 1; i++)
            //{
            //    Vector2 p1 = level.TileToWorld(path.path[i]);
            //    Vector2 p2 = level.TileToWorld(path.path[i + 1]);
            //    Debug.DrawLine(new Vector3(p1.x + 0.5f, p1.y + 0.5f, 0), new Vector3(p2.x + 0.5f, p2.y + 0.5f, 0), Color.green);
            //}
        }
    }

    private void OnDrawGizmos()
    {
        if(path != null)
        {
            Gizmos.color = Color.blue;
            for(int i = 0; i < path.path.Count; i++)
            {
                Vector2 waypoint = level.TileToWorld(path.path[i]);
                Gizmos.DrawSphere(waypoint + new Vector2(0.5f, 0.5f), 0.5f);
            }
        }
    }
}
