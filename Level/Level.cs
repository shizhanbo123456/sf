using LevelCreator;
using LevelCreator.TargetTemplate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class Level : SingleLevel
{
    public static byte[][] map;
    // 0空气 1实心地面 2平台 3破损平台 4风场 5跳台 6地刺 7检查点 8选择点

    private LandscapeInfo info;
    private Vector2Int landscapeSize => new Vector2Int(16 * info.sizeX, 8 * info.sizeY);


    private static Transform TileRoot;
    private static Transform Traps;
    private static Transform Colliders;
    private static List<ObjectPool<Transform>> TilePools=new();

    //可交互的物体
    //破损平台塌陷，value=破损平台的index
    //跳跃平台，value=破损平台的index
    //风区，value=风区的index
    //尖刺，value=尖刺的index
    private Dictionary<Vector2Int, int> InteractableTargetInfoMap = new();//物体在info中的id。broenPlatform在info的id和collider的id可以共用
    private Dictionary<Vector2Int, int> InteractableTargetIndexMap = new();//物体在List中的id

    private List<Transform> Outline;

    private List<GameObject> LevitatingPlatforms = new();
    private List<GameObject> BrokenPlatforms = new();
    private List<GameObject> WindAreas = new();
    private List<Animator> Trampolines = new();
    private List<GameObject> Spikes = new();
    private List<GameObject> CheckPoints = new();
    private List<GameObject> SelectablePoints = new();

    private List<BoxCollider2D> SolidLandColliders = new();
    private List<BoxCollider2D> LevitatingPlatformColliders = new();
    private List<BoxCollider2D> BrokenPlatformColliders = new();

    private void OnDrawGizmos()
    {
        var size = landscapeSize;
        Gizmos.color = Color.red;
        foreach (var t in Tool.SceneController.FlattenTargets.Values)
        {
            //if (!t.UpdateLocally) continue;
            CalculateTargetOverlapArea(t, out var p1, out var p2);
            if (p1.x < 0 || p1.y < 0 || p2.x >= size.x || p2.y >= size.y)
            {
                p1 = new(Mathf.Max(0, p1.x), Mathf.Max(0, p1.y));
                p2 = new(Mathf.Min(size.x - 1, p2.x), Mathf.Min(size.y - 1, p2.y));
            }
            Gizmos.DrawWireCube((Vector2)(p1 + p2) * 0.5f,(Vector2)(p2 - p1));
        }
        Gizmos.color = Color.cyan;
        foreach (var platform in info.levitatingPlatforms)
        {
            int x = platform.leftX;
            int y = platform.leftY;
            int width = platform.width;
            Vector2 center = new Vector2(x + width * 0.5f, y + 0.5f);
            Vector2 areaSize = new Vector2(width, 1);
            Gizmos.DrawWireCube(center, areaSize);
        }

        // 4. 绘制破损平台（橙色）
        Gizmos.color = new Color(1f, 0.6f, 0);
        foreach (var brokenPlatform in info.brokenPlatforms)
        {
            int x = brokenPlatform.leftX;
            int y = brokenPlatform.leftY;
            int width = brokenPlatform.width;
            Vector2 center = new Vector2(x + width * 0.5f, y + 0.5f);
            Vector2 areaSize = new Vector2(width, 1);
            Gizmos.DrawWireCube(center, areaSize);
        }

        // 5. 绘制风场（蓝色）
        Gizmos.color = Color.blue;
        foreach (var wind in info.windAreas)
        {
            StandardizationArea(wind.point1X, wind.point2X, wind.point1Y, wind.point2Y,
                out int xmin, out int xmax, out int ymin, out int ymax);
            Vector2 center = new Vector2(xmin + xmax + 1, ymin + ymax + 1) * 0.5f;
            Vector2 areaSize = new Vector2(xmax - xmin + 1, ymax - ymin + 1);
            Gizmos.DrawWireCube(center, areaSize);
        }

        // 6. 绘制跳台（黄色）
        Gizmos.color = Color.yellow;
        foreach (var trampoline in info.trampolines)
        {
            int x = trampoline.leftX;
            int y = trampoline.leftY;
            int width = trampoline.width;
            Vector2 center = new Vector2(x + width * 0.5f, y + 0.5f);
            Vector2 areaSize = new Vector2(width, 1);
            Gizmos.DrawWireCube(center, areaSize);
        }

        // 7. 绘制地刺（红色/品红）
        Gizmos.color = Color.magenta;
        foreach (var spike in info.spikes)
        {
            StandardizationArea(spike.point1X, spike.point2X, spike.point1Y, spike.point2Y,
                out int xmin, out int xmax, out int ymin, out int ymax);
            Vector2 center = new Vector2(xmin + xmax + 1, ymin + ymax + 1) * 0.5f;
            Vector2 areaSize = new Vector2(xmax - xmin + 1, ymax - ymin + 1);
            Gizmos.DrawWireCube(center, areaSize);
        }

        Gizmos.color = Color.white;
        foreach(var checkPoint in info.checkPoints)
        {
            Gizmos.DrawWireCube(new Vector3(checkPoint.x+0.5f, checkPoint.y+0.5f, 0), Vector3.one);
        }

        Gizmos.color = Color.gray;
        foreach (var selectablePoint in info.selectablePoints)
        {
            Gizmos.DrawWireCube(new Vector3(selectablePoint.x + 0.5f, selectablePoint.y + 0.5f, 0), Vector3.one);
        }
    }
    public void Init(LandscapeInfo info)
    {
        this.info = info;

        if (TileRoot == null)
        {
            TileRoot = new GameObject("TileRoot").transform;
            foreach(var i in Tool.PrefabManager.Tiles)
            {
                var obj = i;
                TilePools.Add(new(() =>
                {
                    return Instantiate(obj, TileRoot).transform;
                },t=>t.gameObject.SetActive(true)
                ,t=>t.gameObject.SetActive(false)
                ));
            }
        }
        if (Traps == null) Traps = new GameObject("Traps").transform;
        if (Colliders == null) Colliders = new GameObject("Colliders").transform;

        DrawLandscape();
    }
    private void DrawLandscape()
    {
        int xsize = landscapeSize.x;
        int ysize = landscapeSize.y;
        map= new byte[ysize][];
        for (int index = 0; index < map.Length; index++) map[index] = new byte[xsize];

        FillMapInfo();

        bool isOuterEdgeFull = OuterEdgeFull();
        if (isOuterEdgeFull) PaintOutline();

        for (int xpos = 0; xpos < xsize; xpos++)
        {
            for (int ypos = 0; ypos < ysize; ypos++)
            {
                if (map[ypos][xpos] == 0x00) continue;
                if (map[ypos][xpos]==0x01)PaintSolidLandTile(xpos, ypos,!isOuterEdgeFull);
                else if (map[ypos][xpos] == 0x02)
                {
                    var obj = Tool.PrefabManager.LevitatingPlatformPool.Get();
                    obj.transform.SetParent(Traps);
                    LevitatingPlatforms.Add(obj);
                    obj.transform.position = new Vector3(xpos, ypos);
                }
                else if (map[ypos][xpos] == 0x03)
                {
                    var obj = Tool.PrefabManager.BrokenPlatformPool.Get();
                    obj.transform.SetParent(Traps);
                    BrokenPlatforms.Add(obj);
                    obj.transform.position = new Vector3(xpos, ypos);
                    InteractableTargetIndexMap.Add(new(xpos, ypos), BrokenPlatforms.Count - 1);
                }/*//风整块渲染
                else if (map[ypos][xpos] == 0x04)
                {
                    var obj = Tool.PrefabManager.WindPool.Get();
                    WindAreas.Add(obj);
                    obj.transform.position = new Vector3(xpos + 0.5f, ypos + 0.5f);
                    InteractableTargetIndexMap.Add(new(xpos, ypos), WindAreas.Count - 1);
                }*/
                else if (map[ypos][xpos] == 0x05)
                {
                    var obj = Tool.PrefabManager.TrampolinePool.Get();
                    obj.transform.SetParent(Traps);
                    Trampolines.Add(obj.GetComponent<Animator>());
                    obj.transform.position = new Vector3(xpos, ypos);
                    InteractableTargetIndexMap.Add(new(xpos, ypos), Trampolines.Count - 1);
                }
                else if (map[ypos][xpos] == 0x06)
                {
                    var obj = Tool.PrefabManager.SpikePool.Get();
                    obj.transform.SetParent(Traps);
                    Spikes.Add(obj);
                    obj.transform.position = new Vector3(xpos, ypos);
                    InteractableTargetIndexMap.Add(new(xpos, ypos), Spikes.Count - 1);
                }
                else if (map[ypos][xpos] == 0x07)
                {
                    var obj = Tool.PrefabManager.CheckPointPool.Get();
                    obj.transform.SetParent(Traps);
                    CheckPoints.Add(obj);
                    obj.transform.position = new Vector3(xpos, ypos);
                    InteractableTargetIndexMap.Add(new(xpos, ypos), CheckPoints.Count - 1);
                }
                else if (map[ypos][xpos] == 0x08)
                {
                    var obj = Tool.PrefabManager.SelectablePointPool.Get();
                    obj.transform.SetParent(Traps);
                    SelectablePoints.Add(obj);
                    obj.transform.position = new Vector3(xpos, ypos);
                    InteractableTargetIndexMap.Add(new(xpos, ypos), SelectablePoints.Count - 1);
                }
            }
        }
        foreach(var i in info.windAreas)
        {
            var obj = Tool.PrefabManager.WindPool.Get();
            obj.transform.SetParent(Traps);
            WindAreas.Add(obj);
            obj.transform.position = new Vector3((i.point1X + i.point2X) * 0.5f + 0.5f, (i.point1Y + i.point2Y) * 0.5f + 0.5f);
            obj.transform.localScale = new Vector3(i.point2X - i.point1X + 1, i.point2Y - i.point1Y + 1,1);
            for(var x = i.point1X; x <= i.point2X; x++)
            {
                for(var y = i.point1Y; y <= i.point2Y; y++)
                {
                    InteractableTargetIndexMap.Add(new(x,y), WindAreas.Count - 1);
                }
            }
        }


        foreach (var i in info.solidLands)
        {
            var collider=Tool.PrefabManager.TileColliderPool.Get().GetComponent<BoxCollider2D>();
            collider.transform.SetParent(Colliders);
            collider.transform.position = new Vector3((i.point1X + i.point2X) * 0.5f+0.5f, (i.point1Y + i.point2Y) * 0.5f+0.5f);
            collider.size = new Vector2(Mathf.Abs(i.point1X - i.point2X) + 1, Mathf.Abs(i.point1Y - i.point2Y) + 1);
            SolidLandColliders.Add(collider);
        }
        foreach (var i in info.levitatingPlatforms)
        {
            var collider = Tool.PrefabManager.PlatformColliderPool.Get().GetComponent<BoxCollider2D>();
            collider.transform.SetParent(Colliders);
            collider.transform.position = new Vector3(i.leftX+i.width * 0.5f, i.leftY+ 0.7f);
            collider.size = new Vector2(i.width, collider.size.y);
            LevitatingPlatformColliders.Add(collider);
        }
        foreach (var i in info.brokenPlatforms)
        {
            var collider = Tool.PrefabManager.PlatformColliderPool.Get().GetComponent<BoxCollider2D>();
            collider.transform.SetParent(Colliders);
            collider.transform.position = new Vector3(i.leftX + i.width * 0.5f, i.leftY + 0.7f);
            collider.size = new Vector2(i.width, collider.size.y);
            BrokenPlatformColliders.Add(collider);
        }
    }
    private void FillMapInfo()
    {
        int count = 0;
        foreach (var i in info.solidLands)
        {
            StandardizationArea(i.point1X, i.point2X, i.point1Y, i.point2Y, out var xmin, out var xmax, out var ymin, out var ymax);
            for (int xpos = xmin; xpos <= xmax; xpos++)
            {
                for (int ypos = ymin; ypos <= ymax; ypos++)
                {
                    map[ypos][xpos] = 0x01;
                }
            }
            count++;
        }
        count = 0;
        foreach (var i in info.levitatingPlatforms)
        {
            for (int x = i.leftX; x < i.leftX + i.width; x++)
            {
                map[i.leftY][x] = 0x02;
            }
            count++;
        }
        count = 0;
        foreach (var i in info.brokenPlatforms)
        {
            for (int x = i.leftX; x < i.leftX + i.width; x++)
            {
                map[i.leftY][x] = 0x03;
                InteractableTargetInfoMap.Add(new(x, i.leftY), count);
            }
            count++;
        }
        count = 0;
        foreach (var i in info.windAreas)
        {
            StandardizationArea(i.point1X, i.point2X, i.point1Y, i.point2Y, out var xmin, out var xmax, out var ymin, out var ymax);
            for (int xpos = xmin; xpos <= xmax; xpos++)
            {
                for (int ypos = ymin; ypos <= ymax; ypos++)
                {
                    map[ypos][xpos] = 0x04;
                    InteractableTargetInfoMap.Add(new(xpos, ypos), count);
                }
            }
            count++;
        }
        count = 0;
        foreach (var i in info.trampolines)
        {
            for (int x = i.leftX; x < i.leftX + i.width; x++)
            {
                map[i.leftY][x] = 0x05;
                InteractableTargetInfoMap.Add(new(x, i.leftY), count);
            }
            count++;
        }
        count = 0;
        foreach (var i in info.spikes)
        {
            StandardizationArea(i.point1X, i.point2X, i.point1Y, i.point2Y, out var xmin, out var xmax, out var ymin, out var ymax);
            for (int xpos = xmin; xpos <= xmax; xpos++)
            {
                for (int ypos = ymin; ypos <= ymax; ypos++)
                {
                    if (ypos > 0 && map[ypos - 1][xpos] == 0x01 && map[ypos][xpos] == 0x00)
                    {
                        map[ypos][xpos] = 0x06;
                        InteractableTargetInfoMap.Add(new(xpos, ypos), count);
                    }
                }
            }
            count++;
        }
        count = 0;
        foreach (var i in info.checkPoints)
        {
            map[i.y][i.x] = 0x07;
            InteractableTargetInfoMap.Add(new(i.x,i.y), count);
            count++;
        }
        count = 0;
        foreach (var i in info.selectablePoints)
        {
            map[i.y][i.x] = 0x08;
            InteractableTargetInfoMap.Add(new(i.x, i.y), count);
            count++;
        }
    }

    private bool OuterEdgeFull()
    {
        int ysize = map.Length;
        int xsize = map[0].Length;
        bool isOuterEdgeFull = true;

        // 1. 检查 第一行所有 x
        for (int x = 0; x < xsize; x++)
        {
            if (map[0][x]!=0x01)
            {
                isOuterEdgeFull = false;
                break;
            }
        }

        // 2. 检查 最后一行所有 x（只有上一步没问题才继续检查）
        if (isOuterEdgeFull)
        {
            for (int x = 0; x < xsize; x++)
            {
                if (map[ysize - 1][x] != 0x01)
                {
                    isOuterEdgeFull = false;
                    break;
                }
            }
        }

        // 3. 检查 第一列所有 y（排除已经检查过的第一行和最后一行）
        if (isOuterEdgeFull)
        {
            for (int y = 1; y < ysize - 1; y++)
            {
                if (map[y][0] != 0x01)
                {
                    isOuterEdgeFull = false;
                    break;
                }
            }
        }

        // 4. 检查 最后一列所有 y（排除已经检查过的第一行和最后一行）
        if (isOuterEdgeFull)
        {
            for (int y = 1; y < ysize - 1; y++)
            {
                if (map[y][xsize - 1] != 0x01)
                {
                    isOuterEdgeFull = false;
                    break;
                }
            }
        }
        return isOuterEdgeFull;
    }
    private void PaintOutline()
    {
        int ysize = map.Length;
        int xsize = map[0].Length;

        const float thickness = 100f;
        Outline = new();
        float botton = -thickness;
        float top= ysize + thickness;
        float left = -thickness;
        float right = xsize + thickness;
        GetOutlineThickness(out var left1,out var right1, out var top1, out var bottom1);
        left1 -= 1;
        right1 += 1;
        top1 += 1;
        bottom1 -= 1;
        FillSolidLandTile(left, left1+0.5f, top, botton);
        FillSolidLandTile(right1+0.5f, right, top, botton);
        FillSolidLandTile(left, right, bottom1+0.5f, botton);
        FillSolidLandTile(left, right, top, top1+0.5f);
    }
    private void GetOutlineThickness(out int left,out int right,out int up,out int bottom)
    {
        int xsize = landscapeSize.x;
        int ysize = landscapeSize.y;
        
        left = 1;
        right = 1;
        up = 1;
        bottom = 1;
        while (left < xsize && SolidColumn(left)) left++;
        int currentRight = xsize - 1-right;
        while (currentRight >= 0 && SolidColumn(currentRight))
        {
            right++;
            currentRight--;
        }
        right = currentRight;
        while (bottom < ysize && SolidRow(bottom)) bottom++;
        int currentUp = ysize - 1-up;
        while (currentUp >= 0 && SolidRow(currentUp))
        {
            up++;
            currentUp--;
        }
        up = currentUp;
    }
    private bool SolidColumn(int x)
    {
        // 安全判断
        if (x < 0 || x >= map[0].Length) return false;

        for (int y = 0; y < map.Length; y++)
        {
            if (map[y][x] != 0x01)
                return false;
        }
        return true;
    }
    private bool SolidRow(int y)
    {
        // 安全判断
        if (y < 0 || y >= map.Length) return false;

        for (int x = 0; x < map[0].Length; x++)
        {
            if (map[y][x] != 0x01)
                return false;
        }
        return true;
    }

    private void FillSolidLandTile(float left, float right, float top, float bottom)
    {
        var t = Instantiate(Tool.PrefabManager.Tiles[15], transform).transform;
        Outline.Add(t);
        t.localScale = new Vector3(right-left, top-bottom,1);
        t.position = new Vector3((left+right)*0.5f,(top+bottom)*0.5f,-Outline.Count-1);
    }
    private void PaintSolidLandTile(int x,int y,bool outsideEmpty=true)
    {
        bool up = true;//有方块    1
        bool left = true;//有方块  2
        bool down = true;//有方块  4
        bool right = true;//有方块 8

        if (y > 0 && map[y - 1][x]!=0x01) down = false;
        if (y < map.Length - 1 && map[y + 1][x]!=0x01) up = false;
        if (x > 0 && map[y][x - 1]!=0x01) left = false;
        if (x < map[0].Length - 1 && map[y][x + 1] != 0x01) right = false;
        if (outsideEmpty)
        {
            if (x == 0) left = false;
            if (x == map[y].Length-1) right = false;
            if (y == 0) down = false;
            if (y == map.Length - 1) up = false;
        }
        int index = (up ? 1 : 0) + (left ? 2 : 0) + (down ? 4 : 0) + (right ? 8 : 0);

        var obj = TilePools[index].Get();
        obj.position = new Vector3(x + 0.5f, y + 0.5f);
    }


    private float nextSpikeDamageTime;
    private void FixedUpdate()
    {
        bool spikeDamage = Time.time > nextSpikeDamageTime;
        if (spikeDamage) nextSpikeDamageTime = Time.time + 0.5f;

        var size = landscapeSize;
        foreach (var t in Tool.SceneController.FlattenTargets.Values)
        {
            //if (!t.UpdateLocally) continue;
            CalculateTargetOverlapArea(t, out var p1, out var p2);
            if (p1.x < 0 || p1.y < 0 || p2.x >= size.x || p2.y >= size.y)
            {
                p1 = new(Mathf.Max(0, p1.x), Mathf.Max(0, p1.y));
                p2 = new(Mathf.Min(size.x-1, p2.x), Mathf.Min(size.y-1, p2.y));
            }
            p2 = new(p2.x - 1, p2.y - 1);
            //破损平台
            if (p1.y != 0&&t.rb.velocity.y<-10)
            {
                for (int x = p1.x; x <= p2.x; x++)
                {
                    int y = p1.y;
                    if (map[y][x] == 0x03)
                    {
                        BreakPlatformAtPos(x, y);
                        break;
                    }
                }
            }
            //跳台
            if (t.rb.velocity.y < -0.1f)
            {
                for (int x = p1.x; x <= p2.x; x++)
                {
                    int y = p1.y;
                    if (map[y][x] == 0x05)
                    {
                        TrampolineJump(t,x, y);
                        break;
                    }
                }
            }
            //风区
            for (int x = p1.x; x <= p2.x; x++)
            {
                for (int y = p1.y; y <= p2.y; y++)
                {
                    if (map[y][x] == 0x04)
                    {
                        ReachWindArea(t, x, y);
                    }
                }
            }
            //尖刺
            if (spikeDamage)
            {
                for (int x = p1.x; x <= p2.x; x++)
                {
                    for (int y = p1.y; y <= p2.y; y++)
                    {
                        if (map[y][x] == 0x06)
                        {
                            TouchSpike(t, x, y);
                        }
                    }
                }
            }
            //检查点
            if (EnsInstance.HasAuthority)
            {
                for (int x = p1.x; x <= p2.x; x++)
                {
                    for (int y = p1.y; y <= p2.y; y++)
                    {
                        if (map[y][x] == 0x07)
                        {
                            InCheckPointAreaBuffer.Add((t.ObjectId, info.checkPoints[InteractableTargetInfoMap[new(x, y)]].id));
                        }
                    }
                }
            }
        }
        //选择点
        if (SelectablePoints.Count > 0)
        {
            var main = Camera.main;
            var buttons = PlayModeController.Instance.ClickablePointActiveFor(SelectablePoints.Count, SelectablePointClicked);
            for (int i = 0; i < SelectablePoints.Count; i++)
            {
                GameObject p = SelectablePoints[i];
                var pos = main.WorldToScreenPoint(p.transform.position+Vector3.up*1.5f);
                buttons[i].transform.position = pos;
            }
        }
        if(EnsInstance.HasAuthority)InCheckPointPostCheck();

        if (RestructBrokenPlatformOfId.Count > 0)
        {
            int time = -1;
            foreach(var i in RestructBrokenPlatformOfId)
            {
                if (Time.time*1000 > i.Key)
                {
                    time = i.Key;
                    break;
                }
            }
            if (time > 0)
            {
                var pos = RestructBrokenPlatformOfId[time];
                var infoIndex = InteractableTargetInfoMap[pos];
                var bpinfo = info.brokenPlatforms[infoIndex];
                for (int xpos = bpinfo.leftX; xpos < bpinfo.leftX + bpinfo.width; xpos++)
                {
                    BrokenPlatforms[InteractableTargetIndexMap[new(xpos, bpinfo.leftY)]].gameObject.SetActive(true);
                }
                BrokenPlatformColliders[infoIndex].gameObject.SetActive(true);
                RestructBrokenPlatformOfId.Remove(time);
            }
        }
    }
    private static Dictionary<int,Vector2Int> RestructBrokenPlatformOfId=new();
    private static HashSet<(int,int)> InCheckPointArea=new();
    private static HashSet<(int, int)> InCheckPointAreaBuffer = new();
    private void CalculateTargetOverlapArea(Target t,out Vector2Int point1,out Vector2Int point2)
    {
        var c = t.graphic.boxCollider;
        float xsize = c.size.x * Mathf.Abs(c.transform.localScale.x) * 0.5f;// +c.edgeRadius;
        float ysize = c.size.y * c.transform.localScale.y * 0.5f;// +c.edgeRadius;
        point1 = new((int)(t.transform.position.x-xsize),(int)(t.transform.position.y-ysize));
        point2 = new((int)(t.transform.position.x + xsize+1), (int)(t.transform.position.y + ysize)+1);
    }
    private void BreakPlatformAtPos(int x, int y)
    {
        var pos = new Vector2Int(x, y);
        var infoIndex = InteractableTargetInfoMap[pos];
        var key = (int)(Time.time + 3f) * 1000;
        while(RestructBrokenPlatformOfId.ContainsKey(key)) key++;
        RestructBrokenPlatformOfId.Add(key, pos);
        var bpinfo = info.brokenPlatforms[infoIndex];
        for (int xpos = bpinfo.leftX; xpos < bpinfo.leftX + bpinfo.width; xpos++)
        {
            BrokenPlatforms[InteractableTargetIndexMap[new(xpos, bpinfo.leftY)]].gameObject.SetActive(false);
        }
        BrokenPlatformColliders[infoIndex].gameObject.SetActive(false);
    }
    private void TrampolineJump(Target t, int x,int y)
    {
        var pos = new Vector2Int(x, y);
        if (t.controller != null && t.UpdateLocally) t.controller.SetResistance(0.01f, false);
        Trampolines[InteractableTargetIndexMap[pos]].SetTrigger("Anim");//跳板动画
        t.rb.velocity = new(t.rb.velocity.x, info.trampolines[InteractableTargetInfoMap[pos]].velocity);
    }
    private void ReachWindArea(Target t,int x, int y)
    {
        var pos = new Vector2Int(x, y);
        t.rb.velocity = new(t.rb.velocity.x, info.windAreas[InteractableTargetInfoMap[pos]].velocity);
    }
    private void TouchSpike(Target t, int x, int y)
    {
        var pos = new Vector2Int(x, y);
        t.rb.velocity = Vector2.zero;
        if (t.UpdateLocally)
        {
            t.Damaged(null, info.spikes[InteractableTargetInfoMap[pos]].damage);
            t.controller.SetResistance(-0.1f, false);
        }
    }
    private void InCheckPointPostCheck()
    {
        foreach(var i in InCheckPointAreaBuffer)
        {
            if (!InCheckPointArea.Contains(i))
            {
                CustomLevel.EnterCheckPoint(i.Item1, i.Item2);
            }
        }

        InCheckPointArea.Clear();
        foreach (var i in InCheckPointAreaBuffer) InCheckPointArea.Add(i);
        InCheckPointAreaBuffer.Clear();
    }
    private void SelectablePointClicked(int x)
    {
        Tool.NetworkCorrespondent.SelectablePointClickedRpc(info.selectablePoints[x].id);
    }

    public Vector3 GetPos(float x,float y)
    {
        var size = landscapeSize;
        if(x>=0&&x<1)x *= size.x;
        if(y>=0&&y<1)y *= size.y;
        return new Vector3(x,y);
    }
    public void StandardizationArea(int x1,int x2,int y1,int y2,out int xmin,out int xmax,out int ymin,out int ymax)
    {
        xmin = Mathf.Max(Mathf.Min(x1, x2), 0);
        xmax = Mathf.Min(Mathf.Max(x1, x2), map[0].Length - 1);
        ymin = Mathf.Max(Mathf.Min(y1, y2), 0);
        ymax = Mathf.Min(Mathf.Max(y1, y2), map.Length - 1);
    }

    private void OnDestroy()
    {
        PlayModeController.Instance.ClickablePointActiveFor(0, SelectablePointClicked);
        RestructBrokenPlatformOfId.Clear();

        foreach (var i in LevitatingPlatforms)if(i) Tool.PrefabManager.LevitatingPlatformPool.Release(i);
        foreach (var i in BrokenPlatforms) if (i) Tool.PrefabManager.BrokenPlatformPool.Release(i);
        foreach (var i in WindAreas) if (i) Tool.PrefabManager.WindPool.Release(i);
        foreach (var i in Trampolines) if (i) Tool.PrefabManager.TrampolinePool.Release(i.gameObject);
        foreach (var i in Spikes) if (i) Tool.PrefabManager.SpikePool.Release(i);

        foreach (var i in SolidLandColliders) if (i) Tool.PrefabManager.TileColliderPool.Release(i.gameObject);
        foreach (var i in LevitatingPlatformColliders) if (i) Tool.PrefabManager.TileColliderPool.Release(i.gameObject);
        foreach (var i in BrokenPlatformColliders) if (i) Tool.PrefabManager.TileColliderPool.Release(i.gameObject);
    }
}
