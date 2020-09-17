using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSource : FireSource
{
    public Light MyPointLight;
    public MeshRenderer MyRenderer;
    public GameObject Barrier;

    protected override void Start() {
        //base.Start();
        MyRenderer = GetComponent<MeshRenderer>();
    }

    public override bool Light() {
        Lit = true;
        MyRenderer.material = FireController.Instance.LitGreenMat;
        MyPointLight.enabled = true;
        Barrier.SetActive(true);
        gameObject.layer = 8;
        FireController.LatestBarrier = BarrierSourceManager.Instance.GetBarrierNr(this);
        return false;
    }

    public override bool Delight() {
        /*
        Lit = false;
        MyRenderer.material = FireController.Instance.GlassMat;
        MyPointLight.enabled = false;
        Barrier.SetActive(false);
        gameObject.layer = 9;*/
        return false;
    }
}
