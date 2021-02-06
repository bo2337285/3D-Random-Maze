/****************************************************
    文件：Wall.cs
	作者：空银子
    邮箱: 1184840945@qq.com
    日期：#CreateTime#
	功能：墙壁的脚本
*****************************************************/

using UnityEngine;
using System;
using System.Collections.Generic;

public class Wall : MonoBehaviour 
{
    //是否是边界  因为除了出口与入口，边界部分是不允许挖掉的
    public bool isBorder = false;
}