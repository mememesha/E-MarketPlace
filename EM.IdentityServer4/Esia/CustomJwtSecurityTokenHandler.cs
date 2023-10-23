using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Xml;

namespace EM.IdentityServer4.Esia
{
    /// <summary>
    /// Собственная реализация проверки маркера доступа.
    /// НЕ для использования в промышленной среде!
    /// </summary>
    public class CustomSecurityTokenValidator : JwtSecurityTokenHandler
    {
        protected override string ValidateIssuer(string issuer, JwtSecurityToken jwtToken, TokenValidationParameters validationParameters)
            => validationParameters.ValidIssuer;

    }
}
