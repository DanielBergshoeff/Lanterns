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

    public override bool Delight() {
        base.Delight();

        cooldownTime = 1f;
        return true;
    }

    public override bool Light() {
        if (cooldownTime > 0f)
            return false;

        base.Light();

        MyPatternLanternGroup.LanternGotLit(this);

        return true;
    }
}
