﻿
using System;
using System.Collections.Generic;
using UnityEngine;

namespace XCharts.Runtime
{
    /// <summary>
    /// A data item of serie.
    /// |系列中的一个数据项。可存储数据名和1-n维个数据。
    /// </summary>
    [System.Serializable]
    public class SerieData : ChildComponent
    {
        [SerializeField] private int m_Index;
        [SerializeField] private string m_Name;
        [SerializeField] private string m_Id;
        [SerializeField] private string m_ParentId;
        [SerializeField] private List<ItemStyle> m_ItemStyles = new List<ItemStyle>();
        [SerializeField] private List<LabelStyle> m_Labels = new List<LabelStyle>();
        [SerializeField] private List<LabelLine> m_LabelLines = new List<LabelLine>();
        [SerializeField] private List<Emphasis> m_Emphases = new List<Emphasis>();
        [SerializeField] private List<SymbolStyle> m_Symbols = new List<SymbolStyle>();
        [SerializeField] private List<IconStyle> m_IconStyles = new List<IconStyle>();
        [SerializeField] private List<LineStyle> m_LineStyles = new List<LineStyle>();
        [SerializeField] private List<AreaStyle> m_AreaStyles = new List<AreaStyle>();
        [SerializeField] private List<TitleStyle> m_TitleStyles = new List<TitleStyle>();
        [SerializeField] private List<double> m_Data = new List<double>();

        [NonSerialized] public SerieDataContext context = new SerieDataContext();
        [NonSerialized] public InteractData interact = new InteractData();
        [NonSerialized] private bool m_Ignore = false;
        [NonSerialized] private bool m_Selected;
        [NonSerialized] private float m_Radius;
        public ChartLabel labelObject { get; set; }
        public ChartLabel titleObject { get; set; }

        private bool m_Show = true;

        public override int index { get { return m_Index; } set { m_Index = value; } }
        /// <summary>
        /// the name of data item.
        /// |数据项名称。
        /// </summary>
        public string name { get { return m_Name; } set { m_Name = value; } }
        /// <summary>
        /// 数据项的唯一id。唯一id不是必须设置的。
        /// </summary>
        public string id { get { return m_Id; } set { m_Id = value; } }
        public string parentId { get { return m_ParentId; } set { m_ParentId = value; } }
        /// <summary>
        /// 数据项图例名称。当数据项名称不为空时，图例名称即为系列名称；反之则为索引index。
        /// </summary>
        /// <value></value>
        public string legendName { get { return string.IsNullOrEmpty(name) ? ChartCached.IntToStr(index) : name; } }
        /// <summary>
        /// 自定义半径。可用在饼图中自定义某个数据项的半径。
        /// </summary>
        public float radius { get { return m_Radius; } set { m_Radius = value; } }
        /// <summary>
        /// Whether the data item is selected.
        /// |该数据项是否被选中。
        /// </summary>
        public bool selected { get { return m_Selected; } set { m_Selected = value; } }
        /// <summary>
        /// the icon of data.
        /// |数据项图标样式。
        /// </summary>
        public IconStyle iconStyle { get { return m_IconStyles.Count > 0 ? m_IconStyles[0] : null; } }
        /// <summary>
        /// 单个数据项的标签设置。
        /// </summary>
        public LabelStyle label { get { return m_Labels.Count > 0 ? m_Labels[0] : null; } }
        public LabelLine labelLine { get { return m_LabelLines.Count > 0 ? m_LabelLines[0] : null; } }
        /// <summary>
        /// 单个数据项的样式设置。
        /// </summary>
        public ItemStyle itemStyle { get { return m_ItemStyles.Count > 0 ? m_ItemStyles[0] : null; } }
        /// <summary>
        /// 单个数据项的高亮样式设置。
        /// </summary>
        public Emphasis emphasis { get { return m_Emphases.Count > 0 ? m_Emphases[0] : null; } }
        /// <summary>
        /// 单个数据项的标记设置。
        /// </summary>
        public SymbolStyle symbol { get { return m_Symbols.Count > 0 ? m_Symbols[0] : null; } }
        public LineStyle lineStyle { get { return m_LineStyles.Count > 0 ? m_LineStyles[0] : null; } }
        public AreaStyle areaStyle { get { return m_AreaStyles.Count > 0 ? m_AreaStyles[0] : null; } }
        public TitleStyle titleStyle { get { return m_TitleStyles.Count > 0 ? m_TitleStyles[0] : null; } }
        /// <summary>
        /// 是否忽略数据。当为 true 时，数据不进行绘制。
        /// </summary>
        public bool ignore
        {
            get { return m_Ignore; }
            set { if (PropertyUtil.SetStruct(ref m_Ignore, value)) SetVerticesDirty(); }
        }
        /// <summary>
        /// An arbitrary dimension data list of data item.
        /// |可指定任意维数的数值列表。
        /// </summary>
        public List<double> data { get { return m_Data; } set { m_Data = value; } }
        /// <summary>
        /// [default:true] Whether the data item is showed.
        /// |该数据项是否要显示。
        /// </summary>
        public bool show { get { return m_Show; } set { m_Show = value; } }

        private List<double> m_PreviousData = new List<double>();
        private List<float> m_DataUpdateTime = new List<float>();
        private List<bool> m_DataUpdateFlag = new List<bool>();
        private List<Vector2> m_PolygonPoints = new List<Vector2>();

        public void Reset()
        {
            index = 0;
            m_Id = null;
            m_ParentId = null;
            labelObject = null;
            m_Name = string.Empty;
            m_Show = true;
            m_Selected = false;
            m_Radius = 0;
            context.Reset();
            interact.Reset();
            m_Data.Clear();
            m_PreviousData.Clear();
            m_DataUpdateTime.Clear();
            m_DataUpdateFlag.Clear();
            m_IconStyles.Clear();
            m_Labels.Clear();
            m_LabelLines.Clear();
            m_ItemStyles.Clear();
            m_Emphases.Clear();
            m_Symbols.Clear();
            m_LineStyles.Clear();
            m_AreaStyles.Clear();
            m_TitleStyles.Clear();
        }

        public T GetOrAddComponent<T>() where T : ChildComponent
        {
            var type = typeof(T);
            if (type == typeof(ItemStyle))
            {
                if (m_ItemStyles.Count == 0)
                    m_ItemStyles.Add(new ItemStyle() { show = true });
                return m_ItemStyles[0] as T;
            }
            else if (type == typeof(IconStyle))
            {
                if (m_IconStyles.Count == 0)
                    m_IconStyles.Add(new IconStyle() { show = true });
                return m_IconStyles[0] as T;
            }
            else if (type == typeof(LabelStyle))
            {
                if (m_Labels.Count == 0)
                    m_Labels.Add(new LabelStyle() { show = true });
                return m_Labels[0] as T;
            }
            else if (type == typeof(LabelLine))
            {
                if (m_LabelLines.Count == 0)
                    m_LabelLines.Add(new LabelLine() { show = true });
                return m_LabelLines[0] as T;
            }
            else if (type == typeof(Emphasis))
            {
                if (m_Emphases.Count == 0)
                    m_Emphases.Add(new Emphasis() { show = true });
                return m_Emphases[0] as T;
            }
            else if (type == typeof(SymbolStyle))
            {
                if (m_Symbols.Count == 0)
                    m_Symbols.Add(new SymbolStyle() { show = true });
                return m_Symbols[0] as T;
            }
            else if (type == typeof(LineStyle))
            {
                if (m_LineStyles.Count == 0)
                    m_LineStyles.Add(new LineStyle() { show = true });
                return m_LineStyles[0] as T;
            }
            else if (type == typeof(AreaStyle))
            {
                if (m_AreaStyles.Count == 0)
                    m_AreaStyles.Add(new AreaStyle() { show = true });
                return m_AreaStyles[0] as T;
            }
            else if (type == typeof(TitleStyle))
            {
                if (m_TitleStyles.Count == 0)
                    m_TitleStyles.Add(new TitleStyle() { show = true });
                return m_TitleStyles[0] as T;
            }
            else
            {
                throw new System.Exception("SerieData not support component:" + type);
            }
        }

        public void RemoveAllComponent()
        {
            m_ItemStyles.Clear();
            m_IconStyles.Clear();
            m_Labels.Clear();
            m_LabelLines.Clear();
            m_Symbols.Clear();
            m_Emphases.Clear();
            m_LineStyles.Clear();
            m_AreaStyles.Clear();
            m_TitleStyles.Clear();
        }

        public void RemoveComponent<T>() where T : ISerieDataComponent
        {
            var type = typeof(T);
            if (type == typeof(ItemStyle))
                m_ItemStyles.Clear();
            else if (type == typeof(IconStyle))
                m_IconStyles.Clear();
            else if (type == typeof(LabelStyle))
                m_Labels.Clear();
            else if (type == typeof(LabelLine))
                m_LabelLines.Clear();
            else if (type == typeof(Emphasis))
                m_Emphases.Clear();
            else if (type == typeof(SymbolStyle))
                m_Symbols.Clear();
            else if (type == typeof(LineStyle))
                m_LineStyles.Clear();
            else if (type == typeof(AreaStyle))
                m_AreaStyles.Clear();
            else if (type == typeof(TitleStyle))
                m_TitleStyles.Clear();
            else
                throw new System.Exception("SerieData not support component:" + type);
        }
        public double GetData(int index, bool inverse = false)
        {
            if (index >= 0 && index < m_Data.Count)
            {
                return inverse ? -m_Data[index] : m_Data[index];
            }
            else return 0;
        }

        public double GetData(int index, double min, double max)
        {
            if (index >= 0 && index < m_Data.Count)
            {
                var value = m_Data[index];
                if (value < min) return min;
                else if (value > max) return max;
                else return value;
            }
            else return 0;
        }

        public double GetPreviousData(int index, bool inverse = false)
        {
            if (index >= 0 && index < m_PreviousData.Count)
            {
                return inverse ? -m_PreviousData[index] : m_PreviousData[index];
            }
            else return 0;
        }

        public double GetFirstData(float animationDuration = 500f)
        {
            if (m_Data.Count > 0) return GetCurrData(0, animationDuration);
            return 0;
        }

        public double GetLastData()
        {
            if (m_Data.Count > 0) return m_Data[m_Data.Count - 1];
            return 0;
        }

        public double GetCurrData(int index, float animationDuration = 500f, bool inverse = false)
        {
            return GetCurrData(index, animationDuration, inverse, 0, 0);
        }

        public double GetCurrData(int index, float animationDuration, bool inverse, double min, double max)
        {
            if (index < m_DataUpdateFlag.Count && m_DataUpdateFlag[index] && animationDuration > 0)
            {
                var time = Time.time - m_DataUpdateTime[index];
                var total = animationDuration / 1000;

                var rate = time / total;
                if (rate > 1) rate = 1;
                if (rate < 1)
                {
                    CheckLastData();
                    var curr = MathUtil.Lerp(GetPreviousData(index), GetData(index), rate);
                    if (min != 0 || max != 0)
                    {
                        if (inverse)
                        {
                            var temp = min;
                            min = -max;
                            max = -temp;
                        }
                        var pre = m_PreviousData[index];
                        if (pre < min)
                        {
                            m_PreviousData[index] = min;
                            curr = min;
                        }
                        else if (pre > max)
                        {
                            m_PreviousData[index] = max;
                            curr = max;
                        }
                    }
                    curr = inverse ? -curr : curr;
                    return curr;
                }
                else
                {
                    m_DataUpdateFlag[index] = false;
                    return GetData(index, inverse);
                }
            }
            else
            {
                return GetData(index, inverse);
            }
        }

        /// <summary>
        /// the maxinum value.
        /// |最大值。
        /// </summary>
        public double GetMaxData(bool inverse = false)
        {
            if (m_Data.Count == 0) return 0;
            var temp = double.MinValue;
            for (int i = 0; i < m_Data.Count; i++)
            {
                var value = GetData(i, inverse);
                if (value > temp) temp = value;
            }
            return temp;
        }

        /// <summary>
        /// the mininum value.
        /// |最小值。
        /// </summary>
        public double GetMinData(bool inverse = false)
        {
            if (m_Data.Count == 0) return 0;
            var temp = double.MaxValue;
            for (int i = 0; i < m_Data.Count; i++)
            {
                var value = GetData(i, inverse);
                if (value < temp) temp = value;
            }
            return temp;
        }

        public bool UpdateData(int dimension, double value, bool updateAnimation, float animationDuration = 500f)
        {
            if (dimension >= 0 && dimension < data.Count)
            {
                CheckLastData();
                m_PreviousData[dimension] = GetCurrData(dimension, animationDuration);
                //m_PreviousData[dimension] = data[dimension];;
                m_DataUpdateTime[dimension] = Time.time;
                m_DataUpdateFlag[dimension] = updateAnimation;
                data[dimension] = value;
                return true;
            }
            return false;
        }

        public bool UpdateData(int dimension, double value)
        {
            if (dimension >= 0 && dimension < data.Count)
            {
                data[dimension] = value;
                return true;
            }
            return false;
        }

        private void CheckLastData()
        {
            if (m_PreviousData.Count != m_Data.Count)
            {
                m_PreviousData.Clear();
                m_DataUpdateTime.Clear();
                m_DataUpdateFlag.Clear();
                for (int i = 0; i < m_Data.Count; i++)
                {
                    m_PreviousData.Add(m_Data[i]);
                    m_DataUpdateTime.Add(Time.time);
                    m_DataUpdateFlag.Add(false);
                }
            }
        }

        public bool IsDataChanged()
        {
            for (int i = 0; i < m_DataUpdateFlag.Count; i++)
                if (m_DataUpdateFlag[i]) return true;
            return false;
        }

        public float GetLabelWidth()
        {
            if (labelObject != null) return labelObject.GetLabelWidth();
            else return 0;
        }

        public float GetLabelHeight()
        {
            if (labelObject != null) return labelObject.GetLabelHeight();
            return 0;
        }

        public void SetLabelActive(bool flag)
        {
            if (labelObject != null) labelObject.SetLabelActive(flag);
        }
        public void SetIconActive(bool flag)
        {
            if (labelObject != null) labelObject.SetIconActive(flag);
        }

        public void SetPolygon(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            m_PolygonPoints.Clear();
            m_PolygonPoints.Add(p1);
            m_PolygonPoints.Add(p2);
            m_PolygonPoints.Add(p3);
            m_PolygonPoints.Add(p4);
        }
        public void SetPolygon(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, Vector2 p5)
        {
            SetPolygon(p1, p2, p3, p4);
            m_PolygonPoints.Add(p5);
        }

        public bool IsInPolygon(Vector2 p)
        {
            if (m_PolygonPoints.Count == 0) return false;
            var inside = false;
            var j = m_PolygonPoints.Count - 1;
            for (int i = 0; i < m_PolygonPoints.Count; j = i++)
            {
                var pi = m_PolygonPoints[i];
                var pj = m_PolygonPoints[j];
                if (((pi.y <= p.y && p.y < pj.y) || (pj.y <= p.y && p.y < pi.y)) &&
                    (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x))
                    inside = !inside;
            }
            return inside;
        }
    }
}
