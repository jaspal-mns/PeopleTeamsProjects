namespace PeopleTeamsProjectsRepository.Repository;

// ========== Domain Model ==========
public sealed record Person(Guid Id, string Name, Guid? TeamId = null)
{
    public Person(string name, Guid? teamId = null) : this(Guid.NewGuid(), name, teamId) { }
}

public sealed record Team(Guid Id, string Name, IReadOnlyList<Guid> MemberIds)
{
    public Team(string name, IReadOnlyList<Guid> memberIds) : this(Guid.NewGuid(), name, memberIds) { }
}

public sealed record Project(Guid Id, string Name, IReadOnlyList<Guid> TeamIds)
{
    public Project(string name, IReadOnlyList<Guid> teamIds) : this(Guid.NewGuid(), name, teamIds) { }
    public Project(string name) : this(Guid.NewGuid(), name, Array.Empty<Guid>()) { }
}