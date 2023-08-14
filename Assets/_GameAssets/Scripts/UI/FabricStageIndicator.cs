using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FabricStageIndicator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] spriteRenderers;

    public void SetPercent(float value)
    {
        float stage = 1f / spriteRenderers.Length;
        UpdateElements(value, stage);
    }

    private void UpdateElements(float value, float stage)
    { 
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            if (value >= stage)
            {
                LightElement(i);
                value -= stage;
            }
            else
            {
                ShadowElement(i);
            }
        }
    }

    private void LightElement(int i)
    {
        spriteRenderers[i].color = new Color(1f, 0.75f, 0f, 1f);
    }

    private void ShadowElement(int i)
    {
        spriteRenderers[i].color = new Color(0.3f, 0.3f, 0.3f, 1f);
    }
}
