﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SelfDestroy", 1f);
    }

    private void SelfDestroy() {
        Destroy(gameObject);
    }
}
