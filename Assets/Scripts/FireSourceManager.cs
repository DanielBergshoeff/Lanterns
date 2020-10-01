using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSourceManager : MonoBehaviour
{
    public static FireSourceManager Instance;

    public List<FireSource> AllFireSources;
    public List<Lantern> AllLanterns;

    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
        AllFireSources = new List<FireSource>();
        AllLanterns = new List<Lantern>();
    }

    public void AddFireSource(FireSource fs) {
        AllFireSources.Add(fs);
    }

    public void AddLantern(Lantern ln) {
        AllLanterns.Add(ln);
    }

    public FireSource GetClosestActiveLantern(Vector3 pos, float maxDistance) {
        float closest = float.PositiveInfinity;
        Lantern closestLn = null;

        foreach (Lantern ln in AllLanterns) {
            if (!ln.Lit)
                continue;

            float sqrDistance = (ln.MyPointLight.transform.position - pos).sqrMagnitude;

            if (sqrDistance > closest || sqrDistance > maxDistance * maxDistance)
                continue;

            closest = sqrDistance;
            closestLn = ln;
        }

        return closestLn;
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

            if (fs.CompareTag("Player")) {
                sqrDistance = maxDistance * maxDistance;
                if (sqrDistance < closest)
                    continue;
            }

            closest = sqrDistance;
            closestFs = fs;
        }

        return closestFs;
    }
}
