using System;

namespace Fido.ViewModel
{
    public interface IModelAPI
    {
        void PropertyError(string Property, string Message);
        void ModelError(string Message);
        
        bool ModelStateIsValid();
    }
}
