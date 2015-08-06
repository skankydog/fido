using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Fido.Core
{
    public class TypeFinder
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Assembly SourceAssembly { get; set; }

        public TypeFinder()
        {
            SourceAssembly = Assembly.GetCallingAssembly();
        }

        public TypeFinder(Assembly SourceAssembly)
        {
            using (new FunctionLogger(Log))
            {
                Log.DebugFormat("Assembly: {0}", SourceAssembly);
                Log.DebugFormat("Root namespace: {0}", SourceAssembly.GetName().Name);

                this.SourceAssembly = SourceAssembly;
            }
        }

        public List<System.Type> Find<TYPE>()
        {
            using (new FunctionLogger(Log))
            {
                Log.DebugFormat("Source assembly: {0}", SourceAssembly);
                Log.DebugFormat("Root namespace: {0}", SourceAssembly.GetName().Name);
                Log.DebugFormat("Looking for instances of: {0}", typeof(TYPE));

                var TypesToFind = typeof(TYPE);
                return SourceAssembly.GetTypes()
                    .Where(t => TypesToFind.IsAssignableFrom(t))
                    .ToList();
            }
        }
    }
}
