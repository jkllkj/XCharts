﻿
using UnityEngine;
using XCharts.Runtime;

namespace XCharts.Example
{
    [DisallowMultipleComponent]
    [ExecuteInEditMode]
    public class Example13_LineSimple : MonoBehaviour
    {
        void Awake()
        {
            var chart = gameObject.GetComponent<LineChart>();
            if (chart == null)
            {
                chart = gameObject.AddComponent<LineChart>();
                chart.Init();
                chart.SetSize(580, 300);
            }
            chart.GetOrAddChartComponent<Title>().show = true;
            chart.GetOrAddChartComponent<Title>().text = "Line Simple";

            chart.GetOrAddChartComponent<Tooltip>().show = true;
            chart.GetOrAddChartComponent<Legend>().show = false;

            var xAxis = chart.GetOrAddChartComponent<XAxis>();
            var yAxis = chart.GetOrAddChartComponent<YAxis>();
            xAxis.show = true;
            yAxis.show = true;
            xAxis.type = Axis.AxisType.Category;
            yAxis.type = Axis.AxisType.Value;

            xAxis.splitNumber = 10;
            xAxis.boundaryGap = true;

            chart.RemoveData();
            chart.AddSerie<Line>();
            chart.AddSerie<Line>();
            for (int i = 0; i < 200; i++)
            {
                chart.AddXAxisData("x" + i);
                chart.AddData(0, Random.Range(10, 20));
                chart.AddData(1, Random.Range(10, 20));
            }
        }
    }
}