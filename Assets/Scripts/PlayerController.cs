using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 4;
    public GameObject gunObject;
    public Sprite baseGunSprite;

    //public Gun Gun { get; private set; }
    public Animator bodyAnim;
    public SpriteRenderer bodyRenderer;
    private HandController handController;
    private GunController gunController;
    private Physics physics;
    //private BulletPanel bulletPanel;

    private bool wasIdling = false;
    private bool shouldIdle = false;

    //private float movementAngle;
    private Vector2 movementVector;
    public float AimAngle { get; private set; }

    private float elapsed;

    [SerializeField] private Material flashMaterial;

    // Use this for initialization
    void Start()
    {
        handController = GetComponentInChildren<HandController>();
        physics = GetComponent<Physics>();
        gunController = GetComponentInChildren<GunController>();

        bodyRenderer.color = Color.white;
    }

    // Update is called once per frame
    void Update()
    {
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
            physics.AddForce(Vector2.zero);
            shouldIdle = true;
            return;
        }
        var movementVector = new Vector2(xAxis, yAxis).normalized;

        float adjustedSpeed = speed; //- gunObject.GetComponent<GunController>().stats.Weight.ModdedValue();

        var vel = movementVector * adjustedSpeed;
        physics.AddForce(vel);
        shouldIdle = false;
    }

    void HandleShooting()
    {
        // Shoot bullets
        if (Input.GetButton("Shoot"))
        {
            gunController.Shoot();
        }
    }

    void HandleRotation()
    {
        Vector2 player = transform.position;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimVector = (mouse - player).normalized;

        float angle = Mathf.Atan2(aimVector.y, aimVector.x);

        // Convert angle to degrees in range 0-360
        float degrees = angle * Mathf.Rad2Deg;
        if (degrees < 0)
        {
            degrees += 360;
        }

        AimAngle = degrees;
        gunController.SetAimAngle(AimAngle);

        if(Vector2.Dot(movementVector, aimVector) < 0)
        {
            bodyAnim.SetFloat("speed", -1);
        }
        else
        {
            bodyAnim.SetFloat("speed", 1);
        }

        bool flip = gunController.Flipped;

        bodyRenderer.flipX = flip;

        bodyAnim.SetFloat("horizontal", Mathf.Abs(aimVector.x));
        bodyAnim.SetFloat("vertical", aimVector.y);
        bodyAnim.SetFloat("moving", shouldIdle ? 0.0f : 1.0f);

        handController.UpdateTexture(aimVector.y < 0, flip);
        gunController.SetDrawOrder(aimVector.y < 0);
    }

    public void TakeDamage()
    {
        StartCoroutine(DamageRoutine());
    }
    
    private IEnumerator DamageRoutine()
    {
        var damageFilter = GetComponent<DamageFilter>();
        damageFilter.IsInvulnerable = true;

        yield return Flash(0.15f);
        yield return Blinking(0.75f, 0.1f);

        damageFilter.IsInvulnerable = false;
    }

    private IEnumerator Flash(float duration)
    {
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();
        Material[] old = new Material[renderers.Length];
        for(int i = 0; i < renderers.Length; i++)
        {
            var render = renderers[i];
            old[i] = render.material;
            render.material = flashMaterial;
            render.material.SetColor("_BlinkColor", Color.white);
        }
        yield return new WaitForSeconds(duration);

        for(int i = 0; i < renderers.Length; i++)
        {
            var render = renderers[i];
            render.material = old[i];
        }
    }

    private IEnumerator Blinking(float duration, float blinkTime)
    {
        var renderers = GetComponentsInChildren<SpriteRenderer>();

        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            int interval = (int)(time / blinkTime);
            foreach(var render in renderers)
            {
                render.enabled = interval % 2 == 1;
            }
            yield return null;
        }

        // Re-enable all the renderers
        foreach(var render in renderers)
        {
            render.enabled = true;
        }
    }
}
