using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NexusERP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;

    public class JwtDecoder
    {
        public List<string> GetRolesFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
            {
                throw new ArgumentException("Invalid token");
            }

            var jwtToken = handler.ReadJwtToken(token);

            var rolesClaim = jwtToken?.Claims.FirstOrDefault(c => c.Type == "role");

            if (rolesClaim != null)
            {
                return rolesClaim.Value.Split(',').ToList();
            }

            return new List<string>();
        }
    }
}
