using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class ActivatorTrigger : MonoBehaviour {

    public GameObject [] activatables;

    [SerializeField, HideInInspector]
    private List<IActivatable> iActivatables = new List<IActivatable> ();

    private void Start () {
        for (int i = 0; i < activatables.Length; i++) {
            if (activatables[i] != null) {
                IActivatable newIActivatable = activatables [i].GetComponent<IActivatable> ();
                if (newIActivatable != null) {
                    iActivatables.Add (newIActivatable);
                }
            }
        }
    }

    private void OnTriggerEnter2D (Collider2D collision) {
        for (int i = 0; i < activatables.Length; i++) {
            iActivatables [i].Activate ();
        }
    }
}
