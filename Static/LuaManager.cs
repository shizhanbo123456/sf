using UnityEngine;
using XLua;

public class LuaManager : MonoBehaviour
{
    public LuaEnv luaEnv;

    private void Awake()
    {
        Tool.LuaManager=this;

        luaEnv = new LuaEnv();
    }
    private void OnApplicationQuit()
    {
        luaEnv.Dispose();
    }
}