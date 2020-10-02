using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip PlayerRun;
    public AudioClip PlayerWalk;
    public AudioClip LightFire;
    public AudioClip LightBigFire;
    public AudioClip BlowOutFire;

    private void Awake() {
        Instance = this;
    }
}
