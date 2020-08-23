using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public void btn_change_scene(string scene_name, string ui_scene)
    {
        SceneManager.LoadScene(scene_name, LoadSceneMode.Additive);
    }
}
