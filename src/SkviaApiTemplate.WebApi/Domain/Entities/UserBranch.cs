namespace SkviaApiTemplate.WebApi.Domain.Entities;

public class UserBranch
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid BranchId { get; private set; }
    public Branch Branch { get; private set; } = null!;

    private UserBranch() { }

    public static UserBranch Create(Guid userId, Guid branchId)
    {
        return new UserBranch
        {
            UserId = userId,
            BranchId = branchId
        };
    }
}