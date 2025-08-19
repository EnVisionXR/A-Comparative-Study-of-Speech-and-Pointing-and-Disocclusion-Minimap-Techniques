using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PostRequest : MonoBehaviour
{
    string url = "https://cognitivelanguage0122.cognitiveservices.azure.com/language/:analyze-conversations?api-version=2022-10-01-preview";
    static string userQuery = "Make the white sofa blue.";
    string bodyJsonString = "{\"kind\":\"Conversation\",\"analysisInput\":{\"conversationItem\":{\"id\":\"PARTICIPANT_ID_HERE\",\"text\":\"Make the white sofa blue.\",\"modality\":\"text\",\"language\":\"en\",\"participantId\":\"PARTICIPANT_ID_HERE\"}},\"parameters\":{\"projectName\":\"UserQueryUnderstanding\",\"verbose\":true,\"deploymentName\":\"SceneEdit-0121\",\"stringIndexType\":\"TextElement_V8\"}}";
    //string bodyJsonString = "{\"kind\":\"Conversation\",\"analysisInput\":{\"conversationItem\":{\"id\":\"PARTICIPANT_ID_HERE\",\"text\":\"" + userQuery + "\",\"modality\":\"text\",\"language\":\"en\",\"participantId\":\"PARTICIPANT_ID_HERE\"}},\"parameters\":{\"projectName\":\"UserQueryUnderstanding\",\"verbose\":true,\"deploymentName\":\"SceneEdit-0121\",\"stringIndexType\":\"TextElement_V8\"}}";
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Post(url, bodyJsonString));
        }
    }

    public IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6ImtXYmthYTZxczh3c1RuQndpaU5ZT2hIYm5BdyIsImtpZCI6ImtXYmthYTZxczh3c1RuQndpaU5ZT2hIYm5BdyJ9.eyJhdWQiOiJodHRwczovL2NvZ25pdGl2ZXNlcnZpY2VzLmF6dXJlLmNvbSIsImlzcyI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzQ5YTUwNDQ1LWJkZmEtNGI3OS1hZGUzLTU0N2I0ZjM5ODZlOS8iLCJpYXQiOjE3MDcwNDg5MzcsIm5iZiI6MTcwNzA0ODkzNywiZXhwIjoxNzA3MDU0MDIzLCJhY3IiOiIxIiwiYWlvIjoiQVlRQWUvOFZBQUFBR2hJa09UY0h6ZlVVZy9CZzRXbXJlNGxpYTZIYTQvSTRmRTJ0UktiR1VrcUJoeUFRL2ViUWxrQzErV1RUS2t4aEtrTE1FeVNpb042aHRsOHZWV09kMHRNWDkvbE1QYUtVcGc1Q20zbjdnWm4yQkNFOGhKaTZyaFdhcUpLK2JPZWtPSzQxd3gwQ1gyRVBmRDFiZW1vNXpNbXdMNUZKVVR5MWxyTEx4SUFiTFVZPSIsImFtciI6WyJwd2QiLCJtZmEiXSwiYXBwaWQiOiI4ODk0NWRiMC0zYzBlLTQyZGYtOTRmYS01ODVkMWFkNGFhMjAiLCJhcHBpZGFjciI6IjIiLCJmYW1pbHlfbmFtZSI6IkNoZW4iLCJnaXZlbl9uYW1lIjoiSnVubG9uZyIsImdyb3VwcyI6WyIxYjI3MDgwNC1lYzg1LTQxNzEtODcxOC1lZGQwZGNkYTVmN2QiLCI5MWY4MmMwNS02ZDQ0LTQ1ZmEtYTVkZS1lOWFkYjQ0ZDExNTAiLCIzMWJlYWQwOS05YmRhLTQxZDgtODBjOS03YTk5MTFmMzhkMWEiLCIxNTEzM2MwYy02NzczLTQ0OTktOTQzMC0wMmFkNzgzOWQ2MDMiLCJlYWNiNmIxNS01MDRhLTQ0MDUtODljNi0yYTJiZDg3OWFmYzAiLCI1ODcwNWMxNy0zNTBlLTRjNWYtYTljZC1jOGIzYjJhOTUyMDgiLCI2ZGUyZDkxOC01YWIxLTQ2N2UtOWM5NC0xM2EzODNiMTdmZjciLCIyMWVhM2UyMy0yYjY1LTRlOWQtOTYyMC05OWM2YzU4MjJkY2YiLCIxYjhiNDMyZS04ZjQyLTQ5Y2ItODlhNS01MTkzYzVhZDk2NWQiLCJiN2EwZjkzMi01OTY0LTQxYjItOWJiMC05YjhjYWRmNmI5OTkiLCI5N2Q5MmIzOS0yYWUzLTQ5YTYtOTBjOC05OGQ1YjU5ZmUzOGEiLCIwMzI4MzUzOS04MjcxLTQ2YmMtOGViNS01NDE2OGM5MGI4ZTgiLCIyZDRjODI0MS1hNmExLTQ5MWUtYjgxYi0zMzVlYTRhN2Q1MjkiLCJkY2VlNzM0Ni1kNGRlLTQ2N2UtYjBiZi0zZWQ0NmFiMzU2MzIiLCI4MjA5OWM0Yi1jYTZiLTQ5MmUtOGYxMS01N2ZlMjkwM2I1NzgiLCJjNTU1MmY1MS04M2RmLTQ3MGMtYmFkNi1kMjgwMjViOWI0YjIiLCI4YmI5YWE3MS1iOTFkLTRlZWMtYTM0ZC04OWRlZTY3MWY5ODciLCJlZmU5NWQ3My01MWE4LTRlZGQtOGRhMy1lNWI5YTZjMTdjOTEiLCJjMjVlYzE3Yy01Y2RhLTQ3ODMtYWI1OS05OWMyMGU4M2JhYmIiLCI0YjRkYWQ3Zi0yMGRkLTQ4NmQtOGIwNC04MDQwYmFmZjdiMzgiLCJjYzJjZGQ4Yi1lYWNlLTRhNGItYTk1MC05Yjk4OWExODNiOTciLCIwZGNmMTk5Ni0zZDZhLTQ0YzAtYjE1NC1lMTI3YzEyNDIzNzAiLCJmYWU3YzFhZi04MzRiLTQ1NDUtOGJmOS1iNWYxZGQwZDUyZjciLCJlZTRjMDZiZi1kOTNmLTQxNmMtOWJlNC1iY2ZhOWU1MjY2YzEiLCI2OThhOTNmYi0wNTNhLTRhYTktODBkMy1mYWI1NGI2NGQzMWEiLCIwY2JjZDdmYi0xZjE3LTQ4ZmMtYWMzZS00YTIyMTMxZmE5MmQiLCI2NzdhMGVmZS0xZWQ4LTRlZDYtOWZlNC04N2M3NTk1YzI2MGIiXSwiaXBhZGRyIjoiMTMxLjExMS41LjE4NCIsIm5hbWUiOiJKdW5sb25nIENoZW4iLCJvaWQiOiJmZTlhOGI1MC01NTU0LTQ3M2MtYTVmOS00YjQ2NjFiMjA5ZjQiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtMzE0Mjk0MjY1Ni00MTU3MTk3NzM5LTI4MjA5MDU5MDktMjgzOTEzIiwicHVpZCI6IjEwMDMyMDAyMEMwNTczODMiLCJyaCI6IjAuQVVjQVJRU2xTZnE5ZVV1dDQxUjdUem1HNlpBaU1YM0lLRHhIb08yT1UzU2JiVzFIQUQ0LiIsInNjcCI6InVzZXJfaW1wZXJzb25hdGlvbiIsInN1YiI6IldqYTlwRm56U2RjY2NxN3JmRlRXNjYyc2Mwa1ZFcmVESEZQdE9ZNlJMTXMiLCJ0aWQiOiI0OWE1MDQ0NS1iZGZhLTRiNzktYWRlMy01NDdiNGYzOTg2ZTkiLCJ1bmlxdWVfbmFtZSI6ImpjMjM3NUBjYW0uYWMudWsiLCJ1cG4iOiJqYzIzNzVAY2FtLmFjLnVrIiwidXRpIjoiRkVjQmlCNFl4RWF5MTM0b05IdE9BQSIsInZlciI6IjEuMCIsIndpZHMiOlsiYjc5ZmJmNGQtM2VmOS00Njg5LTgxNDMtNzZiMTk0ZTg1NTA5Il19.H3XwhBwgRpeBNUfLWQhujk_5Uk67daXP8kENm33K2wExcYPYyuvKTMCK_3Rwm6ZcZ9S4MSNQ2OUmb69Lo2MC2f_dQ5uXdbNRGdMbS2YYMS9O7Riro2Ic0JdpNSZR6wyAMhzuaGwBKJdxpmVOlF-UZKBQrz5_OZ322Po5wU-Hx30wOFSyAK9QU5hbWuREKmEL1JYkEos2ZYEnWnxn2ein4m_X1VufvNfstKhw4IqjJcLBi2ywE6VVT6gifwU_pcRNFkQkoGbXiaXaQyL1Irr6HIqgKWXZn_1D8RNKCUlFubBDRXSdC22WUO6zDUB_qICPw1SQIWbEXlZ1NUae3Oh3BA");
        request.SetRequestHeader("Apim-Request-Id", "4ffcac1c-b2fc-48ba-bd6d-b69d9942995a");
        yield return request.SendWebRequest();
        Debug.LogError("Status Code: " + request.responseCode);
        
        Debug.LogError(request.downloadHandler.text);
        JObject response = JObject.Parse(request.downloadHandler.text);
        //yield return response;
        var topIntent = response["result"]["prediction"]["topIntent"];
        string topIntent_string = topIntent.ToString();
        Debug.LogError("Top Intent: " + topIntent);

        if (topIntent_string == "ModifyAppearance")
        {
            var entities = response["result"]["prediction"]["entities"];
            JToken originalColor = entities.SelectToken("$[?(@.category == 'OriginalColor')].text");
            Debug.LogError("Original Color: " + originalColor);
            JToken targetColor = entities.SelectToken("$[?(@.category == 'TargetColor')].text");
            Debug.LogError("Target Color: " + targetColor);
        }
    }

}
