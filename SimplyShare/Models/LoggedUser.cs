using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SimplyShare.Models
{
    public class LoggedUser
    {
        private BitmapImage _ProfilePic;
        private string _Nome, _Cognome;
        private Boolean _Modality;

        public LoggedUser(BitmapImage ProfilePic, string Nome, string Cognome, Boolean Modality)
        {
            _ProfilePic = ProfilePic;
            _Nome = Nome;
            _Cognome = Cognome;
            _Modality = Modality;
        }

        public BitmapImage ProfilePic { get { return _ProfilePic; } set { _ProfilePic = value; } }
        public string Nome { get { return _Nome; } set { _Nome = value; } }
        public string Cognome { get { return _Cognome; } set { _Cognome = value; } }
        public Boolean Modality { get { return _Modality; } set { _Modality = value; } }
    }
}
