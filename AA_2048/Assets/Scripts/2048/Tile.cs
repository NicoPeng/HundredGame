using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [HideInInspector] public TileState state; // 状态 用于处理数字和颜色

    [HideInInspector] public TileCell cell; // 所关联的单元格

    [HideInInspector] public bool locked; //是否被锁定 用于单元格发生变化之后，进行移动效果的展示

    public Image imgBackground; //背景组件
    public TextMeshProUGUI txtNumber; //文字组件

    /// <summary>
    /// 设置状态
    /// </summary>
    /// <param name="state"></param>
    public void SetState(TileState state)
    {
        this.state = state;

        imgBackground.color = state.backgroundColor;
        txtNumber.color = state.textColor;
        txtNumber.text = state.number.ToString();
    }

    /// <summary>
    /// 关联Cell 并设置坐标
    /// </summary>
    /// <param name="cell"></param>
    public void LinkCell(TileCell cell)
    {
        //先解除已有的绑定
        if (this.cell) this.cell.tile = null;

        //关联现在的cell
        this.cell = cell;
        this.cell.tile = this;

        //设置到对应的位置
        transform.position = cell.transform.position;
    }

    /// <summary>
    /// 将Tile移动到某个Cell的位置上
    /// </summary>
    /// <param name="cell"></param>
    public void MoveTo(TileCell cell)
    {
        if (this.cell) this.cell.tile = null;
        this.cell = cell;
        this.cell.tile = this;
        //用协程做一个简单的动画效果
        StartCoroutine(MoveAnimate(cell.transform.position, false));
    }

    /// <summary>
    /// 将Tile合并到某个Cell的位置上
    /// </summary>
    /// <param name="cell"></param>
    public void Merge(TileCell cell)
    {
        if (this.cell) this.cell.tile = null;
        this.cell = null;
        cell.tile.locked = true;

        //和移动的区别是最后一个参数，控制是否是合并过去的
        StartCoroutine(MoveAnimate(cell.transform.position, true));
    }

    /// <summary>
    /// 移动动画
    /// </summary>
    /// <param name="to"></param>
    /// <param name="merging"></param>
    /// <returns></returns>
    private IEnumerator MoveAnimate(Vector3 to, bool merging)
    {
        var elapsed = 0f;
        var duration = 0.1f;
        var from = transform.position;

        //在持续时间内，用插值的方式将位置移动过去
        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging) Destroy(gameObject);
    }
}