using System.Collections.Generic;
using Mechanics.Players;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        public List<GameObject> hearths;
        public Sprite fullHearth;
        public Sprite emptyHearth;

        private int _maxHealth;
        private int _currentHealth;
        private PlayableCharacter _character;
        
        
        // Start is called before the first frame update
        void Start()
        {
            _maxHealth = (int)_character.CurrentHealth1;
            _currentHealth = _maxHealth;
            for (int i = 0; i < hearths.Count; i++)
            {
                hearths[i].GetComponent<SpriteRenderer>().sprite = fullHearth;
                if (i >= _maxHealth)
                {
                    hearths[i].SetActive(false);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void LoseHearth(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                if(_currentHealth <=0) break;
                hearths[_currentHealth - 1].GetComponent<SpriteRenderer>().sprite = emptyHearth;
                _currentHealth--;
            }
        }

        public void RefillHearth(int num = 1)
        {
            for (int i = 0; i < num; i++)
            {
                if(_currentHealth > _maxHealth) break;
                hearths[_currentHealth - 1].GetComponent<SpriteRenderer>().sprite = fullHearth;
                _currentHealth++;
            }
        }

        public PlayableCharacter Character
        {
            get => _character;
            set => _character = value;
        }
    }
}
