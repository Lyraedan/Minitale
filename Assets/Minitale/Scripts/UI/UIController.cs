using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Minitale.UI
{
    public class UIController : MonoBehaviour
    {

        public SceneField world;
        public GameObject titleMenu;

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
            titleMenu.SetActive(state);
        }

        public void LoadSingleplayer()
        {
            SceneManager.LoadScene(world.SceneName);
        }
    }
}