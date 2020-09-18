using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierSource : FireSource
{
    public Light MyPointLight;
    public MeshRenderer MyRenderer;
    public GameObject Barrier;

    public AudioSource MyAudioSource;

    private void Awake() {
        MyRenderer = GetComponent<MeshRenderer>();
        MyAudioSource = gameObject.AddComponent<AudioSource>();
    }

    protected override void Start() {
        //base.Start();
    }

    public override bool Light() {
        Lit = true;
        MyRenderer.material = FireController.Instance.LitGreenMat;
        MyPointLight.enabled = true;
        Barrier.SetActive(true);
        gameObject.layer = 8;
        FireController.LatestBarrier = BarrierSourceManager.Instance.GetBarrierNr(this);
        MyAudioSource.PlayOneShot(AudioManager.Instance.LightBigFire, 0.1f);
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
