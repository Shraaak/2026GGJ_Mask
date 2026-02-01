using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PaperUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    public int currentPaparCount = 0;

    void Start()
    {
        text.text = "X 0";
    }

    void Update()
    {
        text.text = "X " + currentPaparCount;
    }
}
