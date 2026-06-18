using SkviaApiTemplate.WebApi.Domain.Common;
using SkviaApiTemplate.WebApi.Domain.Entities;

namespace SkviaApiTemplate.WebApi.Domain.Identity;

public class User : BaseAuditableEntity
{
    public string Name { get; private set; } = null!;
    public required string Username
    {
        get;
        set
        {
            var cleanValue = value?.Trim() ?? string.Empty;
            field = cleanValue;
            NormalizedUsername = cleanValue.ToUpperInvariant();
        }
    }
    public string NormalizedUsername { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string? ProfilePicture { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;
    
    // Agregados por requerimiento de bubba
    public Guid? BranchId { get; set; }
    public Branch? Branch { get; set; }

    private User() {}

    public static User Create(
        string name,
        string username,
        string passwordHash,
        Guid roleId,
        string? email = null,
        string? phoneNumber = null,
        string? profilePicture = null)
    {
        return new User
        {
            Name = name.Trim(),
            Username = username,
            PasswordHash = passwordHash,
            RoleId = roleId,
            Email = email?.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            ProfilePicture = profilePicture
        };
    }

    public void Update(
        string name,
        string username,
        Guid roleId,
        string? email = null,
        string? phoneNumber = null,
        string? profilePicture = null)
    {
        Name = name.Trim();
        Username = username;
        RoleId = roleId;
        Email = email;
        PhoneNumber = phoneNumber;
        ProfilePicture = profilePicture;
    }
    
    

    public void ChangePassword(string newPasswordHash)
        => PasswordHash = newPasswordHash;

    public void AssignBranch(Guid branchId)
        => BranchId = branchId;

    public void RemoveBranch()
        => BranchId = null;
}