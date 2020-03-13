using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : NetworkBehaviour
{

    [SerializeField]
    private float loadSceneAfterTime = 1.0f;

    [SerializeField]
    private SceneLoaderBehavior sceneLoaderBehavior;

    public UnityEvent OnSceneLoadingCompleteForClientAndServer = new UnityEvent();


    //[SyncVar]
    private bool isLoadingScreenHidden = false;
    public bool IsLoadingScreenHidden
    {
        get
        {
            return isLoadingScreenHidden;
        }

        set
        {
            isLoadingScreenHidden = value;

            if (IsLoadingScreenHidden && OnSceneLoadingCompleteForClientAndServer != null)
            {
                OnSceneLoadingCompleteForClientAndServer.Invoke();
            }
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (sceneLoaderBehavior.HideInstantOnStart)
        {
            isLoadingScreenHidden = true;
        }
    }

    private void Update()
    {
        sceneLoaderBehavior.Hidden = isLoadingScreenHidden;
    }


    [Server]
    public void LoadScene(string scene)
    {
        RpcHideLoader(false);

        NetworkManager networkManager = NetworkManager.singleton as NetworkManager;
        if (networkManager != null)
        {
            isLoadingScreenHidden = false;

            Sequence mySequence2 = DOTween.Sequence();
            mySequence2.AppendInterval(loadSceneAfterTime).AppendCallback(() =>
            {
                networkManager.ServerChangeScene(scene);
            });
        } 
    }

    [ClientRpc]
    public void RpcHideLoader(bool hide)
    {
        isLoadingScreenHidden = hide;
    }

}
