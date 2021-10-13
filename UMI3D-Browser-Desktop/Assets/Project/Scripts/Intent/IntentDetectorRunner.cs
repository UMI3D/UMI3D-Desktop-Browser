using System.Collections;
using System.Collections.Generic;
using umi3d.cdk;
using umi3d.cdk.interaction;
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

        private InteractableContainer lastPredictedObject = null;

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
                InteractableContainer predictedTarget = detector.PredictTarget();
                if (predictedTarget != null && predictedTarget != lastPredictedObject)
                {
                    if (lastPredictedObject != null)
                        ResetVisualCue(lastPredictedObject);

                    TriggerVisualCue(predictedTarget);

                    lastPredictedObject = predictedTarget;
                }
            }

        }

        private void TriggerVisualCue(InteractableContainer predictedTarget)
        {
            var renderer = predictedTarget.gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                cachedShaders.Add(renderer, renderer.material.shader);
                renderer.material.shader = outlineShader;
            }
        }

        private void ResetVisualCue(InteractableContainer lastPredictedTarget)
        {
            var renderer = lastPredictedTarget.gameObject.GetComponentInChildren<Renderer>();
            if (renderer != null && renderer.material != null)
            {
                renderer.material.shader = cachedShaders[renderer];
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
