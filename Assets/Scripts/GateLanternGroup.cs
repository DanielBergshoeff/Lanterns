using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateLanternGroup : LanternGroup
{
    public Animator GateAnimator; 

    private int litLanterns = 0;
    private bool gateOpen = false;

    public override void LanternGotLit(PatternLantern litLantern) {
        litLanterns++;

        if(litLanterns >= PatternLanterns.Count) {
            OpenGate();
        }
    }

    public override void LanternGotDelit(PatternLantern delitLantern) {
        litLanterns--;

        if (gateOpen) {
            //CloseGate(); Temporarily turned off for prototype
        }
    }

    private void OpenGate() {
        gateOpen = true;
        GateAnimator.SetBool("Open", true);
    }

    private void CloseGate() {
        gateOpen = false;
        GateAnimator.SetBool("Open", false);
    }
}
