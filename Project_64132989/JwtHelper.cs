using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Project_64132989
{
    public static class JwtHelper
    {
        private const string SecretKey = "74c137a119d9123f7c85a588d29847325fd60f26e28a326126a03bd913efb9ac35fdde596d41e03c4066ab62f30e9290cf05d59134de54d4b0c6b17b3c84fbd95658485810bffdac835a97cad249103b337e325140c9eb455045b56fbd1776d80729d639a6c91af1b1ee88226c24b0bcda03a9941776fe539a29b6a1ca55f84808d4d9eb92225b62a834314a41b980d1c368e588c9156e02a4764331ae750345f46332dcacb8d73b342e1b99f3f4cc3b8410f8e17f2b3621ceb26916688f6dcc9b1f01d84e4d075648c38d9ff0ab4c4d759d6b151de7fabf389f865fb62751dcefbde1dbd95c14d733ae3409ab380689af82ddde347e4a4afd2626642167f652"; // Secret key
        private const string Issuer = "vinhveer_example.com";
        private const string Audience = "vinhveer_example.com";

        // Hàm tạo JWT Token
        public static string GenerateToken(string user_id, string email, string role, int expirationMinutes = 30)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.NameIdentifier, user_id),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Hàm giải mã và xác thực JWT Token
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = Issuer,
                ValidAudience = Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // Không cho phép thời gian đồng hồ chênh lệch
            };

            try
            {
                // Giải mã và xác thực token
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

                // Kiểm tra nếu token không phải là JWT
                if (validatedToken is JwtSecurityToken jwtToken)
                {
                    // Token hợp lệ, bạn có thể lấy thông tin từ đây
                    return principal;
                }

                return null;
            }
            catch (SecurityTokenException)
            {
                // Nếu token không hợp lệ, bạn có thể xử lý lỗi tại đây
                return null;
            }
        }
    }
}