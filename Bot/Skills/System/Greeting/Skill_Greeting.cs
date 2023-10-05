using LisaCore.Bot.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.Bot.Skills.System.Greeting
{
    public class Skill_Greeting : Skill
    {
        public Skill_Greeting() : base("System Greeting", Skill.SkillType.Greeting)
        {

        }

        protected override Result Execute(Conversation conversation)
        {

            return base.Execute(conversation);
        }
    }
}
