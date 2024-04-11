using System.Collections;
using System.Collections.Generic;
using umi3d.baseBrowser.Navigation;
using UnityEngine;

public sealed class UMI3DCameraManager
{
    #region Dependencies

    public Transform playerTransform;
    public Transform viewpoint;
    public Transform neckPivot;
    public Transform head;

    public BaseFPSData data;

    #endregion
    
    public void HandleView()
    {
        //if (
        //        FPSNavigation.state == BaseFPSNavigation.State.Default
        //        && data.WantToLookAround
        //    )
        //{
        //    FPSNavigation.state = BaseFPSNavigation.State.FreeHead;
        //}
        //else if (
        //    FPSNavigation.state == BaseFPSNavigation.State.FreeHead
        //    && !data.WantToLookAround
        //)
        //{
        //    FPSNavigation.state = BaseFPSNavigation.State.Default;
        //    FPSNavigation.changeToDefault = true;
        //}


        Vector2 angularSpeed = data.cameraMovement * data.AngularViewSpeed;

        Vector3 viewpointAngle = viewpoint.rotation.eulerAngles.NormalizeAngle() + ((Vector3)angularSpeed).NormalizeAngle();
        viewpointAngle.x = Mathf.Clamp(viewpointAngle.x, -data.maxXCameraAngle, data.maxXCameraAngle);
        switch (data.cameraMode)
        {
            case E_CameraMode.Locked:
                break;
            case E_CameraMode.Navigation:
                playerTransform.rotation = Quaternion.Euler(0f, viewpointAngle.y, 0f);
                break;
            case E_CameraMode.NeckMovement:
                viewpointAngle.y = Mathf.Clamp(viewpointAngle.y, -data.maxYCameraAngle, data.maxYCameraAngle);
                viewpointAngle.y = Mathf.Clamp(viewpointAngle.y, -data.maxYCameraAngle, data.maxYCameraAngle);
                break;
            default:
                break;
        }
        
        Vector3 headAngle = viewpointAngle;
        headAngle.x = Mathf.Clamp(headAngle.x, data.maxXHeadAngle.x, data.maxXHeadAngle.y);

        viewpoint.transform.rotation = Quaternion.Euler(viewpointAngle);
        head.transform.rotation = Quaternion.Euler(headAngle);
        neckPivot.transform.rotation = Quaternion.Euler(
            Mathf.Clamp(viewpointAngle.x, -data.maxNeckAngle, data.maxNeckAngle),
            viewpointAngle.y,
            viewpointAngle.z
        );
    }

    /// <summary>
    /// Common part of the HandleView method.
    /// </summary>
    /// <param name="angleView"></param>
    /// <param name="angularSpeed"></param>
    public void BaseHandleView(Vector3 angleView, Vector2 angularSpeed)
    {
        //Vector3 result = angleView + ((Vector3)angularSpeed).NormalizeAngle();
        //if (changeToDefault)
        //{
        //    result = lastAngleView;
        //    changeToDefault = false;
        //}

        //if (state == State.Default)
        //{
            
        //    lastAngleView = result;
        //}
        //else
        //{
        //    Vector3 angleNeck = transform.rotation.eulerAngles.NormalizeAngle();
        //    float delta = Mathf.DeltaAngle(result.y, angleNeck.y);

        //    if (delta < data.maxYCameraAngle.x) result.y = -data.maxYCameraAngle.x + angleNeck.y;
        //    if (delta > data.maxYCameraAngle.y) result.y = -data.maxYCameraAngle.y + angleNeck.y;
        //}
        
    }
}
