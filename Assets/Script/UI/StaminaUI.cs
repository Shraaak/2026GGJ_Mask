using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    private Scrollbar scrollbar;
    
    void Start()
    {
        scrollbar = GetComponent<Scrollbar>();
    }

    void Update()
    {
        scrollbar.size = 
        PlayerMove.Instance.stamina.currentStamina/
        PlayerMove.Instance.stamina.maxStamina;
    }
}
