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

    void Awake() {
        instance = this;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwitchMenu(GameObject menu){
        levelMenu.SetActive(false);
        menu.SetActive(true);
    }

    public void ShowLevelMenu(){
        SwitchMenu(levelMenu);
    }

    public void RestartScene(){
        MasterController.restartScene();
    }
}
