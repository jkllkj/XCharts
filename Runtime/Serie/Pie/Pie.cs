
namespace XCharts.Runtime
{
    [System.Serializable]
    [SerieConvert(typeof(Line), typeof(Bar))]
    [SerieHandler(typeof(PieHandler), true)]
    [DefaultAnimation(AnimationType.Clockwise)]
    [SerieExtraComponent(typeof(LabelStyle), typeof(LabelLine), typeof(IconStyle), typeof(Emphasis))]
    public class Pie : Serie
    {
        public override bool useDataNameForColor { get { return true; } }

        public static Serie AddDefaultSerie(BaseChart chart, string serieName)
        {
            var serie = chart.AddSerie<Pie>(serieName);
            chart.AddData(serie.index, 70, "pie1");
            chart.AddData(serie.index, 20, "pie2");
            chart.AddData(serie.index, 10, "pie3");
            return serie;
        }

        public static Pie CovertSerie(Serie serie)
        {
            var newSerie = SerieHelper.CloneSerie<Pie>(serie);
            return newSerie;
        }
    }
}