using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandUpBehaviour : StateMachineBehaviour {

	

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Pone la varible standUp a true cuando se sale del estado
		animator.SetBool("StandUp", true);
		
	}

	
}
