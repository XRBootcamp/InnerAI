using UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuInput : MonoBehaviour
{
    [SerializeField] private MenuManager _menuManager;
    
    public InputActionReference menuButton;

    private void Start()
    {
        menuButton.action.started += MenuButtonPressed;
    }

    private void MenuButtonPressed(InputAction.CallbackContext obj)
    {
        if (_menuManager.gameObject.activeSelf)
        {
            _menuManager.CloseMenu();
            _menuManager.gameObject.SetActive(false);
        }
        else
        {
            _menuManager.gameObject.SetActive(true);
        }
    }
}
