using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BIRService.Models
{
    /// <summary>
    /// Model podmiotu zwracany z bazy Regon
    /// </summary>
    [XmlRoot(ElementName ="dane", Namespace = "")]
    public class DanePodmiotu
    {
        [XmlElement("Regon")]
        public string Regon { get; set; }

        [XmlElement("Nip")]
        public string Nip { get; set; }

        [XmlElement("StatusNip")]
        public string StatusNip { get; set; }

        [XmlElement("Nazwa")]
        public string Nazwa { get; set; }

        [XmlElement("Wojewodztwo")]
        public string Wojewodztwo { get; set; }

        [XmlElement("Powiat")]
        public string Powiat { get; set; }

        [XmlElement("Gmina")]
        public string Gmina { get; set; }

        [XmlElement("Miejscowosc")]
        public string Miejscowosc { get; set; }

        [XmlElement("KodPocztowy")]
        public string KodPocztowy { get; set; }

        [XmlElement("Ulica")]
        public string Ulica { get; set; }

        [XmlElement("NrNieruchomosci")]
        public string NrNieruchomosci { get; set; }

        [XmlElement("NrLokalu")]
        public string NrLokalu { get; set; }

        [XmlElement("Typ")]
        public string Typ { get; set; }

        [XmlElement("SilosID")]
        public int SilosID { get; set; }

        [XmlElement("DataZakonczeniaDzialalnosci")]
        public string DataZakonczeniaDzialalnosci { get; set; }

        [XmlElement("MiejscowoscPoczty")]
        public string MiejscowoscPoczty { get; set; }

        [XmlIgnore]
        public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();

    }
}
