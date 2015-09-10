using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elephanet.Conventions
{
    /// <summary>
    /// Enum to control the behavior of IDocumentSession.GetById
    /// </summary>
    public enum EntityNotFoundBehavior
    {
        /// <summary>
        /// When Entity is not found by Id, throw an EntityNotFoundException. Default behavior.
        /// </summary>
        Throw = 0,

        /// <summary>
        /// When Entity is not found by Id, return null
        /// </summary>
        ReturnNull = 1
    }
}
