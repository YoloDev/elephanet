using System;

namespace Elephanet
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException(Guid id, Type type) 
            : base(string.Format("Entity of type {0} with id {1} could not be found.", type, id))
        {
            
        }
    }
}