using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Panda;

namespace Panda.Examples.Shooter
{
    public class HUD : MonoBehaviour
    {
        public Unit player;

        public Text hpText;
        public Text ammoText;
        public Text targetText;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (player != null)
                UpdateHUD();
        }

        float targetHP;
        float targetMax;
        private void UpdateHUD()
        {
            var hpStr = string.Format("HP:{0:0}/{1:0}", player.health, player.startHealth);
            hpText.text = hpStr;

            var ammoStr = string.Format("ammo:{0:0}/{1:0}", player.ammo, player.startAmmo);
            ammoText.text = ammoStr;

            if (player.lastHit != null)
            {
                targetHP = player.lastHit.health;
                targetMax = player.lastHit.startHealth;
            }else
            {
                targetHP = 0.0f;
            }

            if( Time.time - player.lastHitTime < 3.0f)
                targetText.text = string.Format("HP:{0:0}/{1:0}", targetHP , targetMax );
            else
                targetText.text = "";


            if ((Time.time - player.lastHitTime) > 3.0f)
                targetText.text = "";


        }
    }
}
