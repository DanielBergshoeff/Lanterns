using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FireSource : MonoBehaviour
{
    public bool Lit = false;

    protected virtual void Start() {
        FireSourceManager.Instance.AddFireSource(this);
        if (Lit) {
            Light();
        }
    }

    public abstract bool Light();

    public abstract bool Delight();
}
