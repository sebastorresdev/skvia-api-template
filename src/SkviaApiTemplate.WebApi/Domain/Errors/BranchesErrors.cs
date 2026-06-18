namespace SkviaApiTemplate.WebApi.Domain.Errors;

public static class BranchesErrors
{
    public static Error DuplicateBranch(string name) =>
        Error.Conflict(
            code: "Branch.DuplicateBranch",
            description: $"El nombre de la sede '{name}' ya está en uso.");

    public static Error NotFound(Guid id) =>
        Error.NotFound(
            code: "Branch.NotFound",
            description: $"No se encontro la sede con ID: {id}");
}