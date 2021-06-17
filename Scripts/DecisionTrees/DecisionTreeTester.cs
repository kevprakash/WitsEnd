using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System;

public class DecisionTreeTester : MonoBehaviour
{
    public Explorer e;
    public bool test = false;
    private static Matrix<double> data;
    private static Vector<double> dataLabels;

    static DecisionTreeTester()
    {
        System.Random r = new System.Random();
        double[][] rows = new double[100][];
        double[] labels = new double[100];
        for(int i = 0; i < rows.Length; i++)
        {
            rows[i] = new double[400];
            for(int j = 0; j < rows[i].Length; j++)
            {
                rows[i][j] = r.Next(-100, 100);
            }
            switch(r.Next(0, 4))
            {
                case 0:
                    labels[i] = 0;
                    break;
                case 1:
                    rows[i][200] = r.Next(200, 300);
                    labels[i] = 1;
                    break;
                case 2:
                    rows[i][200] = r.Next(100, 200);
                    rows[i][50] = r.Next(-200, -150);
                    labels[i] = 0;
                    break;
                case 3:
                    rows[i][200] = r.Next(100, 200);
                    labels[i] = 1;
                    break;
            }
        }
        data = CreateMatrix.DenseOfRowArrays<double>(rows);
        dataLabels = CreateVector.DenseOfArray<double>(labels);
    }

    void Start()
    {

        Debug.Log(data.RowCount);
        Debug.Log(data.ColumnCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            test = false;
            //e.decisionTrees[0][0].build(data, dataLabels, 10);
        }
    }
}
