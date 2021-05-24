using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using umi3d.common.volume;
using UnityEngine.Events;

namespace umi3d.cdk.volumes
{
	/// <summary>
	/// Detect volume entrance and exit.
	/// </summary>
	public class VolumeTracker : MonoBehaviour
	{
		public List<VolumeCell> volumesToTrack = new List<VolumeCell>();
		public float detectionFrameRate = 30;

		private Coroutine trackingRoutine = null;

		protected virtual void Awake()
        {
			trackingRoutine = StartCoroutine(Track());
        }

		IEnumerator Track()
        {
            while (true)
            {
				yield return new WaitForSeconds(60f / detectionFrameRate);

				foreach(VolumeCell cell in volumesToTrack)
                {

                }
            }
        }

		public void SubscribeToVolumeEntrance(string volumeId, UnityAction callback)
		{
			throw new System.NotImplementedException(); //todo
		}

		public void SubscribeToVolumeExit(string volumeId, UnityAction callback)
		{
			throw new System.NotImplementedException(); //todo
		}

		public void UnsubscribeToVolumeEntrance(string volumeId, UnityAction callback)
		{
			throw new System.NotImplementedException(); //todo
		}

		public void UnsubscribeToVolumeExit(string volumeId, UnityAction callback)
		{
			throw new System.NotImplementedException(); //todo
		}



	}
}