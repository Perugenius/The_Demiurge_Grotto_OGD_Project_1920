using System.Collections;
using System.Collections.Generic;
using Core;
using Mechanics;
using Mechanics.Players;
using Photon.Pun;
using UnityEngine;

public class Fan : MonoBehaviour
{
    [SerializeField] private float onTime = 5f;
    [SerializeField] private float offTime = 3f;
    [SerializeField] private bool startEnabled;
    [SerializeField] private bool alwaysOn;
    [SerializeField] private float intensity = 5f;
    [SerializeField] private float particleFrequency = 3f;
    [SerializeField] private GameObject particle;
    [SerializeField] private Animator animator;

    private GameObject _body;

    private int _particleCount;
    private Vector2 _direction;
    private bool _enabled;
    private int _count;
    private PhotonView _photonView;
    private static readonly int isOn = Animator.StringToHash("isOn");
    private GameObject _player1;
    private GameObject _player2;
    private Transform _tr;
    private PhotonView _player1PhotonView;
    private PhotonView _player2PhotonView;
    private Transform _player1Transform;
    private Transform _player2Transform;
    private bool _ready;


    // Start is called before the first frame update
    void Start()
    {
        _tr = GetComponent<Transform>();
        _body = transform.Find("Body").gameObject;

        _photonView = GetComponent<PhotonView>();
        
        switch (transform.rotation.eulerAngles.z)
        {
            case 0: _direction = Vector2.up; break;
            case 90: _direction = Vector2.left; break;
            case 180: _direction = Vector2.down; break;
            case 270: _direction = Vector2.right; break;
            case -90: _direction = Vector2.right; break;
            case -180: _direction = Vector2.down; break;
            case -270: _direction = Vector2.left; break;
            default: _direction = Vector2.up; break;
        }

        _enabled = startEnabled || alwaysOn;

        animator.SetBool(isOn,_enabled);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        _player1 = players[0];
        _player2 = players[1];
        _player1Transform = _player1.transform;
        _player1Transform = _player2.transform;
        _player1PhotonView = _player1.GetComponent<PhotonView>();
        _player2PhotonView = _player2.GetComponent<PhotonView>();
        
        //play sound
        if(_enabled  && (_player1PhotonView.IsMine && Vector3.Distance(_player1Transform.position, _tr.position) < 15) || 
           (Vector3.Distance(_player2Transform.position,_tr.position)<15 && _player2PhotonView.IsMine)) AudioManager.Instance.PlaySound("FanSFX");

        _ready = true;
    }

    [PunRPC]
    public void EnableFan(bool enable)
    {
        _enabled = enable;
        if (_ready)
        {
            //play/stop sound
            if(enable && (_player1PhotonView.IsMine && Vector3.Distance(_player1Transform.position, _tr.position) < 15) || 
               (Vector3.Distance(_player2Transform.position,_tr.position)<15 && _player2PhotonView.IsMine)) AudioManager.Instance.PlaySound("FanSFX");
            else AudioManager.Instance.StopSound("FanSFX");
        }
        animator.SetBool(isOn,enable);
    }

    private void OnTriggerStay2D(Collider2D other)
        {
            if(!_enabled) return;
            Movable player = other.gameObject.GetComponent<Movable>();
            if (player != null)
            {
                if (other.gameObject.CompareTag("Player"))
                {
                    //get player photonView 
                    PhotonView photonView = other.gameObject.GetComponent<PhotonView>();
                    photonView = photonView == null ? other.transform.parent.gameObject.GetComponent<PhotonView>() : photonView;
                
                    if(!photonView.IsMine) return;
                }

                Vector2 resistanceForceDirection = _direction;
                float resistanceForceIntensity = intensity;
                player.AddForce(resistanceForceDirection, resistanceForceIntensity);
            }
        }

        private void CreateParticle(Vector3 position)
        {
            GameObject instantiatedParticle = Instantiate(particle, position, Quaternion.identity);
            Rigidbody2D particleRigidbody = instantiatedParticle.GetComponent<Rigidbody2D>();
            particleRigidbody.mass = 3f/2f*intensity;
            particleRigidbody.gravityScale =
                (_direction.x > 0.1 || _direction.x < 0.1) ? 0 : particleRigidbody.gravityScale;
            instantiatedParticle.GetComponent<AirParticle>().Intensity = 14 / 2.5f * intensity;
            StartCoroutine(WaitBeforeDeletingParticle(instantiatedParticle));
        }

        private IEnumerator WaitBeforeDeletingParticle(GameObject instantiatedParticle)
        {
            yield return new WaitForSeconds(1f);
            Destroy(instantiatedParticle);
        }

        /*private void OnTriggerEnter2D(Collider2D other)
        {
            if(!enabled) return;
            GameObject player = other.gameObject;
            if (player != null && player.GetComponent<Rigidbody2D>()!=null)
                player.GetComponent<Rigidbody2D>().gravityScale = player.GetComponent<Rigidbody2D>().gravityScale / gravityReduceFactor;
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if(!enabled) return;
            GameObject player = other.gameObject;
            if (player != null && player.GetComponent<Rigidbody2D>()!=null)
                player.GetComponent<Rigidbody2D>().gravityScale = player.GetComponent<Rigidbody2D>().gravityScale * gravityReduceFactor;
        }*/
    
        // Update is called once per frame
    void FixedUpdate()
    {
        if(_enabled)_particleCount++;
        _count++;
        if (_enabled && _particleCount >= Mathf.Floor(60f / particleFrequency))
        {
            _particleCount = 0;
            CreateParticle(transform.position + (Vector3)_direction*-0.5f + Random.Range(-1f,1f)*0.25f*Vector3.right);
        }

        if(alwaysOn) return;

        if (_enabled && _count >= onTime * 60)
        {
            _count = 0;
            if(PhotonNetwork.IsMasterClient) _photonView.RPC("EnableFan", RpcTarget.All,false);
        }
        
        if (!_enabled && _count >= offTime * 60)
        {
            _count = 0;
            if(PhotonNetwork.IsMasterClient) _photonView.RPC("EnableFan", RpcTarget.All,true);
        }
    }
    
}
