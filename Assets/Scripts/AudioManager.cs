﻿using System.Collections;
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

    // Start is called before the first frame update
    void Start() {
        Instance = this;
    }

    // Update is called once per frame
    void Update() {

    }
}
