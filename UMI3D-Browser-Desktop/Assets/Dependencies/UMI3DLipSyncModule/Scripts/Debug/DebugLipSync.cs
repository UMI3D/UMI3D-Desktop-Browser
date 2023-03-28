using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace umi3dbrowser.module.lipsync
{
    internal class DebugLipSync : MonoBehaviour
    {
        public Canvas debugCanvas;
        private Text text;

        [SerializeField]
        public AbstractLipSyncController lipsController;

        private void Start()
        {
            debugCanvas = GetComponentInChildren<Canvas>();
            text = GetComponentInChildren<Text>();
        }

        private void Update()
        {
            if (lipsController == null)
                return;

            text.text = "";
            foreach (var viseme in lipsController.Visemes)
            {
                text.text += $"Code:  {viseme.code} \t|  Value: <color=#{ColorUtility.ToHtmlStringRGB(Color.Lerp(Color.red, Color.green, viseme.value))}>{viseme.value}</color> \t|";
                switch (lipsController)
                {
                    case LipSyncAnimationController animationC:
                        var animation = animationC.VisemesAnimation[viseme];
                        text.text += $" Weight: {animation.weight} \t| Enabled: <color={(animation.enabled ? "#00ff00" : "#ff0000")}>{animation.enabled}</color> \t| Time: {animation.normalizedTime} \t| Layer: {animation.layer}\n";
                        break;

                    case LipSyncAnimatorController animatorC:
                    case LipSyncBlendshapeController blendshapeC:
                        break;

                    default:
                        break;
                }
            }
            text.text += $"SUM:  {lipsController.Visemes.Sum(x => x.value)}";
        }
    }
}