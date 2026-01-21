using HDEMS.Infrastructure.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HDEMS.Infrastructure.Services;

/// <summary>
/// JWT 服务
/// </summary>
public class JwtService
{
    private readonly JwtConfig _jwtConfig;

    public JwtService(IConfiguration configuration)
    {
        _jwtConfig = configuration.GetSection(JwtConfig.SectionName).Get<JwtConfig>() ?? new JwtConfig();
    }

    /// <summary>
    /// 生成 JWT Token
    /// </summary>
    public string GenerateToken(Guid userId, string username, List<string> roles, Guid? hospitalId, bool isCommissionUser)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim("HospitalId", hospitalId?.ToString() ?? ""),
            new Claim("IsCommissionUser", isCommissionUser.ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtConfig.SecretKey ?? "DefaultSecretKeyMustBe32CharsLong!"));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtConfig.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// 刷新 JWT Token
    /// </summary>
    public string RefreshToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.SecretKey ?? "DefaultSecretKeyMustBe32CharsLong!");

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // 不验证过期时间，因为我们正在刷新
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidAudience = _jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var expClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "exp");
            var refreshExpClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "refresh_exp");

            if (expClaim != null)
            {
                var expTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expClaim.Value));
                if (DateTime.UtcNow > expTime.AddMinutes(-_jwtConfig.RefreshBeforeMinutes))
                {
                    throw new Exception("Token已过期，无法刷新");
                }
            }

            // 生成新 token
            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString();
            var userId = Guid.Parse(userIdStr);
            var username = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;
            var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            var hospitalIdStr = principal.FindFirst("HospitalId")?.Value;
            Guid? hospitalId = Guid.TryParse(hospitalIdStr, out var hid) ? hid : null;
            var isCommissionUser = bool.Parse(principal.FindFirst("IsCommissionUser")?.Value ?? "false");

            return GenerateToken(userId, username, roles, hospitalId, isCommissionUser);
        }
        catch
        {
            throw new Exception("Token刷新失败");
        }
    }

    /// <summary>
    /// 从 Token 中获取用户 ID
    /// </summary>
    public Guid? GetUserIdFromToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtConfig.SecretKey ?? "DefaultSecretKeyMustBe32CharsLong!");

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _jwtConfig.Issuer,
                ValidAudience = _jwtConfig.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out var validatedToken);

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}
