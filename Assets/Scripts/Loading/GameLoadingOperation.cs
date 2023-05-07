using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Common;
using Loading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameLoadingOperation : ILoadingOperation
{
    public string Description => "Game loading...";
    
    public async Task Load(Action<float> onProgress)
    {
        onProgress?.Invoke(0.5f);
        var loadOp = SceneManager.LoadSceneAsync(Constants.Scenes.QUICK_GAME, LoadSceneMode.Single);

        while (loadOp.isDone == false)
        {
            await Task.Delay(1);
        }
        onProgress?.Invoke(1f);
    }
}
