using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image imagenRadial; 
    public Health scriptVida;  

    void Update()
    {
        if (Camera.main != null)
            transform.forward = Camera.main.transform.forward;

        if (scriptVida != null)
            imagenRadial.fillAmount = scriptVida.CurrentHealth / scriptVida.MaxHealth;
    }
}
