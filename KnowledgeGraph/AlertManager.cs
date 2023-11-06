using BrickSchema.Net;
using BrickSchema.Net.Classes;
using BrickSchema.Net.Classes.Equipments;
using OpenNLP.Tools.Graphs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LisaCore.KnowledgeGraph
{
    public class AlertManager
    {
        private readonly Timer timer;
        private readonly object lockObject = new object();
        private bool isCallbackInProgress = false;
        private BrickSchemaManager _graph;
        public AlertManager(BrickSchemaManager graph)
        {
            _graph = graph;
            timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(60));
        }

        private void TimerCallback(object state)
        {
            if (isCallbackInProgress)
            {
                // Skip the timer event if a callback is already in progress
                return;
            }

            lock (lockObject)
            {
                if (isCallbackInProgress)
                {
                    // Skip the timer event if a callback is already in progress
                    return;
                }

                isCallbackInProgress = true;
            }

            try
            {

                lock (lockObject)
                {
                   
                    //Do work
                    var bricks = _graph.GetEntities();
                    foreach (var brick in bricks)
                    {
                        if (brick is Equipment equipment)
                        {

                            double serverity = equipment.GetRelationshipDependancySore();
                            double priority = 0;
                            var faults = equipment.GetBehaviorFaultValues();
                            var alert = equipment.GetAlert();
                            alert.Activities = new();
                            if (faults.Where(x => x.GetValue<bool>()).Any())
                            {
                                foreach (var fault in faults)
                                {
                                    var behavior = equipment.GetBehaviorById(fault.BehaviorId);

                                    double p = (fault.Weight / 2) * 100;
                                    priority = Math.Max(p, priority);

                                }
                                if (alert.Status != BrickSchema.Net.Alerts.AlertStatuses.Active
                                    && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned
                                    && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.Active;
                                    alert.Timestamp = DateTime.UtcNow;

                                }
                                else if (alert.Status != BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned;
                                    alert.Timestamp = DateTime.UtcNow;

                                }
                                else
                                {
                                    var clone = alert.Clone(includeActivities: true);
                                    alert.Set(new() { Status = BrickSchema.Net.Alerts.AlertStatuses.Active });
                                }




                            }
                            else
                            {
                                alert.FaultBehaviorIds = new();
                                if (alert.Status == BrickSchema.Net.Alerts.AlertStatuses.Active)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.ReturnToNormal;
                                    alert.Timestamp = DateTime.UtcNow;


                                }
                                else if (alert.Status == BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned;
                                    alert.Timestamp = DateTime.UtcNow;

                                }

                                if (!alert.Latch && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned
                                    && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.Cleared;
                                    alert.Timestamp = DateTime.UtcNow;

                                }
                            }


                            alert.Severity = serverity;
                            alert.Priority = priority;

                            equipment.SetAlert(alert);
                        }
                    }
                }
            }
            finally
            {
                lock (lockObject)
                {
                    isCallbackInProgress = false;
                }
            }
        }
    }
}
