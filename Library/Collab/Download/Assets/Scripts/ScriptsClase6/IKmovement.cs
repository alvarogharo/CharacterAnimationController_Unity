using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKmovement : MonoBehaviour {

	public Transform sphere;
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
		//sphere = GameObject.Find("Sphere").transform;
		anim =  GetComponent<Animator>();
		weight = 0f;
		aux =false;
		pick = false;
		oneTime = true;
		decrease = false;
	}
	
	// Update is called once per frame
	void Update () {

		float hor = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
		float ver = Input.GetAxis("Vertical") * speed * Time.deltaTime;

		if (Input.GetKeyDown("space")){
			anim.SetTrigger("Grab");
			aux = true;
			StartCoroutine("resetAux");
		}
		if (weight < 0.6f && aux){
			weight += 0.05f;
		}else if (weight >= 0f && !aux){
			weight -= 0.05f;
		}
		

		sphere.Translate(-hor,ver,0f);
		if (!anim.GetBool("StandUp")){
			grabWeight = anim.GetFloat("IKgrab");
		}
	}

	void LateUpdate()
	{
		if (!anim.GetBool("StandUp")){
			if (grabWeight == 1){
				pick = true;
				
				Transform[] list = hand.GetComponentsInChildren<Transform>();

				foreach (Transform item in list){
					item.localRotation =  Quaternion.Euler(0,0,-30);
				}
				
				//hand.localPosition =  new Vector3(0.071f,-0.044f,0.009f);
				//hand.localRotation =  Quaternion.Euler(-48.554f,78.352f,-71.784f);
				
			}
			if (pick){
				Transform[] list = hand.GetComponentsInChildren<Transform>();

				foreach (Transform item in list){
					item.localRotation =  Quaternion.Euler(0,0,-30);
				}

				sphere.parent =  hand;
				
				sphere.localRotation = Quaternion.Euler(-48.554f,78.352f,-71.784f);
				sphere.localPosition =  new Vector3(0.065f,-0.0599f,0.06399f);
			}
		}else{
			
			if (oneTime){
				oneTime=false;
				sphere.GetComponent<Animator>().SetTrigger("Grabbed");
				sphere.parent = null;
				sphere.GetComponent<Animator>().SetTrigger("Drink");
				grabWeight = 1f;
			}else{
				StartCoroutine("IKTimer");
				if (decrease){
					grabWeight -= 0.05f;
				}
			}

		}
		
	}

	private IEnumerator IKTimer(){
		yield return new WaitForSeconds(2f);
		decrease = true;
	}

	void OnAnimatorIK(int layerIndex)
	{
		anim.SetIKPosition(AvatarIKGoal.RightHand,sphere.position);
		anim.SetIKPositionWeight(AvatarIKGoal.RightHand,grabWeight);

		anim.SetLookAtPosition(sphere.position);
		anim.SetLookAtWeight(weight);
	}

	IEnumerator resetAux(){
		yield return new WaitForSeconds(7.5f);
		aux = false;
	}


}
