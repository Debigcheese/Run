using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    private PlayerMovement pm;
    public ParticleSystem jumpParticle;

    // Start is called before the first frame update
    void Start()
    {
        pm = GetComponentInParent<PlayerMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (pm.isJumping)
        {
            jumpParticle.Play();
        }
    }
}
