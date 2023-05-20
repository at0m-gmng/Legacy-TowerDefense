using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Playables;

namespace GameResult
{
    [RequireComponent(typeof(PlayableDirector))]
    public class GameResultIntroAnimation : MonoBehaviour
    {
        [SerializeField]
        private List<GameResultSetting> _settings;

        public void Play(GameResultType result)
        {
            foreach (var s in _settings)
            {
                s.Object.SetActive(s.Type == result);
            }
        }
        
        [Serializable]
        private class GameResultSetting
        {
            public GameResultType Type;
            public GameObject Object;
        }
    }
}
