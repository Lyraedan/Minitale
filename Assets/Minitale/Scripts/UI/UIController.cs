using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minitale.UI
{
    public class UIController : MonoBehaviour
    {

        public Camera menuCamera;

        public SceneField world;

        public GameObject titleMenu;
        public GameObject joinMenu;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ToggleTitleMenu(bool state)
        {
            if(state) menuCamera.gameObject.SetActive(true);
            titleMenu.SetActive(state);
        }

        public void ToggleJoinMenu(bool state)
        {
            joinMenu.SetActive(state);
        }

        public void JoinGame()
        {

        }

        public void Host()
        {
            ToggleJoinMenu(false);
            ToggleTitleMenu(false);
            LoadWorld();
        }

        public void LoadWorld()
        {
            menuCamera.gameObject.SetActive(false);
            SceneManager.LoadScene(world.SceneName);
        }
    }
}