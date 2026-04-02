using System;
using System.Threading.Tasks;
using UnityEngine;

public class TransitionController:Singleton<TransitionController>
{
    public BasePage view
    {
        get
        {
            if (Tool.PageManager&&Tool.PageManager.Pages.TryGetValue(PageManager.PageType.Transition, out var value)) return value.gameObject.activeSelf ? value: null;
            return null;
        }
    }
    public string Label;
    private void Repaint()
    {
        if(Tool.PageManager) Tool.PageManager.PageRepaint(PageManager.PageType.Transition);
    }
    private void Show()
    {
        Label = string.Empty;
        if (Tool.PageManager) Tool.PageManager.PageActive(PageManager.PageType.Transition, true);
        Repaint();
    }
    private void Hide()
    {
        if (Tool.PageManager) Tool.PageManager.PageActive(PageManager.PageType.Transition, false);
    }
    public void SetLabel(string text)
    {
        Label = text;
        Repaint();
    }
    public async void ExecuteWithLoading(Func<Task<bool>> asyncOperation, Action<bool> callback)
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
    public async void ExecuteWithLoading(Func<Task> asyncOperation, Action<bool> callback)
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
    public async void ExecuteWithLoading(Func<Task<bool>> asyncOperation)
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
    public async void ExecuteWithLoading(Func<Task> asyncOperation)
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
    public void ExecuteSignalOnly(bool active,string label)
    {
        if (active) Show();
        else Hide();

        SetLabel(label);
    }
}