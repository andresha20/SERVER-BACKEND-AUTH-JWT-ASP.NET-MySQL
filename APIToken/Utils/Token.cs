using APIToken.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace APIToken.Utils
{
    public class Token
    {
        public string GenerateToken(UserModel user, HttpRequest request, String keyProp)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, $"{user.User_name} {user.User_lastname}"),
                new Claim(ClaimTypes.Email, user.User_email),
                new Claim("Id", $"{user.User_id}")
            };
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(keyProp));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var securityToken = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: creds);
            string token = new JwtSecurityTokenHandler().WriteToken(securityToken);
            Console.WriteLine("token " + token);

            ///////////////
            var uriBuilder = new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }
            var baseUri = uriBuilder.Uri.AbsoluteUri;

            var resp = new HttpResponseMessage();

            var cookie = new CookieHeaderValue("jwt", token)
            {
                Expires = DateTimeOffset.Now.AddDays(1),
                Domain = baseUri,
                Path = "/",
                HttpOnly = false,
            };

            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            return token;
        }
    }
}
