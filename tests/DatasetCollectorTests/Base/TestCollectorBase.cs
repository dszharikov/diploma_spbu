using AutoMapper;
using DatasetCollector.DataBases;
using DatasetCollector.Parsers;
using DatasetCollector.Profiles;

namespace DatasetCollector;

public abstract class TestCollectorBase
{
    protected IParser Parser;
    protected AppDbContext Context;
    protected IMapper Mapper;

    public TestCollectorBase()
    {
        Mapper = new Mapper(new MapperConfiguration(
            cfg => cfg.AddProfile<MatchProfile>()));
        Parser = new OpenDotaParser(Mapper);
        Context = MatchContextFactory.Create();
    }
}