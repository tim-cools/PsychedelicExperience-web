using System;
using System.Collections.Generic;
using MemBus;
using StructureMap;

namespace PsychedelicExperience.Common.MessageBus
{
    internal class MemoryBusIoCAdapter : IocAdapter
    {
        private readonly IContext _context;

        public MemoryBusIoCAdapter(IContext context)
        {
            _context = context;
        }

        public IEnumerable<object> GetAllInstances(Type desiredType)
        {
            return _context.GetAllInstances(desiredType);
        }
    }
}