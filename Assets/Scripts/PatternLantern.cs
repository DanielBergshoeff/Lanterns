using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLantern : Lantern
{
    public PatternLanternGroup MyPatternLanternGroup;

    private float cooldownTime = 0f;


    private void Update() {
        if (cooldownTime > 0f)
            cooldownTime -= Time.deltaTime;
    }

    public override void Delight() {
        base.Delight();

        cooldownTime = 1f;
    }

    public override void Light() {
        if (cooldownTime > 0f)
            return;

        base.Light();

        MyPatternLanternGroup.LanternGotLit(this);
    }
}
