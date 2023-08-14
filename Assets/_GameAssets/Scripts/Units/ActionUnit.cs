using System;

public class ActionUnit
{
    public UnitController parent;
    public Action action;


    public ActionUnit(UnitController parent, Action action)
    {
        this.parent = parent;
        this.action = action;
    }


    public bool Equals(ActionUnit other)
    {
        return other.parent = this.parent;
    }
    
}
