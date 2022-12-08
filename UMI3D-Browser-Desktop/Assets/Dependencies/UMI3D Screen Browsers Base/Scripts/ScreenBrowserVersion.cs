/*
Copyright 2019 - 2023 Inetum

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

namespace umi3d.baseBrowser
{
    static public class ScreenBrowserVersion
    {
        /// <summary>
        /// Return screen browser version : Major.Minor.BuildCount.Date
        /// </summary>
        public static string Version { get { return major + "." + minor + "." + buildCount + "." + date; } }
        public readonly static string major = "1";
        public readonly static string minor = "0";
        /// <summary>
        /// Build count, to be increament by 1.
        /// </summary>
        public readonly static string buildCount = "2";
        /// <summary>
        /// Year-Month-Day.
        /// </summary>
        public readonly static string date = "221207";
    }
}

