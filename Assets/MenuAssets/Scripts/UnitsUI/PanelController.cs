using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    public enum AnimStates { StatStayClose, StatStayOpen, StatClose, StatOpen }

    [SerializeField] private Animator panelAnimator;
    [SerializeField] private string panelOpenAnim;
    [SerializeField] private string panelCloseAnim;

    [SerializeField] private Animator[] animators;

    [SerializeField] private StatUnit gunStat;
    [SerializeField] private StatUnit armorStat;
    [SerializeField] private StatUnit speedStat;

    [SerializeField] private GameObject speedInfoPanel;

    public StatUnit GunStat { get { return gunStat; } }
    public StatUnit ArmorStat { get { return armorStat; } }
    public StatUnit SpeedStat { get { return speedStat; } }

    public void PlayOpen()
    {
        foreach(Animator animator in animators)
            animator.Play(AnimStates.StatOpen.ToString());

        speedInfoPanel.SetActive(false);
    }

    public void PlayClose()
    {
        foreach (Animator animator in animators)
            animator.Play(AnimStates.StatClose.ToString());

        speedInfoPanel.SetActive(false);
    }

    public void StayOpen()
    {
        foreach (Animator animator in animators)
            animator.Play(AnimStates.StatStayOpen.ToString());

        speedInfoPanel.SetActive(false);
    }

    public void StayClose()
    {
        foreach (Animator animator in animators)
            animator.Play(AnimStates.StatStayClose.ToString());

        speedInfoPanel.SetActive(false);
    }

    public void PanelPlayOpen()
    {
        panelAnimator.Play(panelOpenAnim);
        PlayOpen();

        speedInfoPanel.SetActive(false);
    }

    public void PanelPlayClose()
    {
        panelAnimator.Play(panelCloseAnim);
        PlayClose();

        speedInfoPanel.SetActive(false);
    }

    public void PanelStayClose()
    {
        panelAnimator.Play("StayClose");
        StayClose();

        speedInfoPanel.SetActive(false);
    }

    public void PanelStayOpen()
    {
        panelAnimator.Play("StayOpen");
        StayOpen();

        speedInfoPanel.SetActive(false);
    }
}
