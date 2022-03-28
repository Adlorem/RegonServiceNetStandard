using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BIRService.Models
{
    /// <summary>
    /// Model błędu z serwisu regon.
    /// </summary>
    [XmlRoot(ElementName = "dane", Namespace = "")]
    public class ErrorModel
    {
        [XmlElement("ErrorCode")]
        public string ErrorCode { get; set; }

        [XmlElement("ErrorMessagePl")]
        public string ErrorMessagePl { get; set; }

        [XmlElement("ErrorMessageEn")]
        public string ErrorMessageEn { get; set; }
    }
}
