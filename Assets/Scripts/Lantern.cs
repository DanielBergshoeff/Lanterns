using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : FireSource
{
    public Light MyPointLight;
    public MeshRenderer MyRenderer;

    protected override void Start() {
        base.Start();
        MyRenderer = GetComponent<MeshRenderer>();
    }

    public override bool Light() {
        Lit = true;
        MyRenderer.material = FireController.Instance.LitMat;
        MyPointLight.enabled = true;
        gameObject.layer = 8;
        return true;
    }

    public override bool Delight() {
        Lit = false;
        MyRenderer.material = FireController.Instance.GlassMat;
        MyPointLight.enabled = false;
        gameObject.layer = 9;
        return true;
    }
}
