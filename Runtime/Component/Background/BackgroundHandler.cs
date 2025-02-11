
using System;
using UnityEngine;
using UnityEngine.UI;
using XUGL;

namespace XCharts.Runtime
{
    [UnityEngine.Scripting.Preserve]
    internal sealed class BackgroundHandler : MainComponentHandler<Background>
    {
        private readonly string s_BackgroundObjectName = "background";
        public override void InitComponent()
        {
            component.painter = chart.painter;
            component.refreshComponent = delegate ()
            {
                var backgroundObj = ChartHelper.AddObject(s_BackgroundObjectName, chart.transform, chart.chartMinAnchor,
                    chart.chartMaxAnchor, chart.chartPivot, chart.chartSizeDelta);
                component.gameObject = backgroundObj;
                backgroundObj.hideFlags = chart.chartHideFlags;

                var backgroundImage = ChartHelper.GetOrAddComponent<Image>(backgroundObj);
                ChartHelper.UpdateRectTransform(backgroundObj, chart.chartMinAnchor,
                    chart.chartMaxAnchor, chart.chartPivot, chart.chartSizeDelta);
                backgroundImage.sprite = component.image;
                backgroundImage.type = component.imageType;
                backgroundImage.color = component.imageColor;

                backgroundObj.transform.SetSiblingIndex(0);
                backgroundObj.SetActive(component.show);
            };
            component.refreshComponent();
        }

        public override void DrawBase(VertexHelper vh)
        {
            if (!component.show)
                return;

            var p1 = new Vector3(chart.chartX, chart.chartY + chart.chartHeight);
            var p2 = new Vector3(chart.chartX + chart.chartWidth, chart.chartY + chart.chartHeight);
            var p3 = new Vector3(chart.chartX + chart.chartWidth, chart.chartY);
            var p4 = new Vector3(chart.chartX, chart.chartY);
            var backgroundColor = chart.theme.GetBackgroundColor(component);
            
            UGL.DrawQuadrilateral(vh, p1, p2, p3, p4, backgroundColor);
        }
    }
}