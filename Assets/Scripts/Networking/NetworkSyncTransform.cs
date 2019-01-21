using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkSyncTransform : NetworkBehaviour
{

    [SerializeField]
    private float _posLerpRate = 15;
    [SerializeField]
    private float _rotLerpRate = 15;
    [SerializeField]
    private float _posThreshold = 0.1f;
    [SerializeField]
    private float _rotThreshold = 1f;

    [SyncVar]
    private Vector3 _lastPosition;

    [SyncVar]
    private Vector3 _lastRotation;

    //[SyncVar]
    //private Vector3 _lastArmPosition;

    //[SyncVar]
    //private Vector3 _lastArmRotation;

    //GameObject arm;

    private void Start()
    {
        //arm = GameObject.FindGameObjectWithTag("Arm");
    }

    void Update()
    {
        if (isLocalPlayer)
            return;

        InterpolatePosition();
        InterpolateRotation();
    }

    void FixedUpdate()
    {
        if (!isLocalPlayer)
            return;

        var posChanged = IsPositionChanged();
        //var posArmChanged = IsArmPositionChanged();

        if (posChanged)
        {
            CmdSendPosition(transform.position);
            _lastPosition = transform.position;
        }
        //if (posArmChanged)
        //{
        //    CmdSendArmPosition(arm.transform.position);
        //    _lastArmPosition = arm.transform.position;
        //}

        var rotChanged = IsRotationChanged();
        //var rotArmChanged = IsArmRotationChanged();

        if (rotChanged)
        {
            CmdSendRotation(transform.localEulerAngles);
            _lastRotation = transform.localEulerAngles;
        }
        //if (rotArmChanged)
        //{
        //    CmdSendArmRotation(arm.transform.localEulerAngles);
        //    _lastArmRotation = arm.transform.localEulerAngles;
        //}
    }

    private void InterpolatePosition()
    {
        transform.position = Vector3.Lerp(transform.position, _lastPosition, Time.deltaTime * _posLerpRate);
        //arm.transform.position = Vector3.Lerp(transform.position + arm.transform.position, _lastArmPosition, Time.deltaTime * _posLerpRate);
    }

    private void InterpolateRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(_lastRotation), Time.deltaTime * _rotLerpRate);
        //arm.transform.rotation = Quaternion.Lerp(arm.transform.rotation, Quaternion.Euler(_lastArmRotation), Time.deltaTime * _rotLerpRate);
    }

    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSendPosition(Vector3 pos)
    {
        _lastPosition = pos;
    }

    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSendRotation(Vector3 rot)
    {
        _lastRotation = rot;
    }

    //[Command(channel = Channels.DefaultUnreliable)]
    //private void CmdSendArmPosition(Vector3 pos)
    //{
    //    _lastArmPosition = pos;
    //}

    //[Command(channel = Channels.DefaultUnreliable)]
    //private void CmdSendArmRotation(Vector3 rot)
    //{
    //    _lastArmRotation = rot;
    //}

    private bool IsPositionChanged()
    {
        return Vector3.Distance(transform.position, _lastPosition) > _posThreshold;
    }

    private bool IsRotationChanged()
    {
        return Vector3.Distance(transform.localEulerAngles, _lastRotation) > _rotThreshold;
    }

    //private bool IsArmPositionChanged()
    //{
    //    return Vector3.Distance(arm.transform.position, _lastArmPosition) > _posThreshold;
    //}

    //private bool IsArmRotationChanged()
    //{
    //    return Vector3.Distance(arm.transform.localEulerAngles, _lastArmRotation) > _rotThreshold;
    //}

    public override int GetNetworkChannel()
    {
        return Channels.DefaultUnreliable;
    }

    public override float GetNetworkSendInterval()
    {
        return 0.05f;
    }
}
