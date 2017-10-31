using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Awesomium.Core;
using Awesomium.Windows;
using AngleSharp.Parser.Html;
using ZaraCut.Core.HH;
using AngleSharp.Dom.Html;
using System.Data.SqlClient;

namespace ZaraCut.Core
{
    class PageParser: IDisposable
    {
        public void Dispose()
        {
           
        }

        public Anketa ParseRetailStar(string html)
        {
            Anketa anketa = new Anketa();
            anketa.Source = 3;//!!! изменить 
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            #region FIO
            var fioHTML = (document.All.Where(m => m.LocalName == "h5"));
            string fio="";
            foreach (var item in fioHTML)
            {
                fio = item.TextContent;
            }
            string[] newFIO = GetFIO(fio);
            int result = newFIO.Length;
            switch (result)
            {
                case 0:
                    {
                        // Добавить исключение
                        break;
                    }
                case 1:
                    {
                        AddFIOInAnketa(anketa, "---", newFIO[0], "");
                        break;
                    }
                case 2:
                    {
                        AddFIOInAnketa(anketa, newFIO[0], newFIO[1], "");
                        break;
                    }
                case 3:
                    {
                        AddFIOInAnketa(anketa, newFIO[0], newFIO[1], newFIO[2]);
                        break;
                    }
                default:
                    {

                        break;
                    }
            }
            #endregion

            string age = ReturnValueFromAnketa(document, "[class='age']");
            if (age!="")
            {
                age = age.Trim(' ');
                string[] masAge = age.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                anketa.Age = masAge[0];
                anketa.Birthday = GetDate(masAge[1]);
            }
            string returnValue = document.QuerySelector("[class='location']").InnerHtml;
            //returnValue = 
            int n = returnValue.IndexOf("</i>");
            returnValue = returnValue.Substring(n+4);
            string location = ReturnValueFromAnketa(document, "[class='location']");
            //парсир город и гражданство
            return anketa;
        }

        

        public Anketa ParseHH(string html, string html2, string html3, string html4)
        {
            Anketa anketa = new Anketa();
            anketa.Source = 3;
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            #region Vacancy
            anketa.Vacancy = html4;
            #endregion
            #region Salary
            anketa.Salary = html3;
            #endregion
            #region Nationality
            switch (html2)
            {
                case "Россия":
                    {
                        anketa.Nationality = 1;
                        break;
                    }
                case "Беларусь":
                    {
                        anketa.Nationality = 2;
                        break;
                    }
                case "Казахстан":
                    {
                        anketa.Nationality = 3;
                        break;
                    }
                default:
                    {
                        anketa.Nationality = 4;
                        break;
                    }

            }
            #endregion
            //получение ФИО
            #region GetFIO
            var fioHTML = document.All.Where(m => m.LocalName == "h1" && m.ClassName == "header");
            string fio = "";
            foreach (var item in fioHTML)
            {
                fio = item.TextContent;
            }
            string[] newFIO = GetFIO(fio);
            int result = newFIO.Length;
            switch (result)
            {
                case 0:
                    {
                        // Добавить исключение
                        break;
                    }
                case 1:
                    {
                        AddFIOInAnketa(anketa,"---", newFIO[0], "");
                        break;
                    }
                case 2:
                    {
                        AddFIOInAnketa(anketa, newFIO[0], newFIO[1], "");
                        break;
                    }
                case 3:
                    {
                        AddFIOInAnketa(anketa, newFIO[0], newFIO[1], newFIO[2]);
                        break;
                    }
                default:
                    {

                        break;
                    } 
            }
            //Console.WriteLine("{0} {1} {2}", anketa.FirstName, anketa.LastName, anketa.Patronymic);
            #endregion
            #region Gender
            var gender = ReturnValueFromAnketa(document,"[data-qa='resume-personal-gender']");
            gender = gender.ToLower();
            switch (gender)
            {
                case "женщина":
                    {
                        anketa.Gender = 2;
                        break;
                    }
                case "мужчина":
                    {
                        anketa.Gender = 1;
                        break;
                    }
                default:
                    {
                        anketa.Gender = -1;
                        break;
                    }
                    
            }
            #endregion
            #region Age
            string age = ReturnValueFromAnketa(document, "[data-qa='resume-personal-age']");
            //if (age != "")
            //{
            //    //class='resume-header-block'
            //    var ageParser = ReturnValueFromAnketa(document, "[]");
            //    string[] masfio = ageParser.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            //}
            anketa.Age = age;
            #endregion
            #region City
            //var city = document.QuerySelector("[data-qa='resume-personal-address']").TextContent;
            var city = ReturnValueFromAnketa(document, "[data-qa='resume-personal-address']");
            anketa.City = city;
            //anketa.City = GetIdCity(city);
            #endregion
            #region MobPhone
            //var phone = document.QuerySelector("[itemprop='telephone']").TextContent;
            var phone = ReturnValueFromAnketa(document, "[itemprop='telephone']");
            
            anketa.MobPhone =GetPhone( phone);
            #endregion
            #region Email
            //var email = document.QuerySelector("[itemprop='email']").TextContent;
            var email = ReturnValueFromAnketa(document, "[itemprop='email']");
            anketa.Email = email;
            #endregion
            #region Metro
            var metro = ReturnValueFromAnketa(document, "[data-qa='resume-personal-metro']");
            if (metro != "")
            {
                metro = metro.Replace("м. ", "");
            }
            anketa.Metro = metro;
            #endregion
            anketa.Birthday = "";
            anketa.HomePhone = "";
            return anketa;
            //System.IndexOutOfRangeException
            //1 element
            //var emphasize = document.QuerySelector("em");
            //Console.WriteLine(emphasize.ToHtml());   //<em> bold <u>and</u> italic </em>
            // Console.WriteLine(emphasize.TextContent);// bold and italic 
            //Console.WriteLine(emphasize.InnerHtml);  // bold <u>and</u> italic

            //All elements
            //Do something with LINQ
            //var blueListItemsLinq = document.All.Where(m => m.LocalName == "li" && m.ClassList.Contains("blue"));

            ////Or directly with CSS selectors
            //var blueListItemsCssSelector = document.QuerySelectorAll("li.blue");

        }
        private string GetPhone(string phone)
        {
            phone = phone.Replace(" ", "");
            phone = phone.Replace(" ", "");
            phone = phone.Replace("\n", "");
            phone = phone.Replace("+7", "8");
            return phone;
        }
        private string GetDate(string date)
        {
            string newDate = "";
            string[] dateBorn = date.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            DateTime dateBirthday;
            int month = 0;
            switch (dateBorn[1].ToLower())
            {
                case "января":
                    {
                        month = 1;
                        break;
                    }
                case "февраля":
                    {
                        month = 2;
                        break;
                    }
                case "марта":
                    {
                        month = 3;
                        break;
                    }
                case "апреля":
                    {
                        month = 4;
                        break;
                    }
                case "мая":
                    {
                        month = 5;
                        break;
                    }
                case "июня":
                    {
                        month = 6;
                        break;
                    }
                case "июля":
                    {
                        month = 7;
                        break;
                    }
                case "августа":
                    {
                        month = 8;
                        break;
                    }
                case "сентября":
                    {
                        month = 9;
                        break;
                    }
                case "октября":
                    {
                        month = 10;
                        break;
                    }
                case "ноября":
                    {
                        month = 11;
                        break;
                    }
                case "декабря":
                    {
                        month = 12;
                        break;
                    }
                default:
                    break;
            }
            //new DateTime()
            dateBirthday = new DateTime(Convert.ToInt32(dateBorn[2]), month, Convert.ToInt32(dateBorn[0]));
            newDate = dateBirthday.ToString();
            return newDate;
        }
        private string GetIdMetro(string metro)
        {
            string idMetro = "";
            SqlConnection sqlConnection = new SqlConnection(Path.SQLPath);
            sqlConnection.Open();
            //sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("select isnull(max(stationid),0) stationid from station where station ='" + metro.Trim() + "'", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                while (sqlDataReader.Read())
                {
                    idMetro = sqlDataReader["stationid"].ToString();
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            idMetro = idMetro + ",";
            return idMetro;
        }
        private int GetIdCity(string city)
        {
            SqlConnection sqlConnection = new SqlConnection(Path.SQLPath);
            sqlConnection.Open();
            int idCity = 0;
            //sqlConnection.Open();
            SqlCommand sqlCommand = new SqlCommand("select isnull(max(cityid),0) Cityid from city where city ='" + city.Trim() + "'", sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                while (sqlDataReader.Read())
                {
                    idCity = Convert.ToInt32(sqlDataReader["Cityid"].ToString());
                }
            }
            finally
            {
                sqlDataReader.Close();
                //sqlConnection
            }
            return idCity;
        }
        private string ReturnValueFromAnketa(IHtmlDocument document,string seelector)
        {
            var returnValue = "";
            try
            {
                returnValue = document.QuerySelector(seelector).TextContent;
            }
            catch (Exception)
            {
                returnValue = "";
            }
            return returnValue;
        }
        private void AddFIOInAnketa( Anketa anketa, string lastName, string firstName,  string patronymic)
        {
            anketa.FirstName = firstName.Trim();
            anketa.LastName = lastName.Trim();
            anketa.Patronymic = patronymic.Trim();
        }
        private string[] GetFIO(string fio)
        {
            string[] masfio = fio.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return masfio;
        }
    }
}
