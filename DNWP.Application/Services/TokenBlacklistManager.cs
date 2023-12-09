using DNWP.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Application.Services;

public class TokenBlacklistManager : ITokenBlacklistManager
{
    public readonly List<(string Token, DateTimeOffset ExpiresAt)> _tokenBucket = new();
    public void Add(string token, DateTimeOffset expiresAt)
    {
        _tokenBucket.Add(new(token, expiresAt));
    }

    public void RemoveExpired()
    {
        _tokenBucket.RemoveAll(a => a.ExpiresAt < DateTimeOffset.Now);
    }

    public List<(string Token, DateTimeOffset ExpiresAt)> GetBucket()
    {
        return _tokenBucket;
    }
}
