using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class PrepareGamePanel : MonoBehaviour
{
    [FormerlySerializedAs("_colors")] [SerializeField]
    private GameObject[] countDownObjects;
    [SerializeField]
    private GameObject _go;
    [SerializeField]
    private Vector3 _defaultScale;
    [SerializeField]
    private Vector3 _bigScale;
    
    public async Task<bool> Prepare(float seconds, CancellationToken cancellationToken)
    {
        Reset();
        gameObject.SetActive(true);
        
        var elementsCount = countDownObjects.Length + 1;
        var unitTime = seconds / elementsCount;

        for (var i = 0; i < countDownObjects.Length; i++)
        {
            countDownObjects[i].gameObject.SetActive(true);
            if (i > 0)
            {
                countDownObjects[i - 1].transform.localScale = _defaultScale;
            }
            
            countDownObjects[i].transform.localScale = _bigScale;
            
            await Task.Delay(TimeSpan.FromSeconds(unitTime), cancellationToken);
            
            if (cancellationToken.IsCancellationRequested)
            {            
                gameObject.SetActive(false);
                return false;
            }
            countDownObjects[i].SetActive(false);
        }
        
        foreach (var countDownObject in countDownObjects)
        {
            countDownObject.gameObject.SetActive(false);
        }
        _go.SetActive(true);
        
        await Task.Delay(TimeSpan.FromSeconds(unitTime), cancellationToken);
        if (cancellationToken.IsCancellationRequested)
        {
            gameObject.SetActive(false);
            return false;
        }
        
        gameObject.SetActive(false);
        return true;
    }

    private void Reset()
    {
        foreach (var countDownObject in countDownObjects)
        {
           countDownObject.transform.localScale = _defaultScale;
           countDownObject.gameObject.SetActive(false);
        }
        _go.SetActive(false);
    }
}
