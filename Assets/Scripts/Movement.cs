using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//beans
public class Movement : MonoBehaviour
{
    public Animator squishAnim;
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public float speed = 10;
    public float currentSpeed = 0;
    public float accel = 70;

    public float jumpForce = 50;
    public float slideSpeed = 5;
    public float wallJumpLerp = 10;
    public float dashSpeed = 20;
    public float dashMomentum = 14;
    public float playerGrav = 3;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;
    public String mode = "basic";
    internal uint moveset = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;

    [Space]
    [Header("Audio")]
    public AudioSource jumpSound;
    public AudioSource dashSound;


    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        if (Input.GetKeyDown(KeyCode.E)) {
            if (mode == "basic") mode = "polished";
             else if (mode == "polished") mode = "basic";
        }

        updateSpeed();

        //walk code
        Walk(dir);
        anim.SetHorizontalMovement(x, y, rb.velocity.y);

        //wallgrab code
        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if(side != coll.wallSide)
                anim.Flip(side*-1);
            wallGrab = true;
            wallSlide = false;
        }

        //wallgrab code
        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
        }

        if (Input.GetKeyDown("1"))
            updateMoveset(1);
        if (Input.GetKeyDown("2"))
            updateMoveset(2);
        if (Input.GetKeyDown("3"))
            updateMoveset(3);

        if(Input.GetKeyDown("r"))
            rb.transform.position = new Vector2(-9, -3);

        //resetting wall jumps
        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }
        
        //wallgrab code
        if (wallGrab && !isDashing)
        {
            rb.gravityScale = 0;
            if(x > .2f || x < -.2f)
            rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
            rb.gravityScale = playerGrav;

        //wallgrab code
        if(coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        //reset wallgrab code
        if (!coll.onWall || coll.onGround)
            wallSlide = false;


        //jump code
        if (Input.GetButtonDown("Jump"))
        {
            if(moveset > 1 && coll.onGround || moveset > 1 && coll.onWall)
                jumpSound.Play();
            anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector2.up, false);
            if (coll.onWall && !coll.onGround)
                WallJump();
        }

        //dash code
        if (mode == "polished") {
            if ((Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.DownArrow))&& !hasDashed)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow)) Dash(-1, 0);
                else if (Input.GetKeyDown(KeyCode.RightArrow)) Dash(1, 0);
                else if (Input.GetKeyDown(KeyCode.UpArrow)) Dash(0, 1);
                else if (Input.GetKeyDown(KeyCode.DownArrow)) Dash(0, -1);
            }
        } else if (mode == "basic") {
            if (Input.GetButtonDown("Fire1") && !hasDashed) {
                    if(xRaw != 0 || yRaw != 0)
                        Dash(xRaw, yRaw);
                }
        }

        //landing code
        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        //jump code
        if(!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        WallParticle(y);

        //hardcoded solution to a bug
        if (wallGrab || wallSlide)
            return;

        //direction facing code
        if(x > 0)
        {
            side = 1;
            anim.Flip(side);
        }
        if (x < 0)
        {
            side = -1;
            anim.Flip(side);
        }


    }//end of update

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;

        side = anim.sr.flipX ? -1 : 1;

        jumpParticle.Play();
    }

    private void Dash(float x, float y)
    {
        Camera.main.transform.DOComplete();
        Camera.main.transform.DOShakePosition(.2f, .5f, 14, 90, false, true);

        hasDashed = true;

        if(moveset > 1)
            dashSound.Play();
        anim.SetTrigger("dash");

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y);

        rb.velocity += dir.normalized * dashSpeed;
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        FindObjectOfType<GhostTrail>().ShowGhost();
        StartCoroutine(GroundDash());
        DOVirtual.Float(dashMomentum, 0, .8f, RigidbodyDrag);

        dashParticle.Play();
        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;

        yield return new WaitForSeconds(.3f);

        dashParticle.Stop();
        rb.gravityScale = playerGrav;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }

    private void WallJump()
    {
        if ((side == 1 && coll.onRightWall) || side == -1 && !coll.onRightWall)
        {
            side *= -1;
            anim.Flip(side);
        }

        StopCoroutine(DisableMovement(0));
        StartCoroutine(DisableMovement(.1f));

        Vector2 wallDir = coll.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        wallJumped = true;
    }

    private void WallSlide()
    {
        if (Input.GetKey("space") && rb.velocity.y > 0 && moveset > 1)
            return;
        if(coll.wallSide != side)
         anim.Flip(side * -1);

        if (!canMove)
            return;

        bool pushingWall = false;
        if((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;

        if (!wallJumped && moveset > 1)
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
        }
        else if(!wallJumped)
        {
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), wallJumpLerp * Time.deltaTime);
        }
    }

    private void Jump(Vector2 dir, bool wall)
    {
        if(moveset > 1)
            squishAnim.Play("SquashStretch");
        slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
        ParticleSystem particle = wall ? wallJumpParticle : jumpParticle;

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

        particle.Play();
    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    void WallParticle(float vertical)
    {
        var main = slideParticle.main;

        if (wallSlide || (wallGrab && vertical < 0))
        {
            slideParticle.transform.parent.localScale = new Vector3(ParticleSide(), 1, 1);
            main.startColor = Color.white;
        }
        else
        {
            main.startColor = Color.clear;
        }
    }

    int ParticleSide()
    {
        int particleSide = coll.onRightWall ? 1 : -1;
        return particleSide;
    }

    private void updateMoveset(uint newMoveset)
    {
        moveset = newMoveset;
        coll.moveset = newMoveset;

        if (moveset > 2)
        {
            playerGrav = 2;
            dashMomentum = 4;
        }
        else
        {
            playerGrav = 3;
            dashMomentum = 14;
        }

    }

    private void updateSpeed()
    {
        
        currentSpeed += accel * Time.deltaTime * Input.GetAxisRaw("Horizontal");
        if(Input.GetAxisRaw("Horizontal") != Math.Sign(currentSpeed))
            currentSpeed += accel * Time.deltaTime * Input.GetAxisRaw("Horizontal");

        //if no input, approach zero
        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            int speedSign = Math.Sign(currentSpeed);
            currentSpeed -= speedSign * accel * Time.deltaTime;
            if (speedSign != Math.Sign(currentSpeed))
                currentSpeed = 0;

        }

        currentSpeed = Mathf.Clamp(currentSpeed, -speed, speed);
        if (moveset > 1)
            anim.playerMoveSpeed = currentSpeed;
        else
            anim.playerMoveSpeed = Input.GetAxis("Horizontal");
    }
}
