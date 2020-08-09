using System;
using Core;
using Core.SaveLoadData;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PlayerCollectiblesUI : MonoBehaviour
    {
        [SerializeField] private GameObject gems;
        [SerializeField] private GameObject lettersTeammate;
        [SerializeField] private GameObject lettersEldaan;

        private Text _gemsText;
        private Text _lettersTeammateText;
        private Text _lettersEldaanText;

        private void Awake()
        {
            if(EventsCollector.Instance.GetEvent("saveData")==null) EventsCollector.Instance.CreateEvent("saveData",UpdateCollectiblesLabels);
        }

        // Start is called before the first frame update
        void Start()
        {
            _gemsText = gems.GetComponent<Text>();
            _lettersEldaanText = lettersEldaan.GetComponent<Text>();
            _lettersTeammateText = lettersTeammate.GetComponent<Text>();
        
            PlayerData playerData = SaveSystem.LoadPlayerData();

            _gemsText.text = playerData.gems.ToString();
            _lettersEldaanText.text = playerData.eldaanLetters.ToString();
            _lettersTeammateText.text = playerData.teammateLetters.ToString();
        }

        public void UpdateCollectiblesLabels()
        {
            //Debug.Log("Event triggered");

            PlayerData playerData = SaveSystem.LoadPlayerData();

            _gemsText.text = playerData.gems.ToString();
            _lettersEldaanText.text = playerData.eldaanLetters.ToString();
            _lettersTeammateText.text = playerData.teammateLetters.ToString();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
