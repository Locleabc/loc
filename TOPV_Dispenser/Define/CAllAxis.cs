using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using TopCom;
using TopMotion;

namespace TOPV_Dispenser.Define
{
    public class CAllAxis
    {
        public IMotion X1Axis { get; set; }
        public IMotion Y1Axis { get; set; }
        public IMotion Z1Axis { get; set; }

        public IMotion X2Axis { get; set; }
        public IMotion Y2Axis { get; set; }
        public IMotion Z2Axis { get; set; }

        [JsonIgnore]
        public ObservableCollection<IMotion> AxisList { get; set; }

        public CAllAxis()
        {
            int axisIndex = 0;
            X1Axis = new MotionAjinAXL(axisIndex++, AxisName.X1Axis);
            Y1Axis = new MotionAjinAXL(axisIndex++, AxisName.Y1Axis);
            Z1Axis = new MotionAjinAXL(axisIndex++, AxisName.Z1Axis);
            X2Axis = new MotionAjinAXL(axisIndex++, AxisName.X2Axis);
            Y2Axis = new MotionAjinAXL(axisIndex++, AxisName.Y2Axis);
            Z2Axis = new MotionAjinAXL(axisIndex++, AxisName.Z2Axis);

            AxisList = new ObservableCollection<IMotion>
            {
                X1Axis,
                Y1Axis,
                Z1Axis,
                X2Axis,
                Y2Axis,
                Z2Axis,
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

                this.X1Axis.Speed = tmpAllAxis.X1Axis.Speed;
                this.X1Axis.AllowPositionDiff = tmpAllAxis.X1Axis.AllowPositionDiff;

                this.Y1Axis.Speed = tmpAllAxis.Y1Axis.Speed;
                this.Y1Axis.AllowPositionDiff = tmpAllAxis.Y1Axis.AllowPositionDiff;

                this.Z1Axis.Speed = tmpAllAxis.Z1Axis.Speed;
                this.Z1Axis.AllowPositionDiff = tmpAllAxis.Z1Axis.AllowPositionDiff;

                this.X2Axis.Speed = tmpAllAxis.X2Axis.Speed;
                this.X2Axis.AllowPositionDiff = tmpAllAxis.X2Axis.AllowPositionDiff;

                this.Y2Axis.Speed = tmpAllAxis.Y2Axis.Speed;
                this.Y2Axis.AllowPositionDiff = tmpAllAxis.Y2Axis.AllowPositionDiff;

                this.Z2Axis.Speed = tmpAllAxis.Z2Axis.Speed;
                this.Z2Axis.AllowPositionDiff = tmpAllAxis.Z2Axis.AllowPositionDiff;
            }
            catch { }
        }
    }
}
