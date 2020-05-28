using Photon.Pun;
using UnityEngine;

namespace Mechanics.Traps
{
    public class PressurePlate : MonoBehaviour
    {
        [SerializeField] private StoneFace stoneFace;
        private int _playersInside = 0;
        [SerializeField] private GameObject activeBody;
        [SerializeField] private GameObject inactiveBody;

        private void OnTriggerEnter2D(Collider2D other)
        {
            _playersInside++;
            if(_playersInside > 1) return;
            activeBody.SetActive(true);
            inactiveBody.SetActive(false);
            if (PhotonNetwork.IsMasterClient) stoneFace.SwitchState(true);
        }
    
        private void OnTriggerExit2D(Collider2D other)
        {
            if(_playersInside > 0) _playersInside--;
            if(_playersInside > 0) return;
            activeBody.SetActive(false);
            inactiveBody.SetActive(true);
            if (PhotonNetwork.IsMasterClient) stoneFace.SwitchState(false);
        }
    }
}
