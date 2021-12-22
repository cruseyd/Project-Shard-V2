using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform,
        Row,
        Column,
        FixedRow,
        FixedColumn
    }

    public FitType fitType;
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;
    public bool fitX;
    public bool fitY;


    public override void CalculateLayoutInputVertical()
    {

        float sqrRt = Mathf.Sqrt(transform.childCount);

        switch (fitType)
        {
            case FitType.Uniform:
                fitX = true; fitY = true;
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(sqrRt);
                break;
            case FitType.Row:
                fitX = true; fitY = true;
                columns = Mathf.CeilToInt(sqrRt);
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                
                break;
            case FitType.Column:
                fitX = true; fitY = true;
                rows = Mathf.CeilToInt(sqrRt);
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
            case FitType.FixedRow:
                fitX = false; fitY = true;
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
                break;
            case FitType.FixedColumn:
                fitX = true; fitY = false;
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
                break;
        }

        rows = Mathf.Max(rows, 1);
        columns = Mathf.Max(columns, 1);

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        float spacingModX = spacing.x * (columns - 1) / ((float)columns);
        float spacingModY = spacing.y * (rows - 1) / ((float)rows);

        float cellWidth = parentWidth / (float)columns - spacingModX  - (padding.left / (float)columns) - (padding.right / (float)columns);
        float cellHeight = parentHeight / (float)rows  - spacingModY  - (padding.top / (float)rows) - (padding.bottom / (float)rows);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        if (fitType == FitType.FixedRow)
        {
            rectTransform.sizeDelta = new Vector2(columns * cellSize.x + (columns - 1) * spacing.x + padding.left + padding.right, parentHeight);
        }
        if (fitType == FitType.FixedColumn)
        {
            rectTransform.sizeDelta = new Vector2(parentWidth, rows * cellSize.y);
        }

        int colCount = 0;
        int rowCount = 0;

        for (int ii = 0; ii < rectChildren.Count; ii++)
        {
            rowCount = ii / columns;
            colCount = ii % columns;

            var item = rectChildren[ii];
            var xPos = (cellSize.x * colCount) + (spacing.x * colCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }
    }
    public override void SetLayoutVertical()
    {
    }
    public override void SetLayoutHorizontal()
    {
    }
}
