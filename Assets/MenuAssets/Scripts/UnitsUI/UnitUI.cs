using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    [SerializeField] private UnitStats unitStats;
    [SerializeField] private Transform slot;
    [SerializeField] private Animator animator;

    [SerializeField] public PanelController panelController;

    [SerializeField] public Text text;

    private bool isMoving = false;
    private int slotId;

    private UnitsScroll unitsScroll;

    public UnitsScroll UnitsScroll 
    { 
        get { return unitsScroll; } 
        set { unitsScroll = value; } 
    }

    public int SlotId
    {
        get { return slotId; }
        set { slotId = value; }
    }
    public bool IsMoving { get { return isMoving; } }

    public Transform Slot { get { return slot; } set { slot = value; } }

    public UnitUI(UnitStats unitStats)
    {
        this.unitStats = unitStats;
    }

    public UnitStats GetStats() 
    { 
        return unitStats;
    }

    public void SetStats(UnitStats stats)
    {
        unitStats = stats;
        panelController.GunStat.SetUpUnit(stats, StatType.Gun);
        panelController.ArmorStat.SetUpUnit(stats, StatType.Armor);
        panelController.SpeedStat.SetUpUnit(stats, StatType.Speed);
    }

    public void InitMoving(Transform transform, int slotId)
    {
        this.isMoving = true;
        this.slot = transform;
        this.slotId = slotId;
    }

    public void OnEnable()
    {
        PutInSlot();
    }

    public void PullTrigger()
    {
        if (UnitsScroll.triggerPulled) return;
        UnitsScroll.triggerPulled = true;
        UnitsScroll.UnitsUpdate();
    }

    public void PutInSlot()
    {
        this.transform.SetParent(slot);

        //if (slotId != 2)
        //{
        //    animator.Rebind();
        //}
        
        isMoving = false;
    }

    public void CallScroll()
    {
        if (slotId == 2) return;

        if (slotId < 2)
        {
            unitsScroll.SetNext();
        }
        else
        {
            unitsScroll.SetPrev();
        }
    }

    public void PlayRight()
    {
        animator.Play("SwipRight");
        isMoving = true;
    }

    public void PlayCenterToRight()
    {
        animator.Play("CenterToRight");
        panelController.PanelPlayClose();
        isMoving = true;
    }

    public void PlayRightToCenter()
    {
        animator.Play("RightToCenter");
        panelController.PanelPlayOpen();
        isMoving = true;
    }

    public void PlayLeft()
    {
        animator.Play("SwipLeft");
        isMoving = true;
    }

    public void PlayCenterToLeft()
    {
        animator.Play("CenterToLeft");
        panelController.PanelPlayClose();
        isMoving = true;
    }

    public void PlayLeftToCenter()
    {
        animator.Play("LeftToCenter");
        panelController.PanelPlayOpen();
        isMoving = true;
    }

    public void PlayCenter()
    {
        animator.Play("StayCenterRight");
    }
}
