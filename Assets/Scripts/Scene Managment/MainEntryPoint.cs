//using Global.Audio;
using Global.SaveSystem;
using UnityEngine;

namespace SceneManagement
{
    public class MainEntryPoint : MonoBehaviour
    {
        private const string COMMON_OBJECT_TAG = "CommonObject";

        public void Awake()
        {
            if (GameObject.FindGameObjectWithTag(COMMON_OBJECT_TAG)) return;

            var commonObjectPrefab = Resources.Load<CommonObject>("CommonObject");
            var commonObject = Instantiate(commonObjectPrefab);
            DontDestroyOnLoad(commonObject);

            //commonObject.AudioManager.LoadOnce();

            commonObject.SceneLoader.Initialize(/*commonObject.AudioManager*/);

            commonObject.SaveSystem = new();
            SaveSystem saveSystem = commonObject.SaveSystem;

            commonObject.SceneLoader.LoadMetaScene();

            commonObject.MoneyCounter.Initialize(saveSystem);

            commonObject.SkillCounter.Initialize(saveSystem);

            commonObject.BalanceManager.Initialize(saveSystem, commonObject.Formuls);
        }
    }
}