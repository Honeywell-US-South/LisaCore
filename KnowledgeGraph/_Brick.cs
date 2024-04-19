using BrickSchema.Net.Classes;
using BrickSchema.Net.Relationships;
using BrickSchema.Net;
using LisaCore.KnowledgeGraph.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using BrickSchema.Net.Shapes;
using BrickSchema.Net.Classes.Equipments.HVACType;
using System.Xml.Linq;
using System.Reflection.Metadata.Ecma335;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using BrickSchema.Net.Behaviors;
using BrickSchema.Net.ThreadSafeObjects;


//Keep this as LisaCore
namespace LisaCore
{
    /// <summary>
    /// Part of Lisa class
    /// </summary>
    public partial class Lisa
    {
        public event EventHandler<BehaviorExecutedEventArgs> OnBehaviorExecuted;

        public BrickSchemaManager Graph { get { return _graph; } }

        public BrickEntity? GetEntity(string id, bool byRefrence = false)
        {
            var entity = Graph.GetEntity(id, byRefrence);
            return entity;
        }

        public ThreadSafeList<BrickEntity> GetEntities()
        {
            return Graph.GetEntities();
        }

        public void UpdateBehaviorsProperty() => Graph.UpdateBehaviorsProperty();
		public void GetEntities(ThreadSafeList<BrickEntity> entities, List<string>? entityIds = null)
        {
            
            if (entityIds == null || entityIds?.Count == 0)
            {
                Graph.GetEntities(entities);
            }
            else
            {
                ThreadSafeList<BrickEntity> list = new();

				Graph.GetEntities(list);
                entities = list.Where(x=> entityIds?.Contains(x.Id)??false).ToThreadSafeList();
            }
  
        }
        public BrickEntity? GetEntitiy(string id)
        {
            ThreadSafeList<BrickEntity> entities = new();
			GetEntities(entities, new() { id});
            return entities.FirstOrDefault();
		}

        public ThreadSafeList<BrickEntity> GetEntities<T>(ThreadSafeList<BrickEntity> entities, List<string>? entityIds = null) where T : BrickEntity
        {

            if (entityIds == null || entityIds?.Count == 0)
            {
                Graph.GetEntities<T>(entities);
            }
            else
            {
				ThreadSafeList<BrickEntity> list = new();

				Graph.GetEntities<T>(list);
				entities = list.Where(x => entityIds?.Contains(x.Id) ?? false).ToThreadSafeList();
            }
            return entities;
        }

        public ThreadSafeList<BrickEntity> GetEquipmentEntities(List<string>? equipmentIds = null, bool byReference = true)
        {
            ThreadSafeList<BrickEntity> equipments = _graph.GetEquipments(equipmentIds ?? new(), byReference); ;
            return equipments;
        }

        public ThreadSafeList<BrickBehavior> GetEquipmentBehaviors(string equipmentId, bool byReference = true)
        {
            var brickBehaviors = _graph.GetEquipmentBehaviors(equipmentId, byReference);
            return brickBehaviors;
        }

        public Dictionary<string, string> GetRegisteredEquipmentBehaviors(string equipmentId, bool byReference = true)
        {
            var brickBehaviors = _graph.GetRegisteredEquipmentBehaviors(equipmentId, byReference);
            return brickBehaviors;
        }

        public ThreadSafeList<BrickBehavior> GetBehaviors(List<string>? behaviorIds = null, bool byReference = true)
        {
            var brickBehaviors = _graph.GetBehaviors(behaviorIds ?? new(), byReference);


            return brickBehaviors;
        }

        

        public ThreadSafeList<BrickBehavior> GetBehaviorsByShapeType(BehaviorFunction.Types type, List<string>? behaviorIds = null, bool byReference = true)
        {
            var brickBehaviors = _graph.GetBehaviorsByShapeType(behaviorIds ?? new(), type, byReference);


            return brickBehaviors;
        }

        public async Task SaveGraphAsync()
        {
            await _graph.SaveSchemaAsync();
        }

        //public void ArchiveEntityProperties(string entityId, int olderThanDays = 30)
        //{
        //    _graph.ArchiveEntityProperties(entityId, olderThanDays = 30);
        //}

        public Tenant AddTenant(string id, string name)
        {

			var tenant = _graph.AddTenant(id);
            tenant.Name = name;
            return tenant;
        }
        public bool IsTenant(BrickEntity entity)
        {
            return entity.EntityTypeName == typeof(Tenant).Name;
        }

        public Location? AddLocation(LocationTypes type, string id, string name)
        {

            switch (type)
            {
                case LocationTypes.Building:
                    var building = _graph.AddLocationBuilding(id);
                    if (!building.Name.Equals(name))
                    {
                        building.Name = name;
        
                    }
                    return building;
                case LocationTypes.CommonSpace:
                    var commonSpace = _graph.AddLocationCommonSpace(id);
                    if (!commonSpace.Name.Equals(name))
                    {
                        commonSpace.Name = name;
          
                    }
                    return commonSpace;
                case LocationTypes.Entrance:
                    var enterance = _graph.AddLocationEntrance(id);
                    if (!enterance.Name.Equals(name))
                    {
                        enterance.Name = name;
                
                    }
                    return enterance;
                case LocationTypes.Floor:
                    var floor = _graph.AddLocationFloor(id);
                    if (!floor.Name.Equals(name))
                    {
                        floor.Name = name;
             
                    }
                    return floor;
                case LocationTypes.GateHouse:
                    var gateHouse = _graph.AddLocationGateHouse(id);
                    if (!gateHouse.Name.Equals(name))
                    {
                        gateHouse.Name = name;
          
                    }
                    return gateHouse;
                case LocationTypes.MediaHotDesk:
                    var mediaHotDesk = _graph.AddLocationMediaHotDesk(id);
                    if (!mediaHotDesk.Name.Equals(name))
                    {
                        mediaHotDesk.Name = name;
              
                    }
                    return mediaHotDesk;
                case LocationTypes.OutdoorArea:
                    var outdoorArea = _graph.AddLocationOutdoorArea(id);
                    if (!outdoorArea.Name.Equals(name))
                    {
                        outdoorArea.Name = name;
           
                    }
                    return outdoorArea;
                case LocationTypes.Outside:
                    var outside = _graph.AddLocationOutside(id);
                    if (!outside.Name.Equals(name))
                    {
                        outside.Name = name;
      
                    }
                    return outside;
                case LocationTypes.Parking:
                    var parking = _graph.AddLocationParking(id);
                    if (!parking.Name.Equals(name))
                    {
                        parking.Name = name;
          
                    }
                    return parking;
                case LocationTypes.Region:
                    var region = _graph.AddLocationRegion(id);
                    if (!region.Name.Equals(name))
                    {
                        region.Name = name;
    
                    }
                    return region;
                case LocationTypes.Room:
                    var room = _graph.AddLocationRoom(id);
                    if (!room.Name.Equals(name))
                    {
                        room.Name = name;
         
                    }
                    return room;
                case LocationTypes.Site:
                    var site = _graph.AddLocationSite(id);
                    if (!site.Name.Equals(name))
                    {
                        site.Name = name;
       
                    }
                    return site;
                case LocationTypes.Space:
                    var space = _graph.AddLocationSpace(id);
                    if (!space.Name.Equals(name))
                    {
                        space.Name = name;
             
                    }
                    return space;
                case LocationTypes.Storey:
                    var storey = _graph.AddLocationBasement(id);
                    if (!storey.Name.Equals(name))
                    {
                        storey.Name = name;
            
                    }
                    return storey;
                case LocationTypes.TicketingBooth:
                    var ticketingBooth = _graph.AddLocationTicketingBooth(id);
                    if (!ticketingBooth.Name.Equals(name))
                    {
                        ticketingBooth.Name = name;
           
                    }
                    return ticketingBooth;
                case LocationTypes.Tunnel:
                    var tunnel = _graph.AddLocationTunnel(id);
                    if (!tunnel.Name.Equals(name))
                    {
                        tunnel.Name = name;
      
                    }
                    return tunnel;
                case LocationTypes.VerticalSpace:
                    var verticalSpace = _graph.AddLocationVerticalSpace(id);
                    if (!verticalSpace.Name.Equals(name))
                    {
                        verticalSpace.Name = name;
   
                    }
                    return verticalSpace;
                case LocationTypes.WaterTank:
                    var waterTank = _graph.AddLocationWaterTank(id);
                    if (!waterTank.Name.Equals(name))
                    {
                        waterTank.Name = name;

                    }
                    return waterTank;
                case LocationTypes.Wing:
                    var wing = _graph.AddLocationWing(id);
                    if (!wing.Name.Equals(name))
                    {
                        wing.Name = name;

                    }
                    return wing;
                case LocationTypes.Zone:
                    var zone = _graph.AddLocationZone(id);
                    if (!zone.Name.Equals(name))
                    {
                        zone.Name = name;
                    
                    }
                    return zone;
                case LocationTypes.ChilledWaterPlant:
                    var cwp = _graph.AddLocationChilledWaterPlant(id);
                    if (!cwp.Name.Equals(name))
                    {
                        cwp.Name = name;
                      
                    }
                    return cwp;
                case LocationTypes.HotWaterPlant:
                    var hwp = _graph.AddLocationHotWaterPlant(id);
                    if (!hwp.Name.Equals(name))
                    {
                        hwp.Name = name;

                    }
                    return hwp;
            }
            return null;
           
        }

        public bool IsLocation(BrickEntity entity)
        {
            return Enum.TryParse<LocationTypes>(entity.EntityTypeName, out var location);
        }

        public Equipment? AddEquipment(EquipmentTypes type, string id, string name)
        {
     
            switch (type)
            {
                case EquipmentTypes.AHU:
                    
                    var ahu = _graph.AddEquipmentHVACAHU(id);
                    if (!ahu.Name.Equals(name))
                    {
                        ahu.Name = name;
                    }
                    return ahu;
                case EquipmentTypes.Chiller:
                    var chiller = _graph.AddEquipmentHVACChiller(id);
                    if (!chiller.Name.Equals(name))
                    {
                        chiller.Name = name;
                       
                    }
                    return chiller;
                case EquipmentTypes.CoolingTower:
                    var coolingTower = _graph.AddEquipmentHVACCoolingTower(id);
                    if (!coolingTower.Name.Equals(name))
                    {
                        coolingTower.Name = name;
                       
                    }
                    return coolingTower;
                case EquipmentTypes.Fan:
                    var fan = _graph.AddEquipmentHVACFan(id);
                    if (!fan.Name.Equals(name))
                    {
                        fan.Name = name;
                        
                    }
                    return fan;
                case EquipmentTypes.FCU:
                    var fcu = _graph.AddEquipmentHVACTerminalUnitFCU(id);
                    if (!fcu.Name.Equals(name))
                    {
                        fcu.Name = name;
                       
                    }
                    return fcu;
                case EquipmentTypes.Meter:
                    var meter = _graph.AddEquipmentMeter(id);
                    if (!meter.Name.Equals(name))
                    {
                        meter.Name = name;
                      
                    }
                    return meter;
                case EquipmentTypes.Pump:
                    var pump = _graph.AddEquipmentHVACPump(id);
                    if (!pump.Name.Equals(name))
                    {
                        pump.Name = name;
                       
                    }
                    return pump;
                case EquipmentTypes.VAV:
                    var vav = _graph.AddEquipmentHVACTerminalUnitVAV(id);
                    if (!vav.Name.Equals(name))
                    {
                        vav.Name = name;
                       
                    }
                    return vav;
                case EquipmentTypes.ThermalStorage:
                    var ts = _graph.AddEquipmentThermalStorage(id);
                    if (!ts.Name.Equals(name))
                    {
                        ts.Name = name;

                    }
                    return ts;
                case EquipmentTypes.WaterDistribution:
                    var wd = _graph.AddEquipmentWaterDistribution(id);
                    if (!wd.Name.Equals(name))
                    {
                        wd.Name = name;

                    }
                    return wd;
            }
            return null;   
        }

        public bool IsEquipment(BrickEntity entity)
        {
            return Enum.TryParse<EquipmentTypes>(entity.EntityTypeName, true, out var equipmentType);
           
        }
        public Point? AddPoint(PointTypes type, string id, string name, object? readFunction = null, object? writeFunction = null)
        {
            bool save = !_graph.IsEntity(id);
            switch (type)
            {
                case PointTypes.Alarm:
                    var alarm = _graph.AddPointAlarm(id);
                    if (!alarm.Name.Equals(name))
                    {
                        alarm.Name = name;
                        save = true;
                    }
                    return alarm;
                case PointTypes.Command:
                    var cmd = _graph.AddPointCommand(id);
                    if (!cmd.Name.Equals(name))
                    {
                        cmd.Name = name;
                        save = true;
                    }
                    return cmd;
                case PointTypes.Parameter:
                    var parameter = _graph.AddPointParameter(id);
                    if (!parameter.Name.Equals(name))
                    {
                        parameter.Name = name;
                        save = true;
                    }
                    return parameter;
                case PointTypes.Sensor:
                    var sensor = _graph.AddPointSensor(id);
                    if (!sensor.Name.Equals(name))
                    {
                        sensor.Name = name;
                        save = true;
                    }
                    return sensor;
                case PointTypes.Setpoint:
                    var setpoint = _graph.AddPointSetpoint(id);
                    if (!setpoint.Name.Equals(name))
                    {
                        setpoint.Name = name;
                        save = true;
                    }
                    return setpoint;
                case PointTypes.Status:
                    var status = _graph.AddPointStatus(id);
                    if (!status.Name.Equals(name))
                    {
                        status.Name = name;
                        save = true;
                    }
                    return status;
            }
            return null;
        }

        public bool IsPoint(BrickEntity entity)
        {
            return Enum.TryParse<PointTypes>(entity.EntityTypeName, true, out var equipmentType);

        }
        public Tag? AddTag(string name)
        {
            bool save = !_graph.IsTag(name);
            var tag = _graph.AddTag(name);

            return tag;
        }

        public Tag? GetTag(string name, bool byReference = false)
        {
            return _graph.GetTag(name, byReference);
        }

        public void AddRelationship(RelationshipTypes type, string id, string parentId)
        {
            var entity = _graph.GetEntity(id, true);
            if (entity != null)
            {
                string RelationshipName = string.Empty;
                switch (type)
                {
                    case RelationshipTypes.AssociatedWith:
                        RelationshipName = typeof(AssociatedWith).Name;
                        break;
                    case RelationshipTypes.Fedby:
                        RelationshipName = typeof(FedBy).Name;
                        break;
                    case RelationshipTypes.LocationOf:
                        RelationshipName = typeof(LocationOf).Name;
                        break;
                    case RelationshipTypes.MeterBy:
                        RelationshipName = typeof(MeterBy).Name;
                        break;
                    case RelationshipTypes.PartOf:
                        RelationshipName = typeof(PartOf).Name;
                        break;
                    case RelationshipTypes.PointOf:
                        RelationshipName = typeof(PointOf).Name; ;
                        break;
                    case RelationshipTypes.SubmeterOf:
                        RelationshipName = typeof(SubmeterOf).Name;
                        break;
                    case RelationshipTypes.TagOf:
                        RelationshipName = typeof(TagOf).Name;
                        break;
                }
                var relationship = entity.Relationships.FirstOrDefault(x => x.ParentId.Equals(parentId) && (x.EntityTypeName?.Equals(RelationshipName) ?? false));
                if (relationship == null)
                {
                    switch (type)
                    {
                        case RelationshipTypes.AssociatedWith:
                            entity.AddRelationshipAssociatedWith(parentId);
                            break;
                        case RelationshipTypes.Fedby:
                            entity.AddRelationshipFedBy(parentId);
                            break;
                        case RelationshipTypes.LocationOf:
                            entity.AddRelationshipLocationOf(parentId);
                            break;
                        case RelationshipTypes.MeterBy:
                            entity.AddRelationshipMeterBy(parentId);
                            break;
                        case RelationshipTypes.PartOf:
                            entity.AddRelationshipPartOf(parentId);
                            break;
                        case RelationshipTypes.PointOf:
                            entity.AddRelationshipPointOf(parentId);
                            break;
                        case RelationshipTypes.SubmeterOf:
                            entity.AddRelationshipSubmeterOf(parentId);
                            break;
                        case RelationshipTypes.TagOf:
                            entity.AddRelationshipTagOf(parentId);
                            break;
                    }
                    
                }
            }
        }

        public void AddBehavior(string entityId, BrickBehavior behavior)
        {
            if (behavior == null) return;

            var entity = _graph.GetEntity(entityId, true);
            if (entity != null)
            {
                var behaviors = entity.GetBehaviors(behavior.EntityTypeName, true);
                if (behaviors.Count >= 1)
                {
                    for (int i = 0; i < behaviors.Count - 1; i++)
                    {
                        _behaviorManager.Unsubscribe(behaviors[i].OnTimerTick);
                        entity.RemoveBehavior(behaviors[i]);
                    }
                    var foundBehavior = behaviors[behaviors.Count - 1];
                    if (foundBehavior.Parent == null) foundBehavior.Parent = entity;
                }
                else
                {
                    if (entity.RegisteredBehaviors.ContainsKey(behavior.EntityTypeName))
                    {
                        behavior.Id = entity.RegisteredBehaviors[behavior.EntityTypeName];
                    }
                    else
                    {
                        entity.RegisteredBehaviors.Add(behavior.EntityTypeName, behavior.Id);
                    }
                    if (!behavior.IsLogger)
                    {
                        behavior.SetLogger(_logger);
                    }
                    behavior.Parent = entity; //must set this before start
                    behavior.OnBehaviorExecuted += ProcessOnBehaviorExecuted;
                    behavior.Start();
                    entity.Behaviors.Add(behavior);

                    _behaviorManager.Subscribe(behavior.OnTimerTick);
                    
                }
            }

        }

        private void ProcessOnBehaviorExecuted(object? sender, BehaviorExecutedEventArgs e)
        {
            OnBehaviorExecuted?.Invoke(sender, e);
        }
        public void StartBehavior(string entityId, string type)
        {
            var entity = _graph.GetEntity(entityId, true);
            if (entity != null)
            {
                var behaviors = entity.GetBehaviors(type, true);
                foreach (var behavior in behaviors)
                {
                    behavior.Start();
                }
            }

        }

        public void StopBehavior(string entityId, string type)
        {
            var entity = _graph.GetEntity(entityId, true);
            if (entity != null)
            {
                var behaviors = entity.GetBehaviors(type, true);
                foreach (var behavior in behaviors)
                {
                    behavior.Stop();
                }
            }

        }

        public Dictionary<int, string> ProcessGraphToContext()
        {
            List<string> sentences = new List<string>();
            ThreadSafeList<BrickEntity> entities = new();
            _graph.GetEntities(entities);
            foreach (var e in entities)
            { 
                sentences.Add($"Entity name {e.GetProperty<string>(BrickSchema.Net.EntityProperties.PropertiesEnum.Name)} is a {e.EntityTypeName}");
                foreach (var property in e.Properties)
                {
                    sentences.Add($"Entity {e.GetProperty<string>(BrickSchema.Net.EntityProperties.PropertiesEnum.Name)} {property.Name} is {property.Value}");
                }
                foreach (var relationship in e.Relationships)
                {
                    string relationshipType = relationship.EntityTypeName;
                    var parent = e.GetEntity(relationship.ParentId);
                    // You can continue to extract more details and form the required sentence.
                    sentences.Add($"Entity {e.GetProperty<string>(BrickSchema.Net.EntityProperties.PropertiesEnum.Name)} of type {e.EntityTypeName} has a relationship of type {relationship.EntityTypeName} with parent name {parent?.GetProperty<string>(BrickSchema.Net.EntityProperties.PropertiesEnum.Name)}.");
                }
            }
            Dictionary<int, string> context = new Dictionary<int, string>();
            for (int i = 0; i < sentences.Count;i++)
            {
                context.Add(i, sentences[i]);
            }
            return context;
        }
    }
}
