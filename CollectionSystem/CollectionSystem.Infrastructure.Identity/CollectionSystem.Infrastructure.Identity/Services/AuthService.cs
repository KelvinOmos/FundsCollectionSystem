using CollectionSystem.Application.DTOs.Account;
using CollectionSystem.Application.Interfaces;
using CollectionSystem.Application.Wrappers;
using CollectionSystem.Domain.Entities;
using CollectionSystem.Domain.Settings;
using CollectionSystem.Infrastructure.Identity.Helpers;
using CollectionSystem.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using CollectionSystem.Infrastructure.Identity.Contexts;
using CollectionSystem.Service.Application.Interfaces.Identity;
using Serilog;

namespace CollectionSystem.Infrastructure.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IOptions<JWTSettings> _jwtSettings;
        private readonly IdentityContext _identityContext;
        private readonly ILogger<AuthService> _logger;
        private readonly IHttpContextAccessor _httpContext;
        protected HttpContext HttpContext => _httpContext.HttpContext;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWTSettings> jwtSettings, SignInManager<ApplicationUser> signInManager, ILogger<AuthService> logger, IHttpContextAccessor httpContext, IdentityContext identityContext)
        {
            _userManager = userManager;
            _jwtSettings = jwtSettings;
            _signInManager = signInManager;
            _roleManager = roleManager;         
            _logger = logger;
            _identityContext = identityContext;
            _httpContext = httpContext;
        }

        private static RSAParameters RSAParameters
        {
            get => new RSAParameters()
            {
                Modulus = Convert.FromBase64String("z7eXmrs9z3Xm7VXwYIdziDYzXGfi3XQiozIRa58m3ApeLVDcsDeq6Iv8C5zJ2DHydDyc0x6o5dtTRIb23r5/ZRj4I/UwbgrwMk5iHA0bVsXVPBDSWsrVcPDGafr6YbUNQnNWIF8xOqgpeTwxrqGiCJMUjuKyUx01PBzpBxjpnQ++Ryz6Y7MLqKHxBkDiOw5wk9cxO8/IMspSNJJosOtRXFTR74+bj+pvNBa8IJ+5Jf/UfJEEjk+qC+pohCAryRk0ziXcPdxXEv5KGT4zf3LdtHy1YwsaGLnTb62vgbdqqCJaVyHWOoXsDTQBLjxNl9o9CzP6CrfBGK6JV8pA/xfQlw=="),
                Exponent = Convert.FromBase64String("AQAB"),
                P = Convert.FromBase64String("+VsETS2exORYlg2CxaRMzyG60dTfHSuv0CsfmO3PFv8mcYxglGa6bUV5VGtB6Pd1HdtV/iau1WR/hYXQphCP99Pu803NZvFvVi34alTFbh0LMfZ+2iQ9toGzVfO8Qdbj7go4TWoHNzCpG4UCx/9wicVIWJsNzkppSEcXYigADMM="),
                Q = Convert.FromBase64String("1UCJ2WAHasiCdwJtV2Ep0VCK3Z4rVFLWg3q1v5OoOU1CkX5/QAcrr6bX6zOdHR1bDCPsH1n1E9cCMvwakgi9M4Ch0dYF5CxDKtlx+IGsZJL0gB6HhcEsHat+yXUtOAlS4YB82G1hZqiDw+Q0O8LGyu/gLDPB+bn0HmbkUC2kP50="),
                DP = Convert.FromBase64String("CBqvLxr2eAu73VSfFXFblbfQ7JTwk3AiDK/6HOxNuL+eLj6TvP8BvB9v7BB4WewBAHFqgBIdyI21n09UErGjHDjlIT88F8ZtCe4AjuQmboe/H2aVhN18q/vXKkn7qmAjlE78uXdiuKZ6OIzAJGPm8nNZAJg5gKTmexTka6pFJiU="),
                DQ = Convert.FromBase64String("ND6zhwX3yzmEfROjJh0v2ZAZ9WGiy+3fkCaoEF9kf2VmQa70DgOzuDzv+TeT7mYawEasuqGXYVzztPn+qHhrogqJmpcMqnINopnTSka6rYkzTZAtM5+35yz0yvZiNbBTFdwcuglSK4xte7iU828stNs/2JR1mXDtVeVvWhVUgCE="),
                InverseQ = Convert.FromBase64String("Heo0BHv685rvWreFcI5MXSy3AN0Zs0YbwAYtZZd1K/OzFdYVdOnqw+Dg3wGU9yFD7h4icJFwZUBGOZ0ww/gZX/5ZgJK35/YY/DeV+qfZmywKauUzC6+DPsrDdW1uf1eAety6/huRZTduBFTwIOlPdZ+PY49j6S38DjPFNImn0cU="),
                D = Convert.FromBase64String("IvjMI5cGzxkQqkDf2cC0aOiHOTWccqCM/GD/odkH1+A+/u4wWdLliYWYB/R731R5d6yE0t7EnP6SRGVcxx/XnxPXI2ayorRgwHeF+ScTxUZFonlKkVK5IOzI2ysQYMb01o1IoOamCTQq12iVDMvV1g+9VFlCoM+4GMjdSv6cxn6ELabuD4nWt8tCskPjECThO+WdrknbUTppb2rRgMvNKfsPuF0H7+g+WisbzVS+UVRvJe3U5O5X5j7Z82Uq6hw2NCwv2YhQZRo/XisFZI7yZe0OU2JkXyNG3NCk8CgsM9yqX8Sk5esXMZdJzjwXtEpbR7FiKZXiz9LhPSmzxz/VsQ==")
            };
        }

        public async Task<Response<AuthenticationResponse>> Token(AuthenticationRequest request)
        {
            Log.Information("WELCOME TO GENERATE TOKEN SERVICE");
            bool retStatus = false;
            var retResponse = "";
            //Token service
            try
            {
                var user = await _userManager.FindByEmailAsync(request.UserID);
                if (user == null)
                {
                    user = await _userManager.FindByNameAsync(request.UserID);
                }
                if (user == null)
                {
                    user = await _userManager.FindByIdAsync(request.UserID);
                }
                if (user == null)
                {
                    Log.Information($"No Profile details found for {request.UserID}");
                    retResponse = $"No Profile details found for {request.UserID}";
                    return await Task.FromResult(new Response<AuthenticationResponse>(succeeded: false, message: $"No Profile with {request.UserID}.", code: (long)ApiResponseCodes.INVALID_REQUEST, data: null));
                }
                var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
                if (!result.Succeeded)
                {
                    Log.Information($"Invalid Credential");
                    retResponse = $"Invalid Credential";
                    return await Task.FromResult(new Response<AuthenticationResponse>(succeeded: false, message: $"Invalid Credentials for '{request.UserID}'.", data: null));
                }
             
                AuthenticationResponse response = new AuthenticationResponse();
                response.Token = await GenerateJWToken(user);
                response.Status = user.Status;
                var refreshToken = GenerateRefreshToken();
                response.RefreshToken = refreshToken;
                retStatus = true;
                retResponse = "Successful";
                return await Task.FromResult(new Response<AuthenticationResponse>(succeeded: true, message: "Successful", data: response, code: (long)ApiResponseCodes.OK));
            }
            catch (Exception eex)
            {
                Log.Error($"GENERATE TOKEN EXCEPTION:: {eex?.ToString()}");
                retResponse = eex?.Message;
                return await Task.FromResult(new Response<AuthenticationResponse>(succeeded: false, message: $"Error:: {eex?.Message}", data: null, code: (long)ApiResponseCodes.EXCEPTION));
            }            
        }

        private async Task<string> GenerateJWToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            Guid userid = Guid.Parse(user.Id);
            var roles = await _userManager.GetRolesAsync(user);

            Guid role = Guid.Empty;
            if (roles.Count > 0)
            {
                var roleObj = await _roleManager.FindByNameAsync(roles.FirstOrDefault());
                role = Guid.Parse(roleObj.Id);
            }

            var roleClaims = new List<Claim>();            

            for (int i = 0; i < roles.Count; i++)
            {
                roleClaims.Add(new Claim("roles", roles[i]));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id),
                new Claim("role", roles.FirstOrDefault()),
                new Claim("UserId", user.Id),
                new Claim("status", user.Status.ToString()),
            }
            //.Union(userClaims)
            .Union(roleClaims);

            return GenerateAccessToken(claims);
        }

        public string GenerateAccessToken(IEnumerable<Claim> claims)
        {
            RSACryptoServiceProvider p = new RSACryptoServiceProvider();
            p.FromXmlString("<RSAKeyValue><Modulus>u9zwdVNeGMvOX5rzkuMjHqWyRFaJXBijPtU2ul3OqcnM2luDII2Th7k64NZ3K3cURiaSFibynznbpR+M5kw0UJn/fFYGgYi1PUxLcZYlM33audlX3hhzGpMitpHGx28fzuktvuEEN7tIG/utewmHzOtJmyKf8U+wLUN+qtgxVxE=</Modulus><Exponent>AQAB</Exponent><P>3GuqSth9TpHWk+W+X41IPQwpbXtCLy53r2nDN54LuYIomG3wLM/SKWLtMMPhIOwsRhMNj/DCj/QGTdp4ehCCQw==</P><Q>2i/qOyFzGg2P4WIVDjSYHwdJAAe2b//yLvy+IQDammAh5B6wgfhOLK2FCD6HHU1+JUUbS9wE2y6j1iBwIHVeGw==</Q><DP>f2932yHjAIPsrUFMmW3TcAgSA4wZrbGN3Mqm5Qbo/G22Drqw+xeECA172I/HHwOsbS8izi+CLTwGUQiYUuoshQ==</DP><DQ>dhWKklgHIGmInjVkKd0DG3/o3VBPR4xg+VcmW1xH81bl4L1PT/gf7wQ2RID6xTwkcm1VWZgJNMqoNwI1TQvYUw==</DQ><InverseQ>YiiVowCE7oiIgOraxtuuL63bEsq15FUbjB0FW40FLktaGGp4C5geA7uNewYfzBtNsNm66DYBjBAGTSqd5waBCg==</InverseQ><D>hksldamX7X/b0kpRbqKCW4k1i1aVMMnIAMWoLseaUZOZpkqezSmH2hEWvt1xKRgh3Rf7fGxxKlPQ6RRJw+ObzP0OUQtPwHrAQelIg8n5XtcZvrud4EC1UuCQXjeyg1/WtRB2iMB+ZncLuIYZhwDgpRbzAa8RirQAWUixBqzRIrU=</D></RSAKeyValue>");
            RSAParameters rsasec = p.ExportParameters(true);
            var issuer = _jwtSettings.Value.Issuer;
            var audience = _jwtSettings.Value.Audience;
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Value.Key);
            var rsakey = Encoding.ASCII.GetBytes(_jwtSettings.Value.SecretKey);
            // RSAParameters rSAParameters = RSAParameters;

            var jwtToken = new JwtSecurityToken(issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_jwtSettings.Value.DurationInMinutes.ToString())),
                signingCredentials: new SigningCredentials(new RsaSecurityKey(rsasec), SecurityAlgorithms.RsaSsaPssSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var key = RSA.Create();
                key.FromXmlString(_jwtSettings.Value.SecretKey);
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null) return null;
                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new RsaSecurityKey(key.ExportParameters(false))
                    //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Value.SecretKey))
                };
                SecurityToken securityToken;
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out securityToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool ValidateToken(string token, out string username)
        {
            username = null;

            var simplePrinciple = GetPrincipal(token);
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

    }
}
 