using UnityEngine;
using UnityEngine.Networking;

public class Mail
{
    public void Send(string Title, string Message)
    {
        string email = "nolan.guyon@inetum.com";
        string subject = MyEscapeURL(Title);
        string body = MyEscapeURL(Message);
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&amp;body=" + body);
        Debug.Log("mailto:" + email + "?subject=" + subject + "&amp;body=" + body);
    }

    private string MyEscapeURL(string pUrl)
    {
        return UnityWebRequest.EscapeURL(pUrl).Replace("+", "%20");
    }
}