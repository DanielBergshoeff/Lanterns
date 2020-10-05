﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FireSource : MonoBehaviour
{
    public bool Lit = false;
    [HideInInspector] public Flame MyFlame;
    [HideInInspector] public Vector3 Core;

    protected virtual void Start() {
        FireSourceManager.Instance.AddFireSource(this);
        if (Lit) {
            Flame f = Instantiate(FireController.Instance.FlamePrefab).GetComponent<Flame>();
            f.transform.position = Core;
            Light(f);
        }
        Core = Vector3.zero;
    }

    public abstract bool Light(Flame flame);

    public abstract bool Delight(FireSource target);

    public abstract bool CanReceiveLight();

    public abstract bool CanSendLight();
}
