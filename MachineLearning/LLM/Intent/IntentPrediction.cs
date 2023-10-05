using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.MachineLearning.LLM.Intent
{
    public class IntentPrediction
    {
        [ColumnName("PredictedLabel")]
        public uint Label { get; set; }

        public float[] Score { get; set; } 
    }
}
