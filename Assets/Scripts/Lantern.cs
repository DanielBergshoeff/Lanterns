using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : FireSource
{
    public Light MyPointLight;
    public MeshRenderer MyRenderer;
    public AudioSource MyAudioSource;

    private void Awake() {
        MyRenderer = GetComponent<MeshRenderer>();
        MyAudioSource = gameObject.AddComponent<AudioSource>();
        MyAudioSource.volume = 1f;
        MyAudioSource.spatialBlend = 1f;
    }

    protected override void Start() {
        base.Start();
    }

    public override bool Light() {
        Lit = true;
        MyRenderer.material = FireController.Instance.LitMat;
        MyAudioSource.PlayOneShot(AudioManager.Instance.LightFire, 0.5f);
        MyPointLight.enabled = true;
        gameObject.layer = 8;
        return true;
    }

    public override bool Delight() {
        Lit = false;
        MyRenderer.material = FireController.Instance.GlassMat;
        MyAudioSource.PlayOneShot(AudioManager.Instance.BlowOutFire, 1f);
        MyPointLight.enabled = false;
        gameObject.layer = 9;
        return true;
    }
}
