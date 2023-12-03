using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Common.Helpers;

public interface ILoggedInUserService
{
    public long? UserId { get; }
    public string UserEmail { get; }
    public string FullName { get; }
    public string MobileNumber { get; }
    public string UserName { get; }
    public List<string> Roles { get; }
    public bool IsAuthenticated { get; }
    public string AccessToken { get; }
    public DateTimeOffset JwtExpiresAt { get; }
}