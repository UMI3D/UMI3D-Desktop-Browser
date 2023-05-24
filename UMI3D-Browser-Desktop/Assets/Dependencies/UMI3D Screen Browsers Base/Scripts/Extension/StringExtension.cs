/*
Copyright 2019 - 2022 Inetum

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
namespace umi3d.baseBrowser.extension
{
    public static class StringExtension
    {
        public static string CapitalizeAndRemoveSpace(this string s)
        {
            var words = s.Split(" ");
            var valueFormated = "";
            foreach (var word in words)
            {
                if (word.Length == 0) continue;
                valueFormated += char.ToUpper(word[0]) + word.Substring(1);
            }
            return valueFormated;
        }
    }
}