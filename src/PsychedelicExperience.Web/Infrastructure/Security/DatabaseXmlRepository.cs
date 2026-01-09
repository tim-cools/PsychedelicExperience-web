using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Marten;
using Microsoft.AspNetCore.DataProtection.Repositories;
using StructureMap;

namespace PsychedelicExperience.Web.Infrastructure.Security
{
    public class DatabaseXmlRepository : IXmlRepository
    {
        private readonly IServiceProvider _services;

        public DatabaseXmlRepository(IServiceProvider services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            _services = services;
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            using (var context = CreateChildContainer())
            using (var session = context.GetInstance<IQuerySession>())
            {
                return session.Query<XmlDocument>()
                    .ToList()
                    .Select(document => document.Document)
                    .ToList();
            }
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            using (var context = CreateChildContainer())
            using (var session = context.GetInstance<IDocumentSession>())
            {
                var document = new XmlDocument
                {
                    Id = friendlyName,
                    Document = element
                };

                session.Store(document);
                session.SaveChanges();
            }
        }

        private IContainer CreateChildContainer()
        {
            var container = _services.GetService(typeof(IContainer)) as IContainer;
            if (container == null)
            {
                throw new InvalidOperationException("Could not get container from services collection.");
            }
            return container.CreateChildContainer();
        }
    }
}