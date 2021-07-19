using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Threading.Tasks;
using System;

public class DecisionTree
{

    private DecisionTreeNode root = null;

    public async Task build(Matrix<double> data, Vector<double> labels, int maxDepth, double impurityThreshold=0, double sampleMinLength=1, float minRatio=0.0f)
    {
        List < List < (DecisionTreeNode, (Matrix<double>, Vector<double>))>> levels = new List<List<(DecisionTreeNode, (Matrix<double>, Vector<double>))>>();
        levels.Add(new List<(DecisionTreeNode, (Matrix<double>, Vector<double>))>());
        levels[0].Add((await makeNode(data, labels), (data, labels)));
        int lev = 1;
        int totalNodes = 1;
        Debug.Log("Starting building on " + data.RowCount + " rows of length " + data.ColumnCount);
        while(lev < maxDepth && levels[lev-1].Count > 0)
        {
            levels.Add(new List<(DecisionTreeNode, (Matrix<double>, Vector<double>))>());
            List<(DecisionTreeNode, (Matrix<double>, Vector<double>))> nodes = levels[lev - 1];
            foreach((DecisionTreeNode, (Matrix<double>, Vector<double>))  ndl in nodes)
            {
                DecisionTreeNode n = ndl.Item1;
                Matrix<double> d = ndl.Item2.Item1;
                Vector<double> l = ndl.Item2.Item2;
                if(l.Count > sampleMinLength && GINIImpurity(l.ToList<double>()) > impurityThreshold)
                {
                    bool[] evalualtions = n.batchEvaluate(d);
                    double[][] rows = d.ToRowArrays();
                    double[] labelArr = labels.ToArray();
                    double[][] d_l = new double[evalualtions.Count(e => e)][];
                    double[] dll = new double[d_l.Length];
                    double[][] d_r = new double[rows.Length - d_l.Length][];
                    double[] drl = new double[d_r.Length];
                    int dl_i = 0;
                    int dr_i = 0;
                    for(int i = 0; i < evalualtions.Length; i++)
                    {
                        if (evalualtions[i])
                        {
                            d_l[dl_i] = rows[i];
                            dll[dl_i] = labelArr[i];
                            dl_i++;
                        }
                        else
                        {
                            d_r[dr_i] = rows[i];
                            drl[dr_i] = labelArr[i];
                            dr_i++;
                        }
                    }
                    if(dll.Length > sampleMinLength && GINIImpurity(new List<double>(dll)) > impurityThreshold)
                    {
                        Matrix<double> dlm = CreateMatrix.DenseOfRowArrays<double>(d_l);
                        Vector<double> dlv = CreateVector.Dense<double>(dll);
                        DecisionTreeNode leftNode = await makeNode(dlm, dlv, minRatio:minRatio);
                        levels[lev].Add((leftNode, (dlm, dlv)));
                        n.setChildNode(leftNode, true);
                        totalNodes++;
                    }
                    if (drl.Length > sampleMinLength && GINIImpurity(new List<double>(drl)) > impurityThreshold)
                    {
                        Matrix<double> drm = CreateMatrix.DenseOfRowArrays<double>(d_r);
                        Vector<double> drv = CreateVector.Dense<double>(drl);
                        DecisionTreeNode rightNode = await makeNode(drm, drv, minRatio:minRatio);
                        levels[lev].Add((rightNode, (drm, drv)));
                        n.setChildNode(rightNode, false);
                        totalNodes++;
                    }
                    //Debug.Log("Total nodes: " + totalNodes);
                }
            }
            lev++;
        }
        Debug.Log("Finished building");
        Debug.Log("Total nodes: " + totalNodes + " in " + levels.Count + " levels");
        root = levels[0][0].Item1;
        //root.debugPrint();
    }

    public static async Task<DecisionTreeNode> makeNode(Matrix<double> data, Vector<double> labels, int asyncCount=100, float minRatio=0.125f)
    {
        int columnCount = data.ColumnCount;
        DecisionTreeNode bestNode = null;
        double bestScore = float.MaxValue;
        (DecisionTreeNode, double) bestComparisons = await calculateBestComparison(data, labels, minRatio);
        bestNode = bestComparisons.Item1;
        bestScore = bestComparisons.Item2;
        for(int c = 0; c < columnCount && bestScore > 0; c++)
        {
            (DecisionTreeNode, double) bestSplit = calculateBestSplit(data, labels, c, minRatio);
            DecisionTreeNode testNode = bestSplit.Item1;
            double testScore = bestSplit.Item2;
            if(testScore < bestScore)
            {
                bestNode = testNode;
                bestScore = testScore;
                
                if(bestScore <= 0.0)
                {
                    break;
                }
            }
            if((c+1) % asyncCount == 0)
            {
                await waitNextFrame();
            }
        }
        //Debug.Log("Best score: " + bestScore);
        //Debug.Log("Best score: " + bestScore + " on " + bestNode);
        return bestNode;
    }

    private static async Task waitNextFrame()
    {
        //Debug.Log("Waiting for next Frame");
        await Task.Delay(TimeSpan.FromMilliseconds(1));
        //Debug.Log("Finished waiting");
    }

    public static (DecisionTreeNode, double) calculateBestSplit(Matrix<double> data, Vector<double> labels, int index, float minRatio=0.125f)
    {
        DecisionTreeNode bestNode = null;
        double bestScore = double.MaxValue;
        double[] column = data.Column(index).AsArray();
        foreach(int c in column){
            bool[] tf = new bool[] { true, false };
            foreach (bool lt in tf)
            {
                DecisionTreeNode testNode = new DecisionTreeNode(index, c, lt);
                bool[] evaluation = testNode.batchEvaluate(data);
                List<double> evalTrue = new List<double>();
                List<double> evalFalse = new List<double>();
                for(int i = 0; i < evaluation.Length; i++)
                {
                    if (evaluation[i])
                    {
                        evalTrue.Add(labels[i]);
                    }
                    else
                    {
                        evalFalse.Add(labels[i]);
                    }
                }
                if(evalTrue.Count >= evaluation.Length * minRatio && evalFalse.Count >= evaluation.Length * minRatio)
                {
                    double scoreTrue = score(evalTrue, 1);
                    double scoreFalse = score(evalFalse, 0);
                    double scoreBoth = scoreTrue + scoreFalse;
                    if(scoreBoth < bestScore)
                    {
                        bestNode = testNode;
                        bestScore = scoreBoth;


                        if(bestScore <= 0.0)
                        {
                            break;
                        }
                    }
                }

            }
            if(bestScore <= 0.0)
            {
                break;
            }
        }
        return (bestNode, bestScore);
    }

    public static double score(List<double> data, double trueLabel)
    {
        Vector<double> vd = CreateVector.DenseOfEnumerable(data);
        Vector<double> vl = CreateVector.Dense(data.Count, trueLabel);
        Vector<double> diff = vd.Subtract(vl);
        return diff.L1Norm();
    }

    public static double GINIImpurity(List<double> data)
    {
        int dataLength = data.Count;
        int onesCount = data.ToArray().Count(d => d == 1);
        int zeroesCount = dataLength - onesCount;
        double onesRatio = (onesCount + 0.0) / dataLength;
        double zeroesRatio = (zeroesCount + 0.0) / dataLength;
        double gini = onesRatio * (1 - onesRatio) + zeroesRatio * (1 - zeroesRatio);
        Debug.Log("Gini Impurity: " + gini);
        return gini;
    }

    public DecisionTreeNode getRoot()
    {
        return root;
    }

    public static async Task<(DecisionTreeNode, double)> calculateBestComparison(Matrix<double> data, Vector<double> labels, float minRatio = 0.125f, int asyncCount=10)
    {
        DecisionTreeNode bestNode = null;
        double bestScore = double.MaxValue;
        for(int index1 = 0; index1 < data.ColumnCount; index1++)
        {
            for (int index2 = 0; index2 < data.ColumnCount; index2++)
            {
                if (index1 == index2)
                {
                    continue;
                }
                DecisionTreeNode testNode = new DecisionTreeNode(index1, index2, 0, false);
                bool[] evaluation = testNode.batchEvaluate(data);
                List<double> evalTrue = new List<double>();
                List<double> evalFalse = new List<double>();
                for (int i = 0; i < evaluation.Length; i++)
                {
                    if (evaluation[i])
                    {
                        evalTrue.Add(labels[i]);
                    }
                    else
                    {
                        evalFalse.Add(labels[i]);
                    }
                }
                if (evalTrue.Count >= evaluation.Length * minRatio && evalFalse.Count >= evaluation.Length * minRatio)
                {
                    double scoreTrue = score(evalTrue, 1);
                    double scoreFalse = score(evalFalse, 0);
                    double scoreBoth = scoreTrue + scoreFalse;
                    if (scoreBoth < bestScore)
                    {
                        bestNode = testNode;
                        bestScore = scoreBoth;


                        if (bestScore <= 0.0)
                        {
                            break;
                        }
                    }
                }

            }
            if (bestScore <= 0.0)
            {
                break;
            }
            if((index1 + 1) % asyncCount == 0)
            {
                await waitNextFrame();
            }
        }
        return (bestNode, bestScore);
    }

    public bool evaluate(int[] row)
    {
        if(root == null)
        {
            return UnityEngine.Random.Range(0, 2) == 1;
        }
        else
        {
            return root.traverse(row);
        }
    }
}
