using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class SceneFade : MonoBehaviour
{
    public static SceneFade instance = null;

    public Texture2D fadeTexture;
    public float fadeTime = 0.6f;
    private int drawDepth = -1000;
    private float alpha = 1f;
    private int fadeDirection = -1;

    private bool isFading = false;

    void Update()
    {
        if(isFading)
        {
            alpha += (fadeDirection * Time.deltaTime) * (1 / fadeTime);
            alpha = Mathf.Clamp01(alpha);
            if (alpha <= 0)
                isFading = false;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void OnGUI()
    {
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeTexture);
    }

    public float BeginFade(int direction, float duration = 0.2f)
    {
        fadeDirection = direction;
        isFading = true;
        fadeTime = duration;
        return fadeTime;
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAndFade(sceneName));
    }

    IEnumerator LoadAndFade(string sceneName)
    {
        float waitTime = BeginFade(1);
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene(sceneName);
    }

}
