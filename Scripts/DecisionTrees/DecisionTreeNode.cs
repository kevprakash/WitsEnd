using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class DecisionTreeNode
{
    public int index;
    private int index2 = -1;
    private int value;
    private bool lessThan;
    private DecisionTreeNode left = null;   // True node
    private DecisionTreeNode right = null;  // False node

    public DecisionTreeNode(int index, int value, bool lessThan)
    {
        this.index = index;
        this.value = value;
        this.lessThan = lessThan;
    }

    public DecisionTreeNode(int index, int index2, int value, bool lessThan)
    {
        this.index = index;
        this.index2 = index2;
        this.value = value;
        this.lessThan = lessThan;
    }

    public bool evaluate(int[] row)
    {
        if (index2 < 0)
        {
            if (lessThan)
            {
                return row[index] <= value;
            }
            else
            {
                return row[index] >= value;
            }
        }
        else
        {
            if (lessThan)
            {
                return row[index] <= row[index2];
            }
            else
            {
                return row[index] >= row[index2];
            }
        }
    }

    public bool traverse(int[] row)
    {
        bool eval = evaluate(row);
        DecisionTreeNode next = getChild(eval);
        if(next is null)
        {
            return eval;
        }
        else
        {
            return next.traverse(row);
        }

    }

    public DecisionTreeNode getChild(bool isLeft)
    {
        if (isLeft)
        {
            return left;
        }
        else
        {
            return right;
        }
    }

    public bool[] batchEvaluate(Matrix<double> data)
    {
        if (index2 < 0)
        {
            double[] column = data.Column(index).AsArray();
            bool[] evals = new bool[column.Length];
            for (int i = 0; i < evals.Length; i++)
            {
                evals[i] = (lessThan && column[i] < value) || (!lessThan && column[i] > value);
            }
            return evals;
        }
        else
        {
            double[] column1 = data.Column(index).AsArray();
            double[] column2 = data.Column(index).AsArray();
            bool[] evals = new bool[column1.Length];
            for(int i = 0; i < evals.Length; i++)
            {
                evals[i] = (lessThan && column1[i] <= column2[i]) || (!lessThan && column1[i] >= column2[i]);
            }
            return evals;
        }
    }

    public bool hasChildren()
    {
        return !(left is null) && !(right is null);
    }

    public void setChildNode(DecisionTreeNode newChild, bool isLeft)
    {
        if (isLeft)
        {
            left = newChild;
        }
        else
        {
            right = newChild;
        }
    }

    public void debugPrint()
    {
        Debug.Log(this);
        if(left != null)
        {
            left.debugPrint();
        }
        if(right != null)
        {
            right.debugPrint();
        }
    }

    public override string ToString()
    {
        if (index2 < 0)
        {
            return "Column " + index + (lessThan ? " <= " : " >= ") + value;

        }
        else
        {
            return "Column " + index + (lessThan ? " >= " : " >= ") + "Column " + index2;
        }
    }
}
