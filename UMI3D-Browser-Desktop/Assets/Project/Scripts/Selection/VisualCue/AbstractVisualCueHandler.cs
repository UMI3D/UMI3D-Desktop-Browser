using System.Collections;
using System.Collections.Generic;
using umi3d.cdk.interaction;
using umi3d.common;
using UnityEngine;

namespace BrowserDesktop.Selection
{
    public abstract class AbstractVisualCueHandler : MonoBehaviour
    {
        public abstract void ActivateSelectedVisualCue(InteractableContainer interactable);

        public abstract void DeactivateSelectedVisualCue(InteractableContainer interactable);
    }
}
