using UnityEngine;
using UnityEngine.SceneManagement;

public class AddScene : MonoBehaviour
{
    public void btn_add_scene(string scene_name)
    {
        SceneManager.LoadScene(scene_name,LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("menu");
    }
}
