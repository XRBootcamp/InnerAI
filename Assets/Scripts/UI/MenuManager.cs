using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> windows;
        
        private void Start()
        {
            CloseMenu();
            OpenWindow(0);
        }
        
        public void OpenWindow(int index)
        {
            //StartCoroutine(OpenWindowCoroutine(index));
            foreach (var win in windows)
            {
                win.SetActive(false);
            }
            
            windows[index].SetActive(true);
            
            
        }

        public void CloseMenu()
        {
            CloseAllWindows();
            gameObject.SetActive(false);
        }
        
        private void CloseAllWindows()
        {
            foreach (var win in windows)
            {
                win.SetActive(false);
            }
        }
    }
}
