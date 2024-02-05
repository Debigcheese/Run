using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    private Animator anim;
    public Collider2D[] blockCollider;

    private bool shakeStarted = false;
    private bool cancelShake = false;
    public float startShake = 3f;
    public float respawnCooldown = 8f;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        for(int i = 0; i< blockCollider.Length; i++)
        {
            blockCollider[0].enabled = true;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(blockCollider[0].IsTouchingLayers(LayerMask.GetMask("Player")) && !shakeStarted)
        {
            StartCoroutine(BlockShake());
        }
        else if(!blockCollider[0].IsTouchingLayers(LayerMask.GetMask("Player")))
        {
            cancelShake = true;
        }
    }

    private IEnumerator BlockShake()
    {
        shakeStarted = true;
        cancelShake = false;
        yield return new WaitForSeconds(startShake);

        if (!cancelShake)
        {
            anim.SetBool("ShakeBlock", true);

            yield return new WaitForSeconds(2f);
            anim.SetBool("ShakeBlock", false);
            anim.SetBool("FallBlock", true);

            yield return new WaitForSeconds(.5f);

            for (int i = 0; i < blockCollider.Length; i++)
            {
                blockCollider[i].enabled = false;
            }

            yield return new WaitForSeconds(.5f);
            anim.SetBool("FallBlock", false);

            yield return new WaitForSeconds(respawnCooldown);
            anim.SetBool("RespawnBlock", true);

            yield return new WaitForSeconds(1f);
            anim.SetBool("RespawnBlock", false);
            shakeStarted = false;

            for (int i = 0; i < blockCollider.Length; i++)
            {
                blockCollider[i].enabled = true;
            }
        }
        else
        {
            cancelShake = true;
            shakeStarted = false;
        }


    }
}
