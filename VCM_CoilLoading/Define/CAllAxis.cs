using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using TopCom;
using TopMotion;

namespace VCM_CoilLoading.Define
{
    public class CAllAxis
    {
        public IMotion X1Axis { get; set; }
        public IMotion X2Axis { get; set; }
        public IMotion ZZ1Enc { get; set; }
        public IMotion ZZ2Enc { get; set; }

        public IMotion Y1Axis { get; set; }
        public IMotion Y2Axis { get; set; }
        public IMotion Z1Axis { get; set; }
        public IMotion Z2Axis { get; set; }
        public IMotion Z3Axis { get; set; }
        public IMotion T1Axis { get; set; }
        public IMotion T2Axis { get; set; }
        public IMotion XXAxis { get; set; }
        public IMotion YY1Axis { get; set; }
        public IMotion YY2Axis { get; set; }

        [JsonIgnore]
        public ObservableCollection<IMotion> AxisList { get; set; }

        public CAllAxis()
        {
            X1Axis = new MotionFas16000(0, AxisName.X1Axis);
            X2Axis = new MotionFas16000(1, AxisName.X2Axis);
            ZZ1Enc = new MotionFas16000(2, AxisName.ZZ1Enc)
            {
                IsEncOnly = true
            };
            ZZ2Enc = new MotionFas16000(3, AxisName.ZZ2Enc)
            {
                IsEncOnly = true
            };
            Y1Axis = new MotionPlusE(8, AxisName.Y1Axis);
            Y2Axis = new MotionPlusE(9, AxisName.Y2Axis);
            Z1Axis = new MotionPlusE(10, AxisName.Z1Axis);
            Z2Axis = new MotionPlusE(11, AxisName.Z2Axis);
            Z3Axis = new MotionPlusE(12, AxisName.Z3Axis);
            T1Axis = new MotionPlusE(13, AxisName.T1Axis);
            T2Axis = new MotionPlusE(14, AxisName.T2Axis);
            XXAxis = new MotionPlusE(15, AxisName.XXAxis)
            {
                AllowPositionDiff = 0.1,
            };
            YY1Axis = new MotionPlusE(16, AxisName.YY1Axis)
            {
                AllowPositionDiff = 0.1,
            };
            YY2Axis = new MotionPlusE(17, AxisName.YY2Axis)
            {
                AllowPositionDiff = 0.1,
            };

            AxisList = new ObservableCollection<IMotion>
            {
                X1Axis,
                X2Axis,
                ZZ1Enc,
                ZZ2Enc,
                Y1Axis,
                Y2Axis,
                Z1Axis,
                Z2Axis,
                Z3Axis,
                T1Axis,
                T2Axis,
                XXAxis,
                YY1Axis,
                YY2Axis,
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

            this.X1Axis.Speed = tmpAllAxis.X1Axis.Speed;
            this.X1Axis.AllowPositionDiff = tmpAllAxis.X1Axis.AllowPositionDiff;

            this.X2Axis.Speed = tmpAllAxis.X2Axis.Speed;
            this.X2Axis.AllowPositionDiff = tmpAllAxis.X2Axis.AllowPositionDiff;

            this.ZZ1Enc.Speed = tmpAllAxis.ZZ1Enc.Speed;
            this.ZZ1Enc.AllowPositionDiff = tmpAllAxis.ZZ1Enc.AllowPositionDiff;

            this.ZZ2Enc.Speed = tmpAllAxis.ZZ2Enc.Speed;
            this.ZZ2Enc.AllowPositionDiff = tmpAllAxis.ZZ2Enc.AllowPositionDiff;



            this.Y1Axis.Speed = tmpAllAxis.Y1Axis.Speed;
            this.Y1Axis.AllowPositionDiff = tmpAllAxis.Y1Axis.AllowPositionDiff;

            this.Y2Axis.Speed = tmpAllAxis.Y2Axis.Speed;
            this.Y2Axis.AllowPositionDiff = tmpAllAxis.Y2Axis.AllowPositionDiff;

            this.Z1Axis.Speed = tmpAllAxis.Z1Axis.Speed;
            this.Z1Axis.AllowPositionDiff = tmpAllAxis.Z1Axis.AllowPositionDiff;

            this.Z2Axis.Speed = tmpAllAxis.Z2Axis.Speed;
            this.Z2Axis.AllowPositionDiff = tmpAllAxis.Z2Axis.AllowPositionDiff;

            this.Z3Axis.Speed = tmpAllAxis.Z3Axis.Speed;
            this.Z3Axis.AllowPositionDiff = tmpAllAxis.Z3Axis.AllowPositionDiff;

            this.T1Axis.Speed = tmpAllAxis.T1Axis.Speed;
            this.T1Axis.AllowPositionDiff = tmpAllAxis.T1Axis.AllowPositionDiff;

            this.T2Axis.Speed = tmpAllAxis.T2Axis.Speed;
            this.T2Axis.AllowPositionDiff = tmpAllAxis.T2Axis.AllowPositionDiff;

            this.XXAxis.Speed = tmpAllAxis.XXAxis.Speed;
            this.XXAxis.AllowPositionDiff = tmpAllAxis.XXAxis.AllowPositionDiff;

            this.YY1Axis.Speed = tmpAllAxis.YY1Axis.Speed;
            this.YY1Axis.AllowPositionDiff = tmpAllAxis.YY1Axis.AllowPositionDiff;

            this.YY2Axis.Speed = tmpAllAxis.YY2Axis.Speed;
            this.YY2Axis.AllowPositionDiff = tmpAllAxis.YY2Axis.AllowPositionDiff;
        }
    }
}
