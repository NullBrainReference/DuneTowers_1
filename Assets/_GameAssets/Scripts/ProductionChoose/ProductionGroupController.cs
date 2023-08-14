using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionGroupController : MonoBehaviour
{
    public List<ProductionUnit> productionUnits;

    public void Select(ProductionUnit unit)
    {
        foreach (ProductionUnit productionUnit in productionUnits)
        {
            productionUnit.Switch(productionUnit == unit);
        }
    }

    public void InitGroup(ProductionType production)
    {
        ProductionUnit productionUnit = productionUnits[0];

        foreach (ProductionUnit unit in productionUnits)
        {
            if (unit.productionType == production)
            {
                productionUnit = unit;
                break;
            }
        }

        Select(productionUnit);
    }
}
