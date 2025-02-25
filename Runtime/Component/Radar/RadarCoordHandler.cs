
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using XUGL;

namespace XCharts.Runtime
{
    [UnityEngine.Scripting.Preserve]
    internal sealed class RadarCoordHandler : MainComponentHandler<RadarCoord>
    {
        private const string INDICATOR_TEXT = "indicator";

        public override void InitComponent()
        {
            InitRadarCoord(component);
        }

        public override void Update()
        {
            if (!chart.isPointerInChart)
            {
                component.context.isPointerEnter = false;
                return;
            }
            var radar = component;
            radar.context.isPointerEnter = radar.show
                && Vector3.Distance(radar.context.center, chart.pointerPos) <= radar.context.radius;
        }

        public override void DrawBase(VertexHelper vh)
        {
            DrawRadarCoord(vh, component);
        }

        private void InitRadarCoord(RadarCoord radar)
        {
            float txtWid = 100;
            float txtHig = 20;
            radar.painter = chart.GetPainter(radar.index);
            radar.refreshComponent = delegate ()
            {
                radar.UpdateRadarCenter(chart.chartPosition, chart.chartWidth, chart.chartHeight);
                var radarObject = ChartHelper.AddObject("Radar" + radar.index, chart.transform, chart.chartMinAnchor,
                     chart.chartMaxAnchor, chart.chartPivot, chart.chartSizeDelta);
                radar.gameObject = radarObject;
                radar.gameObject.hideFlags = chart.chartHideFlags;
                var textStyle = radar.axisName.textStyle;
                ChartHelper.HideAllObject(radarObject.transform, INDICATOR_TEXT);
                for (int i = 0; i < radar.indicatorList.Count; i++)
                {
                    var indicator = radar.indicatorList[i];
                    var pos = radar.GetIndicatorPosition(i);
                    var objName = INDICATOR_TEXT + "_" + i;

                    var labelGameObject = ChartHelper.AddObject(objName, radarObject.transform, new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(txtWid, txtHig));
                    var label = ChartHelper.GetOrAddComponent<ChartLabel>(labelGameObject);
                    label.label = ChartHelper.AddTextObject("Text", label.gameObject.transform, new Vector2(0.5f, 0.5f),
                        new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(txtWid, txtHig), textStyle, chart.theme.common);
                    label.SetAutoSize(true);
                    label.label.SetAlignment(textStyle.GetAlignment(TextAnchor.MiddleCenter));
                    label.SetText(radar.GetFormatterIndicatorContent(i));
                    label.SetActive(radar.indicator);
                    label.color = textStyle.backgroundColor;

                    var offset = new Vector3(textStyle.offset.x, textStyle.offset.y);
                    AxisHelper.AdjustCircleLabelPos(label, pos, radar.context.center, txtHig, offset);
                }
                chart.RefreshBasePainter();
            };
            radar.refreshComponent.Invoke();
        }

        private void DrawRadarCoord(VertexHelper vh, RadarCoord radar)
        {
            if (!radar.show) return;
            radar.UpdateRadarCenter(chart.chartPosition, chart.chartWidth, chart.chartHeight);
            if (radar.shape == RadarCoord.Shape.Circle)
            {
                DrawCricleRadar(vh, radar);
            }
            else
            {
                DrawPolygonRadar(vh, radar);
            }
        }

        private void DrawCricleRadar(VertexHelper vh, RadarCoord radar)
        {
            float insideRadius = 0, outsideRadius = 0;
            float block = radar.context.radius / radar.splitNumber;
            int indicatorNum = radar.indicatorList.Count;
            Vector3 p = radar.context.center;
            Vector3 p1;
            float angle = 2 * Mathf.PI / indicatorNum;
            var lineColor = radar.axisLine.GetColor(chart.theme.axis.splitLineColor);
            var lineWidth = radar.axisLine.GetWidth(chart.theme.axis.lineWidth);
            var lineType = radar.axisLine.GetType(chart.theme.axis.lineType);
            var splitLineColor = radar.splitLine.GetColor(chart.theme.axis.splitLineColor);
            var splitLineWidth = radar.splitLine.GetWidth(chart.theme.axis.splitLineWidth);
            for (int i = 0; i < radar.splitNumber; i++)
            {
                var color = radar.splitArea.GetColor(i, chart.theme.axis);
                outsideRadius = insideRadius + block;
                if (radar.splitArea.show)
                {
                    UGL.DrawDoughnut(vh, p, insideRadius, outsideRadius, color, Color.clear,
                          0, 360, chart.settings.cicleSmoothness);
                }
                if (radar.splitLine.show)
                {
                    UGL.DrawEmptyCricle(vh, p, outsideRadius, splitLineWidth, splitLineColor,
                        Color.clear, chart.settings.cicleSmoothness);
                }
                insideRadius = outsideRadius;
            }
            if (radar.axisLine.show)
            {
                for (int j = 0; j <= indicatorNum; j++)
                {
                    float currAngle = j * angle;
                    p1 = new Vector3(p.x + outsideRadius * Mathf.Sin(currAngle),
                        p.y + outsideRadius * Mathf.Cos(currAngle));
                    ChartDrawer.DrawLineStyle(vh, lineType, lineWidth, p, p1, lineColor);
                }
            }
        }

        private void DrawPolygonRadar(VertexHelper vh, RadarCoord radar)
        {
            float insideRadius = 0, outsideRadius = 0;
            float block = radar.context.radius / radar.splitNumber;
            int indicatorNum = radar.indicatorList.Count;
            Vector3 p1, p2, p3, p4;
            Vector3 p = radar.context.center;
            float angle = 2 * Mathf.PI / indicatorNum;
            var lineColor = radar.axisLine.GetColor(chart.theme.axis.splitLineColor);
            var lineWidth = radar.axisLine.GetWidth(chart.theme.axis.lineWidth);
            var lineType = radar.axisLine.GetType(chart.theme.axis.lineType);
            var splitLineColor = radar.splitLine.GetColor(chart.theme.axis.splitLineColor);
            var splitLineWidth = radar.splitLine.GetWidth(chart.theme.axis.splitLineWidth);
            var splitLineType = radar.splitLine.GetType(chart.theme.axis.splitLineType);
            for (int i = 0; i < radar.splitNumber; i++)
            {
                var color = radar.splitArea.GetColor(i, chart.theme.axis);
                outsideRadius = insideRadius + block;
                p1 = new Vector3(p.x + insideRadius * Mathf.Sin(0), p.y + insideRadius * Mathf.Cos(0));
                p2 = new Vector3(p.x + outsideRadius * Mathf.Sin(0), p.y + outsideRadius * Mathf.Cos(0));
                for (int j = 0; j <= indicatorNum; j++)
                {
                    float currAngle = j * angle;
                    p3 = new Vector3(p.x + outsideRadius * Mathf.Sin(currAngle),
                        p.y + outsideRadius * Mathf.Cos(currAngle));
                    p4 = new Vector3(p.x + insideRadius * Mathf.Sin(currAngle),
                        p.y + insideRadius * Mathf.Cos(currAngle));
                    if (radar.splitArea.show)
                    {
                        UGL.DrawQuadrilateral(vh, p1, p2, p3, p4, color);
                    }
                    if (radar.splitLine.NeedShow(i))
                    {
                        ChartDrawer.DrawLineStyle(vh, splitLineType, splitLineWidth, p2, p3, splitLineColor);
                    }
                    p1 = p4;
                    p2 = p3;
                }
                insideRadius = outsideRadius;
            }
            if (radar.axisLine.show)
            {
                for (int j = 0; j <= indicatorNum; j++)
                {
                    float currAngle = j * angle;
                    p3 = new Vector3(p.x + outsideRadius * Mathf.Sin(currAngle),
                        p.y + outsideRadius * Mathf.Cos(currAngle));
                    ChartDrawer.DrawLineStyle(vh, lineType, lineWidth, p, p3, lineColor);
                }
            }
        }
    }
}