using System.Collections.Generic;

namespace umi3dbrowser.module.lipsync
{
    internal class Viseme
    {
        public int index;

        /// <summary>
        /// Viseme code in Oculus convention.
        /// </summary>
        public string code;

        /// <summary>
        /// Name in the Oculus convention.
        /// </summary>
        public string OculusName => $"oculus_{code}";

        /// <summary>
        /// Didimo viseme code specific mapping. Keys are Oculus code and values are Didimo renaming.
        /// </summary>
        public static readonly Dictionary<string, string> didimoNamingExceptions = new Dictionary<string, string>()
        {
            {"ih", "I" },
            {"oh", "O" },
            {"ou", "U" },
        };

        /// <summary>
        /// Name in the Didimo convention.
        /// </summary>
        public string DimimoName
        {
            get
            {
                if (didimoNamingExceptions.ContainsKey(code))
                    return $"oculus_{didimoNamingExceptions[code]}";
                else
                    return $"oculus_{code}";
            }
        }

        /// <summary>
        /// Last recorded value of the viseme.
        /// </summary>
        public float value;

        public static List<Viseme> GetOVRVisemes()
        {
            var visemes = new List<Viseme>();
            int i = 0;
            foreach (object visemeName in System.Enum.GetValues(typeof(OVRLipSync.Viseme)))
            {
                var viseme = new Viseme()
                {
                    code = visemeName.ToString(),
                    value = 0f,
                    index = i
                };
                visemes.Add(viseme);
                i++;
            }
            return visemes;
        }
    }
}