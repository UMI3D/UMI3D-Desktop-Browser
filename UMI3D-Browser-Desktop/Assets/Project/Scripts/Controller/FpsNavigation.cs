/*
Copyright 2019 Gfi Informatique

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using BrowserDesktop.Controller;
using BrowserDesktop.Menu;
using umi3d.baseBrowser.connection;
using UnityEngine;

public class FpsNavigation : umi3d.baseBrowser.Navigation.BaseFPSNavigation
{
    #region Methods

    protected override bool OnUpdate()
    {
        if (!base.OnUpdate()) return false;

        if (state == State.Default && Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView))) state = State.FreeHead;
        else if (state == State.FreeHead && !Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView)))
        {
            state = State.Default;
            changeToDefault = true;
        }

        HandleMovement();
        HandleView();

        return true;
    }

    #region Movement

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void UpdateMovement()
    {
        if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Forward))) Movement.x += 1;
        if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Backward))) Movement.x -= 1;
        if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Right))) Movement.y += 1;
        if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Left))) Movement.y -= 1;
        if (Movement != Vector2.zero) PlayerMoved.Invoke();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="move"></param>
    /// <param name="height"></param>
    protected override void Walk(ref Vector2 move, ref float height)
    {
        IsCrouching = Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat)) || (IsCrouching && !CanJump());

        if (IsCrouching)
        {
            move.x *= (move.x > 0) ? data.forwardSpeed.y : data.backwardSpeed.y;
            move.y *= data.lateralSpeed.y;
        }
        else if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Sprint)))
        {
            move.x *= (move.x > 0) ? data.forwardSpeed.z : data.backwardSpeed.z;
            move.y *= data.lateralSpeed.z;
        }
        else
        {
            move.x *= (move.x > 0) ? data.forwardSpeed.x : data.backwardSpeed.x;
            move.y *= data.lateralSpeed.x;
        }

        skeleton.transform.localPosition = new Vector3(0, Mathf.Lerp(skeleton.transform.localPosition.y, (IsCrouching) ? data.squatHeight : data.standHeight, data.squatSpeed == 0 ? 1000000 : Time.deltaTime / data.squatSpeed), 0);

        ComputeGravity(Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Jump)), ref height);
    }

    #endregion

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void HandleView()
    {
        if (state == State.FreeMousse) return;
        Vector3 angleView = viewpoint.rotation.eulerAngles.NormalizeAngle();

        Vector2 angularSpeed = new Vector2
        (
            -1 * Input.GetAxis("Mouse Y") * data.AngularViewSpeed.x, 
            Input.GetAxis("Mouse X") * data.AngularViewSpeed.y
        );

        BaseHandleView(angleView, angularSpeed);
    }

    #endregion
}
