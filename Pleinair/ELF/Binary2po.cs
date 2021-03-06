﻿// Copyright (C) 2019 Pedro Garau Martínez
//
// This file is part of Pleinair.
//
// Pleinair is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Pleinair is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Pleinair. If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.Collections.Generic;
using System.Text;
using Yarhl.FileFormat;
using Yarhl.IO;
using Yarhl.Media.Text;

namespace Pleinair.ELF
{
    public class Binary2Po : IConverter<BinaryFormat, Po>
    {
        private List<ushort> Text { get; set; }
        public int SizeBlock { get; set; }
        private string TextNormalized { get; set; }
        private int Count { get; set; }
        public Binary2Po()
        {
            Text = new List<ushort>();
        }

        public Po Convert(BinaryFormat source)
        {
            //Read the language used by the user' OS, this way the editor can spellcheck the translation. - Thanks Liquid_S por the code
            System.Globalization.CultureInfo currentCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            Po po = new Po
            {
                Header = new PoHeader("Disgaea", "dummy@dummy.com", currentCulture.Name)
            };

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = new UTF8Encoding(),
                Endianness = EndiannessMode.LittleEndian,
            };

            //Go to the first block where the executable contains text
            reader.Stream.Position = 0x151FFC; //Block 1
            SizeBlock = 0x45E; //Size Block 1
            DumpBlock(reader, po);

            //Go to the second block
            reader.Stream.Position = 0x153194; //Block 2
            SizeBlock = 0x45C; //Block 2
            DumpBlock(reader, po);

            //Go to the third block
            reader.Stream.Position = 0x1ACA00; //Block 3
            SizeBlock = 0x34; //Block 3
            DumpBlock(reader, po);

            //Go to the fourth block
            reader.Stream.Position = 0x137448; //Block 4
            SizeBlock = 0x2; //Block 4
            DumpBlock(reader, po);

            //Go to the fifth block
            reader.Stream.Position = 0x3FDBA; //Block 5
            SizeBlock = 0x1; //Block 5
            DumpBlock(reader, po);

            //Go to the sixth block
            reader.Stream.Position = 0x3F9FA; //Block 6
            SizeBlock = 0x1; //Block 6
            DumpBlock(reader, po);

            //Go to the seventh block
            reader.Stream.Position = 0x40C61; //Block 7
            SizeBlock = 0x1; //Block 7
            DumpBlock(reader, po);


            /*
             * Lugares del YES (Para el importador)
             * 40C61
             * 40C7C
             * 410C4
             * 410DF
             * 4130F
             * 4132A
             *
             */

            //Dump save/load font
            
            //Half width
            reader.Stream.Position = 0x3DAC8;
            SizeBlock = 0x1;
            DumpBlock(reader, po);

            /*
             * 3DAC8
             * 3DAD2
             * 3DB0A
             */

            //3fe2f

            //Full width
            reader.Stream.Position = 0x3DB18;
            SizeBlock = 0x1;
            DumpBlock(reader, po, true);

            return po;
        }

        private void DumpBlock(DataReader reader, Po po, bool fullWidth=false)
        {
            for (int i = 0; i < SizeBlock; i++)
            {
                if (!(JapaneseStrings.Contains(Count) || BadPointers.Contains(Count)))
                {
                    PoEntry entry = new PoEntry(); //Generate the entry on the po file

                    //Get position
                    int position = reader.ReadInt32() - 0x401600;
                    reader.Stream.PushToPosition(position);

                    //Get the text
                    DumpText(reader);

                    //Normalize the text
                    NormalizeText(fullWidth);

                    //Return to the original position
                    reader.Stream.PopPosition();

                    //entry.Original = ReplaceText(TextNormalized, true);  //Add the string block
                    entry.Original = TextNormalized.Replace("\0", "");
                    entry.Context = Count.ToString(); //Context
                    po.Add(entry);

                    //Clear the text
                    TextNormalized = "";
                }
                else
                {
                    reader.Stream.Position += 4;
                }
                
                Count++;
            }
        }

        private void DumpText(DataReader reader)
        {
            ushort textReaded;
            byte check;
            do
            {
                //Fix the import process because the executable contains bad spaces on the strings
                check = reader.ReadByte();
                if (check == 0x80 || check == 0x81 || check == 0x82 || check == 0x83 || check == 0x87) {
                    reader.Stream.Position -= 1;
                    textReaded = reader.ReadUInt16();
                    Text.Add(textReaded);
                }
                else Text.Add(check);
            }
            while (check != 00);
        }

        private void NormalizeText(bool full)
        {
            for (int i = 0; i < Text.Count; i++)
            {
                byte[] arraysjis = BitConverter.GetBytes(Text[i]);
                string temp = TALKDAT.Binary2Po.SJIS.GetString(arraysjis);
                if (!full && !japaneseChars.Contains(temp))
                {
                    TextNormalized += temp.Normalize(NormalizationForm.FormKC);
                }
                else
                    TextNormalized += temp;
            }
            //Delete the /0/0
            TextNormalized = TextNormalized.Remove(TextNormalized.Length - 2);
            //Add text to the empty string
            if (string.IsNullOrEmpty(TextNormalized))
                TextNormalized = "<!empty>";
            Text.Clear();
        }

        //Dictionaries

        //Japanese strings
        public List<int> JapaneseStrings = new List<int>()
        {
            11, 12, 13, 201, 300, 326, 327, 407, 408, 409, 410, 411, 412, 413,
            414, 415, 416, 919, 920, 921, 922, 923, 924, 925, 926, 927, 928, 929,
            930, 931, 932, 933, 934, 935, 936, 937, 938, 939, 940, 941, 942, 943, 944,
            945, 950, 951, 953, 954, 955, 956, 962, 974, 976, 977, 993, 994, 995, 996, 997,
            998, 999, 1000, 1001, 1002, 1003, 1004, 1005, 1006, 1007, 1008, 1009, 1010, 1011,
            1012, 1013, 1014, 1015, 1016, 1017, 1018, 1019, 1020, 1021, 1022, 1023, 1024, 1025,
            1026, 1027, 1028, 1029, 1030, 1031, 1032, 1033, 1034, 1035, 1036, 1037, 1038, 1039, 1040,
            1041, 1042, 1043, 1044, 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1054, 1055, 1056,
            1057, 1058, 1059, 1060, 1061, 1062, 1063, 1064, 1065, 1066, 1067, 1068, 1069, 1070, 1071,
            1072, 1073, 1074, 1075, 1076, 1077, 1078, 1079, 1080, 1081, 1082, 1083, 1084, 1085, 1086,
            1087, 1088, 1089, 1090, 1091, 1092, 1093, 1094, 1095, 1096, 1097, 1098, 1099, 1100, 1101,
            1102, 1103, 1104, 1105, 1106, 1107, 1108, 1109, 1110, 1111, 1112, 1113, 1114, 1115, 1118, 1119,
            1120, 1121, 1122, 1123, 1124, 1125, 1126, 1127, 1128, 1129, 1130, 1131, 1132, 1133, 1134, 1135,
            1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143, 1144, 1145, 1146, 1147, 1151, 1152, 1154, 1155,
            1156, 1157, 1158, 1159, 1160, 1161, 1162, 1164, 1165, 1166, 1167, 1168, 1169, 1170, 1171, 1172,
            1173, 1174, 1175, 1176, 1177, 1178, 1179, 1180, 1181, 1182, 1183, 1184, 1185, 1186, 1187, 1188,
            1189, 1190, 1191, 1192, 1193, 1194, 1195, 1197, 1198, 1199, 1200, 1201, 1202, 1203, 1204, 1205,
            1206, 1207, 1208, 1209, 1210, 1211, 1212, 1213, 1214, 1215, 1216, 1217, 1218, 1219, 1220, 1221,
            1222, 1223, 1225, 1226, 1227, 1228, 1229, 1230, 1231, 1232, 1233, 1234, 1235, 1236, 1237, 1238,
            1239, 1240, 1241, 1242, 1243, 1244, 1245, 1246, 1247, 1248, 1249, 1250, 1251, 1252, 1253, 1254,
            1255, 1256, 1257, 1258, 1259, 1260, 1261, 1262, 1263, 1264, 1265, 1266, 1267, 1268, 1269, 1270,
            1271, 1272, 1273, 1274, 1275, 1276, 1277, 1278, 1279, 1280, 1281, 1282, 1283, 1284, 1285, 1286,
            1287, 1288, 1289, 1290, 1291, 1292, 1293, 1294, 1295, 1296, 1297, 1298, 1299, 1300, 1301, 1302,
            1303, 1304, 1305, 1306, 1307, 1308, 1309, 1310, 1311, 1312, 1313, 1316, 1318, 1319, 1320, 1321,
            1322, 1323, 1324, 1329, 1331, 1332, 1333, 1337, 1338, 1339, 1340, 1343, 1345, 1346, 1347, 1349,
            1350, 1351, 1352, 1353, 1354, 1355, 1356, 1357, 1358, 1359, 1360, 1361, 1362, 1363, 1364, 1365,
            1368, 1369, 1370, 1371, 1372, 1373, 1376, 1377, 1378, 1379, 1380, 1381, 1382, 1383, 1384, 1386,
            1387, 1388, 1389, 1390, 1391, 1393, 1394, 1395, 1396, 1397, 1398, 1399, 1400, 1401, 1402, 1403,
            1404, 1405, 1406, 1407, 1408, 1409, 1410, 1411, 1412, 1413, 1414, 1415, 1416, 1417, 1418, 1419,
            1420, 1421, 1422, 1423, 1424, 1426, 1427, 1428, 1429, 1430, 1431, 1432, 1433, 1434, 1435, 1436,
            1437, 1438, 1439, 1440, 1441, 1442, 1443, 1444, 1445, 1446, 1447, 1448, 1449, 1450, 1451, 1452,
            1453, 1454, 1455, 1456, 1457, 1458, 1459, 1460, 1461, 1462, 1463, 1464, 1465, 1466, 1467, 1468,
            1469, 1470, 1471, 1472, 1473, 1474, 1475, 1476, 1477, 1478, 1479, 1480, 1481, 1482, 1483, 1484,
            1485, 1486, 1487, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497, 1498, 1499, 1500,
            1501, 1502, 1503, 1504, 1505, 1506, 1507, 1508, 1509, 1510, 1511, 1512, 1513, 1514, 1515, 1516,
            1517, 1518, 1519, 1520, 1521, 1522, 1523, 1524, 1525, 1526, 1527, 1528, 1529, 1530, 1531, 1532,
            1533, 1534, 1536, 1537, 1538, 1539, 1540, 1541, 1542, 1543, 1544, 1545, 1546, 1547, 1548, 1549,
            1550, 1551, 1552, 1553, 1554, 1555, 1556, 1557, 1558, 1559, 1560, 1561, 1562, 1563, 1564, 1565,
            1566, 1567, 1568, 1569, 1570, 1571, 1572, 1573, 1574, 1575, 1576, 1577, 1578, 1579, 1580, 1581,
            1582, 1583, 1586, 1587, 1588, 1589, 1590, 1591, 1592, 1593, 1594, 1595, 1596, 1597, 1598, 1599,
            1600, 1601, 1602, 1603, 1621, 1622, 1623, 1624, 1625, 1626, 1627, 1628, 1629, 1630, 1631, 1632,
            1633, 1635, 1636, 1637, 1638, 1639, 1640, 1641, 1642, 1643, 1644, 1645, 1646, 1647, 1648, 1649,
            1650, 1651, 1652, 1653, 1654, 1655, 1656, 1657, 1658, 1659, 1660, 1661, 1662, 1663, 1664, 1665,
            1666, 1667, 1668, 1669, 1670, 1671, 1672, 1673, 1674, 1675, 1676, 1685, 1686, 1687, 1688, 1689,
            1690, 1691, 1692, 1693, 1694, 1695, 1696, 1697, 1698, 1699, 1700, 1701, 1702, 1703, 1704, 1705,
            1706, 1707, 1708, 1709, 1710, 1711, 1712, 1713, 1714, 1715, 1716, 1741, 1742, 1743, 1744, 1745,
            1746, 1747, 1748, 1749, 1750, 1751, 1752, 1753, 1754, 1755, 1756, 1757, 1758, 1759, 1760, 1761,
            1762, 1763, 1764, 1765, 1766, 1767, 1768, 1769, 1770, 1771, 1772, 1773, 1774, 1775, 1776, 1777,
            1778, 1779, 1781, 1782, 1783, 1784, 1785, 1786, 1787, 1788, 1789, 1790, 1791, 1792, 1793, 1794,
            1795, 1796, 1797, 1798, 1799, 1800, 1801, 1802, 1803, 1804, 1805, 1806, 1807, 1808, 1809, 1810,
            1811, 1812, 1813, 1814, 1815, 1816, 1817, 1818, 1819, 1820, 1821, 1822, 1823, 1824, 1825, 1826,
            1827, 1828, 1829, 1830, 1831, 1832, 1833, 1834, 1835, 1836, 1837, 1838, 1839, 1840, 1841, 1842,
            1843, 1844, 1845, 1846, 1847, 1848, 1849, 1850, 1851, 1852, 1853, 1854, 1856, 1857, 1858, 1859,
            1862, 1863, 1864, 1865, 1866, 1867, 1868, 1979, 1980, 1981, 1982, 1983, 1984, 1985, 1986, 1987,
            1988, 1989, 1990, 1991, 1992, 1993, 1994, 1995, 1996, 1997, 1998, 1999, 2000, 2001, 2002, 2003,
            2004, 2005, 2006, 2007, 2008, 2009, 2010, 2011, 2012, 2013, 2014, 2018, 2019, 2020, 2021, 2022,
            2023, 2024, 2025, 2026, 2028, 2029, 2030, 2031, 2032, 2033, 2034, 2035, 2036, 2037, 2038, 2040,
            2041, 2042, 2043, 2044, 2045, 2046, 2047, 2048, 2050, 2051, 2052, 2053, 2054, 2055, 2056, 2057,
            2058, 2059, 2060, 2061, 2062, 2063, 2065, 2066, 2067, 2068, 2069, 2070, 2071, 2072, 2073, 2074,
            2080, 2092, 2094, 2095, 2111, 2112, 2113, 2114, 2115, 2116, 2117, 2118, 2119, 2120, 2121, 2122,
            2123, 2124, 2125, 2126, 2127, 2128, 2129, 2130, 2131, 2132, 2133, 2134, 2135, 2136, 2137, 2138,
            2139, 2140, 2141, 2143, 2144, 2145, 2146, 2147, 2148, 2149, 2150, 2151, 2152, 2153, 2154, 2155,
            2156, 2157, 2158, 2159, 2162, 2163, 2164, 2165, 2166, 2167, 2168, 2169, 2170, 2171, 2172, 2173,
            2174, 2175, 2176, 2177, 2178, 2179, 2180, 2181, 2182, 2183, 2184, 2185, 2186, 2187, 2188, 2189,
            2190, 2191, 2192, 2193, 2194, 2195, 2196, 2197, 2198, 2199, 2200, 2201, 2202, 2203, 2204, 2205,
            2206, 2207, 2208, 2209, 2210, 2211, 2212, 2213, 2214, 2215, 2216, 2217, 2218, 2219, 2220, 2221,
            2222, 2223, 2224, 2225, 2226, 2227, 2228, 2229, 2230, 2231, 2232, 2233, 2246, 2247, 2248, 2249,
            2250, 2251, 2252, 2253, 2254, 2255, 2256, 2257, 2258, 2259, 2260, 2261, 2262, 2263
        };


        //Bad pointers
        public List<int> BadPointers = new List<int>()
        {
            35, 45, 78, 106, 307, 417, 467, 499, 502, 516, 622, 662, 737, 751, 859,
            860, 864, 868, 891, 909, 1116, 1117, 1153, 1163, 1196,
            1224, 1425, 1535, 1585, 1617, 1620, 1634, 1740, 1780,
            1855, 1869, 1977, 1978, 2027
        };

        private List<string> japaneseChars = new List<string>()
        {
            "⑮",
            "⑯",
            "⑰",
            "⑱",
            "⑲",
            "⑳",
            "Ⅰ",
            "Ⅱ"
        };

        //Variables
        /*private Dictionary<string, string> Variables = new Dictionary<string, string>()
        {
            {"%\0s\0", "{25}{73}"},
            {"%\0d\0", "{25}{64}"},
            {"0\0x\0%\0x\0", "{30}{78}{25}{78}"},
            {"%\02\0d\0", "{25}{32}{64}"},
            {"%\03\0d\0", "{25}{33}{64}"},
            {"%\04\0d\0", "{25}{34}{64}"},
            {"%\07\0d\0", "{25}{37}{64}"},
            {"%\00\02\0d\0", "{25}{30}{32}{64}"},
            {"%\00\03\0d\0", "{25}{30}{33}{64}"},
            {"%\00\04\0d\0", "{25}{30}{34}{64}"},
            {"@", "{40}"},
            {".\0", "{2E}"},
            {"L\01\0", "{4C}{31}"},
            {"R\01\0", "{52}{31}"},
            {"?N\0", "{87}{4E}"},
            {"?O\0", "{87}{4F}"},
            {"?P\0", "{87}{50}"},
            {"?Q\0", "{87}{51}"},
            {"?R\0", "{87}{52}"},
            {"?S\0", "{87}{53}"},
            {"?T\0", "{87}{54}"},
            {"?U\0", "{87}{55}"}
        };*/

        /*public String ReplaceText(string line, bool export)
        {
            string result = line;
            foreach (var replace in Variables)
            {
                if (export) result = result.Replace(replace.Key, replace.Value);
                else result = result.Replace(replace.Value, replace.Key);
            }
            return result;
        }*/
    }
}
