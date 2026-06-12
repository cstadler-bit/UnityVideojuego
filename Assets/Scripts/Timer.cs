using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float tiempoRestante = 60f;
    public TMP_Text timerText;

    void Update()
    {
        tiempoRestante -= Time.deltaTime;

        timerText.text = Mathf.Ceil(tiempoRestante).ToString();
    }
    
}