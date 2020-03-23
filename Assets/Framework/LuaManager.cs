using System;
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
        private static Dictionary<string, AssetRequest> assets = new Dictionary<string, AssetRequest>();

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
            foreach (var item in assets)
            {
                item.Value.Release();
            }
            assets.Clear();
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

        private static byte[] ReadBytesFromAssetBundle(ref string filepath)
        {
            var path = "Assets/Bytes/Lua/" + filepath + ".lua.bytes";
            AssetRequest a; 
            if (!assets.TryGetValue(path, out a))
            {
                a = Assets.LoadAsset(path, typeof(TextAsset));
                assets[path] = a;
            }
            var ta = a.asset as TextAsset;
            if (ta != null)
            {
                return ta.bytes;
            }
            return null;
        }

        private static byte[] ReadBytesFromEditor(ref string filename)
        {
            var path = System.Environment.CurrentDirectory
                + "/Lua/"
                + filename
                + ".lua";

            if (!System.IO.File.Exists(path))
            {
                throw new System.IO.FileNotFoundException(path);
            }

            return System.IO.File.ReadAllBytes(path);
        }
    }
}
