using FluentAssertions;
using Mapster;
using MapsterIssue.Test.Entities;
using MapsterIssue.Test.Models;
using MapsterMapper;

namespace MapsterIssue.Test;

public class MappingTests
{
    private static TypeAdapterConfig GetMappingConfig()
    {
        var config = new TypeAdapterConfig();
        config.NewConfig<ParentEntity, ParentModel>()
            .TwoWays()
            .Include<ChildEntityA, ChildModelA>()
            .Include<ChildEntityB, ChildModelB>()
            .Map(dest => dest.Id, src => src.Guid);
        config.NewConfig<ChildEntityA, ChildModelA>();
        config.NewConfig<ChildEntityB, ChildModelB>();
        return config;
    }

    private static ParentModel MapEntityToModel(Mapper mapper, ParentEntity parentEntity)
        => mapper.Map<ParentModel>(parentEntity);
    
    [Fact]
    public void Map_ChildEntityA_To_ChildModelA()
    {
        var config = GetMappingConfig();
        var mapper = new Mapper(config);
        var childEntityA = new ChildEntityA()
        {
            Id = 420,
            Guid = Guid.NewGuid(),
            ChildPropertyA = "foo",
        };

        var result = MapEntityToModel(mapper, childEntityA);
        result.Should().BeOfType<ChildModelA>();

        var childModelA = (ChildModelA) result;
        result.Id.Should().Be(childEntityA.Guid);
        childModelA.ChildPropertyA.Should().Be(childEntityA.ChildPropertyA);
    }

    private static List<ParentModel> MapEntityListToModelList(Mapper mapper, List<ParentEntity> parentEntities)
        => mapper.Map<List<ParentModel>>(parentEntities);
    
    [Fact]
    public void Map_List_ChildEntityA_To_List_ChildModelA()
    {
        var config = GetMappingConfig();
        var mapper = new Mapper(config);

        var childEntityA = new ChildEntityA()
        {
            Id = 69,
            Guid = Guid.NewGuid(),
            ChildPropertyA = "bar",
        };
        var childEntityAList = new List<ParentEntity>()
            { childEntityA };
        
        var results = MapEntityListToModelList(mapper, childEntityAList);

        results.Should().HaveCount(1);
        var result = results.Single();

        result.Should().BeOfType<ChildModelA>();
        var childModelA = (ChildModelA) result;
        
        childModelA.Id.Should().Be(childEntityA.Guid);
        childModelA.ChildPropertyA.Should().Be(childEntityA.ChildPropertyA);
    }
}