
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XUGL;

namespace XCharts.Runtime
{
    [UnityEngine.Scripting.Preserve]
    internal sealed class HeatmapHandler : SerieHandler<Heatmap>
    {
        private GridCoord m_SerieGrid;

        public override int defaultDimension { get { return 2; } }

        public override void Update()
        {
            base.Update();
            UpdateSerieContext();
        }

        public override void DrawSerie(VertexHelper vh)
        {
            var colorIndex = chart.GetLegendRealShowNameIndex(serie.legendName);
            DrawHeatmapSerie(vh, colorIndex, serie);
        }

        public override void UpdateTooltipSerieParams(int dataIndex, bool showCategory, string category,
            string marker, string itemFormatter, string numericFormatter,
            ref List<SerieParams> paramList, ref string title)
        {
            dataIndex = serie.context.pointerItemDataIndex;
            if (dataIndex < 0)
                return;

            var serieData = serie.GetSerieData(dataIndex);
            if (serieData == null)
                return;

            if (string.IsNullOrEmpty(category))
            {
                var xAxis = chart.GetChartComponent<XAxis>(serie.xAxisIndex);
                if (xAxis != null)
                    category = xAxis.GetData((int)serieData.GetData(0));
            }

            title = serie.serieName;

            var param = serie.context.param;
            param.serieName = serie.serieName;
            param.serieIndex = serie.index;
            param.dimension = defaultDimension;
            param.serieData = serieData;
            param.color = serieData.context.color;
            param.marker = SerieHelper.GetItemMarker(serie, serieData, marker);
            param.itemFormatter = SerieHelper.GetItemFormatter(serie, serieData, itemFormatter);
            param.numericFormatter = SerieHelper.GetNumericFormatter(serie, serieData, numericFormatter);
            param.columns.Clear();

            param.columns.Add(param.marker);
            param.columns.Add(category);
            param.columns.Add(ChartCached.NumberToStr(serieData.GetData(defaultDimension), param.numericFormatter));

            paramList.Add(param);
        }

        private void UpdateSerieContext()
        {
            if (m_SerieGrid == null)
                return;

            var needCheck = (chart.isPointerInChart && m_SerieGrid.IsPointerEnter()) || m_LegendEnter;
            var needInteract = false;
            if (!needCheck)
            {
                if (m_LastCheckContextFlag != needCheck)
                {
                    m_LastCheckContextFlag = needCheck;
                    serie.context.pointerItemDataIndex = -1;
                    serie.context.pointerEnter = false;
                    foreach (var serieData in serie.data)
                    {
                        serieData.context.highlight = false;
                    }
                    chart.RefreshPainter(serie);
                }
                return;
            }
            m_LastCheckContextFlag = needCheck;
            if (m_LegendEnter)
            {
                serie.context.pointerEnter = true;
                foreach (var serieData in serie.data)
                {
                    serieData.context.highlight = true;
                }
            }
            else
            {
                serie.context.pointerItemDataIndex = -1;
                serie.context.pointerEnter = false;
                foreach (var serieData in serie.data)
                {
                    if (!needInteract && serieData.context.rect.Contains(chart.pointerPos))
                    {
                        serie.context.pointerItemDataIndex = serieData.index;
                        serie.context.pointerEnter = true;
                        serieData.context.highlight = true;
                        needInteract = true;
                    }
                    else
                    {
                        serieData.context.highlight = false;
                    }
                }
            }
            if (needInteract)
            {
                chart.RefreshPainter(serie);
            }
        }

        private void DrawHeatmapSerie(VertexHelper vh, int colorIndex, Heatmap serie)
        {
            if (serie.animation.HasFadeOut()) return;
            XAxis xAxis;
            YAxis yAxis;
            if (!chart.TryGetChartComponent<XAxis>(out xAxis, serie.xAxisIndex)) return;
            if (!chart.TryGetChartComponent<YAxis>(out yAxis, serie.yAxisIndex)) return;
            m_SerieGrid = chart.GetChartComponent<GridCoord>(xAxis.gridIndex);
            xAxis.boundaryGap = true;
            yAxis.boundaryGap = true;
            var visualMap = chart.GetVisualMapOfSerie(serie);
            var emphasis = serie.emphasis;
            var xCount = xAxis.data.Count;
            var yCount = yAxis.data.Count;
            var xWidth = m_SerieGrid.context.width / xCount;
            var yWidth = m_SerieGrid.context.height / yCount;

            var zeroX = m_SerieGrid.context.x;
            var zeroY = m_SerieGrid.context.y;
            var rangeMin = visualMap.rangeMin;
            var rangeMax = visualMap.rangeMax;
            var color = chart.theme.GetColor(serie.index);
            var borderWidth = serie.itemStyle.show ? serie.itemStyle.borderWidth : 0;
            var rectWid = xWidth - 2 * borderWidth;
            var rectHig = yWidth - 2 * borderWidth;

            var borderColor = serie.itemStyle.opacity > 0
                ? serie.itemStyle.borderColor
                : ChartConst.clearColor32;
            borderColor.a = (byte)(borderColor.a * serie.itemStyle.opacity);

            var borderToColor = serie.itemStyle.opacity > 0
                ? serie.itemStyle.borderToColor
                : ChartConst.clearColor32;
            borderToColor.a = (byte)(borderToColor.a * serie.itemStyle.opacity);

            serie.context.dataPoints.Clear();
            serie.animation.InitProgress(0, xCount);
            var animationIndex = serie.animation.GetCurrIndex();
            var dataChangeDuration = serie.animation.GetUpdateAnimationDuration();
            var dataChanging = false;
            serie.containerIndex = m_SerieGrid.index;
            serie.containterInstanceId = m_SerieGrid.instanceId;
            for (int n = 0; n < serie.dataCount; n++)
            {
                var serieData = serie.data[n];
                serieData.index = n;
                var i = (int)serieData.GetData(0);
                var j = (int)serieData.GetData(1);
                var dimension = VisualMapHelper.GetDimension(visualMap, serieData.data.Count);
                if (serie.IsIgnoreValue(serieData, dimension))
                {
                    serie.context.dataPoints.Add(Vector3.zero);
                    continue;
                }
                var value = serieData.GetCurrData(dimension, dataChangeDuration, yAxis.inverse,
                    yAxis.context.minValue, yAxis.context.maxValue);
                if (serieData.IsDataChanged()) dataChanging = true;
                var pos = new Vector3(zeroX + (i + (xAxis.boundaryGap ? 0.5f : 0)) * xWidth,
                    zeroY + (j + (yAxis.boundaryGap ? 0.5f : 0)) * yWidth);
                serie.context.dataPoints.Add(pos);
                serieData.context.position = pos;

                serieData.context.canShowLabel = false;
                serieData.context.rect = new Rect(pos.x - rectWid / 2, pos.y - rectHig / 2, rectWid, rectHig);
                if (value == 0) continue;
                if ((value < rangeMin && rangeMin != visualMap.min)
                    || (value > rangeMax && rangeMax != visualMap.max))
                {
                    continue;
                }
                if (!visualMap.IsInSelectedValue(value)) continue;
                if (animationIndex >= 0 && i > animationIndex) continue;
                color = visualMap.GetColor(value);
                if (serieData.context.highlight)
                    color = ChartHelper.GetHighlightColor(color);

                serieData.context.canShowLabel = true;
                serieData.context.color = color;

                var highlight = (serieData.context.highlight)
                    || visualMap.context.pointerIndex > 0;

                //UGL.DrawRectangle(vh, pos, rectWid / 2, rectHig / 2, color);
                UGL.DrawRectangle(vh, serieData.context.rect, color);

                if (borderWidth > 0 && !ChartHelper.IsClearColor(borderColor))
                {
                    UGL.DrawBorder(vh, pos, rectWid, rectHig, borderWidth, borderColor, borderToColor);
                }
                if (visualMap.hoverLink && highlight && emphasis != null && emphasis.show
                    && emphasis.itemStyle.borderWidth > 0)
                {
                    var emphasisBorderWidth = emphasis.itemStyle.borderWidth;
                    var emphasisBorderColor = emphasis.itemStyle.opacity > 0
                        ? emphasis.itemStyle.borderColor : ChartConst.clearColor32;
                    var emphasisBorderToColor = emphasis.itemStyle.opacity > 0
                        ? emphasis.itemStyle.borderToColor : ChartConst.clearColor32;
                    UGL.DrawBorder(vh, pos, rectWid, rectHig, emphasisBorderWidth, emphasisBorderColor,
                        emphasisBorderToColor);
                }

            }
            if (!serie.animation.IsFinish())
            {
                serie.animation.CheckProgress(xCount);
                chart.RefreshPainter(serie);
            }
            if (dataChanging)
            {
                chart.RefreshPainter(serie);
            }
        }
    }
}