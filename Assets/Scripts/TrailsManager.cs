using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

public class TrailsManager : MonoBehaviour
{
    public List<string> sceneKeys = new List<string>();

    private void OnEnable()
    {
        for (int i = 0; i < sceneKeys.Count; i++)
        {
            CheckAndLoadScene(sceneKeys[i]);
        }
    }

    public void CloseTrailsManager()
    {
        gameObject.SetActive(false);
    }

    private void CheckAndLoadScene(string sceneKey)
    {
        LoadingScreen.LoadingOn?.Invoke();
        // Load the addressable locations based on a key
        Addressables.LoadResourceLocationsAsync(sceneKey).Completed += OnLocationsLoaded;
    }

    private void OnLocationsLoaded(AsyncOperationHandle<IList<IResourceLocation>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            LoadingScreen.progression = 0f;
            LoadingScreen.mbDownloaded = 0f;
            LoadingScreen.mbToDownload = 0f;
            var locations = handle.Result;
            if (locations.Count > 0)
            {
                string sceneAddress = locations[0].PrimaryKey;

                // Check if the scene is cached
                Addressables.GetDownloadSizeAsync(sceneAddress).Completed += sizeHandle =>
                {
                    if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        long downloadSize = sizeHandle.Result;
                        LoadingScreen.mbToDownload = downloadSize / (1024f * 1024f); // convert bytes to megabytes
                        if (downloadSize > 0)
                        {
                            // The scene is not cached, download it
                            var downloadHandle = Addressables.DownloadDependenciesAsync(sceneAddress);
                            downloadHandle.Completed += downloadCompletedHandle =>
                            {
                                if (downloadCompletedHandle.Status == AsyncOperationStatus.Succeeded)
                                {
                                    // Scene downloaded, now load it
                                    LoadingScreen.LoadingOff?.Invoke();
                                    Debug.Log($"<color=green>Scene {sceneAddress} downloaded </color>");
                                }
                                else
                                {
                                    LoadingScreen.LoadingOff?.Invoke();
                                    CloseTrailsManager();
                                    Debug.LogError($"<color=red>Failed to download the scene {sceneAddress}.</color>");
                                }
                            };

                            // Update progress while downloading
                            StartCoroutine(TrackDownloadProgress(downloadHandle));
                        }
                        else
                        {
                            LoadingScreen.LoadingOff?.Invoke();
                            // The scene is already cached, load it
                            Debug.Log($"<color=green>Scene {sceneAddress} already downloaded </color>");
                        }
                    }
                    else
                    {
                        LoadingScreen.LoadingOff?.Invoke();
                        CloseTrailsManager();
                        Debug.LogError("Failed to get download size.");
                    }
                };
            }
            else
            {
                LoadingScreen.LoadingOff?.Invoke();
                CloseTrailsManager();
                Debug.LogError("No locations found for the given key.");
            }
        }
        else
        {
            LoadingScreen.LoadingOff?.Invoke();
            CloseTrailsManager();
            Debug.LogError("Failed to load resource locations.");
        }
    }

    private System.Collections.IEnumerator TrackDownloadProgress(AsyncOperationHandle downloadHandle)
    {
        while (!downloadHandle.IsDone)
        {
            LoadingScreen.progression = downloadHandle.PercentComplete;
            LoadingScreen.mbDownloaded = LoadingScreen.mbToDownload * downloadHandle.PercentComplete;
            yield return null;
        }
        LoadingScreen.progression = 1f;
        LoadingScreen.mbDownloaded = LoadingScreen.mbToDownload;
    }

    public void LoadScene(string sceneAddress)
    {
        Addressables.LoadSceneAsync(sceneAddress);
    }
}
