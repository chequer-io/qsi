using System.Collections.Generic;

namespace Qsi.Data;

public class QsiUserInfo
{
    public string Username { get; set; }

    public Dictionary<string, object> Properties { get; } = new();
}
