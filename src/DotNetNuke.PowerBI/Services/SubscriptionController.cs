using DotNetNuke.Entities.Users;
using DotNetNuke.PowerBI.Data;
using DotNetNuke.PowerBI.Data.Bookmarks;
using DotNetNuke.PowerBI.Data.Models;
using DotNetNuke.PowerBI.Data.Subscriptions;
using DotNetNuke.PowerBI.Data.Subscriptions.Models;
using DotNetNuke.Security;
using DotNetNuke.Security.Roles;
using DotNetNuke.Services.Social.Subscriptions.Entities;
using DotNetNuke.Web.Api;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using static DotNetNuke.PowerBI.Services.SubscriptionController;
using Subscription = DotNetNuke.PowerBI.Data.Subscriptions.Models.Subscription;

namespace DotNetNuke.PowerBI.Services
{
    [SupportedModules("DotNetNuke.PowerBI.ContentView")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)]
    public class SubscriptionController : DnnApiController
    {
        public class SubscriptionViewModel
        {
            public int Id { get; set; }
            public int PortalId { get; set; }
            public int UserId { get; set; }
            public string ReportId { get; set; }
            public string GroupId { get; set; }
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string RepeatPeriod { get; set; }
            public TimeSpan RepeatTime { get; set; }
            public string TimeZone { get; set; }
            public string EmailSubject { get; set; }
            public string Message { get; set; }
            public string ReportPages { get; set; }
            public bool Enabled { get; set; }
            public string Users { get; set; }
            public string Roles { get; set; }

        }

        public class UserViewModel
        {
            public int UserID { get; set; }
            public string DisplayName { get; set; }
        }

        public class RoleViewModel
        {
            public int RoleID { get; set; }
            public string RoleName { get; set; }
        }
        [HttpGet]
        public HttpResponseMessage GetSubscriptions(string reportId)
        {
            try
            {
                var userId = UserController.Instance.GetCurrentUserInfo().UserID;
                var portalId = PortalSettings.PortalId;
                var subscriptions = SubscriptionsRepository.Instance.GetSubscriptionsByReportId(reportId, portalId);
                // I need that foreach subscription, I get the users and roles that are subscribed to it
                foreach (var subscription in subscriptions)
                {
                    var subscriptionSubscribers = SubscriptionsSubscribersRepository.Instance.GetSubscribersBySubscription(subscription.Id);
                    var users = new List<UserViewModel>();
                    var roles = new List<RoleViewModel>();
                    foreach (var subscriptionSubscriber in subscriptionSubscribers)
                    {
                        if (subscriptionSubscriber.UserId.HasValue)
                        {
                            var user = UserController.Instance.GetUserById(portalId, subscriptionSubscriber.UserId.Value);
                            if (user != null)
                            {
                                users.Add(new UserViewModel
                                {
                                    UserID = user.UserID,
                                    DisplayName = user.DisplayName
                                });

                            }
                        }
                        else if (subscriptionSubscriber.RoleId.HasValue)
                        {
                            var role = RoleController.Instance.GetRoleById(portalId, subscriptionSubscriber.RoleId.Value);
                            if (role != null)
                            {
                                roles.Add(new RoleViewModel
                                {
                                    RoleID = role.RoleID,
                                    RoleName = role.RoleName
                                });
                            }
                        }
                    }
                    var serializer = new JavaScriptSerializer();

                    var usersArray = serializer.Serialize(users);
                    var rolesArray = serializer.Serialize(roles);
                    subscription.Users = usersArray;
                    subscription.Roles = rolesArray;
                }
                if (subscriptions != null && subscriptions.Any())
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                        Data = subscriptions
                    });
                }

                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = false,
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage AddSubscription(SubscriptionViewModel subscriptionViewModel)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                Subscription subscription = new Subscription
                {
                    PortalId = subscriptionViewModel.PortalId,
                    ReportId = subscriptionViewModel.ReportId,
                    GroupId = subscriptionViewModel.GroupId,
                    Name = subscriptionViewModel.Name,
                    StartDate = subscriptionViewModel.StartDate,
                    EndDate = subscriptionViewModel.EndDate,
                    RepeatPeriod = subscriptionViewModel.RepeatPeriod,
                    RepeatTime = subscriptionViewModel.RepeatTime,
                    TimeZone = subscriptionViewModel.TimeZone,
                    EmailSubject = subscriptionViewModel.EmailSubject,
                    Message = subscriptionViewModel.Message,
                    ReportPages = subscriptionViewModel.ReportPages,
                    Enabled = subscriptionViewModel.Enabled
                };
                int subscriptionId = SubscriptionsRepository.Instance.AddSubscription(subscription);
                var usersArray = subscriptionViewModel.Users.Split(',');
                var rolesArray = subscriptionViewModel.Roles.Split(',');
                if (subscriptionId >= 0)
                {
                    if (usersArray.Length > 0)
                    {
                        foreach (string user in usersArray)
                        {
                            if (int.TryParse(user, out int userId))
                            {
                                SubscriptionsSubscribersRepository.Instance.AddSubscriptionSubscriber(new SubscriptionSubscriber
                                {
                                    SubscriptionId = subscriptionId,
                                    UserId = userId
                                });
                            }

                        }
                    }
                    if (rolesArray.Length > 0)
                    {
                        foreach (string role in rolesArray)
                        {
                            if (int.TryParse(role, out int roleId))
                            {
                                SubscriptionsSubscribersRepository.Instance.AddSubscriptionSubscriber(new SubscriptionSubscriber
                                {
                                    SubscriptionId = subscriptionId,
                                    RoleId = roleId,
                                });
                            }
                        }
                    }

                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = false,
                });
            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e
                });
            }
        }


        [HttpPost]
        public HttpResponseMessage EditSubscription(SubscriptionViewModel subscriptionViewModel)
        {
            try
            {
                var serializer = new JavaScriptSerializer();
                Subscription subscription = SubscriptionsRepository.Instance.GetSubscriptionById(subscriptionViewModel.Id);
                subscription.Name = subscriptionViewModel.Name;
                subscription.StartDate = subscriptionViewModel.StartDate;
                subscription.EndDate = subscriptionViewModel.EndDate;
                subscription.RepeatPeriod = subscriptionViewModel.RepeatPeriod;
                subscription.RepeatTime = subscriptionViewModel.RepeatTime;
                subscription.TimeZone = subscriptionViewModel.TimeZone;
                subscription.EmailSubject = subscriptionViewModel.EmailSubject;
                subscription.Message = subscriptionViewModel.Message;
                subscription.ReportPages = subscriptionViewModel.ReportPages;
                subscription.Enabled = subscriptionViewModel.Enabled;
                bool success = SubscriptionsRepository.Instance.EditSubscription(subscription);

                var usersArray = subscriptionViewModel.Users.Split(',');
                var rolesArray = subscriptionViewModel.Roles.Split(',');
                List<SubscriptionSubscriber> subscriptionSubscribers = SubscriptionsSubscribersRepository.Instance.GetSubscribersBySubscription(subscription.Id);
                foreach (SubscriptionSubscriber subscriptionSubscriber in subscriptionSubscribers)
                {
                    SubscriptionsSubscribersRepository.Instance.DeleteSubscriptionSubscriber(subscriptionSubscriber.Id);
                }
                if (usersArray.Length > 0)
                {
                    foreach (string user in usersArray)
                    {
                        if (int.TryParse(user, out int userId))
                        {
                            SubscriptionsSubscribersRepository.Instance.AddSubscriptionSubscriber(new SubscriptionSubscriber
                            {
                                SubscriptionId = subscription.Id,
                                UserId = int.Parse(user),

                            });
                        }
                    }
                }
                if (rolesArray.Length > 0)
                {

                    foreach (string role in rolesArray)
                    {
                        if (int.TryParse(role, out int roleId))
                        {
                            SubscriptionsSubscribersRepository.Instance.AddSubscriptionSubscriber(new SubscriptionSubscriber
                            {
                                SubscriptionId = subscription.Id,
                                RoleId = int.Parse(role)
                            });
                        }
                    }
                }

                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = false,
                });

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e
                });
            }
        }

        [HttpPost]
        public HttpResponseMessage DeleteSubscription(SubscriptionViewModel subscriptionViewModel)
        {
            try
            {
                bool success = SubscriptionsRepository.Instance.DeleteSubscription(subscriptionViewModel.Id);
                if (success)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = false,
                });

            }
            catch (Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = e
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage SearchUsers(int portalId, string searchName)
        {
            try
            {
                List<ObjectPermission> objectPermissions = ObjectPermissionsRepository.Instance.GetObjectPermissionsByPortalExtended(portalId)
                .Where(permission => permission.PermissionID == 1 && permission.AllowAccess)
                .ToList();
                List<UserViewModel> users = new List<UserViewModel>();
                foreach (ObjectPermission permission in objectPermissions)
                {
                    if (permission.UserID != null)
                    {
                        UserInfo addingUser = UserController.GetUserById(portalId, permission.UserID.Value);

                        if (addingUser != null && addingUser.DisplayName.ToLower().Contains(searchName.ToLower()))
                        {
                            if (users.Where(user => user.UserID == addingUser.UserID).Count() == 0)
                            {
                                users.Add(new UserViewModel
                                {
                                    UserID = addingUser.UserID,
                                    DisplayName = addingUser.DisplayName
                                });
                            }
                        }
                    }
                    else if (permission.RoleID != null)
                    {
                        List<UserInfo> roleUsers = RoleController.Instance.GetUsersByRole(portalId, permission.RoleName).ToList();
                        foreach (UserInfo user in roleUsers)
                        {
                            if (user.DisplayName.ToLower().Contains(searchName.ToLower()))
                            {
                                if (users.Where(roleUser => roleUser.UserID == user.UserID).Count() == 0)
                                {
                                    users.Add(new UserViewModel
                                    {
                                        UserID = user.UserID,
                                        DisplayName = user.DisplayName
                                    });
                                }
                            }
                        }
                    }
                }
                if (users != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                        Data = users
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = false,
                });
            } catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = ex
                });
            }
        }

        [HttpGet]
        public HttpResponseMessage SearchRoles(int portalId, string searchName)
        {
            try
            {
                List<ObjectPermission> objectPermissions = ObjectPermissionsRepository.Instance.GetObjectPermissionsByPortalExtended(portalId)
                .Where(permission => permission.PermissionID == 1 && permission.AllowAccess)
                .ToList();
                List<RoleViewModel> roles = new List<RoleViewModel>();
                foreach (ObjectPermission permission in objectPermissions)
                {
                    if (permission.RoleID != null)
                    {
                        RoleInfo addingRole = RoleController.Instance.GetRoleById(portalId, permission.RoleID.Value);

                        if (addingRole != null && addingRole.RoleName.ToLower().Contains(searchName.ToLower()))
                        {
                            if (roles.Where(role => role.RoleID == addingRole.RoleID).Count() == 0)
                            {
                                roles.Add(new RoleViewModel
                                {
                                    RoleID = addingRole.RoleID,
                                    RoleName = addingRole.RoleName
                                });
                            }
                        }
                    }
                }
                if (roles != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        Success = true,
                        Data = roles
                    });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new
                {
                    Success = false,
                });
            } catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new
                {
                    Success = false,
                    Error = ex
                });
            }
        }
    }
}