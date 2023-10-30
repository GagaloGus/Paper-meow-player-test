using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class PlayerController : MonoBehaviour
{
    public Rigidbody rb;
    public float moveSpeed, jumpForce;

    private Vector2 moveInput;
    private Animator animator;

    private Transform groundPoint;
    //SerializeField hace que aunque la variable sea privada se vea en el inspector
    [SerializeField] private bool isGrounded, movingBackwards;



    //variables del sprite
    private Animator c_animator;
    private SpriteRenderer c_sprtRend;

    //Asignamos las variables en el Awake (va antes del start)
    private void Awake()
    {
        //Groundpoint es el punto en el que el raycast detecta si el personaje esta tocando el suelo
        groundPoint = transform.Find("GroundCheckPoint");
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        //coje los componentes del sprite
        c_animator = transform.Find("Sprite").gameObject.GetComponent<Animator>();
        c_sprtRend = transform.Find("Sprite").gameObject.GetComponent<SpriteRenderer>();
    }
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        rb.velocity = new Vector3(moveInput.x * moveSpeed, rb.velocity.y, moveInput.y * moveSpeed);

        //lanza un rayo de longitud 0.3 hacia abajo desde la posicion del ground point, tambien que solo detecte la layer del suelo
        isGrounded =
            Physics.Raycast(
                groundPoint.position,          // posicion de origen del rayo
                Vector3.down,                  // vector de direccion del rayo
                0.3f,                          // distancia del rayo
                LayerMask.GetMask("Ground"));  // Mascara del suelo, para que solo detecte el suelo

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded )
        {
            rb.velocity += new Vector3(0, jumpForce, 0);
        }

        //si estamos mirando a la derecha y nuestra vel en x es menor que 0, el sprite se da la vuelta
        //tambien que solo se pueda dar la vuelta si estamos en el suelo
        if(!c_sprtRend.flipX && moveInput.x < 0 && isGrounded)
        {
            c_sprtRend.flipX = true;
            SpinFlipPlayer();
        }
        //si estamos mirando a la izquiera y nuestra vel en x es mayor que 0, el sprite se da la vuelta otra vez
        else if (c_sprtRend.flipX && moveInput.x > 0 && isGrounded)
        {
            c_sprtRend.flipX = false;
            SpinFlipPlayer();
        }

        //lo mismo de antes, pero ahora es si estoy moviendome hacia atras y la vel en z
        if(!movingBackwards && moveInput.y > 0)
        {
            movingBackwards = true;
            SpinFlipPlayer();
        }
        else if(movingBackwards && moveInput.y < 0)
        {
            movingBackwards = false;
            SpinFlipPlayer();
        }

        //cambiar los parametros del animator
        c_animator.SetBool("onGround", isGrounded);
        c_animator.SetFloat("moveSpeed", moveInput.magnitude);
        c_animator.SetBool("movingBackwards", movingBackwards);
    }

    void SpinFlipPlayer()
    {
        //funciona como el animator.Play()
        animator.SetTrigger("Flip");
    }
}
