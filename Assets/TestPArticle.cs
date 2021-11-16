using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPArticle : MonoBehaviour
{
    private ParticleSystem fx;

    private void Start()
    {
        fx = GetComponent<ParticleSystem>();

        //var m = fx.emission;
        //var burst = m.GetBurst(0);
        //burst.count = 1;
        //m.SetBurst(0, burst);
    }

}
