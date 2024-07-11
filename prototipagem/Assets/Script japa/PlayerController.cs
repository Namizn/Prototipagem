using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(PlayerCollider))]
public class PlayerController : MonoBehaviour
{
    const float speed = 5;
    const float fallMultiplier = 2.5f;
    const float lowJumpMultiplier = 2f;
    const float jumpForce = 10;
    const float dashForce = 10;

    bool jumping, dashing, agachar;
    Vector2 direction;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform firePoint;
    SpriteRenderer render;
    Rigidbody2D rb;
    PlayerCollider playerCollider;
    Animator animator;
    float contSuperJump;
    Inputs inputs;
    bool superJumpAcert;
    int printContSuperJump;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<PlayerCollider>();
        animator = GetComponent<Animator>();

        inputs = new Inputs();

        inputs.Player.Pular.performed += ctx => Jump();
        inputs.Player.Pular.canceled += ctx => jumping = false;
        inputs.Player.Andar.performed += ctx => direction = ctx.ReadValue<Vector2>();
        inputs.Player.Agachar.performed += ctx => Agachar();
        inputs.Player.Agachar.canceled += ctx => agachar = false;
        inputs.Player.SuperPulo.started += ctx => superJumpAcert = true;
        inputs.Player.SuperPulo.canceled += ctx => superJumpAcert = false;
    }
    private void Update()
    {
        SetGravity();
        Movement();
        SuperJump();
    }

    private void Movement()
    {
        if (!dashing)
        {
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
        }

        render.flipX = rb.velocity.x < 0;
    }

    private void SetGravity()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * fallMultiplier * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jumping)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * lowJumpMultiplier * Time.deltaTime;
        }
    }
    private void Agachar()
    {
    }
    private void Jump()

    {
        if (playerCollider.OnGround)
        {
            jumping = true;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            print("Pulo normal");
        }
             
    }
      private void SuperJump()
    {
        if (superJumpAcert)
        {
            contSuperJump += Time.deltaTime;
            printContSuperJump++;
            print(printContSuperJump);
            if (contSuperJump >= 2)
            {
                float newJumpForce;
                newJumpForce = jumpForce + 2;

                jumping = true;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.velocity = new Vector2(rb.velocity.x, newJumpForce);
                print("Super pulo");
                contSuperJump = 0;
                printContSuperJump = 0;
            }
        }
        
    }

    private IEnumerator Dash()
    {
        if (!dashing)
        {
            animator.SetTrigger("Dashing");
            dashing = true;
            rb.velocity = new Vector2(dashForce * direction.x, 0);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;

            yield return new WaitForSeconds(0.8f);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            dashing = false;
        }
    }


    private void Shoot()
    {
        Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
    }

    private void OnEnable()
    {
        inputs.Enable();
        
    }

    private void OnDisable()
    {
        inputs.Disable();
    }
}
