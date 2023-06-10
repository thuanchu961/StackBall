using UnityEngine;

namespace CBGames
{
    public class ServicesManager : MonoBehaviour
    {
        public static ServicesManager Instance { private set; get; }

        [SerializeField] private SoundManager soundManager = null;
        [SerializeField] private AdManager adManager = null;

        public SoundManager SoundManager { get { return soundManager; } }
        public AdManager AdManager { get { return adManager; } }

        void Awake()
        {
            if (Instance)
            {
                DestroyImmediate(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}

