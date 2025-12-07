using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    public Text loadingText;
    private float recorder;
    public Text LabelText;
    public static string Label
    {
        set
        {
            Tool.PageManager.Transition.LabelText.text = value;
        }
    }
    private int last = -1;
    private void Update()
    {
        recorder += Time.deltaTime;
        if (recorder > 4f) recorder = 0;
        if (last != (int)recorder)
        {
            last = (int)recorder;
            switch (last)
            {
                case 0: loadingText.text = "加载中"; break;
                case 1: loadingText.text = "加载中."; break;
                case 2: loadingText.text = "加载中.."; break;
                case 3: loadingText.text = "加载中..."; break;
            }
        }
    }
    public static void Show()
    {
        Label = string.Empty;
        Tool.PageManager.Transition.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        Tool.PageManager.Transition.gameObject.SetActive(false);
    }
    public static async void ExecuteWithLoading(Func<Task<bool>> asyncOperation, Action<bool> callback)
    {
        // 显示加载界面
        Show();

        bool success = false;
        try
        {
            // 执行异步操作并等待完成
            success = await asyncOperation();
        }
        catch (Exception e)
        {
            Debug.LogError($"异步操作执行出错: {e.Message}");
            success = false;
        }
        finally
        {
            // 调用回调通知结果
            callback?.Invoke(success);

            // 隐藏加载界面
            Hide();
        }
    }
    public static async void ExecuteWithLoading(Func<Task> asyncOperation, Action<bool> callback)
    {
        Show();

        bool success = false;
        try
        {
            await asyncOperation();
            success = true;
        }
        catch (Exception e)
        {
            Debug.LogError($"异步操作执行出错: {e.Message}");
            success = false;
        }
        finally
        {
            callback?.Invoke(success);
            Hide();
        }
    }
    public static async void ExecuteWithLoading(Func<Task<bool>> asyncOperation)
    {
        // 显示加载界面
        Show();
        try
        {
            // 执行异步操作并等待完成
            _ = await asyncOperation();
        }
        catch (Exception e)
        {
            Debug.LogError($"异步操作执行出错: {e.Message}");
        }
        finally
        {
            // 隐藏加载界面
            Hide();
        }
    }
    public static async void ExecuteWithLoading(Func<Task> asyncOperation)
    {
        Show();

        try
        {
            await asyncOperation();
        }
        catch (Exception e)
        {
            Debug.LogError($"异步操作执行出错: {e.Message}");
        }
        finally
        {
            Hide();
        }
    }
    /*
    async () =>
    {
        await Method1();
        await Method2();
        await Method3();
        // 可以添加更多异步方法
    };
    async () =>
    {
        // 同时启动所有异步操作
        Task task1 = Method1();
        Task task2 = Method2();
        Task task3 = Method3();
        
        // 等待所有操作完成
        await Task.WhenAll(task1, task2, task3);
    };
     */
}