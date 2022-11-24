/*
Copyright 2019 - 2021 Inetum
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
using umi3d.baseBrowser.Navigation;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Vector2 Movement => BaseFPSNavigation.Instance.Movement;

    private Animator anim;
    private IKControl IKControl;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null && transform.parent != null)
            anim = transform.parent.GetComponent<Animator>();
        IKControl = this.GetComponent<IKControl>();
    }

    private void Update()
    {
        Move();

        IKControl.feetIkActive = IKControl.overrideFeetIk || BaseFPSNavigation.Instance.IsCrouching;
    }

    private void Move()
    {
        if(Movement.x != 0 && !Input.GetKey(KeyCode.LeftShift)) Walk();
        else if (Movement.x != 0 && Input.GetKey(KeyCode.LeftShift)) Run();
        else if(Movement.x == 0) Idle();
    }

    private void Idle() => anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);

    private void Walk() => anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);

    private void Run() => anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
}
