using System;
using System.Collections.Generic;
using System.ServiceModel;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AssociateDisassociate
{
    public class Class1 : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                EntityReference targetEntity = null;
                string strRelationshipName = string.Empty;
                EntityReferenceCollection relatedEntities = null;
                EntityReference relatedEntity = null;

                if (context.InputParameters.Contains("Relationship"))
                {
                    strRelationshipName = context.InputParameters["Relationship"].ToString();
                }

                if (strRelationshipName != "new_new_entitybtest_new_entityatest.")
                {
                    return;
                }

                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is EntityReference 
                    && context.InputParameters.Contains("RelatedEntities") && context.InputParameters["RelatedEntities"] is EntityReferenceCollection)
                {
                    targetEntity = (EntityReference)context.InputParameters["Target"];
                    relatedEntities = context.InputParameters["RelatedEntities"] as EntityReferenceCollection;
                    relatedEntity = relatedEntities[0];

                    if (context.MessageName == "Associate")
                    {
                        Entity entityBTest1 = new Entity("new_entitybtest");
                        entityBTest1.Id = targetEntity.Id;
                        entityBTest1["new_entitytesta"] = new EntityReference("new_entityatest", relatedEntity.Id);
                        service.Update(entityBTest1);
                    }
                    if (context.MessageName == "Disassociate")
                    {
                        Entity entityBTest1 = new Entity("new_entitybtest");
                        entityBTest1.Id = relatedEntity.Id;
                        entityBTest1["new_entitytesta"] = null;
                        service.Update(entityBTest1);
                    }
                }
            }
            catch (FaultException<OrganizationServiceFault> ex)
            {
                throw new InvalidPluginExecutionException("An error occurred in the Associate/Disassociate plug-in.", ex);
            }
            catch (Exception ex)
            {
                tracingService.Trace("Associate/Disassociate: {0}", ex.ToString());
                throw;
            }
        }
    }
}