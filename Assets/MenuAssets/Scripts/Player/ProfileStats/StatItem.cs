using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProfileStat { Kill, Death, Win, Lose, Wave }

[System.Serializable]
public class StatItem
{
    public int id;
    public float value;
    public ProfileStat type;

    public void ValueIncrement() {
        value++; 
    }
}
