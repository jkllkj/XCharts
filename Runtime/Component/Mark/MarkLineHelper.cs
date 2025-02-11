
using UnityEngine;

namespace XCharts.Runtime
{
    internal static class MarkLineHelper
    {
        public static string GetFormatterContent(Serie serie, MarkLineData data)
        {
            var serieLabel = data.label;
            var numericFormatter = serieLabel.numericFormatter;
            if (serieLabel.formatterFunction != null)
            {
                return serieLabel.formatterFunction(data.index, data.runtimeValue);
            }
            if (string.IsNullOrEmpty(serieLabel.formatter))
                return ChartCached.NumberToStr(data.runtimeValue, numericFormatter);
            else
            {
                var content = serieLabel.formatter;
                FormatterHelper.ReplaceSerieLabelContent(ref content, numericFormatter, data.runtimeValue,
                    0, serie.serieName, data.name, data.name, Color.clear);
                return content;
            }
        }

        public static Vector3 GetLabelPosition(MarkLineData data)
        {
            if (!data.label.show) return Vector3.zero;
            var dir = (data.runtimeEndPosition - data.runtimeStartPosition).normalized;
            var horizontal = Mathf.Abs(Vector3.Dot(dir, Vector3.right)) == 1;
            var labelWidth = data.runtimeLabel == null ? 50 : data.runtimeLabel.GetLabelWidth();
            var labelHeight = data.runtimeLabel == null ? 20 : data.runtimeLabel.GetLabelHeight();
            switch (data.label.position)
            {
                case LabelStyle.Position.Start:
                    if (horizontal) return data.runtimeStartPosition + data.label.offset + labelWidth / 2 * Vector3.left;
                    else return data.runtimeStartPosition + data.label.offset + labelHeight / 2 * Vector3.down;
                case LabelStyle.Position.Middle:
                    var center = (data.runtimeStartPosition + data.runtimeCurrentEndPosition) / 2;
                    if (horizontal) return center + data.label.offset + labelHeight / 2 * Vector3.up;
                    else return center + data.label.offset + labelWidth / 2 * Vector3.right;
                default:
                    if (horizontal) return data.runtimeCurrentEndPosition + data.label.offset + labelWidth / 2 * Vector3.right;
                    else return data.runtimeCurrentEndPosition + data.label.offset + labelHeight / 2 * Vector3.up;
            }
        }
    }
}