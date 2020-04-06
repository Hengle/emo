using System.Collections.Generic;
using UnityEngine;
using libx;
using XLua;

namespace emo
{
    public class LuaManager
    {
        private static LuaEnv luaEnv = new LuaEnv(); 
        private static List<LuaBehaviour> luaBehaviours = new List<LuaBehaviour>();
        private static Dictionary<string, byte[]> buffers = new Dictionary<string, byte[]>();

        public static void Register(LuaBehaviour lua)
        {
            luaBehaviours.Add(lua);
        }

        public static void Unregister(LuaBehaviour lua)
        {
            luaBehaviours.Remove(lua);
        }

        public static void Init(bool assetBundleMode)
        {
            if (assetBundleMode)
            {
                luaEnv.AddLoader(ReadBytesFromAssetBundle);
            }
            else
            {
                luaEnv.AddLoader(ReadBytesFromEditor); 
            }

            luaEnv.DoString("require 'Main'"); 
        }

        public static void Clear()
        { 
            buffers.Clear();
        } 

        public static void Dispose()
        {
            Clear();

            foreach (var item in luaBehaviours)
            {
                item.Clear();
            }

            luaEnv.Dispose();
            luaEnv = null;
            Debug.Log("[LuaManager]Dispose");
        } 

        public static T GetFunc<T>(string name)
        {
            return luaEnv.Global.Get<T>(name);
        }

        private const string luafile_format = "Assets/Lua/{0}.lua.bytes";

        private static byte[] ReadBytesFromAssetBundle(ref string filename)
        {
            var path = string.Format(luafile_format, filename); 
            byte[] bytes;
            if (!buffers.TryGetValue(path, out bytes))
            {
                var request = Assets.LoadAsset(path, typeof(TextAsset));
                var ta = request.asset as TextAsset;
                if (ta != null)
                { 
                    bytes = ta.bytes;
                    buffers[path] = bytes;
                }
                Resources.UnloadAsset(ta); 
                request.Release();
                request = null;
            } 
            return bytes;
        }

        private static byte[] ReadBytesFromEditor(ref string filename)
        {
            var path = string.Format(luafile_format, filename);
            if (!System.IO.File.Exists(path))
            {
                throw new System.IO.FileNotFoundException(path);
            }

            byte[] bytes; 
            if (!buffers.TryGetValue(path, out bytes))
            {
                bytes = System.IO.File.ReadAllBytes(path);
                buffers[path] = bytes;
            } 
            return bytes;
        } 

        public static void AttachProfiler()
        {
            var attachProfiler = GetFunc<LuaFunction>("AttachProfiler"); 
            if (attachProfiler != null)
            {
                attachProfiler.Call();
                attachProfiler.Dispose();
                attachProfiler = null; 
            }
        }
        
        public static void DetachProfiler()
        { 
            var detachProfiler = GetFunc<LuaFunction>("DetachProfiler");  
            if (detachProfiler != null)
            { 
                detachProfiler.Call();
                detachProfiler.Dispose();
                detachProfiler = null;
            }
        }
    }
}
