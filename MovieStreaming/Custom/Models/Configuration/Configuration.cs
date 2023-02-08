namespace MovieStreaming.Custom.Models.Configuration
{
    public enum CRUDOperation
    {
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public enum AuthorizationType
    {
        ReadWrite = 1,
        Read = 2,
        Block = 3
    }

    public enum RoleAccess
    {
        FullAccess = 1,
        Read = 2,
        Denied = 3
    }
    public enum RoleList
    {
        Admin = 1,
        User = 2

    }

    public enum EnumIntOrString
    {
        Int = 4,
        String = 5
    }

    public enum ChangeLogTable
    {
        Users = 1,
        Movies = 2,
        ViewAuthorization = 3
    }

    public enum ChangeLogAction
    {
        Inserte = 10,
        Update = 11,
        Delete = 12,
        Login = 13,
        Logout = 14
    }

    public enum Views
    {
        Users = 1,
        Role = 2,
        Complaints = 3,
        Movie = 4,
        ChangeLog = 5
    }

    public enum ConstantConfiguration
    {
        Integer = 1,
        String = 2,
        Float = 3,
        Bool = 4,
        Decimal = 5,
        Byte = 6,
        Date = 7,
        Time = 8

    }

}
