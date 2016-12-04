using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using HtmlAgilityPack;
using LiteDB;

namespace WCFparser
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Parser" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Parser.svc или Parser.svc.cs в обозревателе решений и начните отладку.
    public class Parser: IParser
    {
        
        public Artist ShowArtistByFullname(string artist)
        {
            return Functions.ShowArtistsByFullname(artist);
        }

        public Artist ShowArtistByKeyword(string keyword)
        {
            return Functions.ShowArtistsByKeyword(keyword);
        }
    }
}
