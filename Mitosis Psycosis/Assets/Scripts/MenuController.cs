using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    public static bool menuShow;

    //Menus
    public GameObject levelMenu;
    public GameObject deathMenu;
    public static bool dead;

    void Awake() {
        instance = this;
        dead = false;
        Hide();
    }
    
    public void Show() {
        ShowLevelMenu();
        gameObject.SetActive(true);
        Time.timeScale = 0;
        menuShow = true;
    }

    public void Hide() {
        gameObject.SetActive(false);
        Time.timeScale = 1;
        menuShow = false;
    }

    public void LoadLevel(string scene){
        SceneManager.LoadScene(scene);
    }

    void SwitchMenu(GameObject menu){
        levelMenu.SetActive(false);
        deathMenu.SetActive(false);
        menu.SetActive(true);
    }

    public void ShowLevelMenu(){
        SwitchMenu(levelMenu);
    }

    public void ShowDeathMenu(){
        SwitchMenu(deathMenu);
        dead = true;
    }

    public void RestartScene(){
        MasterController.restartScene();
    }
}
