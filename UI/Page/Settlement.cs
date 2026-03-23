using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settlement : MonoBehaviour
{
    public float Smooth=0.95f;
    public Text ModeName;
    public Transform Subscore;//-152->0
    public Text Score1;
    public Text Score2;
    public Text Score3;
    public Transform ScoreObj1;
    public Transform ScoreObj2;
    public Transform ScoreObj3;
    public Text Sum;
    public Transform SumObject;
    public List<Text> Mark;
    public GameObject MarkColumnObj;
    public Transform MarkObj;
    public CanvasGroup MarkAlpha;
    private static List<int> ScoreThreshold = new List<int>() {400,600,1000,1500,2000,2500,3000,4000,5000,10000,99999999 };//进入下一级的分数
    private static List<string> MarkText = new List<string>() { "F","E","D","C","B","A","S","SS","SSS","SSSR","Ω"};
    public List<Color> MarkColor = new List<Color>();
    private List<GameObject> BufferVFX=new List<GameObject>();
    [Space]
    public GameObject Particle;
    public Transform P1;
    public Transform P2;
    public Transform P3;
    public Transform PSum;
    public GameObject Camera;

    public void Settle(int score1,int score2,int score3)
    {
        gameObject.SetActive(true);
        ModeName.text = LevelCreator.CustomLevel.ModePath;
        Score1.text = score1.ToString();
        Score2.text = score2.ToString();
        Score3.text = score3.ToString();
        int sum = score1 + score2 + score3;
        Sum.text = sum.ToString();
        int i = 0;
        while (ScoreThreshold[i] < sum) i++;
        Mark[0].color = MarkColor[i];
        foreach(var m in Mark)
        {
            m.text = MarkText[i];
        }

        ScoreObj1.localScale = new Vector3();
        ScoreObj2.localScale = new Vector3();
        ScoreObj3.localScale = new Vector3();
        SumObject.localScale= new Vector3();
        MarkColumnObj.SetActive(false);

        StartCoroutine(nameof(Anim));
    }
    private IEnumerator Anim()
    {
        Camera.SetActive(true);
        Subscore.localPosition = new Vector3(0, -152, 0);
        MarkObj.localScale = Vector3.one * 4;
        #region//子分数
        BufferVFX.Add(Instantiate(Particle, P1.transform.position, Quaternion.identity));
        float t=0;
        while(t<0.5f)
        {
            t += Time.deltaTime;
            ScoreObj1.localScale = (ScoreObj1.localScale.x*Smooth+1-Smooth)*Vector3.one;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        BufferVFX.Add(Instantiate(Particle, P2.transform.position, Quaternion.identity));
        t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            ScoreObj2.localScale = (ScoreObj2.localScale.x * Smooth + 1 - Smooth) * Vector3.one;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        BufferVFX.Add(Instantiate(Particle, P3.transform.position, Quaternion.identity));
        t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            ScoreObj3.localScale = (ScoreObj3.localScale.x * Smooth + 1 - Smooth) * Vector3.one;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        #endregion
        t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            Subscore.localPosition*=Smooth;//终点为000
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);

        BufferVFX.Add(Instantiate(Particle, PSum.position, Quaternion.identity));
        t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            SumObject.localScale = (SumObject.localScale.x * Smooth + 1 - Smooth) * Vector3.one;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        
        MarkColumnObj.SetActive(true);
        t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            MarkObj.localScale = (MarkObj.localScale .x* Smooth+1-Smooth)*Vector3.one;
            MarkAlpha.alpha = t * 2;
            yield return null;
        }
    }
    public void Exit()
    {
        foreach (var i in BufferVFX) Destroy(i);
        BufferVFX.Clear();
        Camera.SetActive(false);
        gameObject.SetActive(false);
    }
}
