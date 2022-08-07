using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DopplerSample.Controllers;

[ApiController]
[Route("[controller]")]
public class GetKeysController : ControllerBase
{
    private readonly ILogger<GetKeysController> _logger;
    private readonly IConfiguration _configuration;
    private readonly PositionOptions _positionOptions;

    public GetKeysController(ILogger<GetKeysController> logger, IConfiguration configuration,
        IOptionsMonitor<PositionOptions> options)
    {
        _logger = logger;
        _configuration = configuration;
        _positionOptions = options.CurrentValue;
    }

    [HttpGet(Name = "GetKeys")]
    public JsonResult Get()
    {
        var Upperkey1 = _configuration.GetValue<string>("KEY1");
        var AsIskey1 = _configuration.GetValue<string>("Key1");
        var AsIskey2 = _configuration.GetValue<string>("Key2");
        var NameFromConfig = _configuration.GetValue<string>("POSITION:NAME");
        var TitleFromConfig = _configuration.GetValue<string>("POSITION:TITLE");
        var OrganizationFromConfig= _configuration.GetValue<string>("POSITION:ORGANIZATION");

        var NameFromOption = _positionOptions.Name;
        var TitleFromOption = _positionOptions.Title;
        var OrganizationFromOption = _positionOptions.Organization;

        return new JsonResult(new
        {
            Upperkey1,
            AsIskey1,
            AsIskey2,
            NameFromConfig,
            TitleFromConfig,
            OrganizationFromConfig,
            NameFromOption,
            TitleFromOption,
            OrganizationFromOption
        });
    }
}