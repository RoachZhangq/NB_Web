﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NB.Core.Common
{
    public class IDCardHelper
    {
        public static bool CheckIDCard(string Id)
        {
            bool flag = false;
            switch (Id.Length)
            {
                case 15:
                    flag = CheckIDCard15(Id);
                    break;
                case 18:
                    flag = CheckIDCard18(Id);
                    break;
            }
            return flag;
        }
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;
            }
            string[] arrVarifyCode = "1,0,x,9,8,7,6,5,4,3,2".Split(',');
            string[] Wi = "7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2".Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;
            }
            return true;//正确
        }

        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;
            }
            return true;//正确
        }

        public static IDCard GetIDCardInfo(string Id)
        {
            IDCard idCard = new IDCard();
            string strSex = string.Empty;
            if (Id.Length == 18)//处理18位的身份证号码从号码中得到生日和性别代码
            {
                idCard.Birthday = DateTime.Parse(Id.Substring(6, 4) + "-" + Id.Substring(10, 2) + "-" + Id.Substring(12, 2));
                strSex = Id.Substring(14, 3);
            }
            if (Id.Length == 15)
            {
                idCard.Birthday = DateTime.Parse("19" + Id.Substring(6, 2) + "-" + Id.Substring(8, 2) + "-" + Id.Substring(10, 2));
                strSex = Id.Substring(12, 3);
            }
            if (int.Parse(strSex) % 2 == 0)//性别代码为偶数是女性奇数为男性
            {
                idCard.Sex = "女";
            }
            else
            {
                idCard.Sex = "男";
            }
            DateTime nowDateTime = DateTime.Now;
            idCard.Age = nowDateTime.Year - idCard.Birthday.Year;
            if (nowDateTime.Month < idCard.Birthday.Month || nowDateTime.Month == idCard.Birthday.Month && nowDateTime.Day < idCard.Birthday.Day)
            {
                idCard.Age--;
            }
            return idCard;
        }
    }

    public class IDCard
    {
        public DateTime Birthday { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
    }
}
