using CodifyDotnet.Services;
using Microsoft.AspNetCore.Mvc;

namespace CodifyDotnet.Controllers;

[ApiController]
[Route("/")]
public class DefaultController
{
    private ITokenService _tokenService;
    public DefaultController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    // [HttpGet]
    // public string GetTokens()
    // {
    //     return _tokenService.CreateToken();
    // }
    
    [HttpGet]
    public object GetTokens()
    {
        return _tokenService.GetTokens(AuthenticateRequest());
    }
    
    [HttpPost]
    public string GenerateKey(CreateKeyModel model)
    {
        var claims = new Dictionary<string, string>();
        foreach(var claim in CreateKeyModel.SupportedClaims)
        {
            if(model.PassedClaims.Contains(claim))
                claims.Add(claim,"Allow");
            else
                claims.Add(claim,"Deny");
        }
        return _tokenService.GenerateKey(AuthenticateRequest(),claims);
    }

    [HttpPost]
    [Route("authenticate")]
    public string Authenticate(AuthenticateModel model)
    {
        return _tokenService.GenerateToken(model.ApiKey);
    }
    
    [HttpDelete]
    public void RevokeToken(RevokeTokenModel model)
    {
        _tokenService.RevokeKey(model.ApiKey);
    }
    
    [HttpGet]
    [Route("status")]
    public StatusResponce GetStatus()
    {
        var status = new StatusResponce() { Version = "1.0.0.0" };
        return status;
    }

    private string AuthenticateRequest()
    {
        return "90796938-782c-4bd1-bd68-be0dd61a2bd3";
    }
}

public class StatusResponce
{
    public string Version { get; set; }
}