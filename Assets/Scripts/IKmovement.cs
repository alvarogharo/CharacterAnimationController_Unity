using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKmovement : MonoBehaviour {

	public Transform drink;
	public Transform hand;
	private Animator anim;

	public float speed;
	private float weight;
	private float grabWeight;
	bool aux;
	bool pick;
	bool oneTime;
	bool decrease;
	// Use this for initialization
	void Start () {
		anim =  GetComponent<Animator>();
		weight = 0f;
		aux =false;
		pick = false;
		oneTime = true;
		decrease = false;
	}
	
	// Update is called once per frame
	void Update () {

		//Comienza la animacion al pulsar espacio
		if (Input.GetKeyDown("space")){
			anim.SetTrigger("Grab");
			aux = true;
			StartCoroutine("resetAux");
		}
		//Incremento o decremento de pesos para el lookAt de la cabeza
		if (weight < 0.6f && aux){
			weight += 0.05f;
		}else if (weight >= 0f && !aux){
			weight -= 0.05f;
		}
		
		//Si el personaje no se ha levantado entonces comienza a leer IKGrab de las curves
		if (!anim.GetBool("StandUp")){
			grabWeight = anim.GetFloat("IKgrab");
		}
	}

	void LateUpdate()
	{
		//Si el personaje no se ha levantado ya
		//Comoenzar toda la animacion
		if (!anim.GetBool("StandUp")){
			if (grabWeight == 1){
				pick = true;
				
				Transform[] list = hand.GetComponentsInChildren<Transform>();

				//Apertura de mano
				foreach (Transform item in list){
					item.localRotation =  Quaternion.Euler(0,0,-30);
				}
				
				
			}
			if (pick){
				Transform[] list = hand.GetComponentsInChildren<Transform>();
				
				//Recolocacion de mano
				foreach (Transform item in list){
					item.localRotation =  Quaternion.Euler(0,0,-30);
				}
				
				//Ajuste de las posiciones de la bebida
				drink.parent =  hand;
				
				drink.localRotation = Quaternion.Euler(-48.554f,78.352f,-71.784f);
				drink.localPosition =  new Vector3(0.065f,-0.0599f,0.06399f);
			}
		}else{
			
			//Cuando ya se ha levantado desvinculamos la bebida para poder reproduccir la animacion
			//y ponemos el peso del IK a 1f
			if (oneTime){
				oneTime=false;
				drink.GetComponent<Animator>().SetTrigger("Grabbed");
				drink.parent = null;
				drink.GetComponent<Animator>().SetTrigger("Drink");
				grabWeight = 1f;
			}else{
				StartCoroutine("IKTimer");
				if (decrease){
					grabWeight -= 0.05f;
				}
			}

		}
		
	}

	//Temporizador para comenzar a decrecer los pesos
	private IEnumerator IKTimer(){
		yield return new WaitForSeconds(2f);
		decrease = true;
	}

	//Temporizador para el reseteo de la variable aux
	IEnumerator resetAux(){
		yield return new WaitForSeconds(7.5f);
		aux = false;
	}

	//Asignacion de pesos y GameObjects de los IK
	void OnAnimatorIK(int layerIndex)
	{
		anim.SetIKPosition(AvatarIKGoal.RightHand,drink.position);
		anim.SetIKPositionWeight(AvatarIKGoal.RightHand,grabWeight);

		anim.SetLookAtPosition(drink.position);
		anim.SetLookAtWeight(weight);
	}
}
