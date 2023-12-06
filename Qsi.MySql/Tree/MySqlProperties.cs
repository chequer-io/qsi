using Qsi.Data;

namespace Qsi.MySql.Tree;

public static class MySqlProperties
{
    // QsiUserNode
    public static class User
    {
        // Is user password created by RANDOM PASSWORD option? (bool)
        public const string IsRandomPassword = "IS_RANDOM_PASSWORD";
        public static readonly QsiQualifiedIdentifier IsRandomPasswordIdentifier = new(new QsiIdentifier(IsRandomPassword, false));
    }
}
