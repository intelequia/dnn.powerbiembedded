/*
' Copyright (c) 2019 Intelequia
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Profile;
using DotNetNuke.Entities.Users;
using DotNetNuke.Entities.Users.Social;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Scheduling;
using DotNetNuke.Services.Search.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetNuke.PowerBI
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Controller class for DotNetNuke.PowerBI
    /// 
    /// The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    /// DotNetNuke will poll this class to find out which Interfaces the class implements. 
    /// 
    /// The IPortable interface is used to import/export content from a DNN module
    /// 
    /// The ISearchable interface is used by DNN to index the content of a module
    /// 
    /// The IUpgradeable interface allows module developers to execute code during the upgrade 
    /// process for a module.
    /// 
    /// Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class FeatureController : ModuleSearchBase, IPortable, IUpgradeable
    {
        // feel free to remove any interfaces that you don't wish to use
        // (requires that you also update the .dnn manifest file)

        #region Optional Interfaces

        /// <summary>
        /// Gets the modified search documents for the DNN search engine indexer.
        /// </summary>
        /// <param name="moduleInfo">The module information.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();
            //var controller = new ItemController();
            //var items = controller.GetItems(moduleInfo.ModuleID);

            //foreach (var item in items)
            //{
            //    if (item.LastModifiedOnDate.ToUniversalTime() <= beginDate.ToUniversalTime() ||
            //        item.LastModifiedOnDate.ToUniversalTime() >= DateTime.UtcNow)
            //        continue;

            //    var content = string.Format("{0}<br />{1}", item.ItemName, item.ItemDescription);

            //    var searchDocumnet = new SearchDocument
            //    {
            //        UniqueKey = string.Format("Items:{0}:{1}", moduleInfo.ModuleID, item.ItemId),  // any unique identifier to be able to query for your individual record
            //        PortalId = moduleInfo.PortalID,  // the PortalID
            //        TabId = moduleInfo.TabID, // the TabID
            //        AuthorUserId = item.LastModifiedByUserId, // the person who created the content
            //        Title = moduleInfo.ModuleTitle,  // the title of the content, but should be the module title
            //        Description = moduleInfo.DesktopModule.Description,  // the description or summary of the content
            //        Body = content,  // the long form of your content
            //        ModifiedTimeUtc = item.LastModifiedOnDate.ToUniversalTime(),  // a time stamp for the search results page
            //        CultureCode = moduleInfo.CultureCode, // the current culture code
            //        IsActive = true  // allows you to remove the item from the search index (great for soft deletes)
            //    };

            //    searchDocuments.Add(searchDocumnet);
            //}

            return searchDocuments;
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ExportModule implements the IPortable ExportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be exported</param>
        /// -----------------------------------------------------------------------------
        public string ExportModule(int moduleId)
        {
            //var controller = new ItemController();
            //var items = controller.GetItems(moduleId);
            //var sb = new StringBuilder();

            //var itemList = items as IList<Item> ?? items.ToList();

            //if (!itemList.Any()) return string.Empty;

            //sb.Append("<Items>");

            //foreach (Item item in itemList)
            //{
            //    sb.Append("<Item>");

            //    sb.AppendFormat("<AssignedUserId>{0}</AssignedUserId>", item.AssignedUserId);
            //    sb.AppendFormat("<CreatedByUserId>{0}</CreatedByUserId>", item.CreatedByUserId);
            //    sb.AppendFormat("<CreatedOnDate>{0}</CreatedOnDate>", item.CreatedOnDate);
            //    sb.AppendFormat("<ItemId>{0}</ItemId>", item.ItemId);
            //    sb.AppendFormat("<ItemDescription>{0}</ItemDescription>", XmlUtils.XMLEncode(item.ItemDescription));
            //    sb.AppendFormat("<ItemName>{0}</ItemName>", XmlUtils.XMLEncode(item.ItemName));
            //    sb.AppendFormat("<LastModifiedByUserId>{0}</LastModifiedByUserId>", item.LastModifiedByUserId);
            //    sb.AppendFormat("<LastModifiedOnDate>{0}</LastModifiedOnDate>", item.LastModifiedOnDate);
            //    sb.AppendFormat("<ModuleId>{0}</ModuleId>", item.ModuleId);

            //    sb.Append("</Item>");
            //}

            //sb.Append("</Items>");

            //// you might consider doing something similar here for any important module settings

            //return sb.ToString();
            return "";
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// ImportModule implements the IPortable ImportModule Interface
        /// </summary>
        /// <param name="moduleId">The Id of the module to be imported</param>
        /// <param name="content">The content to be imported</param>
        /// <param name="version">The version of the module to be imported</param>
        /// <param name="userId">The Id of the user performing the import</param>
        /// -----------------------------------------------------------------------------
        public void ImportModule(int moduleId, string content, string version, int userId)
        {
            //var controller = new ItemController();
            //var items = DotNetNuke.Common.Globals.GetContent(content, "Items");
            //var xmlNodeList = items.SelectNodes("Item");

            //if (xmlNodeList == null) return;

            //foreach (XmlNode item in xmlNodeList)
            //{
            //    var newItem = new Item()
            //    {
            //        ModuleId = moduleId,
            //        // assigning everything to the current UserID, because this might be a new DNN installation
            //        // your use case might be different though
            //        CreatedByUserId = userId,
            //        LastModifiedByUserId = userId,
            //        CreatedOnDate = DateTime.Now,
            //        LastModifiedOnDate = DateTime.Now
            //    };

            //    // NOTE: If moving from one installation to another, this user will not exist
            //    newItem.AssignedUserId = int.Parse(item.SelectSingleNode("AssignedUserId").InnerText, NumberStyles.Integer);
            //    newItem.ItemDescription = item.SelectSingleNode("ItemDescription").InnerText;
            //    newItem.ItemName = item.SelectSingleNode("ItemName").InnerText;

            //    controller.CreateItem(newItem);
            //}
        }

        /// -----------------------------------------------------------------------------
        /// <summary>
        /// UpgradeModule implements the IUpgradeable Interface
        /// </summary>
        /// <param name="version">The current version of the module</param>
        /// -----------------------------------------------------------------------------
        public string UpgradeModule(string version)
        {
            try
            {
                // Add PowerBiGroup profile property
                var portalController = new PortalController();
                var portals = portalController.GetPortals();
                foreach (PortalInfo portal in portals)
                {
                    var property = ProfileController.GetPropertyDefinitionByName(portal.PortalID, "PowerBiGroup");
                    if (property == null)
                    {
                        var propertyDefinition = new ProfilePropertyDefinition
                        {
                            PortalId = portal.PortalID,
                            PropertyCategory = "PowerBi Embedded RLS",
                            PropertyName = "PowerBiGroup",
                            DataType = 349,
                            DefaultVisibility = UserVisibilityMode.AdminOnly,
                            Deleted = false,
                            ProfileVisibility = new ProfileVisibility
                            {
                                VisibilityMode = UserVisibilityMode.AdminOnly,
                                RelationshipVisibilities = new List<Relationship>(),
                                RoleVisibilities = new List<RoleInfo>()
                            },
                            Length = 100,
                            ReadOnly = false,
                            Visible = false,
                        };

                        ProfileController.AddPropertyDefinition(propertyDefinition);
                    }
                }

                // Programmed Task for Subscribers.
                var subscribeTaskFullName = "DotNetNuke.PowerBI.Tasks.SubscribeTask, DotNetNuke.PowerBI";
                var subscribeTaskStartTime = DateTime.Now;
                var subscribeTaskTimeLapse = 15;
                var subscribeTaskTimeLapseMeasurement = "m";
                var subscribeTaskRetryTimeLapse = 5;
                var subscribeTaskRetryTimeLapseMeasurement = "m";
                var subscribeTaskRetainHistoryNum = 0;
                var subscribeTaskAttachToEvent = "";
                var subscribeTaskCatchUpEnabled = false;
                var subscribeTaskEnabled = true;
                var subscribeTaskObjectDependencies = "";
                var subscribeTaskServers = "";
                var subscribeTaskFriendlyName = "Subscribers Email Sender";

                var subscribeTask = SchedulingController.GetSchedule()
                    .FirstOrDefault(x => x.TypeFullName == subscribeTaskFullName);

                subscribeTask = subscribeTask ?? new ScheduleItem();

                subscribeTask.ScheduleStartDate = subscribeTaskStartTime;
                subscribeTask.TimeLapse = subscribeTaskTimeLapse;
                subscribeTask.TimeLapseMeasurement = subscribeTaskTimeLapseMeasurement;
                subscribeTask.RetryTimeLapse = subscribeTaskRetryTimeLapse;
                subscribeTask.RetainHistoryNum = subscribeTaskRetainHistoryNum;
                subscribeTask.AttachToEvent = subscribeTaskAttachToEvent;
                subscribeTask.CatchUpEnabled = subscribeTaskCatchUpEnabled;
                subscribeTask.Enabled = subscribeTaskEnabled;
                subscribeTask.ObjectDependencies = subscribeTaskObjectDependencies;
                subscribeTask.Servers = subscribeTaskServers;
                subscribeTask.FriendlyName = subscribeTaskFriendlyName;

                if (subscribeTask.ScheduleID > 0)
                {
                    SchedulingController.UpdateSchedule(subscribeTask);
                }
                else
                {
                    // Add the scheduled task
                    SchedulingController.AddSchedule(
                        subscribeTaskFullName, subscribeTaskTimeLapse, subscribeTaskTimeLapseMeasurement, subscribeTaskRetryTimeLapse, subscribeTaskRetryTimeLapseMeasurement,
                        subscribeTaskRetainHistoryNum, subscribeTaskAttachToEvent, subscribeTaskCatchUpEnabled, subscribeTaskEnabled, subscribeTaskObjectDependencies, subscribeTaskServers,
                        subscribeTaskFriendlyName, subscribeTaskStartTime);
                }

                // Programmed Task for Capacity Rule Evaluation.
                var capacityRuleTaskFullName = "DotNetNuke.PowerBI.Tasks.CapacityRuleTask, DotNetNuke.PowerBI";
                var capacityRuleTaskStartTime = DateTime.Now;
                var capacityRuleTaskTimeLapse = 5; // Run every 5 minutes
                var capacityRuleTaskTimeLapseMeasurement = "m";
                var capacityRuleTaskRetryTimeLapse = 2;
                var capacityRuleTaskRetryTimeLapseMeasurement = "m";
                var capacityRuleTaskRetainHistoryNum = 10;
                var capacityRuleTaskAttachToEvent = "";
                var capacityRuleTaskCatchUpEnabled = false;
                var capacityRuleTaskEnabled = true;
                var capacityRuleTaskObjectDependencies = "";
                var capacityRuleTaskServers = "";
                var capacityRuleTaskFriendlyName = "Capacity Rule Evaluator";

                var capacityRuleTask = SchedulingController.GetSchedule()
                    .FirstOrDefault(x => x.TypeFullName == capacityRuleTaskFullName);

                capacityRuleTask = capacityRuleTask ?? new ScheduleItem();

                capacityRuleTask.ScheduleStartDate = capacityRuleTaskStartTime;
                capacityRuleTask.TimeLapse = capacityRuleTaskTimeLapse;
                capacityRuleTask.TimeLapseMeasurement = capacityRuleTaskTimeLapseMeasurement;
                capacityRuleTask.RetryTimeLapse = capacityRuleTaskRetryTimeLapse;
                capacityRuleTask.RetainHistoryNum = capacityRuleTaskRetainHistoryNum;
                capacityRuleTask.AttachToEvent = capacityRuleTaskAttachToEvent;
                capacityRuleTask.CatchUpEnabled = capacityRuleTaskCatchUpEnabled;
                capacityRuleTask.Enabled = capacityRuleTaskEnabled;
                capacityRuleTask.ObjectDependencies = capacityRuleTaskObjectDependencies;
                capacityRuleTask.Servers = capacityRuleTaskServers;
                capacityRuleTask.FriendlyName = capacityRuleTaskFriendlyName;

                if (capacityRuleTask.ScheduleID > 0)
                {
                    SchedulingController.UpdateSchedule(capacityRuleTask);
                }
                else
                {
                    // Add the scheduled task
                    SchedulingController.AddSchedule(
                        capacityRuleTaskFullName, capacityRuleTaskTimeLapse, capacityRuleTaskTimeLapseMeasurement, capacityRuleTaskRetryTimeLapse, capacityRuleTaskRetryTimeLapseMeasurement,
                        capacityRuleTaskRetainHistoryNum, capacityRuleTaskAttachToEvent, capacityRuleTaskCatchUpEnabled, capacityRuleTaskEnabled, capacityRuleTaskObjectDependencies, capacityRuleTaskServers,
                        capacityRuleTaskFriendlyName, capacityRuleTaskStartTime);
                }
                return "success";

                /*switch (version)
                {
                    case "01.00.20":
                        return "success";
                    default:
                        return "success";
                }*/
            }
            catch
            {
                return "failure";
            }
        }

        #endregion
    }
}