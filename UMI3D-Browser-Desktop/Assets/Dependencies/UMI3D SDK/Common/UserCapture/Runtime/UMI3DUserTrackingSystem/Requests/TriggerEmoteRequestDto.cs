//using System.Collections;
//using UnityEngine;

//namespace umi3d.common.userCapture
//{
//    /// <summary>
//    /// Request from a browser to trigger an emote for its user on user browsers.
//    /// </summary>
//    public class TriggerEmoteRequest : AbstractBrowserRequestDto
//    {
//        /// <summary>
//        /// UMI3D id of the emote to trigger.
//        /// </summary>
//        public ulong emoteToTriggerId;
//        /// <summary>
//        /// User id of the user planning to trigger an emote.
//        /// </summary>
//        public ulong sendingUserId;

//        /// <inheritdoc/>
//        protected override uint GetOperationId()
//        {
//            return UMI3DOperationKeys.TriggerEmoteRequest;
//        }

//        public override Bytable ToBytableArray(params object[] parameters)
//        {
//            return base.ToBytableArray(parameters)
//                + UMI3DNetworkingHelper.Write(emoteToTriggerId)
//                + UMI3DNetworkingHelper.Write(sendingUserId);

//        }
//    }
//}