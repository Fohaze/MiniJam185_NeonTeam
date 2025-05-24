using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneButton : MonoBehaviour
{
    [Tooltip("Nom de la scène à charger via l'Inspector")]
    public string sceneName;

    public void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("LoadSceneButton: 'sceneName' est vide ou null.");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
