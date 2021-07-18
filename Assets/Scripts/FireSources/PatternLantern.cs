using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLantern : Lantern
{
    public LanternGroup MyLanternGroup;

    private float cooldownTime = 0f;

    private void Update() {
        if (cooldownTime > 0f)
            cooldownTime -= Time.deltaTime;
    }

    public override bool Delight(FireSource target) {
        base.Delight(target);

        MyLanternGroup.LanternGotDelit(this);

        cooldownTime = 1f;
        return true;
    }

    public override bool Light(Flame flame) {
        if (cooldownTime > 0f)
            return false;

        base.Light(flame);

        MyLanternGroup.LanternGotLit(this);

        return true;
    }
}
