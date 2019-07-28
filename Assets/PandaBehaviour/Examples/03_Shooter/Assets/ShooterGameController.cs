using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using Panda;

namespace Panda.Examples.Shooter
{
    public class ShooterGameController : MonoBehaviour
    {
        public Text displayText;

        public Unit player = null;
        List<Unit> enemies = new List<Unit>();

        static ShooterGameController _instance = null;

        static public ShooterGameController instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = FindObjectOfType<ShooterGameController>();
                }
                return _instance;
            }
        }


        public void OnUnitDestroy( Unit unit )
        {
            if (enemies.Contains(unit))
                enemies.Remove(unit);
        }

        [Task]
        bool IsPlayerDead()
        {
            return player == null;
        }

        [Task]
        bool IsLevelCompleted()
        {
            return enemies.Count == 0;
        }

        [Task]
        bool ReloadLevel()
        {
#if UNITY_5_2 
            Application.LoadLevel(Application.loadedLevel);
#else
            UnityEngine.SceneManagement.SceneManager.LoadScene( UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
#endif
            return true;
        }


        [Task]
        bool Display(string text)
        {
            if( displayText != null)
            {
                displayText.text = text;
                displayText.enabled = text != "";
            }
            return true;
        }

        // Use this for initialization
        void Start()
        {
            enemies.AddRange(FindObjectsOfType<Unit>());
            enemies.RemoveAll( (u) => u.team == player.team) ;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
