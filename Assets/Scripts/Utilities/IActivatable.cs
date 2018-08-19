using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivatable {

    bool Activated {
        get; set;
    }

    void Activate ();
}
