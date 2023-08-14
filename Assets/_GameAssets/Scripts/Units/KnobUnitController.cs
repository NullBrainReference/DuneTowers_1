using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KnobUnitController : MonoBehaviour
{
    public bool isOn = true;
#if UNITY_EDITOR
    public bool testUnitActive;
#endif

    public void UpdateStatusUnitActive()
    {
        if (isOn)
        {
            SetUnitNotActive();
        }
        else
        {
            SetUnitActive();
        }
    }

    private void SetUnitActive()
    {
        if (!isOn)
        {
            isOn = true;
            Animator animator = GetComponent<Animator>();
            animator.SetBool("KnobOn", true);
        }

    }

    private void SetUnitNotActive()
    {
        if (isOn)
        {
            isOn = false;
            Animator animator = GetComponent<Animator>();
            animator.SetBool("KnobOn", false);
        }

    }

#if UNITY_EDITOR
    void Update()
    {
        if (testUnitActive)
        {
            testUnitActive = false;
            UpdateStatusUnitActive();
        }
    }
#endif

}
