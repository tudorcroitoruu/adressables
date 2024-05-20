using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject trailsMenu;
    [SerializeField] private LoadingScreen _loadingScreen;

    
    private void Awake()
    {
        if (!FindObjectOfType<LoadingScreen>())
        {
            LoadingScreen temp = Instantiate(_loadingScreen);
            DontDestroyOnLoad(temp.gameObject);
        }
    }

    public void GoToTrails()
    {
        trailsMenu.SetActive(true);
    }
}
