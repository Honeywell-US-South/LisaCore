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
                            var faults = equipment.GetBehaviorFaultValues();
                            var alert = equipment.GetAlert();
                            if (faults.Any())
                            {

                                if (alert.Status != BrickSchema.Net.Alerts.AlertStatuses.Active
                                    && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned
                                    && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.Active;
                                    alert.Timestamp = DateTime.UtcNow;
                                    alert.Activities.Add(new()
                                    {
                                        Activity = "Status changed.",
                                        Description = BrickSchema.Net.Alerts.AlertStatuses.Active.ToString()
                                    });
                                } else if (alert.Status != BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned;
                                    alert.Timestamp = DateTime.UtcNow;
                                    alert.Activities.Add(new()
                                    {
                                        Activity = "Status changed.",
                                        Description = BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned.ToString()
                                    });
                                } else
                                {
                                    var clone = alert.Clone(includeActivities:true);
                                    alert.Set(new() { Status = BrickSchema.Net.Alerts.AlertStatuses.Active });
                                }

                                double serverity = 100;
                                double priority = 100;
                                if (alert.Severity != serverity)
                                {
                                    alert.Activities.Add(new()
                                    {
                                        Activity = "Severity changed.",
                                        Description = alert.Severity.ToString("P2")
                                    });
                                    alert.Severity = serverity;
                                }

                                if (alert.Priority != priority)
                                {
                                    alert.Activities.Add(new()
                                    {
                                        Activity = "Priority changed.",
                                        Description = alert.Priority.ToString("P2")
                                    });
                                    alert.Priority = priority;
                                }

                            }
                            else
                            {
                                if (alert.Status == BrickSchema.Net.Alerts.AlertStatuses.Active)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.ReturnToNormal;
                                    alert.Timestamp = DateTime.UtcNow;
                                    alert.Activities.Add(new()
                                    {
                                        Activity = "Status changed.",
                                        Description = BrickSchema.Net.Alerts.AlertStatuses.ReturnToNormal.ToString()
                                    });
                                   
                                } else if (alert.Status == BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned;
                                    alert.Timestamp = DateTime.UtcNow;
                                    alert.Activities.Add(new()
                                    {
                                        Activity = "Status changed.",
                                        Description = BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned.ToString()
                                    });
                                }

                                if (!alert.Latch && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.WorkAssigned 
                                    && alert.Status != BrickSchema.Net.Alerts.AlertStatuses.RtnWorkAssigned)
                                {
                                    alert.Status = BrickSchema.Net.Alerts.AlertStatuses.Cleared;
                                    alert.Timestamp = DateTime.UtcNow;
                                    alert.Activities.Add(new()
                                    {
                                        Activity = "Status changed.",
                                        Description = BrickSchema.Net.Alerts.AlertStatuses.Cleared.ToString()
                                    });
                                }
                            }

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
