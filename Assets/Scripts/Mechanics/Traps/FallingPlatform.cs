using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mechanics;
using Packages.Rider.Editor;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class FallingPlatform : Movable
{
    /// <summary>
    /// if true the platform will fall until it founds an obstacle
    /// </summary>
    [SerializeField] private bool limitFallDistance = false;

    /// <summary>
    /// max distance that the platform will reach by falling
    /// </summary>
    [SerializeField] private float fallDistance = 5f;
    
    /// <summary>
    /// how many seconds the platform will wait before starting falling after that the fall is triggered
    /// </summary>
    [SerializeField] private float timeBeforeFalling = 0.35f;

    [SerializeField] private float fallingSpeed = 3f;
    [SerializeField] private float risingSpeed = 3f;
    [SerializeField] private float risingAcceleration = 3f;


    /// <summary>
    /// Time while platform is off after that it is fallen  
    /// </summary>
    [SerializeField] private float offTime = 5f;
    
    [SerializeField] private Animator animator;
    
    private bool _isDown = false;
    private static readonly int isOn = Animator.StringToHash("isOn");
    private const float MaxRayCastDistance = 40f;
    private const float FallingAcceleration = 9.81f;
    private Vector3 _startingPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        _startingPosition = Tr.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isDown && other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            _isDown = true;
            GetComponent<PhotonView>().RPC("StartFalling", RpcTarget.All);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!MoveFixedDistanceAccelerated && !_isDown && other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<PhotonView>().IsMine)
        {
            _isDown = true;
            GetComponent<PhotonView>().RPC("StartFalling", RpcTarget.All);
        }
    }

    [PunRPC]
    public void StartFalling()
    {
        StartCoroutine(WaitBeforeFalling());
    }

    private IEnumerator WaitBeforeFalling()
    {
        yield return new WaitForSeconds(timeBeforeFalling);
        Fall();
    }

    public void Fall()
    {
        var maxDistance = (limitFallDistance) ? fallDistance : MaxRayCastDistance;
        var layerMasks = new List<LayerMask>{LayerMask.GetMask("Obstacle"), LayerMask.GetMask("EnemyPhysic")};

        var hitDistances = new List<float> {maxDistance};

        foreach (var layerMask in layerMasks)
        {
            var platformPosition = Tr.position + 0.35f*Vector3.down;
            var leftHitInfo = Physics2D.Raycast(platformPosition + Vector3.left, Vector3.down, maxDistance, layerMask);
            var centerHitInfo = Physics2D.Raycast(platformPosition, Vector3.down, maxDistance, layerMask);
            var rightHitInfo = Physics2D.Raycast(platformPosition + Vector3.right, Vector3.down, maxDistance, layerMask);

            if (leftHitInfo.collider != null)
            {
                hitDistances.Add(leftHitInfo.distance);
            }
            if (centerHitInfo.collider != null)
            {
                hitDistances.Add(centerHitInfo.distance);
            }
            if (rightHitInfo.collider != null)
            {
                hitDistances.Add(rightHitInfo.distance);
            }
        }
        
        var smallerDistance = hitDistances.Min();
        if(!limitFallDistance)animator.SetBool(isOn, false);
        SetFixedDistanceAccelerated(Vector2.down, fallingSpeed, smallerDistance, FallingAcceleration);
        StartCoroutine(WaitBeforeRising());
    }

    private IEnumerator WaitBeforeRising()
    {
        yield return new WaitForSeconds(offTime);
        Rise();
    }

    private void Rise()
    {
        if(!limitFallDistance)animator.SetBool(isOn, true);
        SetFixedDistanceAccelerated(_startingPosition, risingSpeed, risingAcceleration);
        _isDown = false;
    }
}
