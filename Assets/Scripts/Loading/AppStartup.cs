using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppInfo;
using Loading;

public class AppStartup : MonoBehaviour
{
    private void Start()
    {
        var appInfoContainer = new AppInfoContainer();
        var loadingOrepations = new Queue<ILoadingOperation>();
        loadingOrepations.Enqueue(new LoadingOperation(appInfoContainer));
        loadingOrepations.Enqueue(new ConfigOperation(appInfoContainer));
        loadingOrepations.Enqueue(new MenuLoadingOperation());
        LoadingScreen.Instance.Load(loadingOrepations);
    }
}
