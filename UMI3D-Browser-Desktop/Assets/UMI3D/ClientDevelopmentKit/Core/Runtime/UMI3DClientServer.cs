﻿using System;
using umi3d.common;

namespace umi3d.cdk
{
    public class UMI3DClientServer : PersistentSingleton<UMI3DClientServer>
    {
        /// <summary>
        /// Environment connected to.
        /// </summary>
        protected MediaDto environment;

        /// <summary>
        /// Environment connected to.
        /// </summary>
        public static MediaDto Media
        {
            get {
                return Exists ? Instance.environment : null;
            }
            set {
                if (Exists)
                    Instance.environment = value;
            }
        }

        static public string getAuthorization()
        {
            if (Exists)
                return Instance._getAuthorization();
            return null;
        }
        protected virtual string _getAuthorization() { return null; }

        static public void SendData(AbstractBrowserRequestDto dto, bool reliable)
        {
            if (Exists)
                Instance._Send(dto, reliable);
        }

        protected virtual void _Send(AbstractBrowserRequestDto dto, bool reliable) { }

        static public void SendTracking(AbstractBrowserRequestDto dto)
        {
            if (Exists)
                Instance._SendTracking(dto);
        }
        protected virtual void _SendTracking(AbstractBrowserRequestDto dto) { }


        static public void GetFile(string url, Action<byte[]> callback, Action<string> onError)
        {
            if (Exists)
                Instance._GetFile(url, callback, onError);
        }

        protected virtual void _GetFile(string url, Action<byte[]> callback, Action<string> onError) { onError.Invoke("GetFile Not Implemented"); }

        static public void GetEntity(ulong id,Action<LoadEntityDto> callback, Action<string> onError)
        {
            if (Exists)
                Instance._GetEntity(id,callback, onError);
        }

        protected virtual void _GetEntity(ulong id,Action<LoadEntityDto> callback, Action<string> onError) { onError.Invoke("GetEntity Not Implemented"); }

        public virtual ulong GetId() { return 0; }

        /// <summary>
        /// return time server in millisecond, use synchronised time in collaborative cases.
        /// </summary>
        /// <returns></returns>
        public virtual ulong GetTime() { return (ulong)DateTime.Now.Millisecond; }

    }
}