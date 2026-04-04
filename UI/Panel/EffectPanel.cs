using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class EffectPanel : MonoBehaviour
{
    [Header("击杀标识")]
    [SerializeField] private CanvasGroup KilledSign;

    [Header("命中闪烁")]
    [SerializeField] private Image FlickEffect;

    [Header("轮廓颜色轮换")]
    [SerializeField] private Image OutlineEffect;
    private List<Color> alternatingColors = new List<Color>(); // 存储轮换颜色
    private Coroutine colorCycleCoroutine; // 颜色轮询协程

    [Header("文字标题")]
    [SerializeField] private Text Title;
    [SerializeField] private Text Subtitle;

    // 协程引用：用于打断未完成的动画
    private Coroutine killedSignCoroutine;
    private Coroutine titleCoroutine;
    private Coroutine subtitleCoroutine;
    private Coroutine hitCoroutine;

    // 淡入淡出总时长配置
    private readonly float fadeInTime = 0.5f;   // 淡入0.5s
    private readonly float stayTime = 1f;      // 保持1s
    private readonly float fadeOutTime = 0.5f; // 淡出0.5s

    // 命中效果时长配置
    private readonly float hitTotalTime = 0.2f;   // 总时长0.2s
    private readonly float hitShowTime = 0.1f;    // 显示阶段0.1s
    private readonly float hitAlpha = 0.2f;      // 命中透明度

    private void OnEnable()
    {
        // 启用时清除所有特效、重置状态
        ClearAllEffects();
    }

    /// <summary>
    /// 清空所有正在运行的特效，重置透明度和文字
    /// </summary>
    public void ClearAllEffects()
    {
        // 停止所有协程
        if (killedSignCoroutine != null) StopCoroutine(killedSignCoroutine);
        if (titleCoroutine != null) StopCoroutine(titleCoroutine);
        if (subtitleCoroutine != null) StopCoroutine(subtitleCoroutine);
        if (hitCoroutine != null) StopCoroutine(hitCoroutine);
        if (colorCycleCoroutine != null) StopCoroutine(colorCycleCoroutine);

        // 重置透明度
        if (KilledSign != null) KilledSign.alpha = 0;
        if (FlickEffect != null) FlickEffect.canvasRenderer.SetAlpha(0);
        if (Title != null) Title.canvasRenderer.SetAlpha(0);
        if (Subtitle != null) Subtitle.canvasRenderer.SetAlpha(0);

        // 清空文字
        if (Title != null) Title.text = string.Empty;
        if (Subtitle != null) Subtitle.text = string.Empty;

        // 重置轮廓颜色
        if (OutlineEffect != null) OutlineEffect.color = Color.clear;
        alternatingColors.Clear();
    }

    /// <summary>
    /// 显示击杀标识：淡入→保持→淡出，重复调用会打断旧动画
    /// </summary>
    public void ShowKilledSign()
    {
        if (KilledSign == null) return;

        // 打断上一个未完成的动画
        if (killedSignCoroutine != null)
            StopCoroutine(killedSignCoroutine);

        killedSignCoroutine = StartCoroutine(FadeCoroutine(KilledSign));
    }

    /// <summary>
    /// 显示主标题
    /// </summary>
    public void ShowTitle(string message)
    {
        if (Title == null) return;

        if (titleCoroutine != null)
            StopCoroutine(titleCoroutine);

        Title.text = message;
        titleCoroutine = StartCoroutine(FadeCoroutine(Title));
    }

    /// <summary>
    /// 显示子标题
    /// </summary>
    public void ShowSubTitle(string message)
    {
        if (Subtitle == null) return;

        if (subtitleCoroutine != null)
            StopCoroutine(subtitleCoroutine);

        Subtitle.text = message;
        subtitleCoroutine = StartCoroutine(FadeCoroutine(Subtitle));
    }

    /// <summary>
    /// 命中效果：0.1s显→0.1s隐，0.2s内重复调用无效
    /// </summary>
    public void OnHit()
    {
        if (FlickEffect == null) return;
        if (hitCoroutine != null) return; // 正在播放中，直接忽略

        hitCoroutine = StartCoroutine(HitFlickCoroutine());
    }

    /// <summary>
    /// 添加轮换颜色，返回颜色索引
    /// </summary>
    public int AddAlternatingColor(Color color)
    {
        alternatingColors.Add(color);

        // 如果是第一个颜色，启动轮询
        if (alternatingColors.Count == 1 && OutlineEffect != null)
        {
            if (colorCycleCoroutine != null)
                StopCoroutine(colorCycleCoroutine);
            colorCycleCoroutine = StartCoroutine(ColorCycleCoroutine());
        }

        return alternatingColors.Count - 1;
    }

    /// <summary>
    /// 根据索引移除轮换颜色
    /// </summary>
    public void RemoveAlternatingColor(int index)
    {
        if (index < 0 || index >= alternatingColors.Count)
            return;

        alternatingColors.RemoveAt(index);

        // 没有颜色了，清空轮廓并停止轮询
        if (alternatingColors.Count == 0 && OutlineEffect != null)
        {
            OutlineEffect.color = Color.clear;
            if (colorCycleCoroutine != null)
            {
                StopCoroutine(colorCycleCoroutine);
                colorCycleCoroutine = null;
            }
        }
    }

    #region 核心协程

    /// <summary>
    /// 通用淡入淡出协程（CanvasGroup / Graphic）
    /// </summary>
    private IEnumerator FadeCoroutine(Graphic graphic)
    {
        // 淡入
        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            graphic.canvasRenderer.SetAlpha(Mathf.Lerp(0, 1, timer / fadeInTime));
            yield return null;
        }
        graphic.canvasRenderer.SetAlpha(1);

        // 保持
        yield return new WaitForSeconds(stayTime);

        // 淡出
        timer = 0;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            graphic.canvasRenderer.SetAlpha(Mathf.Lerp(1, 0, timer / fadeOutTime));
            yield return null;
        }
        graphic.canvasRenderer.SetAlpha(0);
    }

    /// <summary>
    /// CanvasGroup 专用淡入淡出
    /// </summary>
    private IEnumerator FadeCoroutine(CanvasGroup canvasGroup)
    {
        // 淡入
        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, timer / fadeInTime);
            yield return null;
        }
        canvasGroup.alpha = 1;

        // 保持
        yield return new WaitForSeconds(stayTime);

        // 淡出
        timer = 0;
        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, timer / fadeOutTime);
            yield return null;
        }
        canvasGroup.alpha = 0;
    }

    /// <summary>
    /// 命中闪烁协程
    /// </summary>
    private IEnumerator HitFlickCoroutine()
    {
        // 前0.1s：透明度0.2
        FlickEffect.canvasRenderer.SetAlpha(hitAlpha);
        yield return new WaitForSeconds(hitShowTime);

        // 后0.1s：透明度0
        FlickEffect.canvasRenderer.SetAlpha(0);
        yield return new WaitForSeconds(hitTotalTime - hitShowTime);

        // 动画结束，允许下次调用
        hitCoroutine = null;
    }

    /// <summary>
    /// 颜色轮询协程（每秒切换一次）
    /// </summary>
    private IEnumerator ColorCycleCoroutine()
    {
        if (OutlineEffect == null) yield break;

        int index = 0;
        while (alternatingColors.Count > 0)
        {
            OutlineEffect.color = alternatingColors[index];
            index = (index + 1) % alternatingColors.Count;
            yield return new WaitForSeconds(1f);
        }
    }

    #endregion
}