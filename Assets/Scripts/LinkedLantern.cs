using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkedLantern : Lantern
{
    public float WaitTime = 3f;
    public Lantern MyLinkedLantern;

    private float linkTime = 0f;
    private bool linking = false;

    private void Update() {
        if (!linking)
            return;

        if(Time.time >= linkTime) {
            LightLinkedLantern();
        }
    }

    public override void Delight() {
        base.Delight();

        linking = false;
    }

    public override void Light() {
        base.Light();

        linking = true;
        linkTime = Time.time + WaitTime;
    }

    private void LightLinkedLantern() {
        if (MyLinkedLantern != null && Lit) {
            Delight();
            MyLinkedLantern.Light();
        }
    }
}
