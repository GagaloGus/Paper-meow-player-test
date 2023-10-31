using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkoController : MonoBehaviour
{
    public float moveSpeed, speedMult, jumpForce;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Animator animator;

    private Transform groundPoint;
    //SerializeField hace que aunque la variable sea privada se vea en el inspector
    [SerializeField] private bool isGrounded, isFlipped, isFacingBackwards;

    //variables del modelo 3d
    GameObject m_gameobj;
    Animator m_animator;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        groundPoint = transform.Find("GroundCheckPoint");

        m_gameobj = transform.Find("3dmodel").gameObject;
        m_animator = m_gameobj.GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        rb.velocity = new Vector3(moveInput.x * moveSpeed, 0, moveInput.y * moveSpeed) * (Input.GetKey(KeyCode.LeftShift) ? speedMult : 1)
            + Vector3.up * rb.velocity.y;

        isGrounded =
           Physics.Raycast(
               groundPoint.position,          // posicion de origen del rayo
               Vector3.down,                  // vector de direccion del rayo
               0.3f,                          // distancia del rayo
               LayerMask.GetMask("Ground"));  // Mascara del suelo, para que solo detecte el suelo

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity += new Vector3(0, jumpForce, 0);
        }

        #region Flip
        if (isGrounded)
        {
            //se inverte
            if (isFacingBackwards) 
            {    
                if ((!isFlipped && moveInput.x > 0) || (isFlipped && moveInput.x < 0))
                {
                    X_Flip();
                }
            }
            else 
            {    
                if ((!isFlipped && moveInput.x < 0) || (isFlipped && moveInput.x > 0))
                {
                    X_Flip();
                }
            }

            if ((!isFacingBackwards && moveInput.y > 0) || (isFacingBackwards && moveInput.y < 0))
            {
                Back_Flip();
            }

        }

        #endregion

        m_animator.SetBool("isWalking", rb.velocity.x != 0 || rb.velocity.z != 0);
        m_animator.SetFloat("WalkingSpeed", rb.velocity.normalized.magnitude*2);
    }

    void X_Flip()
    {
        m_gameobj.transform.localScale = new(
            m_gameobj.transform.localScale.x, 
            m_gameobj.transform.localScale.y, 
            -1 * m_gameobj.transform.localScale.z);

        isFlipped = !isFlipped;
    }

    void Back_Flip()
    {
        print(isFacingBackwards ? -180 : 180);
        transform.Rotate(0, isFacingBackwards ? -180 : 180, 0);
        X_Flip();
        isFacingBackwards = !isFacingBackwards;
    }

    void SpinFlip()
    {
        animator.SetTrigger("startSpin");
    }
}
