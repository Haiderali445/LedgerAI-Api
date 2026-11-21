namespace LedgerAI.Api.Core.Security;

public static class AppRoles
{
    public const string Owner = "Owner";
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Operator = "Operator";
    public const string Viewer = "Viewer";

    public static readonly IReadOnlyList<string> All = new[]
    {
        Owner, Admin, Manager, Operator, Viewer
    };
}
