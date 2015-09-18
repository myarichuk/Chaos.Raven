using Nancy;
using System;
using System.Linq;

namespace Chaos.Raven.Service
{
    public class ServiceControllerModule : NancyModule
    {
        public ServiceControllerModule()
        {
            Get["/"] = _ =>
            {
                return Response.AsJson(new
                {
                    ActiveActionCount = ChaosService.ExecutingActions.Count,
                    LastTimeActionStarted = ChaosService.ExecutingActions.Count > 0 ? ChaosService.ExecutingActions.Max(x => x.Value) : (DateTime?)null,
                    ErrorCount = ChaosService.ActionErrors.Count,                    
                    ActionSummary = (from actionTypeName in ChaosService.ExecutingActions.Select(x => x.Key.GetType().Name)
                                    group actionTypeName by actionTypeName into g
                                    select new
                                    {
                                        Type = g.Key,
                                        Count = g.Count()
                                    }).ToList()
                });
            };

            Get["/errors"] = _ => Response.AsJson(ChaosService.ActionErrors);

            Get["/actions"] = _ =>
            {
                var actionSummary = ChaosService.ExecutingActions.Select(x => new { ActionType = x.Key.GetType().Name, WhenStarted = x.Value }).ToList();
                return Response.AsJson(actionSummary);
            };
        }
    }
}
