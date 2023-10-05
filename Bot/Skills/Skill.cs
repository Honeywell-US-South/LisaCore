using BrickSchema.Net.Shapes;
using LisaCore.Bot.Conversations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.Bot.Skills
{
    public class Skill
    {
        public enum SkillType
        {
            // Technical Skills
            HvacControl,
            EnergyManagement,
            LightingControl,
            SecuritySystems,
            ElevatorManagement,
            FireSafety,
            Networking,
            FaultDetection,
            PredictiveMaintenance,
            AnalyticsAndReporting,
            AccessControl,
            EmergencyResponse,
            WaterManagement,
            WasteManagement,
            UserExperienceCustomization,
            RoomScheduling,
            OccupancyMonitoring,
            AudioVisualSystems,
            SoftwareUpdates,
            ComplianceMonitoring,
            SystemDiagnostics,
            DataBackup,

            // Soft Skills
            Greeting,
            Farewell,
            CustomerService,
            ConflictResolution,
            TimeManagement,
            EmotionalIntelligence,
            Adaptability,
            ActiveListening,
            FeedbackHandling,
            ProactiveUpdates,
            TaskPrioritization,
            LanguageTranslation,
            SocialEngagement,
            TourGuiding,
            EventNotification
        }

        public string Name { get; set; }

        public SkillType Type { get; set; }

        public Skill (string name, SkillType type)
        {
            Name = name;
            Type = type;
        }

        protected virtual Result Execute(Conversation conversation)
        {
            if (conversation == null)
            {
                return new Result();
            }

            return new();
        }

    }
}
