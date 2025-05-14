using UnityEngine;
using UnityEngine.SceneManagement;
using SVS;

public class ExitTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //FindObjectOfType<SVS.Visualizer>().CreateTown();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡Nivel completado! Reiniciando...");
            // Reinicia la escena actual
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    */
}

