using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupAlphaController : MonoBehaviour
{
    [SerializeField] private Image[] images;
    [SerializeField] private float[] alphas;

    [SerializeField] private Text[] texts;

    [SerializeField] private float alpha;

    [SerializeField] private bool isChanging = false;

    private void Start()
    {
        images = this.gameObject.GetComponentsInChildren<Image>();
        texts = this.gameObject.GetComponentsInChildren<Text>();

        alphas = new float[images.Length];

        for (int i = 0; i < images.Length; i++)
        {
            alphas[i] = images[i].color.a;
        }
    }

    private void FixedUpdate()
    {
        if (isChanging == false) return;
        
        UpdateAlpha();
    }

    public void EnableChanging()
    {
        isChanging = true;
    }

    public void DisableChanging()
    {
        isChanging = false;
    }

    public void UpdateAlpha()
    {
        for (int i = 0; i < images.Length; i++)
        {
            images[i].color = new Color(images[i].color.r, images[i].color.g, images[i].color.b, alpha * alphas[i]);
        }
        foreach (Text text in texts)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
    }
}
