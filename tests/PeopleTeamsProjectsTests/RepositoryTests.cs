using AwesomeAssertions;
using PeopleTeamsProjectsRepository.Repository;
using Xunit;
using Xunit.Abstractions;

namespace PeopleTeamsProjectsTests;

public sealed class RepositoryTests(ITestOutputHelper output)
{
    private readonly Repositories _repos = new ();
    [Fact]
    public void AddOperation_PersistsEntity()
    {
        var repo = _repos.People;
        var person = new Person("Grace");

        repo.Add(person);

        repo.Get(person.Id).Should().Be(person);
        WriteList();
    }

    [Fact]
    public void Delete_RemovesEntity()
    {
        var repo = _repos.Projects;
        WriteList();
        var project=repo.GetByName("Oak Tree");
        repo.Delete(project.Id);  
        WriteList();
        repo.Get(project.Id).Should().BeNull();
    }

    private void WriteList()
    {
        output.WriteLine(_repos.ToFullList());
    }
}