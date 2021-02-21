/****************************************************
    文件：CreateMap.cs
	作者：空银子
    邮箱: 1184840945@qq.com
    日期：#CreateTime#
	功能：生成地图
*****************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

public class CreateMap : MonoBehaviour 
{
    //单例模式固定的写法
    public static CreateMap Instance;
    private void Awake()
    {
        Instance = this;
    }
    public int level;//难度
    //地图大小
    public int X = 5;
    public int Y = 5;
    //地面 以及墙的预制体  墙的预制体也可以只用一个 生成的时候旋转一下角度就行
    public GameObject groundPrefab;
    public GameObject wallPrefab;
    public GameObject wallPrefab1;
    //三个二维数组
    public Floor[,] cubes;
    public Wall[,] walls1;
    public Wall[,] walls2;

    //起点坐标
    public Vector2 startPoint;
    public bool isTest;//是否是测试阶段
   

    //存储哪些无法连接到正确路径上的地面 到时我们要给这些地面挖掉一堵墙 以便他们连通
    public List<Floor> cantMoves = new List<Floor>();
    //生成迷宫的函数 
    public void Creat()
    {
        //清除原有元素  主要是用于重新生成迷宫时
        Clear();
        //一些数组的初始化 与游戏物体的生成
        Init();
        //生成一个粗略的迷宫
        CreateStep1();
        //优化迷宫
        CreateStep2();
    }
    //清除场景原有元素的函数  想重新生成迷宫时须调用
    public void Clear()
    {
        if (cubes != null)
        {
            foreach (var item in cubes)
            {
                if (item!=null)
                {
                    Destroy(item.gameObject);
                }                
            }
        }
        if (walls1 != null)
        {
            foreach (var item in walls1)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        if (walls2 != null)
        {
            foreach (var item in walls2)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
    }
    public void Init()
    {
        //初始化数组 需要注意一下墙的有个索引是要加一的   因为两个格子是3堵墙 得+1
        cubes = new Floor[X, Y];
        walls1 = new Wall[X, Y + 1];
        walls2 = new Wall[X + 1, Y];
        //两层for循环嵌套自动生成 地面 与 墙 的游戏物体
        CreateGround();
        CreateWall1();
        CreateWall2();
        //初始化一下每个地面元素里的两个数组， 并且找到地面元素相邻的地面 和自己的四堵墙
        foreach (var item in cubes)
        {
            item.neighborFloors = new Floor[4];
            item.neighborWalls = new Wall[4];
            item.GetNeighborFloors();
            item.GetNeighborWalls();
        }
    }
    //生成地面
    public void CreateGround()
    {
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                Floor item = Instantiate(groundPrefab).GetComponent<Floor>();
                item.transform.position = new Vector3(i, 0, j);
                cubes[i, j] = item;
                item.index = new Vector2(i, j);
            }
        }
    }
    //生成横着 还是竖着的墙？？
    public void CreateWall1()
    {
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y + 1; j++)
            {

                Wall item = Instantiate(wallPrefab).GetComponent<Wall>();
                item.transform.position = new Vector3(i, 0.5f, j - 0.5f);
                walls1[i, j] = item;
                if (j == 0 || j == Y)
                {
                    item.isBorder = true;
                }
            }
        }
    }
    ////生成横着 还是竖着的墙？？
    public void CreateWall2()
    {
        for (int i = 0; i < X + 1; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                Wall item = Instantiate(wallPrefab1).GetComponent<Wall>();
                item.transform.position = new Vector3(i - 0.5f, 0.5f, j);
                walls2[i, j] = item;
                if (i == X || i == 0)
                {
                    item.isBorder = true;
                }
            }
        }
    }
    public void CreateStep1()
    {
        //先通过setroad函数 求得一条路线
        List<Vector2> road = cubes[(int)startPoint.x, (int)startPoint.y].
            SetRoad((int)startPoint.x, (int)startPoint.y, new List<Vector2>());
        //测试用的代码， 给路线染下颜色  具体用时最好删掉，要不然有最佳路径了还走毛迷宫
        foreach (var item in road)
        {
            if (isTest )
            {
                cubes[(int)item.x, (int)item.y].gameObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            }
        
        }

        for (int i = 0; i < road.Count - 1; i++)
        {
            //先将我们随机出来的这条路挖通 因为这一步代码比较多，所以写成了 DestoryWall（）这一个方法
            DestoryWall(road[i], road[i + 1]);
            if (i == 0)
            {
                //终点与起点的墙也要挖掉 要不然就没出入口了
                Destroy(cubes[(int)road[i].x, (int)road[i].y].neighborWalls[1].gameObject);
                cubes[(int)road[i].x, (int)road[i].y].neighborWalls[1] = null;
            }
            if (i == road.Count - 2)
            {
                Destroy(cubes[(int)road[i + 1].x, (int)road[i + 1].y].neighborWalls[0].gameObject);
                cubes[(int)road[i + 1].x, (int)road[i + 1].y].neighborWalls[0] = null;
            }
        }
        //给四面还都是墙的地面，随机挖掉一堵，初步防止出现封闭的空间  就是玩家压根也进不去的一些地方
        DigWall();

        //将无法连通到 正确路线上的元素找到  但这里做的其实只是把二维数组转成list集合 筛选的工作后面才做
        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Y; j++)
            {
                cantMoves.Add(cubes[i, j]);
            }
        }
    }
    public void CreateStep2()
    {
        while (cantMoves.Count > 0)
        {
            for (int i = 0; i < 5; i++)
            {
                //筛选那些无法移动到 正确路径上的地面  因为是通过判断周围没墙的格子里有没有是在正路上的，来确定他能否移动到正路上
                //等下看了函数里的代码应该就能明白为什么这里用for循环多执行了几次函数
                cantMoves = SelectCantMove();
            }
            //如果不能移动到正路上的元素>0 就在这些不能移动到正路上的元素里随机一个挖堵墙  当然也可以同时多随机几个出来一起挖
            //但是怕这样挖太多，导致墙太少，迷宫太空旷
            //这里的函数名跟函数里的功能关系不大  别顾名思义了
            Level();
            
        }
    }
    private void Update()
    {
        //主要是测试用   一次性生成迷宫 或者先生成粗略的迷宫 在按A优化
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Clear();
            Init();
            CreateStep1();
        }
       
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateStep2();        
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Creat();
        }
    }

    //判断地面周围有没有元素是在正确的路径上的
    public bool NeighborCanMove(Floor item)
    {
        for (int i = 0; i < 4; i++)
        {
            if (item.neighborFloors[i]!=null && item.neighborFloors[i].CantMove)
            {
                return true;
            } 
        }
        return false;
    }
    /// <summary>
    /// 给四面还都是墙的地面，随机挖掉一堵，初步防止出现封闭的空间  就是玩家压根也进不去的一些地方
    /// </summary>
    public void DigWall()
    {
        //代码虽然很多 但大多是重复的，复制粘贴下就行。因为重构花的功夫可能更久，就干脆不重构了
        foreach (var item in cubes)
        {
            bool isFull = true;
            //找到四堵墙都还在的元素
            for (int i = 0; i < 4; i++)
            {
                if (item.neighborWalls[i] == null)
                {
                    isFull = false;
                }
            }
            if (isFull)
            {
                List<Floor> roadNeighbor = new List<Floor>();
                //我们希望优先凿开离 正确的路近的那一面，这样可以最大概率防止封闭空间的出现
                foreach (var cube in item.neighborFloors)
                {
                    if (cube != null && cube.CantMove == true)
                    {
                        //将邻居里，是正确路线上的找到，然后从这里面随机，当然也可能一个都没有，那时就四个里随机一个
                        roadNeighbor.Add(cube);
                    }
                }
                if (roadNeighbor.Count > 0)
                {
                    Floor cubeItem = roadNeighbor[UnityEngine.Random.Range(0, roadNeighbor.Count)];
                    //将当前元素与随机出来的那个地面元素之间的墙挖掉，因为这一步代码比较多，所以写成了 DestoryWall（）这一个方法
                    DestoryWall(item.index, cubeItem.index);
                }
                else
                {
                    List<Wall> walls = new List<Wall>();
                    //将不是边界的墙找到 然后从里面随机挖一睹
                    foreach (var wall in item.neighborWalls)
                    {
                        if (!wall.isBorder)
                        {
                            walls.Add(wall);
                        }
                    }
                    int dir = UnityEngine.Random.Range(0, walls.Count);
                    for (int i = 0; i < 4; i++)
                    {
                        if (item.neighborWalls[i] != null && item.neighborWalls[i] == walls[dir])
                        {
                            //因为不知道随机出来的墙是上下左右哪一睹，所以要遍历一下
                            dir = i;
                            break;
                        }
                    }
                    switch (dir)
                    {
                        case 0:
                            //删除掉这堵墙， 然后将这堵墙的引用从neighborWalls去掉 因为一睹墙有两个地面在引用，所以另一个地面脚本上也要把引用去掉
                            Destroy(item.neighborWalls[0].gameObject);
                            item.neighborWalls[0] = null;
                            if (item.neighborFloors[0] != null)
                                item.neighborFloors[0].neighborWalls[1] = null;
                            break;
                        case 1:
                            Destroy(item.neighborWalls[1].gameObject);
                            item.neighborWalls[1] = null;
                            if (item.neighborFloors[1] != null)
                                item.neighborFloors[1].neighborWalls[0] = null;
                            break;
                        case 2:
                            Destroy(item.neighborWalls[2].gameObject);
                            item.neighborWalls[2] = null;
                            if (item.neighborFloors[2] != null)
                                item.neighborFloors[2].neighborWalls[3] = null;
                            break;
                        case 3:
                            Destroy(item.neighborWalls[3].gameObject);
                            item.neighborWalls[3] = null;
                            if (item.neighborFloors[3] != null)
                                item.neighborFloors[3].neighborWalls[2] = null;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    //挖掉两个地面元素之间的那堵墙
    public void DestoryWall(Vector2 front,Vector2 back)
    {
        if (Mathf.Abs(front.x - back.x) > 0.1)//判断是左右还是上下
        {
            if (front.x - back.x > 0.1)
            {
                //后一个在前一个的左边  那么就删去前一个的左边的墙
                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[2].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[2] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[3] = null;

            }
            else
            {          
                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[3].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[3] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[2] = null;
            }
        }
        else
        {
            if (front.y - back.y > 0.1)
            {
                //后一个在前一个的上面，那么就删除前一个的上边的墙             
                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[1].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[1] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[0] = null;

            }
            else
            {
              
                Destroy(cubes[(int)front.x, (int)front.y].neighborWalls[0].gameObject);
                cubes[(int)front.x, (int)front.y].neighborWalls[0] = null;
                cubes[(int)back.x, (int)back.y].neighborWalls[1] = null;
            }
        }
    }
    //筛选哪些封闭着的元素  就是无法连接到正路上的元素
    public List<Floor> SelectCantMove()
    {     
        List<Floor> cantMoveCubes = new List<Floor>();
        for (int i = 0; i < cantMoves.Count; i++)
        {
            if (cantMoves[i].CantMove)
            {            
                //旧的集合经过下面的代码，部分元素已经改变了，遇到现在是正路上的元素 直接continue
                continue;
            }
            bool canMove = false;
            for (int j = 0; j < 4; j++)
            {
                //找到没墙的那测，如果对面格子是正路，那么说明当前格子也是在正路上了，因为可以走过去
                if (cantMoves[i].neighborWalls[j]==null&&cantMoves[i].neighborFloors[j]!=null &&cantMoves[i].neighborFloors[j].CantMove)
                {
                    canMove = true;
                    cantMoves[i].CantMove = true;
                    if (isTest)
                    {
                        cantMoves[i].GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
                    }
                   //           
                    break;
                }                    
            }
            if (!canMove)
            {
                //将不能移动到正路上的格子筛选出来 然后重复几轮筛选 挖墙 就能生成一个满意的迷宫了
                cantMoveCubes.Add(cantMoves[i]);
            }
        }
        Debug.Log("cantMoveCubes.Count:" + cantMoveCubes.Count);
        return cantMoveCubes;

    }

    public void Level()
    {
        //简单的容错处理
        if (level<=0)
        {
            level = 1;
        }
        if (level>=50)
        {
            level = 50;
        }

        for (int i = 0; i < level; i++)
        {
            if (cantMoves.Count > 0)
            {
                Floor item = null;
                while (true)
                {
                    //我们希望是与正路元素相邻的元素里挑一个，这样更容易快速挖通全部
                    int index1 = UnityEngine.Random.Range(0, cantMoves.Count);
                    item = cantMoves[index1];
                    //NeighborCanMove（） 判断这个元素的邻居里是否有在正路上的 
                    if (NeighborCanMove(item))
                    {
                        break;
                    }
                };

                List<int> dirs = new List<int>();
                for (int k = 0; k < 4; k++)
                {
                    //找到我们随机到的这个元素的四堵墙里  满足不为空 以及不是边界的 
                    if (item.neighborWalls[k] != null && !item.neighborWalls[k].isBorder)
                    {
                        dirs.Add(k);
                    }
                }
                if (dirs.Count > 0)
                {
                    Debug.Log(55);
                    //然后在这些墙里随机一睹 挖掉 
                    int index = UnityEngine.Random.Range(0, dirs.Count);
                    Destroy(item.neighborWalls[dirs[index]].gameObject);
                    item.neighborWalls[dirs[index]] = null;
                    switch (dirs[index])
                    {
                        case 0:
                            item.neighborFloors[dirs[index]].neighborWalls[1] = null;
                            break;
                        case 1:
                            item.neighborFloors[dirs[index]].neighborWalls[0] = null;
                            break;
                        case 2:
                            item.neighborFloors[dirs[index]].neighborWalls[3] = null;
                            break;
                        case 3:
                            item.neighborFloors[dirs[index]].neighborWalls[2] = null;
                            break;
                        default:
                            break;
                    }

                }
                ////挖完之后  在执行几遍这个函数 染下色，便于观看效果  主要是测试用  正式用时删掉这段
                //for (int j = 0; j < 5; j++)
                //{
                //    cantMoves = SelectCantMove();
                //}
            }
            
            
        }
       
    }

}