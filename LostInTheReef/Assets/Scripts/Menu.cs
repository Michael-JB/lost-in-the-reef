using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    private void Start() {
        AudioManager.instance.Play("Main");
        Cursor.visible = true;
    }

    public void PlayGame() {
        SceneManager.LoadScene("SmallMaze");
    }

    public void QuitGame() {
        Application.Quit();
    }

}
