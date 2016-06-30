﻿using Microsoft.VisualBasic.FileIO;
using SH.ShowYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SH.ShowYou.Helpers
{
    public class CsvDatabaseHelpers
    {
        private static string GetPath(string fileName)
        {
            return $"\\CsvDatabase\\{fileName}.csv";
        }

        private static List<T> ReadCsvData<T>(string fileName) where T: class
        {
            var returnvalue = new List<T>();
            using (TextFieldParser parser = new TextFieldParser(AppDomain.CurrentDomain.BaseDirectory + GetPath(fileName)))
            {
                // Skip 1 line and second line.
                parser.ReadLine();
                parser.ReadLine();
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var parts = parser.ReadFields();
                    returnvalue.Add((T)Activator.CreateInstance(typeof(T), new object[] { parts }));
                }
            }          

            return returnvalue;
        }

        public static List<GeoLiteCityBlockViewModel> GetAllGeoLiteCityBlock()
        {
            var cacheKey = "GeoLiteCity-Blocks";
            if (CacheHelpers.Exist(cacheKey))
            {
                return CacheHelpers.Get<List<GeoLiteCityBlockViewModel>>(cacheKey);
            }

            var geoLiteCityBlocks = ReadCsvData<GeoLiteCityBlockViewModel>("GeoLiteCity-Blocks");

            if (geoLiteCityBlocks.Count > 0)
            {
                CacheHelpers.Add(cacheKey, geoLiteCityBlocks);
            }

            return geoLiteCityBlocks;
        }

        public static Dictionary<string, GeoLiteCityLocationViewModel> GetAllGeoLiteCityLocation()
        {
            var cacheKey = "GeoLiteCity-Location";
            if (CacheHelpers.Exist(cacheKey))
            {
                return CacheHelpers.Get<Dictionary<string, GeoLiteCityLocationViewModel>>(cacheKey);
            }

            var dic = new Dictionary<string, GeoLiteCityLocationViewModel>();
            using (TextFieldParser parser = new TextFieldParser(AppDomain.CurrentDomain.BaseDirectory + GetPath("GeoLiteCity-Location")))
            {
                // Skip 1 line and second line.
                parser.ReadLine();
                parser.ReadLine();
                parser.SetDelimiters(",");
                while (!parser.EndOfData)
                {
                    var parts = parser.ReadFields();
                    var geoLiteCity = new GeoLiteCityLocationViewModel(parts);
                    dic.Add(geoLiteCity.Id, geoLiteCity);
                }
            }

            if (dic.Count > 0)
            {
                CacheHelpers.Add(cacheKey, dic);
            }

            return dic;
        }

        public static GeoLiteCityLocationViewModel GetGeoLiteCityLocation(string ipAddress)
        {
            var ipInteger = IpHelpers.ConvertToInt(ipAddress);
            var geoLiteBlock = GetAllGeoLiteCityBlock().FirstOrDefault(p => p.StartIpNum <= ipInteger && p.EndIpNum >= ipInteger);
            if(geoLiteBlock == null)
            {
                return null;
            }

            var geoLiteLocation = GetAllGeoLiteCityLocation()[geoLiteBlock.LocId];
            return geoLiteLocation;
        }
    }
}