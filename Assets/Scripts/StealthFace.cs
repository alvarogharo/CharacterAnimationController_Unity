using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthFace : StateMachineBehaviour {

	private SkinnedMeshRenderer skmsh;

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Cuando se entra en el estado de salto se asigna la cara correcpondiente
		float vert = animator.GetFloat("Vertical");

		if (vert <= -2.5f || (vert >= 0.9f && vert <= 1.1f)){
			skmsh.SetBlendShapeWeight(27,50f);
			skmsh.SetBlendShapeWeight(22,100f);
		}else{
			skmsh.SetBlendShapeWeight(27,0f);
			skmsh.SetBlendShapeWeight(22,0f);
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		skmsh = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
	}
}
