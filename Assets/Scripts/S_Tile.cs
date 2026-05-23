using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class S_Tile : MonoBehaviour
{
    public int Value { get; set; } = 2;
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI valueText;

    private void Start()
    {
        Init(Value);
    }

    public void Init(int value)
    {
        this.Value = value;
        UpdateColor();
        UpdateText();
    }

    public void UpdateColor()
    {
        Color color = Value switch
        {
            2 => new Color(0.93f, 0.89f, 0.85f),
            4 => new Color(0.93f, 0.88f, 0.78f),
            8 => new Color(0.95f, 0.69f, 0.47f),
            16 => new Color(0.96f, 0.58f, 0.39f),
            32 => new Color(0.96f, 0.47f, 0.37f),
            64 => new Color(0.96f, 0.36f, 0.32f),
            128 => new Color(0.93f, 0.81f, 0.45f),
            256 => new Color(0.93f, 0.79f, 0.38f),
            512 => new Color(0.93f, 0.77f, 0.31f),
            1024 => new Color(0.93f, 0.75f, 0.24f),
            2048 => new Color(0.93f, 0.73f, 0.18f),
            _ => new Color(0.1f, 0.1f, 0.1f)  //Mayores a 2048
        };

        background.color = color;   
    }
    
    private void UpdateText()
    {
        if (valueText != null)
            valueText.text = Value.ToString();
    }
    
}