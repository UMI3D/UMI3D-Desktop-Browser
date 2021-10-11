using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using UnityEngine;

namespace BrowserDesktop.Intent
{
    public class IntentDetectorRunner : MonoBehaviour
    {
        bool initialized = false;

        /// <summary>
        /// Stores shaders while the predicted object receives a glowing effect
        /// </summary>
        private Dictionary<Renderer, Shader> cachedShaders = new Dictionary<Renderer, Shader>();

        /// <summary>
        /// Glowing effect shader
        /// </summary>
        [SerializeField]
        private Shader outlineShader;

        private UMI3DNodeInstance lastPredictedObject = null;

        public AbstractSelectionIntentDetector detector;

        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
            StartCoroutine(waitForEnvironment());
        }

        // Update is called once per frame
        void Update()
        {
            if (initialized)
            {
                UMI3DNodeInstance predictedTarget = detector.PredictTarget();
                if (predictedTarget != null && predictedTarget != lastPredictedObject)
                {
                    if (lastPredictedObject != null)
                        ResetVisualCue(lastPredictedObject);

                    TriggerVisualCue(predictedTarget);

                    lastPredictedObject = predictedTarget;
                }
            }

        }

        private void TriggerVisualCue(UMI3DNodeInstance predictedTarget)
        {
            foreach (var renderer in predictedTarget.renderers)
            {
                if (renderer.material != null)
                {
                    cachedShaders.Add(renderer, renderer.material.shader);
                    renderer.material.shader = outlineShader;
                }
            }
        }

        private void ResetVisualCue(UMI3DNodeInstance lastPredictedTarget)
        {
            foreach (var renderer in lastPredictedTarget.renderers)
            {
                if (renderer.material != null)
                {
                    renderer.material.shader = cachedShaders[renderer];
                }
            }
            cachedShaders.Clear();
        }

        IEnumerator waitForEnvironment()
        {
            yield return new WaitUntil(() => { return UMI3DEnvironmentLoader.Exists; });
            UMI3DEnvironmentLoader.Instance.onEnvironmentLoaded.AddListener(() => {
                detector.InitDetector();
                initialized = true;
            });
        }
    }
}
