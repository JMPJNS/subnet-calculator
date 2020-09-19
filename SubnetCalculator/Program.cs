using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using Microsoft.VisualBasic;

namespace SubnetCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "192.168.1.5/16";
            var subnetCount = 3;

            int prefixLength = int.Parse(
                input.Split("/")[1]);

            if (prefixLength > 32)
            {
                throw new Exception("Prefix Length can't be above 32");
            }

            var ip = ParseIPInput(input);

            var actualPrefixLength = CalculateActualPrefix(prefixLength, subnetCount);
            var subnetMask = CalculateSubnetMaskString(actualPrefixLength);
            var broadcastAdress = CalculateBroadcastAdressString(ip, actualPrefixLength);
            var hostCount = CalcuateHostCount(actualPrefixLength);
            var hostRange = CalculateHostRangeString(ip, actualPrefixLength);
            
            Console.Write(Convert.ToString(ip, 2));
        }

        static UInt32 ParseIPInput(string input)
        {
            return input.Split("/")[0].Split(".").Aggregate(Convert.ToUInt32(0), (curr, next) =>
            {
                var i = UInt32.Parse(next);
                if (i > 255)
                {
                    throw new Exception("IP Segment can't be above 255");
                }
                
                return curr << 8 | i;
            });
        }
        
        static int ParsePrefixInput(string input)
        {
            throw new NotImplementedException();
        }
        
        static int CalculateActualPrefix(int prefix, int subnetCount)
        {
            if (subnetCount == 1)
                return prefix;
            
            var addedLength = (int) Math.Log2(subnetCount) + 1;
            
            return prefix + addedLength;
        }

        static UInt32 CalculateSubnetMask(int prefix)
        {
            return Convert.ToUInt32( ~0u << (32 - prefix));
        }
        
        static string CalculateSubnetMaskString(int prefix)
        {
            var t = CalculateSubnetMask(prefix);
            return ConvertFromIntegerToIpAddress(t);
        }

        static UInt32 CalculateBroadcastAdress(UInt32 ip, int prefix)
        {
            var subnetMask = CalculateSubnetMask(prefix);
            var t = Convert.ToUInt32((~subnetMask) | ip);
            return t;
        }
        
        static string CalculateBroadcastAdressString(UInt32 ip, int prefix)
        {
            return ConvertFromIntegerToIpAddress(CalculateBroadcastAdress(ip, prefix));
        }

        static UInt32 CalcuateHostCount(int prefix)
        {
            return (~CalculateSubnetMask(prefix) - 1);
        }

        static (uint firstIp, uint lastIp) CalculateHostRange(UInt32 ip, int prefix)
        {
            var subnetMask = CalculateSubnetMask(prefix);
            var firstIp = Convert.ToUInt32((subnetMask & ip) + 1);
            var lastIp = CalculateBroadcastAdress(ip, prefix) - 1u;
            return (firstIp, lastIp);
        }

        static string CalculateHostRangeString(UInt32 ip, int prefix)
        {
            (UInt32 first, UInt32 last) t = CalculateHostRange(ip, prefix);
            return $"{ConvertFromIntegerToIpAddress(t.first)}...{ConvertFromIntegerToIpAddress(t.last)}";
        }

        public static string ConvertFromIntegerToIpAddress(uint ipAddress)
        {
            byte[] bytes = BitConverter.GetBytes(ipAddress);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }
            return new IPAddress(bytes).ToString();
        }
    }
}