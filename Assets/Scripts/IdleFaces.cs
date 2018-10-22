using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleFaces : StateMachineBehaviour {

	private SkinnedMeshRenderer skmsh;
	private bool nextFace;
	private int lastState;
	private int count;

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		float vert = animator.GetFloat("Vertical");
		
		//Rota entre las diferentes posubles expresiones aleatorias y las cambia cada cierto tiempo
		if (vert > -0.2f && vert < 0.2f){
			if (nextFace){
				skmsh.SetBlendShapeWeight(lastState,0f);
				lastState =  Random.Range(16,35);
				skmsh.SetBlendShapeWeight(lastState,100f);
				nextFace = false;
			}

			if (count >= 200){
				count = 0;
				nextFace = true;
			}

			count++;
		}else{
			skmsh.SetBlendShapeWeight(lastState,0f);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Inicializa valores de variables al entrar al estado
		skmsh = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
		nextFace = true;
		lastState = 16;
	}
}
