using LevelCreator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : SingleLevel
{
    private LandscapeInfo info;
    private Vector2Int landscapeSize => new Vector2Int(16 * info.sizeX, 8 * info.sizeY);


    public Transform TileRoot;
    public Transform LevitatingPlatformRoot;
    public Transform VelocityAreaRoot;
    public Transform ColliderRoot;

    public void Init(LandscapeInfo info)
    {
        this.info = info;
        DrawLandscape();
    }
    private void DrawLandscape()
    {
        int xsize = landscapeSize.x;
        int ysize = landscapeSize.y;
        bool[][] map= new bool[ysize][];
        for (int index = 0; index < map.Length; index++) map[index] = new bool[xsize];
        foreach(var i in info.solidLands)
        {
            int xmin = Mathf.Max(Mathf.Min(i.point1X, i.point2X), 0);
            int xmax = Mathf.Min(Mathf.Max(i.point1X, i.point2X), xsize-1);
            int ymin = Mathf.Max(Mathf.Min(i.point1Y, i.point2Y), 0);
            int ymax = Mathf.Min(Mathf.Max(i.point1Y, i.point2Y), ysize-1);
            for (int xpos=xmin;xpos<=xmax; xpos++)
            {
                for(int ypos=ymin; ypos<=ymax; ypos++)
                {
                    map[ypos][xpos] = true;
                }
            }
        }
        for (int xpos = 0; xpos < xsize; xpos++)
        {
            for (int ypos = 0; ypos < ysize; ypos++)
            {
                if (map[ypos][xpos])PaintTile(map,xpos, ypos);
            }
        }
        foreach(var i in info.levitatingPlatforms)
        {
            Instantiate(Tool.PrefabManager.LevitatingPlatform[i.width - 1], new Vector3(i.centerX, i.centerY), Quaternion.identity, LevitatingPlatformRoot);
        }
        foreach(var i in info.velocityAreas)
        {
            var area=Instantiate(Tool.PrefabManager.VelocityInArea.gameObject, VelocityAreaRoot).GetComponent<VelocityInArea>();
            area.SetInfo(new Vector2(Mathf.Abs(i.point1X - i.point2X) + 1,Mathf.Abs(i.point1Y-i.point2Y)+1),new Vector2(i.velocityX,i.velocityY));
        }
        foreach (var i in info.solidLands)
        {
            var collider=Instantiate(Tool.PrefabManager.TileCollider.gameObject, ColliderRoot).GetComponent<BoxCollider2D>();
            collider.transform.position = new Vector3((i.point1X + i.point2X) * 0.5f+0.5f, (i.point1Y + i.point2Y) * 0.5f+0.5f);
            collider.size = new Vector2(Mathf.Abs(i.point1X - i.point2X) + 1, Mathf.Abs(i.point1Y - i.point2Y) + 1);
        }
    }
    private void PaintTile(bool[][] map,int x,int y)
    {
        bool up = true;//有方块    1
        bool left = true;//有方块  2
        bool down = true;//有方块  4
        bool right = true;//有方块 8
        if (y > 0 && !map[y - 1][x]) down = false;
        if (y < map.Length - 1 && !map[y + 1][x]) up = false;
        if (x > 0 && !map[y][x - 1]) left = false;
        if(x < map.Length - 1 && !map[y][x+1])right= false;
        int index = (up ? 1 : 0) + (left ? 2 : 0) + (down ? 4 : 0) + (right ? 8 : 0);
        Instantiate(Tool.PrefabManager.Tiles[index], new Vector3(x+0.5f,y+0.5f),Quaternion.identity,TileRoot);
    }

    public Vector3 GetPos(float x,float y)
    {
        var size = landscapeSize;
        if(x>=0&&x<=1)x *= size.x;
        if(y>=0&&y<=1)y *= size.y;
        return new Vector3(x,y);
    }
}
