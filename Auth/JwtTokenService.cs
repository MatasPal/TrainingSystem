using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace TrainingSystem.Auth;

public class JwtTokenService
{
    private readonly SymmetricSecurityKey _authSigningKey;
    private readonly string? _issuer;
    private readonly string? _audience;

    public JwtTokenService(IConfiguration configuration)
    {
        _authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]));
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
    }

    public string CreateAccessToken(string userName, string userId, IEnumerable<string> roles)
    {
        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
        };
        
        authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: DateTime.Now.AddMinutes(20),
            claims: authClaims,
            signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string CreateRefreshToken( string userId, DateTime expires)
    {
        var authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, userId),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            expires: expires,
            claims: authClaims,
            signingCredentials: new SigningCredentials(_authSigningKey, SecurityAlgorithms.HmacSha256)
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public bool TryParseRefreshToken(string refreshToken, out ClaimsPrincipal? claims)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler() { MapInboundClaims = false };
            var validationParameters = new TokenValidationParameters
            {
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = _authSigningKey,
                ValidateLifetime = true
            };
            claims = tokenHandler.ValidateToken(refreshToken, validationParameters, out _);
            return true;
        }
        catch 
        {
            claims = null;
            return false;
        }
    }
}