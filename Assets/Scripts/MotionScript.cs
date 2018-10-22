using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionScript : MonoBehaviour {

    Animator anim;
    Rigidbody rb;
    float rotation = 100.0f;
    public float damping = 0.15f;

    private bool nextAnimation;
    private bool startTimer;

	// Use this for initialization
	void Start () {
        anim = this.GetComponent<Animator>();
        rb =  GetComponent<Rigidbody>();
        nextAnimation = true;
        startTimer = true;
	}
	
	// Update is called once per frame
	void Update () {
        //Control de variables de movimiento
        float horizontal = Input.GetAxis("Horizontal") * rotation * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * 2f;
        
        //Rotamos el transform del personaje acorde al input
        this.transform.Rotate(0, horizontal, 0);

        //Controlles de sigilo, agachado y sprint
        if (Input.GetKey(KeyCode.LeftShift)){
            vertical *= 2;
        }else if (Input.GetKey(KeyCode.LeftAlt) && Input.GetAxis("Vertical") == -1f){
            vertical = -3f;
        }else if (Input.GetKey(KeyCode.LeftAlt) && Input.GetAxis("Vertical") == 1f){
            vertical = 1f;
        }
        if (Input.GetKey(KeyCode.LeftControl)){
            anim.SetBool("Crouched", true);
        }else{
            anim.SetBool("Crouched", false);
        }

        //Activación de animaciones de rotación en parado
        if (vertical == 0f){
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.A) ||Input.GetKey(KeyCode.D)){
                anim.SetBool("Turning",true);
            }else{
                anim.SetBool("Turning",false);
            }
            anim.SetFloat("Horizontal", Input.GetAxis("Horizontal"), damping, Time.deltaTime);
        }else{
            anim.SetBool("Turning",false);
        }

        //Control de animacion de salto
        if (Input.GetKey(KeyCode.Space)){
            anim.SetTrigger("Jump");
        }

        //Control de corrutina de la animación de idle despues de 30segs
        if (startTimer && vertical == 0f){
            StartCoroutine("idle30SecsTimer");
            startTimer = false;
        }

        //Control de la aleatorización de animaciones de idle
        if(nextAnimation && vertical == 0f){
            float aux = Random.Range(0f,1f);
            anim.SetFloat("IdleRandom",aux);
            nextAnimation = false;
            StartCoroutine("nextAnimationTimer");
        }else if (vertical != 0f){
            nextAnimation = true;
            startTimer = true;
            StopAllCoroutines();
        }

    
        anim.SetFloat("Vertical", vertical, damping, Time.deltaTime);
	}

    //Temporizador de  tiempo entre animaciones de idle aleatorias
    private IEnumerator nextAnimationTimer(){
        yield return new WaitForSeconds(10f);
        nextAnimation = true;
    }

    //Temporizador de  tiempo para la animación de idle de 30segs
    private IEnumerator idle30SecsTimer(){
        yield return new WaitForSeconds(30f);
        anim.SetTrigger("idle30Secs");
    }

    //Inicialización de la animación de caida
    void OnTriggerEnter(Collider other)
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.angularDrag = 0f;
        if (other.CompareTag("FallTrigger")){
            anim.SetBool("Falling", true);
        }
        else if (other.CompareTag("SitTrigger")){
            anim.SetBool("Sit", true);
        }
        
    }

    //Detención de la animación de caida
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("FallTrigger")){
            anim.SetBool("Falling", false);
        }
        else if (other.CompareTag("SitTrigger")){
            anim.SetBool("Sit", false);
        }
    }
}
