using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EightQueen : MonoBehaviour
{
    private const int queenCount = 8;//皇后数量

    public GridLayoutGroup grid;//按钮网格
    public Button btnPrefab;//按钮预制体模板
    public TextMeshProUGUI txtCurrentPage;//展示当前是第几页
    
    private Button[,] GridBtns= new Button[queenCount, queenCount];
    private int currentShowPage = 0;//当前正在展示的答案
    private List<int[]> solutions;

    /// <summary>
    /// 皇后求解入口函数
    /// </summary>
    /// <param name="queenCount"></param>
    /// <returns></returns>
    public List<int[]> GetSolutions(int queenCount)
    {
        solutions = new List<int[]>();
        
        List<int> queenList = new List<int>();
        for (int i = 0; i < queenCount; i++)
        {
            queenList.Add(0);
        }
        
        //回溯求解
        PutQueen(queenCount,queenList,0);
        
        //打印所有解决方案
        PrintSolutions(solutions);
        
        return solutions;
    }

    public void OnClickPrevPage()
    {
        currentShowPage=Mathf.Max(0,--currentShowPage);
        ShowSolutionByIndex(currentShowPage);
    }

    public void OnClickNextPage()
    {
        currentShowPage=Mathf.Min(solutions.Count -1,++currentShowPage);
        ShowSolutionByIndex(currentShowPage);
    }
    
    private void Start()
    {
        //界面初始化
        InitGridButtons();

        //皇后求解
        GetSolutions(queenCount);

        //可视化答案
        ShowSolutionByIndex(currentShowPage);
    }

    /// <summary>
    /// 初始化网格按钮
    /// </summary>
    private void InitGridButtons()
    {
        for (int i = 0; i < queenCount; i++)
        {
            for (int j = 0; j < queenCount; j++)
            {
                Button btn = Instantiate(btnPrefab, grid.transform);
                GridBtns[i, j] = btn;
            }
        }
        
        //隐藏原始预制体   
        btnPrefab.gameObject.SetActive(false);
    }

    /// <summary>
    /// 递归放置皇后 求解
    /// </summary>
    /// <param name="queenCount"></param>
    /// <param name="queenList"></param>
    /// <param name="nextY"></param>
    private void PutQueen(int queenCount, List<int> queenList, int nextY)
    {
        //本循环只解决位置Y放什么数字，不会和前面冲突
        for (queenList[nextY] = 0; queenList[nextY] < queenCount; queenList[nextY]++)   
        {
            //检查本位置的皇后是否合法，没有冲突则需要判定是继续递归找位置还是返回答案
            //如果本位置有冲突，不合法，则直接跳过该位置，继续往后
            if (CheckConflict(queenList,nextY) == false)
            {
                //是否还有下一个Y位置需要求解
                if (nextY + 1 < queenCount)
                {
                    Debug.Log(nextY+","+queenList[nextY]);
                    //此递归会解决Y位置的递归调用问题
                    PutQueen(queenCount, queenList, nextY+1);
                }
                else
                {
                    solutions.Add(queenList.ToArray());
                }
            }
        }
    }
    
    

    /// <summary>
    /// 检查当前的皇后位置是否合法 
    /// </summary>
    /// <param name="queenList"></param>
    /// <param name="nextY"></param>
    /// <returns></returns>
    private bool CheckConflict(List<int> queenList, int nextY)
    {
        //nextY = 0; queenList[0] = 0;
        //nextY = 1; queenList[1] = 0;
        //nextY = 2; queenList[2] = 0;
        for (int positionY = 0; positionY < nextY; positionY++)
        {
            if (Mathf.Abs(queenList[positionY] - queenList[nextY]) == Mathf.Abs(positionY-nextY)||queenList[positionY] == queenList[nextY])
            {
                return true;
            }
        }

        return false;
    }

    private void ShowSolutionByIndex(int page)
    {
        if (page<0||page>solutions.Count)
        {
            return;
        }
        
        //显示当前是第几个答案
        txtCurrentPage.text = "Current Page Is:"+page;
        
        //获取该页答案
        int[] solution = solutions[page];
        int queenIndex = 0;
        
        //遍历行列，如果他的位置正好和答案的位置对上了 就显示X表示皇后的位置
        for (int i = 0; i < queenCount; i++)
        {
            for (int j = 0; j < queenCount; j++)
            {
                if (!GridBtns[i,j])
                {
                    continue;
                }
                //因为答案简化到了一维，所以它的下标默认就是行数，而下标里存储的值就是列数
                if (i == queenIndex && j == solution[queenIndex])
                {
                    GridBtns[i, j].GetComponentInChildren<TextMeshProUGUI>().text = "X";
                }
                else
                {
                    GridBtns[i, j].GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            }

            queenIndex++;

        }
    }

    private void PrintSolutions(List<int[]> solutions)
    {
        
    }
}
