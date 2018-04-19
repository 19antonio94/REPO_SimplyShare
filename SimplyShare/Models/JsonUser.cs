using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplyShare.Models
{
    class JsonUser
    {
        private string _Nome, _Cognome, _PathDownload;
        public JsonUser(LoggedUser u)
        {
            _Nome = u.Nome;
            _Cognome = u.Cognome;
            if (u.PathDownload == null)
                _PathDownload = Utilities.Persistency.getDownloadDirectory();
            else
                _PathDownload = u.PathDownload;
        }

        public JsonUser()
        {

        }
        public string Nome { get => _Nome; set => _Nome = value; }
        public string Cognome { get => _Cognome; set => _Cognome = value; }
        public string PathDownload { get => _PathDownload; set => _PathDownload = value; }
    }
}
