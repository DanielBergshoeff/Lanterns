using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSourceManager : MonoBehaviour
{
    public static FireSourceManager Instance;

    public List<FireSource> AllFireSources;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        AllFireSources = new List<FireSource>();
    }

    public void AddFireSource(FireSource fs) {
        AllFireSources.Add(fs);
    }

    public FireSource GetClosestActiveSource(Vector3 pos, float maxDistance) {
        float closest = float.PositiveInfinity;
        FireSource closestFs = null;

        foreach(FireSource fs in AllFireSources) {
            if (!fs.Lit)
                continue;

            float sqrDistance = (fs.transform.position - pos).sqrMagnitude;
            if (sqrDistance > closest || sqrDistance > maxDistance * maxDistance)
                continue;

            closest = sqrDistance;
            closestFs = fs;
        }

        return closestFs;
    }

    public FireSource GetClosestActiveSource(Vector3 pos, float maxDistance, FireSource excludeFireSource) {
        float closest = float.PositiveInfinity;
        FireSource closestFs = null;

        foreach (FireSource fs in AllFireSources) {
            if (!fs.Lit || fs == excludeFireSource)
                continue;

            float sqrDistance = (fs.transform.position - pos).sqrMagnitude;
            if (sqrDistance > closest || sqrDistance > maxDistance * maxDistance)
                continue;

            closest = sqrDistance;
            closestFs = fs;
        }

        return closestFs;
    }
}
