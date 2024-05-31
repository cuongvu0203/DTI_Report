using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DTI_Report.Infrastructure.Helper
{
    public static class CoreUtility
    {
        public static string DecodeJwt(string token, string secretKey)
        {
            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                var principal = handler.ValidateToken(token, parameters, out var validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;
                return jwtToken.Payload.SerializeToJson();
            }
            catch (SecurityTokenExpiredException)
            {
                return "{\"error\": \"Token has expired\"}";
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                return "{\"error\": \"Invalid token signature\"}";
            }
            catch (Exception)
            {
                return "{\"error\": \"Failed to decode token\"}";
            }
        }
    }
}
