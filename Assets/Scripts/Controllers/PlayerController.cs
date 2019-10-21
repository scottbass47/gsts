using Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private PlayerStats stats;
    [SerializeField] private Animator bodyAnim;
    [SerializeField] private SpriteRenderer bodyRenderer;
    
    private HandController handController;
    private GunController gunController;
    private Physics physics;
    private Health health;
    private EffectHandler effectsHandler;

    private Vector2 movementVector;
    private bool shouldIdle;
    public float AimAngle { get; private set; }

    private float elapsed;
    private int footstepSoundHandle;

    private EventManager events;

    private void Start()
    {
        events = GameManager.Instance.Events;
        handController = GetComponentInChildren<HandController>();
        physics = GetComponent<Physics>();

        gunController = GetComponentInChildren<GunController>();
        gunController.OnReload += (duration) =>
        {
            events.FireEvent(new Reload { Duration = duration });
        };
        gunController.OnClipChange += (bullets) =>
        {
            events.FireEvent(new WeaponClipChange { BulletsInClip = bullets });
        };

        events.FireEvent(new WeaponMagChange { Bullets = gunController.Stats.MagSize });

        health = GetComponent<Health>();
        health.Amount = stats.MaxHealth;

        bodyRenderer.color = Color.white;
        effectsHandler = GetComponent<EffectHandler>();
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
            SoundManager.StopSoundLooping(footstepSoundHandle);
            events.FireEvent(new PlayerStopMoving());
            return;
        }
        var movementVector = new Vector2(xAxis, yAxis).normalized;

        float adjustedSpeed = stats.MoveSpeed; //- gunObject.GetComponent<GunController>().stats.Weight.ModdedValue();

        var vel = movementVector * adjustedSpeed;

        if (shouldIdle)
        {
            footstepSoundHandle = SoundManager.PlaySoundLooping(Sounds.PlayerFootsteps);
            events.FireEvent(new PlayerStartMoving());
        }
        physics.AddForce(vel);
        shouldIdle = false;
    }

    void HandleShooting()
    {
        // Shoot bullets
        bool holding = Input.GetButton("Shoot");  
        bool click = Input.GetButtonDown("Shoot");  
        if ((holding && !gunController.Stats.IsSemiAuto) ||
             click && gunController.Stats.IsSemiAuto)
        {
            if (gunController.Shoot())
            {
                SoundManager.PlaySound(Sounds.PlayerGunshot);
                events.FireEvent(new WeaponFired());
            }
        }
    }

    void HandleRotation()
    {
        Vector2 player = transform.position;
        Vector2 mouse = Mouse.WorldPos;
        Vector2 aimVector = (mouse - player).normalized;

        float angle = Mathf.Atan2(aimVector.y, aimVector.x);

        // Convert angle to degrees in range 0-360
        float degrees = angle * Mathf.Rad2Deg;
        if (degrees < 0)
        {
            degrees += 360;
        }

        AimAngle = degrees;
        //gunController.SetAimAngle(AimAngle);

        gunController.AimAt(mouse, player);

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

    public void TakeDamage(float amount)
    {
        StartCoroutine(DamageRoutine());

        events.FireEvent(new PlayerDamage());
        events.FireEvent(new PlayerHealthEvent { Health = (int)health.Amount });
    }
    
    private IEnumerator DamageRoutine()
    {
        var damageFilter = GetComponent<DamageFilter>();
        damageFilter.IsInvulnerable = true;

        float blinkTime = 0.1f;
        float flashDuration = 0.15f;
        float blinkDuration = 0.75f;

        effectsHandler.AddEffect(new FlashEffect(flashDuration));
        yield return new WaitForSeconds(flashDuration);

        effectsHandler.AddEffect(new BlinkEffect(blinkDuration, blinkTime));
        yield return new WaitForSeconds(blinkTime);

        damageFilter.IsInvulnerable = false;
    }
}
