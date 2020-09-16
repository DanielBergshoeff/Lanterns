using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSource : FireSource
{
    public Light MyPointLight;
    public MeshRenderer MyRenderer;
    public GameObject Barrier;

    protected override void Start() {
        base.Start();
        MyRenderer = GetComponent<MeshRenderer>();
    }

    public override void Light() {
        base.Light();
        MyRenderer.material = FireController.Instance.LitMat;
        MyPointLight.enabled = true;
        Barrier.SetActive(true);
        gameObject.layer = 8;
    }

    public override void Delight() {
        base.Delight();
        MyRenderer.material = FireController.Instance.GlassMat;
        MyPointLight.enabled = false;
        Barrier.SetActive(false);
        gameObject.layer = 9;
    }
}
