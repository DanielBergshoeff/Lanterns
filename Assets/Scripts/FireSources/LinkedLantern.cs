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

    public override bool Delight(FireSource target) {
        base.Delight(target);

        linking = false;
        return true;
    }

    public override bool Light(Flame flame) {
        base.Light(flame);

        linking = true;
        linkTime = Time.time + WaitTime;
        return true;
    }

    private void LightLinkedLantern() {
        if (MyLinkedLantern != null && Lit) {
            if (MyLinkedLantern.CanReceiveLight()) {
                Delight(MyLinkedLantern);
            }
            else {
                linkTime = Time.time + WaitTime;
            }
        }
    }
}
