using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using String;

public class Collision : MonoBehaviour
{
    internal float coyoteTime = 0.1f;
    [Header("Layers")]
    public LayerMask groundLayer;

    internal uint moveset = 1;

    [Space]

    public bool onGround;
    public bool onWall;
    public bool onRightWall;
    public bool onLeftWall;
    public int wallSide;
    public string mode;
    public float playerMoveSpeed;

    [Space]

    [Header("Collision")]

    public float collisionRadius = 0.25f;
    public Vector2 bottomOffset, rightOffset, leftOffset;
    private Color debugCollisionColor = Color.red;

    // Start is called before the first frame update
    void Start()
    {
        //mode = "basic";
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) {
            //if (mode == "basic") mode = "polished";
            //else if (mode == "polished") mode = "basic";
        }

        //if (mode == "polished") checkCoyote();
       
        checkCoyote();
        

        onWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer) 
            || Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        onRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, collisionRadius, groundLayer);
        onLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, collisionRadius, groundLayer);

        wallSide = onRightWall ? -1 : 1;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        var positions = new Vector2[] { bottomOffset, rightOffset, leftOffset };

        Gizmos.DrawWireSphere((Vector2)transform.position  + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }

    void checkCoyote()
    {
        if(moveset == 1)
        {
            onGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);
            return;
        }

        if (Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer))
        {
            onGround = true;
            StartCoroutine(coyote(coyoteTime));
        }
        else
            onGround = false;
    }

    IEnumerator coyote(float x)
    {
        yield return new WaitForSeconds(x);
        onGround = true;
    }
}
