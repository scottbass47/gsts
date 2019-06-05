using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed = 4;
    public GameObject gunObject;
    public GameObject heart;
    public GameObject hud;
    public Sprite baseGunSprite;

    //public Gun Gun { get; private set; }
    private Rigidbody2D rb2d;
    private Animator anim;
    private AnimState currState;
    private SpriteRenderer spriteRenderer;
    private HandController handController;
    private GunController gunController;
    //private BulletPanel bulletPanel;

    private bool wasIdling = false;
    private bool shouldIdle = false;

    private float movementAngle;
    public float AimAngle { get; private set; }

    private float elapsed;

	// Use this for initialization
	void Start () {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.freezeRotation = true;

        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        handController = GetComponentInChildren<HandController>();

        //heart.GetComponent<HeartController>().SetHealth(10);
        gunController = GetComponentInChildren<GunController>();

        //Gun = new Gun(baseGunSprite, gunController.stats);
        //gunController.Offset = Gun.GetLocalOffset();
        //gunController.HandlePivot = Gun.HandlePivot;
        //gunController.UpdateTexture(Gun.ModdedGun);

        //bulletPanel = hud.GetComponentInChildren<BulletPanel>();
        //int bulletsLeft = (int)gunController.stats.MagSize.ModdedValue();
        //bulletPanel.SetMagSize(bulletsLeft);
    }

    // Update is called once per frame
    void Update () {
        HandleMovement();
        HandleShooting();
        HandleRotation();
    }

    void HandleMovement()
    {
        float xAxis = Input.GetAxisRaw("Horizontal");
        float yAxis = Input.GetAxisRaw("Vertical");

        // Stop the player from moving if no input is active
        if (Mathf.Abs(xAxis) < float.Epsilon && Mathf.Abs(yAxis) < float.Epsilon)
        {
            rb2d.velocity = Vector2.zero;
            shouldIdle = true;
            return;
        }

        float angle = Mathf.Atan2(yAxis, xAxis);

        movementAngle = Mathf.Rad2Deg * angle;
        if(movementAngle < 0)
        {
            movementAngle += 360;
        }

        float adjustedSpeed = speed; //- gunObject.GetComponent<GunController>().stats.Weight.ModdedValue();

        rb2d.velocity = new Vector2(adjustedSpeed * Mathf.Cos(angle), adjustedSpeed * Mathf.Sin(angle));
        rb2d.angularVelocity = 0f;
        shouldIdle = false;
        //transform.eulerAngles = new Vector3(0, 0, Mathf.Rad2Deg * angle);
    }

    void HandleShooting()
    {
        // Shoot bullets
        if(Input.GetButton("Shoot"))
        {
            Vector2 player = transform.position;
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 diff = mouse - player;
            float angle = Mathf.Atan2(diff.y, diff.x);

            //GunController g = gunObject.GetComponent<GunController>();
            //if(g.Shoot())
            //{
            //    bulletPanel.Shoot();
            //    if(g.Reloading)
            //    {
            //        bulletPanel.Reload(g.stats.ReloadSpeed.ModdedValue());
            //    }
            //}
        }
    }

    void HandleRotation()
    {
        Vector2 player = transform.position;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 diff = mouse - player;
        float angle = Mathf.Atan2(diff.y, diff.x);

        // Convert angle to degrees in range 0-360
        float degrees = angle * Mathf.Rad2Deg;
        if (degrees < 0)
        {
            degrees += 360;
        }

        AimAngle = degrees;
        gunController.SetAimAngle(AimAngle);

        // Find difference between movement and aim angles to determine
        // if the player should be backpedalling
        float angleBetween = movementAngle - degrees;

        // Make between -180 and 180
        if (angleBetween < -180) angleBetween += 360;
        else if (angleBetween > 180) angleBetween -= 360;

        // Negative if obtuse, positive if acute (think dot product)
        if(Mathf.Cos(Mathf.Deg2Rad * angleBetween) < 0 && !shouldIdle)
        {
            anim.SetFloat("speed", -1);
        }
        else
        {
            anim.SetFloat("speed", 1);
        }

        //bool flip = degrees > 90 && degrees < 270;

        //if (flip)
        //{
        //    degrees = 180 - degrees;
        //    if (degrees < 0) degrees += 360;
        //}
        bool flip = gunController.Flipped;

        if (flip)
        {
            degrees = 180 - degrees;
            if (degrees < 0) degrees += 360;
        }


        spriteRenderer.flipX = flip;

       // Forward
        if (degrees >= 270 && degrees < 315)
        {
            SwitchState(AnimState.Forward, shouldIdle);
        }
        // Forward 45
        else if ((degrees >= 315 && degrees <= 360) || degrees <= 0)
        {
            SwitchState(AnimState.Forward45, shouldIdle);
        }
        // Backward 45
        else if (degrees <= 45 && degrees > 0)
        {
            SwitchState(AnimState.Backward45, shouldIdle);
        }
        // Backward
        else if (degrees <= 90 && degrees > 45)
        {
            SwitchState(AnimState.Backward, shouldIdle);
        }

        handController.UpdateTexture(currState, flip);
    }

    void SwitchState(AnimState newState, bool idle)
    {
        if (currState == newState && wasIdling == shouldIdle) return;
        currState = newState;

        switch (newState)
        {
            case AnimState.Forward:
                anim.SetTrigger("forward");
                break;
            case AnimState.Forward45:
                anim.SetTrigger("forward_45");
                break;
            case AnimState.Backward:
                anim.SetTrigger("backward");
                break;
            case AnimState.Backward45:
                anim.SetTrigger("backward_45");
                break;
        }
        anim.SetBool("idle", idle);
        wasIdling = shouldIdle;
    }

    //public void AddAttachment(Attachment attachment)
    //{
    //    Gun.AddAttachment(attachment);
    //    gunController.UpdateTexture(Gun.ModdedGun);
    //}

}

public enum AnimState
{
    Forward,
    Backward,
    Forward45,
    Backward45
}
