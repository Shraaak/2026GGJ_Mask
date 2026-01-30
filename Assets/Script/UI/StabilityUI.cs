using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StabilityUI : MonoBehaviour
{
    private Scrollbar scrollbar;

    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    void Update()
    {
        scrollbar.size = 
        PlayerMove.Instance.stability.currentStability/
        PlayerMove.Instance.stability.maxStability;
    }
}
