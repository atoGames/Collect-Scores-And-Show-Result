using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Platformer2D : MonoBehaviour
{
    protected Rigidbody2D m_Rigidbody2D;
    protected CapsuleCollider2D mainCollider;

    [SerializeField] protected float m_Speed = 5f;
    [SerializeField] protected float m_JumpHeight = 8f;
    protected float m_Direction = 0;

    protected bool m_DoubleJump = false;
    [Range(-0.25f, 0.25f), SerializeField] protected float skinWidth = 0f;
    public LayerMask groundLayer;
    void Start()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        mainCollider = GetComponentInChildren<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

        JumpVelocity(Input.GetKeyDown(KeyCode.Space), true);
        // controls
        m_Direction = Input.GetAxisRaw("Horizontal");

        Flip(Mathf.FloorToInt(Mathf.Clamp(m_Direction, -1, 1))); // -1 and 1

    }
    void FixedUpdate()
    {
        // Apply movement 
        if (m_Direction != 0 && CheckGround())
            m_Rigidbody2D.velocity = new Vector2((m_Direction) * m_Speed, m_Rigidbody2D.velocity.y);
    }
    private void Flip(int f)
    {
        if (f != 0 && CheckGround() || CanJumpBack())
            transform.localScale = new Vector3(f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);

        // Make sure this not to be zero
        if (transform.localScale.x == 0)
            transform.localScale = Vector3.one;

    }
    protected void JumpVelocity(bool keyPress, bool useDoubleJump)
    {
        // Normal jump
        if (CheckGround() && keyPress)
        {
            m_DoubleJump = true;
            Jumping();
        }
        else
        {
            //Double Jump
            if (useDoubleJump && m_DoubleJump && keyPress)
            {
                Jumping();
                m_DoubleJump = false;
            }
        }
        // wall jump
        if (!m_DoubleJump && keyPress && CanJumpBack())
            m_Rigidbody2D.velocity = new Vector2(m_Direction * 5, m_JumpHeight);
    }

    private void Jumping() => m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpHeight);

    private bool CheckGround()
    {
        var radius = .05f;

        RaycastHit2D hitBottom = Physics2D.Linecast(bottom, bottom + Vector3.down * radius, groundLayer);
        RaycastHit2D hitGroundLeft = Physics2D.Linecast(bottomLeft, bottomLeft + Vector3.down * radius, groundLayer);
        RaycastHit2D hitGroundRight = Physics2D.Linecast(bottomRight, bottomRight + Vector3.down * radius, groundLayer);

        if (hitBottom || hitGroundLeft || hitGroundRight)
            return true;

        return false;
    }

    private bool CanJumpBack()
    {

        var dir = armLeft + Vector3.left * m_Direction;
        RaycastHit2D hitWall = Physics2D.Linecast(armLeft, dir, groundLayer);

        if (hitWall)
            return true;

        return false;

    }
    private void OnDrawGizmosSelected()
    {
        mainCollider = null ?? GetComponentInChildren<CapsuleCollider2D>();
        var radius = .01f;

        Gizmos.color = Color.red;
        // ground check debug
        Gizmos.DrawSphere(bottomLeft, radius);
        Gizmos.DrawSphere(bottomRight, radius);
        Gizmos.DrawSphere(bottom, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomLeft, bottomLeft + Vector3.down * radius);
        Gizmos.DrawLine(bottomRight, bottomRight + Vector3.down * radius);
        Gizmos.DrawLine(bottom, bottom + Vector3.down * radius);

        // wall jump debug
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(armLeft, radius);
        Gizmos.DrawSphere(armRight, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(armLeft, armLeft + Vector3.left * radius);
        Gizmos.DrawLine(armRight, armRight + Vector3.right * radius);

    }

    protected Vector3 armLeft => new Vector3(mainCollider.bounds.min.x, mainCollider.bounds.center.y, 0);
    protected Vector3 armRight => new Vector3(mainCollider.bounds.max.x, mainCollider.bounds.center.y, 0);
    protected Vector3 bottom => new Vector3(mainCollider.bounds.center.x, mainCollider.bounds.min.y, 0);
    protected Vector3 bottomLeft => mainCollider.bounds.min + Vector3.left * skinWidth;
    protected Vector3 bottomRight => new Vector3(mainCollider.bounds.max.x, mainCollider.bounds.min.y, 0) + Vector3.right * skinWidth;

}