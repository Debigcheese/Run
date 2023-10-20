using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionScript : MonoBehaviour
{
    public Animator transitionAnim;
    public GameObject transitionImage;
    public float transitionDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        transitionImage.SetActive(true);
        transitionAnim.SetBool("TransitionEnd", true);
        StartCoroutine(EndTransition());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadLevelSelecter()
    {
        StartCoroutine(TransitionStart());
    }

    public IEnumerator TransitionStart()
    {
        transitionImage.SetActive(true);
        transitionAnim.SetBool("TransitionStart", true);
        yield return new WaitForSeconds(transitionDuration);

    }

    private IEnumerator EndTransition()
    {
        yield return new WaitForSeconds(1f);
        transitionAnim.SetBool("TransitionEnd", false);
        transitionImage.SetActive(false);
    }

}
