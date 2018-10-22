using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningFace : StateMachineBehaviour {

	private SkinnedMeshRenderer skmsh;

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Cuando se entra en el estado de salto se asigna la cara correcpondiente
		if (animator.GetFloat("Vertical") >= 3f){
			skmsh.SetBlendShapeWeight(29,100f);
		}else{
			skmsh.SetBlendShapeWeight(29,0f);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		skmsh = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
	}
}
