using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LanternGroup : MonoBehaviour
{
    public List<PatternLantern> PatternLanterns;

    // Start is called before the first frame update
    void Awake()
    {
        GetChildLanterns();
    }

    public virtual void LanternGotLit(PatternLantern litLantern) { }

    public virtual void LanternGotDelit(PatternLantern delitLantern) { }

    protected void GetChildLanterns() {
        PatternLanterns = new List<PatternLantern>();
        for (int i = 0; i < transform.childCount; i++) {
            PatternLantern pl = transform.GetChild(i).GetComponent<PatternLantern>();
            if (pl == null)
                continue;
            PatternLanterns.Add(pl);
            pl.MyLanternGroup = this;
        }
    }
}
