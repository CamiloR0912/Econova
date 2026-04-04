using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Econova.Services
{
    public interface IDialogService
    {
        bool Confirmar(string mensaje, string titulo);
        void Informar(string mensaje, string titulo);
    }
}
