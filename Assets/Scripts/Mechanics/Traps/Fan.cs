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
    private GameObject _player;
    private Transform _tr;


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
        
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            if (player.GetPhotonView().IsMine) _player = player;
        }

        _enabled = startEnabled || alwaysOn;

        animator.SetBool(isOn,_enabled);
    }

    [PunRPC]
    public void EnableFan(bool enable)
    {
        _enabled = enable;
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
        if(Vector3.Distance(_player.transform.position,_tr.position) > 50f) return;
        
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
