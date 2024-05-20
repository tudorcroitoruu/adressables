using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject loadingObject;
    public static float progression;
    public static float mbDownloaded;
    public static float mbToDownload;


    [Header("Remote download")] 
    [SerializeField] private Image progressValue;
    [SerializeField] private TMP_Text mbToDownloadText;
    [SerializeField] private TMP_Text mbDownloadedText;
    
    
    public static UnityEvent LoadingOn = new UnityEvent();
    public static UnityEvent LoadingOff = new UnityEvent();
    private void Awake()
    {
        LoadingOn.RemoveAllListeners();
        LoadingOff.RemoveAllListeners();

        LoadingOn.AddListener(LoadingOnFunc);
        LoadingOff.AddListener(LoadingOffFunc);
        
        LoadingOffFunc();
    }


    private void LoadingOnFunc()
    {
        loadingObject.SetActive(true);
    }

    private void LoadingOffFunc()
    {
        loadingObject.SetActive(false);
    }


    private void Update()
    {
        if (progressValue)
        {
            progressValue.fillAmount = progression;
        }

        if (mbToDownloadText && mbDownloadedText)
        {
            mbDownloadedText.text = mbDownloaded.ToString("N0") + "mb";
            mbToDownloadText.text ="/"+ mbToDownload.ToString("N0") + "mb";
        }

    }


    private void SetProgressValue()
    {
        
    }
}
