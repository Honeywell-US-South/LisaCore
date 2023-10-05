using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.MachineLearning.LLM.Intent
{
    public class IntentPredicted
    {
        public string Label { get; set; }
        public float Score { get; set; }
    }
}
