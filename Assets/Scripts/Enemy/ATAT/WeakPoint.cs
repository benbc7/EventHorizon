using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : Living {

	protected override void Die () {
		base.Die ();
		transform.parent.parent.parent.GetComponent<ATATController> ().OnDeath ();
		transform.parent.parent.GetComponent<ATATAnimationController> ().DeathAnimation ();
	}
}