using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using TopCom;
using TopMotion;

namespace PLV_BracketAssemble.Define
{
    public class CAllAxis
    {
        public IMotion XAxis { get; set; }
        public IMotion YAxis { get; set; }
        public IMotion XXAxis { get; set; }

        [JsonIgnore]
        public ObservableCollection<IMotion> AxisList { get; set; }

        public CAllAxis()
        {
            XAxis = new MotionLinkPlusR(1, 0, AxisName.XAxis);
            YAxis = new MotionLinkPlusR(1, 1, AxisName.YAxis);
            XXAxis = new MotionLinkPlusR(1, 2, AxisName.XXAxis);

            AxisList = new ObservableCollection<IMotion>
            {
                XAxis,
                YAxis,
                XXAxis,
            };
        }

        [JsonIgnore]
        protected string SerializeString
        {
            get
            {
                string recipeFile, recipeFileName;

                recipeFileName = "AxisSpeed.json";

                recipeFile = Path.Combine(CDef.CurrentRecipeFolder, recipeFileName);
                if (!File.Exists(recipeFile))
                {
                    using (StreamWriter sw = File.AppendText(recipeFile))
                    {
                        sw.WriteLine(JsonConvert.SerializeObject(this, Formatting.Indented));
                    }
                }

                return File.ReadAllText(recipeFile);
            }
        }

        public void Save()
        {
            string recipeFile, recipeFileName, recipeFileContent;

            recipeFileName = "AxisSpeed.json";

            recipeFile = Path.Combine(CDef.CurrentRecipeFolder, recipeFileName);
            recipeFileContent = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (!File.Exists(recipeFile))
            {
                using (StreamWriter sw = File.AppendText(recipeFile))
                {
                    sw.WriteLine(recipeFileContent);
                }
            }

            FileWriter.WriteAllText(recipeFile, recipeFileContent);
        }

        public void Load()
        {
            try
            {
                CAllAxis tmpAllAxis = JsonConvert.DeserializeObject<CAllAxis>(SerializeString);

                this.XAxis.Speed = tmpAllAxis.XAxis.Speed;
                this.XAxis.AllowPositionDiff = tmpAllAxis.XAxis.AllowPositionDiff;

                this.YAxis.Speed = tmpAllAxis.XAxis.Speed;
                this.YAxis.AllowPositionDiff = tmpAllAxis.YAxis.AllowPositionDiff;

                this.XXAxis.Speed = tmpAllAxis.XAxis.Speed;
                this.XXAxis.AllowPositionDiff = tmpAllAxis.XXAxis.AllowPositionDiff;
            }
            catch { }
        }
    }
}
