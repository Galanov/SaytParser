using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZaraCut.Core
{
    public class Anketa
    {
        public Anketa()
        {
            this.MobPhone = "";
            this.HomePhone = "";
            this.Nationality = 4;
        }
        //фамилия
        public string LastName
        {
            get;set;
        }
        //имя
        public string FirstName
        {
            get;
            set;
        }
        // отчество
        public string Patronymic
        {
            get;

            set;
        }
        //дата рождения
        public string Birthday
        {
            get;
            set;
        }
        // город 
        public string City
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public int Gender
        {
            get;
            set;
        }

        public string HomePhone
        {
            get;

            set;
        }

        

        

        public string MobPhone
        {
            get;
            set;
        }

        

        public int Nationality
        {
            get;
            set;
        }

        public int Source
        {
            get;
            set;
        }
        public string Age { get; set; }
        public string Salary { get; set; }
        public string Vacancy { get; set; }
        public string Info { get; set; }
        public string Metro { get; set; }
        public int Brand { get; set; }
    }
}
