using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpFace : StateMachineBehaviour {

	private SkinnedMeshRenderer skmsh;

    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Cuando se entra en el estado de salto se asigna la cara correcpondiente
		skmsh = GameObject.FindGameObjectWithTag("Player").transform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

		skmsh.SetBlendShapeWeight(29,100f);
	}

	// OnStateExit is called before OnStateExit is called on any state inside this state machine
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Cuando se sale del estado la expresion se anula
		skmsh.SetBlendShapeWeight(29,0f);
	}

}
