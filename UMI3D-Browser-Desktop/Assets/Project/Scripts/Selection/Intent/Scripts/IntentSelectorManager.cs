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
using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
using UnityEngine;

namespace BrowserDesktop.Selection.Intent
{
    public class IntentSelectorManager : MonoBehaviour
    {
        bool initialized = false;

        [SerializeField]
        private IntentSelector intentSelector;

        // Start is called before the first frame update
        void Awake()
        {
            StartCoroutine(waitForEnvironment());
        }

        // Update is called once per frame
        void Update()
        {
            if (initialized)
            {
                intentSelector.Select();
            }
        }


        IEnumerator waitForEnvironment()
        {
            yield return new WaitUntil(() => { return UMI3DEnvironmentLoader.Exists; });
            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => {
                intentSelector.Activate(0); //0 does not have any sense here
                initialized = true;
            });
        }
    }
}
