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
using UnityEngine;

public class FpsNavigation : umi3d.baseBrowser.Navigation.BaseFPSNavigation
{
    #region Methods

    private void Update()
    {
        if (!isActive) return;

        float height = transform.position.y;

        if ((umi3d.baseBrowser.Controller.BaseCursor.Movement == umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.Free || umi3d.baseBrowser.Controller.BaseCursor.Movement == umi3d.baseBrowser.Controller.BaseCursor.CursorMovement.FreeHidden))
        {
            if (navigation != Navigation.Flying)
            {
                ComputeGravity(false, ref height);
                transform.Translate(0, height - transform.position.y, 0);
            }

            return;
        }

        if (TextInputDisplayerElement.isTyping) return;

        if (state == State.Default && Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView))) 
        { 
            state = State.FreeHead; 
        }
        else if (state == State.FreeHead && !Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.FreeView))) 
        {
            state = State.Default; 
            changeToDefault = true; 
        }

        HandleMovement();

        HandleView();
    }

    #region Movement

    /// <summary>
    /// Moves player.
    /// </summary>
    private void HandleMovement()
    {
        float height = transform.position.y;

        Vector2 move = Vector2.zero;

        if (navigateTo)
        {
            var delta = navigationDestination - transform.position;
            move = delta.normalized;

            if (Vector3.Distance(transform.position, navigationDestination) < .5f)
                navigateTo = false;
        }
        else
        {
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Forward))) { move.x += 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Backward))) { move.x -= 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Right))) { move.y += 1; }
            if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Left))) { move.y -= 1; }
            if (Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Forward))
                || Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Backward))
                || Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Right))
                || Input.GetKeyDown(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Left)))
            {
                PlayerMoved.Invoke();
            }
        }

        switch (navigation)
        {
            case Navigation.Walking:
                Walk(ref move, ref height);
                break;
            case Navigation.Flying:
                Fly(ref move, ref height);
                break;
        }

        move *= Time.deltaTime;

        Vector3 pos = transform.rotation * new Vector3(move.y, 0, move.x);

        if (CanMove(pos)) pos += transform.position;
        else pos = transform.position;

        pos.y = height;

        transform.position = pos;
    }

    /// <summary>
    /// Computes <paramref name="move"/> vector to perform a walk movement and applies gravitu.
    /// </summary>
    /// <param name="move"></param>
    private void Walk(ref Vector2 move, ref float height)
    {
        if (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat)))
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

        bool squatting = Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat));
        skeleton.transform.localPosition = new Vector3(0, Mathf.Lerp(skeleton.transform.localPosition.y, (squatting) ? data.squatHeight : data.standHeight, data.squatSpeed == 0 ? 1000000 : Time.deltaTime / data.squatSpeed), 0);

        ComputeGravity(Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Jump)), ref height);
    }

    private void Fly(ref Vector2 Move, ref float height)
    {
        Move.x *= data.flyingSpeed;
        Move.y *= data.flyingSpeed;
        height += data.flyingSpeed * 0.01f * ((Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Squat)) ? -1 : 0) + (Input.GetKey(InputLayoutManager.GetInputCode(InputLayoutManager.Input.Jump)) ? 1 : 0));
    }

    #endregion

    /// <summary>
    /// Rotates camera.
    /// </summary>
    private void HandleView()
    {
        if (state == State.FreeMousse) return;
        Vector3 angleView = viewpoint.rotation.eulerAngles.NormalizeAngle();

        Vector2 angularSpeed = new Vector2
            (
            -1 * Input.GetAxis("Mouse Y") * data.AngularViewSpeed.x, 
            Input.GetAxis("Mouse X") * data.AngularViewSpeed.y
            );
        Vector3 result = angleView + ((Vector3)angularSpeed).NormalizeAngle();
        if (changeToDefault)
        {
            result = lastAngleView;
            changeToDefault = false;
        }
        Vector3 displayResult;

        if (result.x < data.XAngleRange.x) result.x = data.XAngleRange.x;
        if (result.x > data.XAngleRange.y) result.x = data.XAngleRange.y;
        displayResult = result;
        if (displayResult.x < data.XDisplayAngleRange.x) displayResult.x = data.XDisplayAngleRange.x;
        if (displayResult.x > data.XDisplayAngleRange.y) displayResult.x = data.XDisplayAngleRange.y;

        if (state == State.Default)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, result.y, 0));
            lastAngleView = result;
        }
        else
        {
            Vector3 angleNeck = transform.rotation.eulerAngles.NormalizeAngle();
            float delta = Mathf.DeltaAngle(result.y, angleNeck.y);

            if (delta < data.YAngleRange.x) result.y = -data.YAngleRange.x + angleNeck.y;
            if (delta > data.YAngleRange.y) result.y = -data.YAngleRange.y + angleNeck.y;
        }
        viewpoint.transform.rotation = Quaternion.Euler(result);
        neckPivot.transform.rotation = Quaternion.Euler(new Vector3(Mathf.Clamp(result.x, -maxNeckAngle, maxNeckAngle), result.y, result.z));
        head.transform.rotation = Quaternion.Euler(displayResult);
    }

    #endregion
}
