using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using XLua;

public static class TemplateLoader
{
    public static Dictionary<string, string> LevelInfoMap=new();
    private static List<string> LevelInfo = new List<string>();

    // 统一路径拼接（自动适配平台分隔符）
    private static string CustomLevelDirPath => Path.Combine(Application.streamingAssetsPath, "Templates");

    /// <summary>
    /// 异步加载CustomLevel目录下所有.lua文件内容
    /// </summary>
    /// <returns>加载是否成功</returns>
    public static async Task<bool> LoadAsync()
    {
        try
        {
            LevelInfo.Clear();

            // 根据平台选择加载方式
            if (Application.isEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                Application.platform == RuntimePlatform.OSXPlayer ||
                Application.platform == RuntimePlatform.LinuxPlayer)
            {
                // 编辑器/Windows/Mac/Linux：直接文件IO读取（同步IO封装为异步）
                await LoadFromFileAsync();
            }
            else if (Application.platform == RuntimePlatform.Android ||
                     Application.platform == RuntimePlatform.IPhonePlayer)
            {
                // Android/iOS：通过UnityWebRequest异步读取（因为嵌入APK/IPA，无法直接File访问）
                await LoadFromWebRequestAsync();
            }
            else
            {
                Debug.LogError($"Unsupported platform: {Application.platform}");
                return false;
            }

            using (LuaEnv env = new LuaEnv())
            {
                foreach (var s in LevelInfo)
                {
                    try
                    {
                        env.DoString(s);
                        var t = env.Global.Get<string>("TemplateName");
                        LevelInfoMap.Add(t,s);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
            Debug.Log($"Successfully loaded {LevelInfoMap.Count} template files from {CustomLevelDirPath}");

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Load Lua templates files failed: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }

    #region 平台专属加载逻辑
    /// <summary>
    /// 编辑器/桌面平台：从本地文件系统异步读取
    /// </summary>
    private static async Task LoadFromFileAsync()
    {
        // 检查目录是否存在
        if (!Directory.Exists(CustomLevelDirPath))
        {
            Debug.LogWarning($"CustomLevel directory not found: {CustomLevelDirPath}");
            return;
        }

        // 获取所有.lua文件（递归查找子目录，如需仅一级目录则去掉SearchOption.AllDirectories）
        string[] luaFilePaths = Directory.EnumerateFiles(CustomLevelDirPath, "*.*", SearchOption.AllDirectories)
            .Where(file =>
            file.EndsWith(".lua", StringComparison.OrdinalIgnoreCase) ||
            file.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            .ToArray();// = Directory.GetFiles(CustomLevelDirPath, "*.lua", SearchOption.AllDirectories);

        // 异步读取每个文件（避免阻塞主线程）
        foreach (string filePath in luaFilePaths)
        {
            try
            {
                // 使用异步File API读取文件内容（UTF-8编码，可根据实际需求调整）
                string fileContent = await File.ReadAllTextAsync(filePath, System.Text.Encoding.UTF8);

                // 填充数据（按需选择存储方式）
                LevelInfo.Add(fileContent);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to read file {filePath}: {e.Message}");
            }
        }
    }

    /// <summary>
    /// Android/iOS平台：通过UnityWebRequest异步读取
    /// </summary>
    private static async Task LoadFromWebRequestAsync()
    {
        // StreamingAssets在Android/iOS的路径需要转成URI格式
        string baseUri = GetStreamingAssetsUri(CustomLevelDirPath);

        // 步骤1：先获取目录下所有.lua文件（注：Android/iOS无法直接枚举目录，需提前知道文件名列表
        // 方案1：在StreamingAssets下放一个文件清单（如filelist.txt），记录所有lua文件名
        // 方案2：预设已知的lua文件名列表（此处用方案1，更灵活）
        List<string> luaFileNames = await GetLuaFileListFromManifestAsync();
        if (luaFileNames.Count == 0)
        {
            Debug.LogWarning("No Lua file names found in manifest");
            return;
        }

        // 步骤2：逐个加载lua文件
        foreach (string fileName in luaFileNames)
        {
            string fileUri = Path.Combine(baseUri, fileName);
            // 替换路径分隔符（URI必须用/）
            fileUri = fileUri.Replace("\\", "/");

            using (UnityWebRequest request = UnityWebRequest.Get(fileUri))
            {
                // 异步发送请求（适配Unity的协程+Task）
                var requestTask = SendWebRequestAsync(request);
                await requestTask;

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string fileContent = request.downloadHandler.text;
                    // 填充数据
                    LevelInfo.Add(fileContent);
                }
                else
                {
                    Debug.LogError($"Failed to load {fileName}: {request.error} (URI: {fileUri})");
                }
            }
        }
    }
    #endregion

    #region 辅助方法
    /// <summary>
    /// 将StreamingAssets路径转为平台兼容的URI（适配Android的jar协议）
    /// </summary>
    private static string GetStreamingAssetsUri(string path)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            return $"jar:file://{Application.dataPath}!/assets/CustomLevel";
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return $"file://{path}";
        }
        return path;
    }

    /// <summary>
    /// 从manifest文件读取Lua文件名列表（Android/iOS平台必备）
    /// 需提前在CustomLevel目录下创建filelist.txt，每行一个lua文件名（如level1.lua）
    /// </summary>
    private static async Task<List<string>> GetLuaFileListFromManifestAsync()
    {
        List<string> fileList = new List<string>();
        string manifestUri = GetStreamingAssetsUri(Path.Combine(CustomLevelDirPath, "filelist.txt"));
        manifestUri = manifestUri.Replace("\\", "/");

        using (UnityWebRequest request = UnityWebRequest.Get(manifestUri))
        {
            await SendWebRequestAsync(request);

            if (request.result == UnityWebRequest.Result.Success)
            {
                // 按行分割文件名（去除空行和注释）
                string[] lines = request.downloadHandler.text.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string fileName = line.Trim();
                    // 跳过注释行（以//开头）
                    if (!string.IsNullOrEmpty(fileName) && !fileName.StartsWith("//"))
                    {
                        fileList.Add(fileName);
                    }
                }
            }
            else
            {
                Debug.LogError($"Failed to load file list: {request.error}");
            }
        }
        return fileList;
    }

    /// <summary>
    /// 将UnityWebRequest转为Task（适配async/await）
    /// </summary>
    private static Task SendWebRequestAsync(UnityWebRequest request)
    {
        var tcs = new TaskCompletionSource<bool>();
        request.SendWebRequest().completed += operation =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                tcs.SetException(new Exception(request.error));
            }
            else
            {
                tcs.SetResult(true);
            }
        };
        return tcs.Task;
    }

    /// <summary>
    /// 清空加载的数据（可选）
    /// </summary>
    public static void Clear()
    {
        LevelInfo.Clear();
    }
    #endregion
}