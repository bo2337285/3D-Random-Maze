/****************************************************
    文件：Floor.cs
	作者：空银子
    邮箱: 1184840945@qq.com
    日期：#CreateTime#
	功能：地面元素的脚本  用于记录自身在数组里的位置  以及自己边上的地面元素和墙元素
*****************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

public class Floor : MonoBehaviour 
{
    //不可移动的位置  其实他最准确的意思是：这个格子已经移动过，所以下次不要再移动到这了，因为走回头路了
    //后面这个变量还有个用处，就是标志这个位置是可以走到终点
    public bool CantMove = false;
    public Vector2 index;
    public Floor[] neighborFloors;//此地面周遭四个地面
    public Wall[] neighborWalls;//此地面的四堵墙   我采用的是挖墙法，初始时，迷宫是没路的，每个地面都有四堵墙
    /// <summary>
    /// 找到相邻的 前后左右四块地面
    /// </summary>
    public void GetNeighborFloors()
    {
        //前后左右  0 1 2 3  这种顺序   但是需要注意的是 最前面的一行地面是没有更前面的元素的  反之亦然  左右亦然
        if (index.y + 1 < CreateMap.Instance.cubes.GetLength(1))
        {
            neighborFloors[0] = CreateMap.Instance.cubes[(int)index.x, (int)index.y+1];         
        }
        if (index.y-1 >=0)
        {
            neighborFloors[1] = CreateMap.Instance.cubes[(int)index.x , (int)index.y-1];
        }
        if (index.x -1 >= 0)
        {
            neighborFloors[2] = CreateMap.Instance.cubes[(int)index.x - 1, (int)index.y];
        }
        if (index.x+1<CreateMap.Instance.cubes.GetLength(0))
        {
            neighborFloors[3] = CreateMap.Instance.cubes[(int)index.x + 1, (int)index.y];
        }      
    }
    /// <summary>
    /// 找到前后左右四堵墙
    /// </summary>
    public void GetNeighborWalls()
    {
        //每个地面都是有四堵墙的  这个跟上面那个函数有些不一样 但应该很好理解  横的墙与竖的墙分别对应一个数组，walls1 wanlls2 因为这样好计算 
        neighborWalls[0] = CreateMap.Instance.walls1[(int)index.x, (int)index.y + 1];
        neighborWalls[1] = CreateMap.Instance.walls1[(int)index.x, (int)index.y ];
        neighborWalls[2] = CreateMap.Instance.walls2[(int)index.x, (int)index.y];
        neighborWalls[3] = CreateMap.Instance.walls2[(int)index.x+1, (int)index.y];
    }

    /// <summary>
    /// 以当前位置为起点，随机一条到达终点的路线
    /// </summary>
    /// <param name="x">当前位置的x</param>
    /// <param name="y">当前位置的y</param>
    /// <param name="path">记录路线的数组，每个vector2代表一个位置</param>
    /// <returns></returns>
    public List<Vector2> SetRoad(int x,int y,List<Vector2> path)
    {
    //PS：这里的X Y是要用来从二维数组里取元素的，所以最好还是用两个int 而不是vector2 要不然在取数据时需要先把v2的float类型转成int类型
    //如果需要设置终点位置，那么可以用一个傻办法， 就是随到的位置如果不是终点的位置，就重新随机一遍，直到是为止，因为迷宫只用生产一遍，
    //这种暴力法还是很可行的
        CantMove = true;
        if (index.y >= CreateMap.Instance.Y-1)
        {
            //如果已经到达那头 就结束递归  我的算法是计算一头到另一头的  如果想要一侧到另一侧的话，这个函数要大改一下
            path.Add(index);
            return path;
        }
        path.Add(index);        
        List<Floor> cubes = new List<Floor>();
        foreach (var item in neighborFloors)
        {
            if (item!=null)
            {
                if (item.CantMove == false)
                {
                    cubes.Add(item);
                }
            }           
        }
        //以上代码是：先将当前元素记录在路线里  然后从当前元素的周围元素里找满足移动条件的元素
        if (cubes.Count>0)
        {
            //从满足条件的元素里随机一个 然后递归调用setroad函数
            Floor cube = null;          
            cube = cubes[UnityEngine.Random.Range(0, cubes.Count)];                                                    
            return cube.SetRoad(x,y,path);
        }
        else
        {
            //如果没有满足条件的元素那么就得重新随机一次了 但是要先把CantMove设置回默认值
            foreach (var item in CreateMap.Instance.cubes)
            {
                item.CantMove = false;
            }
            return CreateMap.Instance.cubes[x, y].SetRoad(x,y,new List<Vector2>());
        }
       
    }
}