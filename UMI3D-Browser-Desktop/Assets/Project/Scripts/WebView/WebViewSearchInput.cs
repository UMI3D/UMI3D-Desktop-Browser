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

using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace BrowserDesktop
{
    public class WebViewSearchInput : MonoBehaviour
    {
        [SerializeField]
        private WebView webView = null;

        [SerializeField]
        private InputField url = null;

        /// <summary>
        /// Regex to validate an url
        /// </summary>
        Regex validateDateRegex;

        private void Start()
        {
            validateDateRegex = new Regex("^https?:\\/\\/(?:www\\.)?[-a-zA-Z0-9@:%._\\+~#=]{1,256}\\.[a-zA-Z0-9()]{1,6}\\b(?:[-a-zA-Z0-9()@:%_\\+.~#?&\\/=]*)$");

            url.onSubmit.AddListener(SearchUrl);
        }

        public void SearchUrl()
        {
            SearchUrl(url.text);
        }

        public void SearchUrl(string url)
        {
            string uri = url;

            if (!validateDateRegex.IsMatch(url))
            {
                if (url.EndsWith(".com") || url.EndsWith(".net") || url.EndsWith(".fr") || url.EndsWith(".org"))
                    uri = "http://" + uri;
                else
                    uri = "https://www.google.com/search?q=" + uri;
            }

            if (webView.CheckIfUrlValid(uri))
            {
                webView.browser.NavigateUrl(uri);
            }
            else
            {
                webView.LoadNotAccessibleWebPage(uri);
            }
        }
    }
}

