using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.CompareTag("FireSource")) {
            FireSource fs = collision.collider.GetComponent<FireSource>();
            FlameController.Instance.NewFireSource(fs);
        }

        Destroy(gameObject);
    }
}
