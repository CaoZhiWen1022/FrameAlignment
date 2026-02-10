using Assets.Scripts.FGUI;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMgr : MonoBehaviour
{
    private static SceneMgr _instance;
    public static SceneMgr Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("ScenemMgr");
                _instance = go.AddComponent<SceneMgr>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    // 调用此方法触发异步跳转（可通过按钮点击、代码逻辑调用）
    public void StartAsyncLoadScene(string sceneName)
    {
        // 开启协程处理异步加载
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    // 协程：处理异步加载逻辑
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // 加载场景但不立即激活
        AsyncOperation asyncOp = SceneManager.LoadSceneAsync(sceneName);
        asyncOp.allowSceneActivation = false;

        // 临时存储进度，用于平滑更新
        float currentProgress = 0;

        while (currentProgress < 0.9f)
        {
            currentProgress = asyncOp.progress;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        //清理所有打开的UI
        GameUIFrame.Instance.uiFrame.CloseAll();
        GameUIFrame.Instance.uiFrame.Open(new FGUIFrame.OpenUIParam((int)UIID.JumpScenePanel));
        asyncOp.allowSceneActivation = true;

        // 等待场景加载完成
        while (!asyncOp.isDone)
        {
            yield return null;
        }
        JumpScenePanel jumpScenePanel = GameUIFrame.Instance.uiFrame.GetUIInstance((int)UIID.JumpScenePanel) as JumpScenePanel;
        jumpScenePanel.closeAni();
    }
}
