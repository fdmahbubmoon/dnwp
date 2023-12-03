using DNWP.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNWP.Application.Interfaces;

public interface ITokenService
{
    Task<TokenVm> GenerateTokenAsync(string userName, string password);
    Task SignoutAsync();
}
