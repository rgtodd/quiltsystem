//
// Copyright (c) 2019-2020 by Richard G. Todd
// Source code is licensed under the MIT License.  See the LICENSE.txt solution file for more information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json.Linq;

using RichTodd.QuiltSystem.Database.Domain;
using RichTodd.QuiltSystem.Database.Model;
using RichTodd.QuiltSystem.Service.Database.Abstractions;

namespace RichTodd.QuiltSystem.Business.Libraries
{
    public class DatabaseResourceLibrary : AbstractResourceLibrary
    {
        private static readonly Dictionary<string, DatabaseResourceLibrary> m_cachedLibraries = new Dictionary<string, DatabaseResourceLibrary>();

        private readonly IQuiltContextFactory m_quiltContextFactory;
        private readonly long m_resourceLibraryId;
        private readonly string m_name;

        private List<ResourceLibraryEntry> m_cachedEntries;

        private DatabaseResourceLibrary(IQuiltContextFactory quiltContextFactory, long resourceLibraryId, string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));

            m_quiltContextFactory = quiltContextFactory ?? throw new ArgumentNullException(nameof(quiltContextFactory));
            m_resourceLibraryId = resourceLibraryId;
            m_name = name;
        }

        public override string Name
        {
            get
            {
                return m_name;
            }
        }

        public static DatabaseResourceLibrary Create(IQuiltContextFactory quiltContextFactory, JToken json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));

            var libraryName = json.Value<string>(Json_Name);

            using (var ctx = quiltContextFactory.Create())
            {
                var dbResourceLibrary = ctx.ResourceLibraries.Where(r => r.Name == libraryName).SingleOrDefault();
                if (dbResourceLibrary == null)
                {
                    dbResourceLibrary = new ResourceLibrary()
                    {
                        Name = libraryName
                    };
                    _ = ctx.ResourceLibraries.Add(dbResourceLibrary);
                }

                foreach (var jsonEntry in json[Json_Entries])
                {
                    var entry = new ResourceLibraryEntry(jsonEntry);

                    var dbResourceType = ctx.ResourceTypes.Where(r => r.Name == entry.Type).SingleOrDefault();
                    if (dbResourceType == null)
                    {
                        // Look for resource types created below.  These entries aren't normally found until
                        // ctx.SaveChanges is called.
                        //
                        dbResourceType = ctx.ResourceTypes.Local.Where(r => r.Name == entry.Type).SingleOrDefault();
                    }
                    if (dbResourceType == null)
                    {
                        dbResourceType = new ResourceType()
                        {
                            Name = entry.Type
                        };
                        _ = ctx.ResourceTypes.Add(dbResourceType);
                    }

                    var dbResource = dbResourceLibrary.Resources.Where(r => r.Name == entry.Name).SingleOrDefault();
                    if (dbResource == null)
                    {
                        dbResource = new Resource()
                        {
                            Name = entry.Name
                        };
                        _ = ctx.Resources.Add(dbResource);
                    }
                    dbResource.ResourceType = dbResourceType;
                    dbResource.ResourceData = entry.Data;
                }

                _ = ctx.SaveChanges();
            }

            return Load(quiltContextFactory, libraryName);
        }

        public static DatabaseResourceLibrary Load(IQuiltContextFactory quiltContextFactory, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return m_cachedLibraries.TryGetValue(name, out var library)
                ? library :
                LoadSync(quiltContextFactory, name);
        }

        public override void CreateEntry(string name, string type, string data, string[] tags)
        {
            using var ctx = m_quiltContextFactory.Create();

            var dbResourceType = ctx.ResourceTypes.Where(r => r.Name == type).SingleOrDefault();
            if (dbResourceType == null)
            {
                dbResourceType = new ResourceType()
                {
                    Name = type
                };
                _ = ctx.ResourceTypes.Add(dbResourceType);
            }

            var dbResource = new Resource()
            {
                ResourceLibraryId = m_resourceLibraryId,
                Name = name,
                ResourceType = dbResourceType,
                ResourceData = data,
            };
            _ = ctx.Resources.Add(dbResource);

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    var dbTag = ctx.Tags.Where(r => r.TagTypeCode == TagTypeCodes.Block && r.Value == tag).FirstOrDefault();
                    if (dbTag == null)
                    {
                        dbTag = new Tag()
                        {
                            TagTypeCode = TagTypeCodes.Block,
                            Value = tag,
                            CreateDateTimeUtc = DateTime.UtcNow
                        };
                        _ = ctx.Tags.Add(dbTag);
                    }

                    var dbResourceTag = new ResourceTag()
                    {
                        Resource = dbResource,
                        Tag = dbTag,
                        CreateDateTimeUtc = DateTime.UtcNow
                    };
                    _ = ctx.ResourceTags.Add(dbResourceTag);
                }
            }

            _ = ctx.SaveChanges();

            InvalidateCachedEntries();
        }

        public override IReadOnlyList<ResourceLibraryEntry> GetEntries()
        {
            return GetCachedEntries();
        }

        public override ResourceLibraryEntry GetEntry(string name)
        {
            return GetCachedEntries().Where(r => r.Name == name).SingleOrDefault();
        }

        public override void UpdateEntry(string name, string data, string[] tags)
        {
            using var ctx = m_quiltContextFactory.Create();

            var dbResource = ctx.Resources
                .Include(r => r.ResourceTags)
                    .ThenInclude(r => r.Tag)
                .Where(r => r.ResourceLibraryId == m_resourceLibraryId && r.Name == name)
                .Single();

            dbResource.ResourceData = data;

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (!dbResource.ResourceTags.Any(r => r.Tag.Value == tag))
                    {
                        var dbTag = ctx.Tags.Where(r => r.TagTypeCode == TagTypeCodes.Block && r.Value == tag).FirstOrDefault();
                        if (dbTag == null)
                        {
                            dbTag = new Tag()
                            {
                                TagTypeCode = TagTypeCodes.Block,
                                Value = tag,
                                CreateDateTimeUtc = DateTime.UtcNow
                            };
                            _ = ctx.Tags.Add(dbTag);
                        }

                        var dbResourceTag = new ResourceTag()
                        {
                            Resource = dbResource,
                            Tag = dbTag,
                            CreateDateTimeUtc = DateTime.UtcNow
                        };
                        _ = ctx.ResourceTags.Add(dbResourceTag);
                    }
                }

                var dbObsoleteResourceTags = dbResource.ResourceTags.Where(r => !tags.Contains(r.Tag.Value)).ToList();
                ctx.RemoveRange(dbObsoleteResourceTags);
            }
            else
            {
                ctx.RemoveRange(dbResource.ResourceTags);
            }

            _ = ctx.SaveChanges();

            InvalidateCachedEntries();
        }

        private static DatabaseResourceLibrary Create(IQuiltContextFactory quiltContextFactory, string name)
        {
            using var ctx = quiltContextFactory.Create();

            var dbResourceLibrary = ctx.ResourceLibraries.Where(r => r.Name == name).SingleOrDefault();
            if (dbResourceLibrary == null)
            {
                dbResourceLibrary = new ResourceLibrary()
                {
                    Name = name
                };
                _ = ctx.ResourceLibraries.Add(dbResourceLibrary);

                _ = ctx.SaveChanges();
            }

            return new DatabaseResourceLibrary(quiltContextFactory, dbResourceLibrary.ResourceLibraryId, name);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private static DatabaseResourceLibrary LoadSync(IQuiltContextFactory quiltContextFactory, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!m_cachedLibraries.TryGetValue(name, out var library))
            {
                library = Create(quiltContextFactory, name);
                m_cachedLibraries[name] = library;
            }

            return library;
        }

        private List<ResourceLibraryEntry> GetCachedEntries()
        {
            return m_cachedEntries ?? GetCachedEntriesSync();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private List<ResourceLibraryEntry> GetCachedEntriesSync()
        {
            if (m_cachedEntries == null)
            {
                m_cachedEntries = LoadEntries();
            }

            return m_cachedEntries;
        }

        private void InvalidateCachedEntries()
        {
            m_cachedEntries = null;
        }

        private List<ResourceLibraryEntry> LoadEntries()
        {
            var result = new List<ResourceLibraryEntry>();

            using (var ctx = m_quiltContextFactory.Create())
            {
                var dbResources = ctx.Resources
                    .Include(r => r.ResourceType)
                    .Include(r => r.ResourceTags)
                        .ThenInclude(r => r.Tag)
                    .Where(r => r.ResourceLibraryId == m_resourceLibraryId).ToList();

                foreach (var dbResource in dbResources)
                {
                    var tags = dbResource.ResourceTags.Select(r => r.Tag.Value).ToArray();
                    result.Add(new ResourceLibraryEntry(dbResource.Name, dbResource.ResourceType.Name, dbResource.ResourceData, tags));
                }
            }

            return result;
        }
    }
}