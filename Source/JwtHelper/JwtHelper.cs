using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtHelper
{
    public class JwtHelper
    {
        public SymmetricSecurityKey Key { get; set; }
        public TimeSpan TokenDuration { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }

        public JwtHelper(string key)
        {
            Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        }


        public string GenerateToken(List<Claim> claims)
        {
            var credentials = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var currentDate = DateTime.Now;

            var expireTime = currentDate.Add(TokenDuration);
            var minValidDate = currentDate.Subtract(new TimeSpan(hours: 0, minutes: 10, seconds:0));

            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = Issuer,
                Audience = Audience,
                SigningCredentials = credentials,
                Subject = new ClaimsIdentity(claims),
                IssuedAt = currentDate,
                NotBefore = minValidDate,
                Expires = expireTime,
            };

            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(securityTokenDescriptor);

            return handler.WriteToken(token);
        }

        public bool ValidateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = true, // Because there is no audiance in the generated token
                ValidateIssuer = true,   // Because there is no issuer in the generated token
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = Key, // The same key as the one that generate the token
            };

            try
            {
                ClaimsPrincipal claims = tokenHandler.ValidateToken(authToken, validationParameters, out var validatedToken);
                return validatedToken != null;
            }
            catch (SecurityTokenInvalidSignatureException ise)
            {
                Console.WriteLine("Bad signature: " + ise.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }

    //public void ConfigureServices(IServiceCollection services)
    //{
    //    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    //            .AddJwtBearer(options =>
    //            {
    //                options.TokenValidationParameters = new TokenValidationParameters
    //                {
    //                    ValidateIssuerSigningKey = true,
    //                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.
    //                    GetBytes(Configuration.GetSection("AppSettings:JwtToken").Value)),
    //                    ValidateIssuer = false,
    //                    ValidateAudience = false
    //                };
    //            });
    //}
}
