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
        public Anketa ParseJobMo(string html)
        {
            Anketa anketa = new Anketa();
            anketa.Source = 0;
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            var lines = document.All.Where(m => m.LocalName == "tr");
            var value="";
            foreach (var item in lines)
            {
                value = item.TextContent.ToString();
                //value.Replace("\n", "");
                value = value.Replace("\n", "");
                value = value.Trim();
                //switch(value.StartsWith("Муж.")
            }
            #region FIO
            //string fio = ReturnValueFromAnketa(document, "[class='sj_h3']");
            //string[] newFIO = GetFIO(fio);
            //int result = newFIO.Length;
            //switch (result)
            //{
            //    case 0:
            //        {
            //            // Добавить исключение
            //            break;
            //        }
            //    case 1:
            //        {
            //            AddFIOInAnketa(anketa, "---", newFIO[0], "");
            //            break;
            //        }
            //    case 2:
            //        {
            //            AddFIOInAnketa(anketa, newFIO[0], newFIO[1], "");
            //            break;
            //        }
            //    case 3:
            //        {
            //            AddFIOInAnketa(anketa, newFIO[0], newFIO[1], newFIO[2]);
            //            break;
            //        }
            //    default:
            //        {

            //            break;
            //        }
            //}
            #endregion
            return anketa;
        }
        public Anketa ParseSuperJob(string html)
        {
            Anketa anketa = new Anketa();
            anketa.Source = 2;
            var parser = new HtmlParser();
            var document = parser.Parse(html);
            #region FIO
            string fio = ReturnValueFromAnketa(document, "[class='sj_h3']");
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
            var element = document.All.Where(m => m.LocalName == "div");
            //document.QuerySelectorAll("div.items");
            //.cf
            //document.All.Where(m => m.LocalName == "div" && m.ClassName == "text");
            string value = "";

            foreach (var item in element)
            {
                try
                {
                    value = item.TextContent.ToString();
                    //value.Replace("\n", "");
                    value = value.Replace("\n", "");
                    value = value.Trim();
                    if (value.StartsWith("Муж.") || value.StartsWith("Жен."))
                    {
                        List<string> cityList = GetCity();
                        string[] masMain = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);


                        string[] masAgeDate = masMain[1].Split(new char[] { '(' }, StringSplitOptions.RemoveEmptyEntries);
                        #region CityAndCityzenship
                        string[] masCityAndCityzenship = value.Split(new string[] { "  " }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            string[] masCity = masCityAndCityzenship[1].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var r in masCity)
                            {
                                var city = cityList.FirstOrDefault(t => t == r.ToLower());
                                if (city != null)
                                {
                                    anketa.City = city;
                                    break;
                                }
                            }
                        }
                        catch
                        {

                        }
                        try
                        {
                            string[] masCityzenship = masCityAndCityzenship[2].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var r in masCityzenship)
                            {
                                if (r.ToLower().IndexOf("гражданство") > -1)
                                {
                                    string[] mas = r.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    anketa.Nationality = (mas[1].ToLower().Contains("россия") ? 1 : (mas[1].ToLower().Contains("беларусь") ? 2 : (mas[1].ToLower().Contains("казахстан") ? 3 : 4)));
                                }
                            }
                        }
                        catch
                        {
                        }
                        #endregion
                        masAgeDate[1] = masAgeDate[1].Replace(")", "");
                        anketa.Birthday = GetDate(masAgeDate[1]);
                        anketa.Age = masAgeDate[0];
                        switch (masMain[0])
                        {
                            case "Муж.":
                                {
                                    anketa.Gender = 1;
                                    break;
                                }
                            case "Жен.":
                                {
                                    anketa.Gender = 2;
                                    break;
                                }
                            default:
                                break;
                        }
                    }
                }
                catch
                {
                    value = "";
                }
            }
            string doljnost = ReturnValueFromAnketa(document, "[class='sj_h1 sj_block m_b_2 h_font_weight_light h_word_wrap_break_word']");
            anketa.Vacancy = doljnost;
            string price = ReturnValueFromAnketa(document, "[class='h_font_weight_medium']");
            anketa.Salary = price;
            //string phone = ReturnValueFromAnketa(document, "[class='sj_block m_t_2 sj_h3 m_b_0']");
            //string phone2 = ReturnValueFromAnketa(document, "[class='ng-binding']");
            //string phone3 = ReturnValueFromAnketa(document, "div.sj_h3.m_b_0");

            //phone3 =  GetPhone(phone2);
            //phone3 = phone2.Remove(11, phone2.Length - 11);

            List<string> test = new List<string>();
            var res = document.QuerySelectorAll("div.sj_h3.m_b_0");
            foreach (var item in res)
            {
                string val = item.TextContent;
                val = val.Replace("\n", "");
                val = val.Trim();
                val = GetPhone(val);
                if (val != "")
                {
                    val = val.Remove(11, val.Length - 11);
                    test.Add(val);
                }
            }
            foreach (var item in test)
            {
                if (item.StartsWith("849"))
                {
                    if (anketa.HomePhone == "")
                    {
                        anketa.HomePhone = item;
                    }
                }
                else
                {
                    if (anketa.MobPhone == "")
                    {
                        anketa.MobPhone = item;
                    }
                    else
                    {
                        anketa.HomePhone = item;
                    }

                }
            }
            //!! string resPhone = GetPhone(phone != "" ? phone : (phone2 != "" ? phone2 : ""));
            //anketa.MobPhone = resPhone.Remove(11, resPhone.Length - 11);
            string email = ReturnValueFromAnketa(document, "[class='sj_block m_t_0 sj_h3 m_b_0 h_display_inline_block']");
            //var t =document.QuerySelectorAll("a.sj_block.m_t_0.sj_h3.m_b_0.h_display_inline_block");
            email = ReturnValueFromAnketa(document, "a.sj_block.m_t_0.sj_h3.m_b_0.h_display_inline_block");
            //email = ReturnValueFromAnketa(document, "a.ng-binding");
            email = email.Replace("\n", "");
            email = email.Trim();
            anketa.Email = email;
            anketa.Metro = "";

            return anketa;
        }

        public Anketa ParseRetailStar(string html)
        {
            Anketa anketa = new Anketa();
            anketa.Source = 36;
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
            #region Age/Birthday
            string age = ReturnValueFromAnketa(document, "[class='age']");
            if (age!="")
            {
                age = age.Trim(' ');
                string[] masAge = age.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                anketa.Age = masAge[0];
                anketa.Birthday = GetDate(masAge[1]);
            }
            #endregion
            #region Nationality/City
            string returnValue = document.QuerySelector("[class='location']").InnerHtml;
            //returnValue = 
            int n = returnValue.IndexOf("</i>");
            returnValue = returnValue.Substring(n+4);
            string[] masLocation = returnValue.Split(new string[] {"<br>"},StringSplitOptions.RemoveEmptyEntries );
            switch (masLocation[0].ToLower())
            {
                case "российская федерация":
                    {
                        anketa.Nationality = 1;
                        break;
                    }
                case "республика беларусь":
                    {
                        anketa.Nationality = 2;
                        break;
                    }
                case "казахстан":
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
            anketa.City = masLocation[1];
            //(new char[] { "<br/>" }, StringSplitOptions.RemoveEmptyEntries);
            //string location = ReturnValueFromAnketa(document, "[class='location']");
            #endregion
            #region MobPhone
            string mobPhone = ReturnValueFromAnketa(document, "[class='phone']");
            anketa.MobPhone = GetPhone(mobPhone);
            #endregion
            anketa.Email = "";
            anketa.Gender = -1;
            anketa.HomePhone = "";
            anketa.Salary = "";
            anketa.Vacancy = "";
            anketa.Metro = "";
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
            phone = phone.Replace("(", "");
            phone = phone.Replace(")", "");
            phone = phone.Replace("-", "");
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
            SqlConnection sqlConnection = new SqlConnection(DBPath.SQLPath);
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
            SqlConnection sqlConnection = new SqlConnection(DBPath.SQLPath);
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
        private List<string> GetCity()
        {
            List<string> city = new List<string>();
            SqlConnection sqlConnection = new SqlConnection(DBPath.SQLPath);
            SqlCommand sqlCommand = new SqlCommand("select city from city ", sqlConnection);
            sqlConnection.Open();
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
            try
            {
                while (sqlDataReader.Read())
                {
                    city.Add(sqlDataReader["City"].ToString().ToLower());
                }
            }
            finally
            {
                sqlDataReader.Close();
            }
            return city;
        }
        public void Dispose()
        {

        }
    }
}
