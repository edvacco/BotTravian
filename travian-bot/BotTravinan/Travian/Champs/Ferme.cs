﻿/*
    // BotTravian - A C# .NET application
    // Copyright (C) 2018-2019 nbeny
    // <nbeny@student.42.fr>
    // This program is free software: you can redistribute it and/or modify
    // it under the terms of the GNU General Public License as published by
    // the Free Software Foundation, either version 3 of the License, or
    // (at your option) any later version.
    // This program is distributed in the hope that it will be useful,
    // but WITHOUT ANY WARRANTY; without even the implied warranty of
    // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    // GNU General Public License for more details.
    // You should have received a copy of the GNU General Public License
    // along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BotTravinan.Travian.Champs
{
    public class Fermes : List<Ferme>
    {

    }

    public class Ferme
    {
        public string Xpath { get; set; } = "";
        public string Lvl { get; set; } = "";

        public static Bot InitFerme(string Url, ChromeDriver driver, Bot Bot)
        {
            string[] Xpath = {
                "/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/map/area[2]",
                "/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/map/area[8]",
                "/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/map/area[9]",
                "/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/map/area[12]",
                "/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/map/area[13]",
                "/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/map/area[15]",
            };

            // Get the html of the page
            var page = driver.PageSource;

            // Loads the page as html
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(page);

            for (int i = 0; i < Xpath.Length; i++)
            {
                Ferme Ferme = new Ferme();
                Ferme.Xpath = Xpath[i];
                var Lvl = doc.DocumentNode.SelectNodes(Xpath[i]);
                string[] ForetLvl = Lvl[0].Attributes["alt"].Value.Split(' ');
                Ferme.Lvl = ForetLvl[ForetLvl.Length - 1];
                Console.WriteLine(Ferme.Lvl);
                Bot.Champ.Fermes.Add(Ferme);
            }

            return Bot;
        }

        public static void BuildFerme(string Url, ChromeDriver driver, Bot Bot, Login Login)
        {
            for (int i = 0; i < Bot.Champ.Fermes.Count; i++)
            {
                if (Login.Lvl == Int32.Parse(Bot.Champ.Fermes[i].Lvl))
                {
                    IWebElement Ferme = driver.FindElementByXPath(Bot.Champ.Fermes[i].Xpath);
                    Ferme.Click();
                    Thread.Sleep(2000);

                    var page = driver.PageSource;

                    // Loads the page as html
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(page);

                    var Gold = doc.DocumentNode.SelectNodes("/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/div[1]/div[3]/div[4]/div[1]/button/div/div[2]/span");
                    if (Gold == null)
                    {
                        IWebElement Up3 = driver.FindElementByXPath("/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/div[1]/div[3]/div[4]/div[1]/span");
                        string opgg3 = Up3.Text;
                        if (opgg3[1] == ':')
                            opgg3 = "0" + Up3.Text;
                        DateTime time_build = DateTime.ParseExact(opgg3, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

                        DateTime time_now = DateTime.Now;

                        DateTime time_update = time_now.AddHours(time_build.Hour);
                        time_update = time_update.AddMinutes(time_build.Minute);
                        time_update = time_update.AddSeconds(time_build.Second);
                        double millisec = (time_update.ToUniversalTime() - time_now.ToUniversalTime()).TotalMilliseconds;

                        IWebElement Up = driver.FindElementByXPath("/html/body/div[1]/div[2]/div[2]/div[2]/div[2]/div[1]/div[1]/div[3]/div[4]/div[1]/button/div");
                        Up.Click();
                        Thread.Sleep((int)millisec);
                    }
                    else
                    {
                        IWebElement Close = driver.FindElementByXPath("/html/body/div[1]/div[2]/div[2]/div[2]/div[1]/a");
                        if (Close != null)
                            Close.Click();
                        Thread.Sleep(1000);
                    }

                }
            }
        }
    }
}