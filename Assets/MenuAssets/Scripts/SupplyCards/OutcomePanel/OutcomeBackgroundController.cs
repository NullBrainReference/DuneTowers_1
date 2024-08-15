using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcomeBackgroundController : MonoBehaviour
{
    [SerializeField] private GameObject background;

    [SerializeField] private Animator animator;

    public void Open()
    {
        background.SetActive(true);
        PlayShow();
    }

    public void OnClose()
    {
        background.SetActive(false);
        animator.Rebind();
    }

    private void PlayShow()
    {
        animator.Rebind();
        //animator.Play("BackgroundShow");
    }

    public void PlayClose()
    {
        animator.Rebind();
        animator.Play("BackgroundClose");
    }
}
