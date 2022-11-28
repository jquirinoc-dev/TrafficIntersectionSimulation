using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using System;

[Serializable]
public class RequestWithArgs : UnityEvent<CarList>{}

[Serializable]
public class OneTimeRequest : UnityEvent<StepList>{}

public class NetworkManager : MonoBehaviour
{

    public static NetworkManager Instance{
        get;
        private set;
    }

    public CarList cars;
    public StepList stepList;
    public string backendURL = "http://127.0.0.1:5000/all";
    public RequestWithArgs requestWithArgs;
    public OneTimeRequest oneTimeArgsListener;
    private IEnumerator enumerator;


    private void Awake() {
        if(Instance != null){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        enumerator = GetSimulation(CarPoolManager.Instance._poolSize);
        Coroutine coroutine = StartCoroutine(enumerator);
    }

    void Update()
    {
        
    }

    IEnumerator UpdatePositions(int poolSize){
        while(true){
            string url = backendURL + "?size=" + poolSize;
            UnityWebRequest request = UnityWebRequest.Get(backendURL + "?size=" + poolSize);
            yield return request.SendWebRequest();

            if(request.result != UnityWebRequest.Result.Success){
                Debug.LogError("NEL");
                print(request.result);
            } else {
                cars = JsonUtility.FromJson<CarList>(request.downloadHandler.text);
                requestWithArgs?.Invoke(cars);
            }    
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator GetSimulation(int poolSize){
        string url = backendURL + "?size=" + poolSize;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if(request.result != UnityWebRequest.Result.Success){
            Debug.LogError("NEL");
            print(request.result);
        } else {
            stepList = JsonUtility.FromJson<StepList>(request.downloadHandler.text);
            oneTimeArgsListener?.Invoke(stepList);
        }    
        yield return new WaitForSeconds(1);      
    }
}
