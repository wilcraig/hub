using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace hub
{
    public class MyCryptoProvider : ICryptoProvider
    {
        public object Create(string algorithm, params object[] args)
        {
            throw new NotImplementedException();
        }

        public bool IsSupportedAlgorithm(string algorithm, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Release(object cryptoInstance)
        {
            throw new NotImplementedException();
        }
    }
}