using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using TopCom;
using TopMotion;

namespace VCM_FullAssy.Define
{
    public class CAllAxis
    {
        public IMotion XAxis { get; set; }
        public IMotion Y1Axis { get; set; }
        public IMotion Y2Axis { get; set; }
        public IMotion ZAxis { get; set; }
        public IMotion TAxis { get; set; }

        [JsonIgnore]
        public ObservableCollection<IMotion> AxisList { get; set; }

        public CAllAxis()
        {
            XAxis = new MotionPlusE(8, AxisName.XAxis);
            Y1Axis = new MotionPlusE(9, AxisName.Y1Axis);
            Y2Axis = new MotionPlusE(10, AxisName.Y2Axis);
            ZAxis = new MotionPlusE(11, AxisName.ZAxis);
            TAxis = new MotionPlusE(12, AxisName.TAxis);

            AxisList = new ObservableCollection<IMotion>
            {
                XAxis,
                Y1Axis,
                Y2Axis,
                ZAxis,
                TAxis,
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
            CAllAxis tmpAllAxis = JsonConvert.DeserializeObject<CAllAxis>(SerializeString);

            this.XAxis.Speed = tmpAllAxis.XAxis.Speed;
            this.Y1Axis.Speed = tmpAllAxis.Y1Axis.Speed;
            this.Y2Axis.Speed = tmpAllAxis.Y2Axis.Speed;
            this.ZAxis.Speed = tmpAllAxis.ZAxis.Speed;
            this.TAxis.Speed = tmpAllAxis.TAxis.Speed;
        }
    }
}
