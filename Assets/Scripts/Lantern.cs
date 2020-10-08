using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lantern : FireSource
{
    public Light MyPointLight;
    public MeshRenderer MyRenderer;
    public AudioSource MyAudioSource;

    private bool expectingLight = false;

    private void Awake() {
        MyRenderer = GetComponent<MeshRenderer>();
        MyAudioSource = gameObject.AddComponent<AudioSource>();
        MyAudioSource.volume = 1f;
        MyAudioSource.spatialBlend = 1f;
        MyPointLight = GetComponentInChildren<Light>();
        Core = MyPointLight.transform.position;
    }

    protected override void Start() {
        base.Start();
        FireSourceManager.Instance.AddLantern(this);
    }

    public override bool Light(Flame flame) {
        expectingLight = false;
        Lit = true;
        MyFlame = flame;
        Material[] mats = MyRenderer.materials;
        for (int i = 0; i < mats.Length; i++) {
            if(mats[i].name == FireController.Instance.GlassMat.name + " (Instance)") {
                mats[i] = FireController.Instance.LitMat;
            }
        }
        MyRenderer.materials = mats;
        MyAudioSource.PlayOneShot(AudioManager.Instance.LightFire, 0.5f);
        MyPointLight.enabled = true;
        gameObject.layer = 8;
        return true;
    }

    public override bool Delight(FireSource target) {
        Lit = false;
        Material[] mats = MyRenderer.materials;
        for (int i = 0; i < mats.Length; i++) {
            if (mats[i].name == FireController.Instance.LitMat.name + " (Instance)") {
                mats[i] = FireController.Instance.GlassMat;
            }
        }
        MyRenderer.materials = mats;
        MyAudioSource.PlayOneShot(AudioManager.Instance.BlowOutFire, 1f);
        MyPointLight.enabled = false;
        gameObject.layer = 9;

        if (target != null) {
            MyFlame.SetToActive(target);
        }
        else {
            Destroy(MyFlame.gameObject);
        }

        return true;
    }

    public override bool CanReceiveLight() {
        if (Lit || expectingLight)
            return false;

        expectingLight = true;
        return true;
    }

    public override bool CanSendLight() {
        if (!Lit)
            return false;
       
        return true;
    }
}
