using DotNetNuke.Common;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Instrumentation;
using DotNetNuke.PowerBI.Models;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Web.Mvc.Framework.ActionFilters;
using DotNetNuke.Web.Mvc.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DotNetNuke.PowerBI.Controllers
{
    /// <summary>
    /// Builds a catalog of report pages from the DNN site map. A page becomes a catalog entry
    /// when (1) the current user is allowed to view it and (2) it is tagged with the "Report"
    /// term (case-insensitive). The remaining tags act as categories, so a single page can
    /// belong to several categories.
    /// </summary>
    public class ReportCatalogController : DnnController
    {
        private static readonly ILog Logger = LoggerSource.Instance.GetLogger(typeof(ReportCatalogController));

        private const string ReportTag = "Report";
        private const string FileIdPrefix = "FileID=";
        private const string DefaultColor = "#6c757d";

        // Fixed palette assigned to categories by their alphabetical (ascending) ordinal.
        private static readonly string[] Palette =
        {
            "#4263eb", // blue
            "#2fb344", // green
            "#4299e1", // azure
            "#f76707", // orange
            "#ae3ec9", // purple
            "#d63939", // red
            "#0ca678", // teal
            "#d6336c", // pink
            "#f59f00", // yellow
            "#1098ad", // cyan
            "#7048e8", // indigo
            "#74b816", // lime
        };

        // GET: ReportCatalog
        [DnnHandleError]
        public ActionResult Index()
        {
            var model = new ReportCatalogView();

            try
            {
                var portalId = ModuleContext.PortalId;
                var tabs = TabController.Instance.GetTabsByPortal(portalId).AsList();
                var random = new Random();

                var items = new List<ReportCatalogItem>();
                var allCategories = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                foreach (var tab in tabs)
                {
                    if (tab == null || tab.IsDeleted || tab.DisableLink || tab.IsSuperTab)
                    {
                        continue;
                    }

                    var tags = GetTabTags(tab);
                    if (!tags.Any(t => string.Equals(t, ReportTag, StringComparison.OrdinalIgnoreCase)))
                    {
                        continue;
                    }

                    // Only surface pages the current user is allowed to view. The catalog (and the
                    // client-side search that runs on top of it) therefore never exposes a page the
                    // user has no access to.
                    if (!TabPermissionController.CanViewPage(tab))
                    {
                        continue;
                    }

                    var categories = tags
                        .Where(t => !string.Equals(t, ReportTag, StringComparison.OrdinalIgnoreCase))
                        .Select(t => (t ?? string.Empty).Trim())
                        .Where(t => t.Length > 0)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
                        .ToList();

                    foreach (var category in categories)
                    {
                        allCategories.Add(category);
                    }

                    var icon = ResolveIcon(tab);

                    items.Add(new ReportCatalogItem
                    {
                        TabId = tab.TabID,
                        Title = string.IsNullOrWhiteSpace(tab.Title) ? tab.TabName : tab.Title,
                        Description = tab.Description ?? string.Empty,
                        Url = Globals.NavigateURL(tab.TabID),
                        Categories = categories,
                        PrimaryCategory = categories.FirstOrDefault(),
                        IconUrl = icon.Url,
                        IconIsSvg = icon.IsSvg,
                        Views = random.Next(50, 5000), // TODO: replace placeholder with real analytics.
                    });
                }

                // Assign a color to every category from its position in the alphabetically sorted list.
                var sortedCategories = allCategories
                    .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var colorByCategory = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var i = 0; i < sortedCategories.Count; i++)
                {
                    colorByCategory[sortedCategories[i]] = Palette[i % Palette.Length];
                }

                model.Categories = sortedCategories
                    .Select(c => new ReportCatalogCategory { Name = c, Color = colorByCategory[c] })
                    .ToList();

                foreach (var item in items)
                {
                    item.Color = item.PrimaryCategory != null && colorByCategory.ContainsKey(item.PrimaryCategory)
                        ? colorByCategory[item.PrimaryCategory]
                        : DefaultColor;
                }

                model.Items = items
                    .OrderBy(i => i.Title, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                return View(model);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return View(model);
            }
        }

        /// <summary>Returns the taxonomy term names associated with the page.</summary>
        private static List<string> GetTabTags(TabInfo tab)
        {
            var tags = new List<string>();

            try
            {
                if (tab.Terms != null && tab.Terms.Count > 0)
                {
                    tags.AddRange(tab.Terms.Select(t => t.Name));
                }
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not read terms for tab {tab.TabID}", ex);
            }

            return tags;
        }

        /// <summary>
        /// Resolves the page icon to a browser URL. Handles the "FileID=" token stored by DNN
        /// as well as plain/relative paths, and reports whether the icon is an SVG (so it can be
        /// recolored with the category color).
        /// </summary>
        private static (string Url, bool IsSvg) ResolveIcon(TabInfo tab)
        {
            try
            {
                var raw = tab.IconFileRaw;
                if (string.IsNullOrWhiteSpace(raw))
                {
                    return (null, false);
                }

                if (raw.StartsWith(FileIdPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(raw.Substring(FileIdPrefix.Length), out var fileId))
                    {
                        var file = FileManager.Instance.GetFile(fileId);
                        if (file != null)
                        {
                            var url = FileManager.Instance.GetUrl(file);
                            return (url, string.Equals(file.Extension, "svg", StringComparison.OrdinalIgnoreCase));
                        }
                    }

                    return (null, false);
                }

                return (Globals.ResolveUrl(raw), raw.EndsWith(".svg", StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex)
            {
                Logger.Warn($"Could not resolve icon for tab {tab.TabID}", ex);
                return (null, false);
            }
        }
    }
}
