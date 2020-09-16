using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLanternGroup : MonoBehaviour
{
    public List<PatternLantern> PatternLanterns;
    public GameObject BeamPrefab;

    // Start is called before the first frame update
    void Start()
    {
        GetChildLanterns();
    }

    public void LanternGotLit(PatternLantern litLantern) {
        Debug.Log("Lantern got lit");

        PatternLantern litLantern2 = null;
        foreach(PatternLantern pl in PatternLanterns) {
            if (pl == litLantern || !pl.Lit)
                continue;

            litLantern2 = pl;
            break;
        }

        if (litLantern2 == null)
            return;

        Debug.Log("Second lantern!");

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

    private void GetChildLanterns() {
        PatternLanterns = new List<PatternLantern>();
        for (int i = 0; i < transform.childCount; i++) {
            PatternLantern pl = transform.GetChild(i).GetComponent<PatternLantern>();
            if (pl == null)
                continue;
            PatternLanterns.Add(pl);
            pl.MyPatternLanternGroup = this;
        }
    }
}
