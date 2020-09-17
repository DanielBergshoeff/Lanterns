using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSourceManager : MonoBehaviour
{
    public static BarrierSourceManager Instance;
    public List<BarrierSource> BarrierSources;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        GetBarrierSources();
    }

    private void GetBarrierSources() {
        for (int i = 0; i < transform.childCount; i++) {
            BarrierSource bs = transform.GetChild(i).GetComponent<BarrierSource>();
            if (bs != null)
                BarrierSources.Add(bs);
        }
    }

    public int GetBarrierNr(BarrierSource bs) {
        return BarrierSources.IndexOf(bs);
    }

    public BarrierSource GetBarrierByNr(int nr) {
        return BarrierSources[nr];
    }
}
