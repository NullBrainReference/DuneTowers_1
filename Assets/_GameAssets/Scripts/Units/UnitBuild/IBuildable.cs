using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuildable
{
    public SliderUnit BuildSlider { get; }
    public float BuildProgress { get; set; }
    public float BuildPoints { get; set; }
    public bool IsBuilding { get; set; }

    public void Build()
    {

    }
}
