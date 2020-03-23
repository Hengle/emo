using libx;
using UnityEngine;
using XLua;

namespace emo
{
    public class Main : MonoBehaviour
    {

        LuaFunction _updateFunc = null;
        LuaFunction _lateUpdateFunc = null;
        LuaFunction _fixedUpdateFunc = null;
        LuaFunction _focusFunc = null;
        LuaFunction _pauseFunc = null;

        // Use this for initialization
        System.Collections.IEnumerator Start()
        {
            DontDestroyOnLoad(gameObject);
            yield return Assets.Initialize();
            OnInited();
        }

        private void OnInited()
        {
            LuaManager.Init(Assets.assetBundleMode);

            _updateFunc = LuaManager.GetFunc<LuaFunction>("Update");
            _lateUpdateFunc = LuaManager.GetFunc<LuaFunction>("LateUpdate");
            _fixedUpdateFunc = LuaManager.GetFunc<LuaFunction>("FixedUpdate");
            _focusFunc = LuaManager.GetFunc<LuaFunction>("OnApplicationFocus");
            _pauseFunc = LuaManager.GetFunc<LuaFunction>("OnApplicationPause");
        }

        // Update is called once per frame
        private void Update()
        {
            if (_updateFunc != null)
            {
                _updateFunc.Action(Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (_fixedUpdateFunc != null)
            {
                _fixedUpdateFunc.Action(Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (_lateUpdateFunc != null)
            {
                _lateUpdateFunc.Action(Time.deltaTime);
            }
        }

        private void OnApplicationFocus(bool focus)
        {
            if (_focusFunc != null)
            {
                _focusFunc.Action<bool>(focus);
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (_pauseFunc != null)
            {
                _pauseFunc.Action<bool>(pause);
            }
        }

        private void SafeDispose(ref LuaFunction func)
        {
            if (func != null)
            {
                func.Dispose();
                func = null;
            }
        }

        private void OnDestroy()
        {
            SafeDispose(ref _updateFunc);
            SafeDispose(ref _lateUpdateFunc);
            SafeDispose(ref _fixedUpdateFunc);
            SafeDispose(ref _focusFunc);
            SafeDispose(ref _pauseFunc);

            LuaManager.Dispose();
        }
    }
}
