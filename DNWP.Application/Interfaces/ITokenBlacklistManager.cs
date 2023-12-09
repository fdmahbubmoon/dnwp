using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Application.Interfaces;

public interface ITokenBlacklistManager
{
    void Add(string token, DateTimeOffset expiresAt);
    void RemoveExpired();
    List<(string Token, DateTimeOffset ExpiresAt)> GetBucket();
}
