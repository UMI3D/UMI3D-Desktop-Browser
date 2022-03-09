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
using UnityEngine;
using UnityEngine.UI;

namespace umi3d.cdk.interaction.selection.intent
{
    /// <summary>
    /// Unity object that should be attached on a UMI3D controller to enable selection intent prediction
    /// </summary>
    public class IntentSelectorManager : MonoBehaviour
    {
        private bool initialized = false;

        [SerializeField]
        public IntentSelector intentSelector;

        // Start is called before the first frame update
        private void Awake()
        {
            StartCoroutine(waitForEnvironment());
        }



        // Update is called once per frame
        private void Update()
        {
            if (initialized)
            {
                if (!(methodTextContainer?.text is null) && methodTextContainer?.text != currentMethod)
                {
                    currentMethod = methodTextContainer?.text;
                    intentSelector.Deactivate(0);
                    intentSelector.detector = detectors[int.Parse(currentMethod)];
                    intentSelector.Activate(0);
                }
                intentSelector.Select();
            }
        }

        public IEnumerator waitForEnvironment()
        {
            yield return new WaitUntil(() => { return UMI3DEnvironmentLoader.Exists; });
            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() =>
            {
                intentSelector.Activate(0); //0 does not have any sense here
                initialized = true;

                methodSupervisor = GameObject.Find("Method Indicator"); // SCENE SPECIFIC
                methodTextContainer = methodSupervisor.GetComponentInChildren<Text>();
            });
        }

        private GameObject methodSupervisor;
        private Text methodTextContainer;
        private string currentMethod = "0";

        public List<AbstractSelectionIntentDetector> detectors;


    }
}