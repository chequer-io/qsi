using System;
using Qsi.Data;
using Qsi.MongoDB.Internal.Nodes.Location;

namespace Qsi.MongoDB.Acorn
{
    public struct MongoDBStatement
    {
        public QsiScriptType ScriptType { get; set; }

        public Range Range { get; set; }

        public Location Start { get; set; }

        public Location End { get; set; }
    }
}
