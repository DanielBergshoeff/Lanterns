using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLanternGroup : LanternGroup
{
    public GameObject BeamPrefab;

    public override void LanternGotLit(PatternLantern litLantern) {
        PatternLantern litLantern2 = null;
        foreach(PatternLantern pl in PatternLanterns) {
            if (pl == litLantern || !pl.Lit)
                continue;

            litLantern2 = pl;
            break;
        }

        if (litLantern2 == null)
            return;

        litLantern.Delight();
        litLantern2.Delight();

        Vector3 dir = litLantern2.transform.position - litLantern.transform.position;
        Vector3 heading = dir.normalized;

        float length = 0f;

        RaycastHit hit;
        if(Physics.Raycast(litLantern2.transform.position, heading, out hit, 10f)) {
            length = hit.distance;
        }
        else {
            length = 10f;
        }

        length = 10f;

        GameObject go = Instantiate(BeamPrefab);
        go.transform.rotation = Quaternion.LookRotation(dir);
        go.transform.position = litLantern2.transform.position - heading * length / 2f;
        go.transform.GetChild(0).localScale = new Vector3(1f, length / 2f, 1f);
    }

    
}
