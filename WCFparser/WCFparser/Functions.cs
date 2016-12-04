using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HtmlAgilityPack;
using LiteDB;

namespace WCFparser
{
    public class Functions
    {
        public static Artist ShowArtistsByFullname(string artist)
        {

            var artistInput = SearchArtistInDb(artist);
            //Если артиста нет в локальной БД, вызываем функцию получения из сети и добавляем его в БД
            if (artistInput == null)
            {
                artistInput = GetArtistInfo(artist);
                if (artistInput != null)
                {
                    InsertArtistIntoDb(artistInput);
                }
            }
            return artistInput;
        }

        public static Artist ShowArtistsByKeyword(string keyword)
        {
            var keywordInput = SearchArtistInDb(keyword);
            //Если артиста нет в локальной БД, вызываем функцию получения из сети и добавляем его в БД
            if (keywordInput == null)
            {
                keywordInput = GetKeyWordInfo(keyword);
                if (keywordInput != null)
                {
                    InsertArtistIntoDb(keywordInput);
                }
            }
            return keywordInput;
        }

        public static void InsertArtistIntoDb(Artist artist)
        {
            using (var db = new LiteDatabase(@"D:\прожики\WCFparser\WCFParser\MyDb.db"))
            {
                var artistCollection = db.GetCollection<Artist>("artists");

                artistCollection.Insert(artist);
            }
        }

        public static Artist SearchArtistInDb(string art)
        {
            using (var db = new LiteDatabase(@"D:\прожики\WCFparser\WCFParser\MyDb.db"))
            {
                var col = db.GetCollection<Artist>("artists");
                // Поиск по БД, где поле name, содержит введенную строку
                var searchartist = col.FindOne(x => x.Name.Contains(art));
                return searchartist;
            }

        }

        //функция вывода полей артиста в консоль
        public static void PrintArtist(Artist artist)
        {
            Console.WriteLine("Band:" + artist.Name);
            Console.WriteLine("\n\nTop Tracks:");
            foreach (var c in artist.TopTracks)
            {
                Console.WriteLine("*" + c);
            }
            Console.WriteLine("\n\nGenres:");
            foreach (var c in artist.Genres)
            {
                Console.Write(c + " ");
            }
            Console.WriteLine("\n\nTop Albums:");
            foreach (var c in artist.TopAlbums)
            {
                Console.WriteLine("*" + c);
            }
            Console.WriteLine("\n\nSimilar Artists:");
            foreach (var c in artist.SimilarArtists)
            {
                Console.WriteLine("*" + c);
            }
            Console.WriteLine("\n\nBio:" + artist.Bio);
        }

        //функция получения информации об артисте из сети
        public static Artist GetArtistInfo(string artist)
        {
            Console.Clear();
            string htmlName = "http://www.last.fm/music/" + artist;
            HtmlDocument document = null;

            try
            {
                HtmlWeb web = new HtmlWeb();
                document = web.Load(htmlName);
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine("artist doesn't exist");
                Console.WriteLine("press any key to continue");
                Console.ReadLine();
                return null;
            }

            try
            {

                //Название группы
                string bandName = document.DocumentNode.SelectSingleNode("//div/h1[@class='header-title']").InnerText.Trim();

                //Популярные песни
                List<string> topTracks = new List<string>();
                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//span[@class='chartlist-ellipsis-wrap']");

                foreach (var node in nodes)
                {
                    var topTrack = node.SelectSingleNode(".//a").Attributes["title"].Value;
                    topTracks.Add(topTrack);
                }
                //Жанры
                List<string> genres = new List<string>();
                nodes = document.DocumentNode.SelectNodes("//li[@class='tag']");

                foreach (var node in nodes)
                {
                    var genre = node.SelectSingleNode(".//a").InnerText;
                    genres.Add(genre);
                }
                //Популярные альбомы
                List<string> topAlbums = new List<string>();
                nodes = document.DocumentNode.SelectNodes(".//section[@class='grid-items-section section-with-control']//p[@class='grid-items-item-main-text']");

                foreach (var node in nodes)
                {
                    var topAlbum = node.SelectSingleNode(".//a").InnerText;
                    topAlbums.Add(topAlbum);

                }
                //Похожие исполнители
                List<string> similarArtists = new List<string>();
                nodes = document.DocumentNode.SelectNodes(".//section[@class='grid-items-section']//p[@class='grid-items-item-main-text']");

                foreach (var node in nodes)
                {
                    var similarArtist = node.SelectSingleNode(".//a").InnerText;
                    similarArtists.Add(similarArtist);

                }
                //Краткая информация о группе
                string bandInfo = document.DocumentNode.SelectSingleNode("//div[@class='wiki-content']//p").InnerText;

                return new Artist()
                {

                    Name = bandName,
                    TopTracks = topTracks,
                    Genres = genres,
                    TopAlbums = topAlbums,
                    SimilarArtists = similarArtists,
                    Bio = bandInfo

                };
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("\nno more info");
                Console.WriteLine("press any key to continue");
                Console.ReadLine();
                return null;
            }
        }



        //Функция получения информации об артисте из сети по ключевому слову
        public static Artist GetKeyWordInfo(string keyword)
        {
            string firstMatch;
            Console.Clear();
            string htmlName = "http://www.last.fm/search?q=" + keyword;
            HtmlDocument document = null;
            try
            {
                HtmlWeb web = new HtmlWeb();
                document = web.Load(htmlName);
            }
            catch (System.Net.WebException ex)
            {
                Console.WriteLine("\nошибка WebException\n");
            }
            try
            {
                //Название группы, первое в списке совпадений по поиску
                firstMatch = document.DocumentNode.SelectSingleNode(".//p[@class='grid-items-item-main-text']//a").InnerText.Trim();

            }
            catch (System.NullReferenceException ex1)
            {

                Console.WriteLine("no search results");
                Console.WriteLine("press any key to continue");
                Console.ReadLine();
                return null;
            }
            return GetArtistInfo(firstMatch);
        }   
    }
}