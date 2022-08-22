using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



public class PlayerController : MonoBehaviour
{
    [Header("Player Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask roofLayer;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private float roofFadeRate = 1;
    [SerializeField] private GameObject[] roofs;

    [Header("Player Attack Settings")]
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private Weapon currentWeapon;
    [SerializeField] private AudioClip[] attackVfxs;

    private bool controllable = true;

    private Vector2 movement = Vector2.zero;
    private Vector2 cachedPosition;

    private Rigidbody2D rb2D;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private InputAction inputAction;
    private EntityStatus entityStatus;
    private AudioSource audioSource;

    private bool isHidden;
    private bool attackStun;
    private float nextAttack;

    [SerializeField] List<Material> roofMats = new List<Material>();
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        entityStatus = GetComponent<EntityStatus>();
        audioSource = GetComponent<AudioSource>();
        //inputAction = GetComponent<PlayerInput>().actions["aim"];

        foreach(GameObject roof in roofs)
            roofMats.Add(roof.GetComponent<Renderer>().material);
    }

    private void FixedUpdate()
    {
        if (!controllable) return;

        if (!attackStun)
            Move();

        HideRoof();
    }

    private void HideRoof()
    {
        Collider2D hit = Physics2D.OverlapPoint(transform.position, roofLayer);

        if (hit)
        {
            if (roofMats[0].color.a > 0)
            {
                foreach (Material mat in roofMats)
                    mat.color = new Color(1, 1, 1, mat.color.a - roofFadeRate * Time.deltaTime);
            }
           
        }
        else
        {
            if (roofMats[0].color.a < 1)
            {
                foreach (Material mat in roofMats)
                    mat.color = new Color(1, 1, 1, mat.color.a + roofFadeRate * Time.deltaTime);
            }
        }

    }



    public void SetIsHiden(bool value, Vector3 location)
    {
        isHidden = value;

        if (isHidden)
        {
            rb2D.simulated = false;
            spriteRenderer.color = Color.clear;
            transform.position = location;
            
        }
        else
        {
            rb2D.simulated = true;
            spriteRenderer.color = Color.white;
        }

        entityStatus.SetInvincible(value);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 10f, npcLayer);

        if(hits.Length > 0)
        {
            foreach(Collider2D hit in hits)
            {
                Debug.Log("hide hit = " + hit.name);
                hit.GetComponent<NPCController>().SetPlayerHiding(value);
            }
        }
    }

    #region Movement

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!controllable) return;

        if (context.performed)
        {
            movement = context.ReadValue<Vector2>();
            animator.SetBool("isMoving", true);
            UpdateAnimDirection(movement);

            if (isHidden) SetIsHiden(false, Vector3.zero);
        }
        else if (context.canceled)
        {
            movement = Vector2.zero;
            animator.SetBool("isMoving", false);
        }
    }

    private void Move()
    {
        //if (transform.position != cachedPosition)
        //PlayMovementSFXs();
        //else
        //StopMovementSFXs();

        cachedPosition = transform.position;
        rb2D.MovePosition(rb2D.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    private void UpdateAnimDirection(Vector2 direction)
    {
        direction.Normalize();
        animator.SetFloat("directionX", direction.x);
        animator.SetFloat("directionY", direction.y);
    }


    #endregion

    #region Attack

    public void OnPrimaryAttack(InputAction.CallbackContext context)
    {
        if (!controllable || currentWeapon == null || isHidden) return;

        if (context.performed)
        {
            Attack();
        }
        else if (context.canceled)
        {
            
        }
    }

    private void Attack()
    {
        if (Time.time < nextAttack || attackStun) return;

        StartCoroutine(attackCo());

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,
            currentWeapon.range, attackableLayer);

        if(hits.Length > 0)
        {
            foreach(Collider2D hit in hits)
            {
                if(hit.TryGetComponent(out EntityStatus entityStatus))
                {
                    entityStatus.RecieveDamage(currentWeapon.damage);
                }
            }
        }

        if (attackVfxs != null && attackVfxs.Length > 0)
            audioSource.PlayOneShot(attackVfxs[Random.Range(0, attackVfxs.Length)]);

        nextAttack = Time.time + currentWeapon.attackCdr;
    }

    private IEnumerator attackCo()
    {
        animator.SetTrigger("attackTrigger");
        attackStun = true;
        yield return new WaitForSeconds(0.3f);
        attackStun = false;
    }

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
    }

    public Weapon GetCurrentWeapn()
    {
        return currentWeapon;   
    }

    #endregion
}
