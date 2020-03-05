using System;
using System.Net;
namespace CabininkHostHelper
{
   /// <summary>
   /// 表示一个Host条目的类，主要由Host地址IpAddress，映射域名Domain和备注Remarks构成。
   /// </summary>
   public class HostElement
   {
      private string _remarks;
      private const int IP_BIT_DISPL_01 = 24;
      private const int IP_BIT_DISPL_02 = IP_BIT_DISPL_01 - 8;
      private const int IP_BIT_DISPL_03 = IP_BIT_DISPL_02 - 8;
      /// <summary>
      /// 构造函数，通过字符串形式的Host条目创建一个HostElement实例。
      /// </summary>
      /// <param name="hostItemString">字符串表达形式的Host条目，比如说：127.0.0.1 localhost #Loopback_Address。</param>
      public HostElement(string hostItemString) => InitInstance(hostItemString);
      /// <summary>
      /// 构造函数，通过字符串形式的Host地址和映射域名来创建一个HostElement实例。
      /// </summary>
      /// <param name="ipAddress">字符串表达形式的IPv4地址。</param>
      /// <param name="domain">IP地址对应的映射域名。</param>
      public HostElement(string ipAddress, string domain) => InitInstance(ipAddress, domain, $"Server = {domain}");
      /// <summary>
      /// 构造函数，通过字符串形式的Host地址，映射域名和备注创建一个HostElement实例。
      /// </summary>
      /// <param name="ipAddress">字符串表达形式的IPv4地址。</param>
      /// <param name="domain">IP地址对应的映射域名。</param>
      /// <param name="remarks">该Host条目的备注信息。</param>
      public HostElement(string ipAddress, string domain, string remarks) => InitInstance(ipAddress, domain, remarks);
      /// <summary>
      /// 构造函数，通过IPAddress表达形式的Host地址和映射域名来创建一个HostElement实例。
      /// </summary>
      /// <param name="ipAddress">IPAddress表达形式的IPv4地址。</param>
      /// <param name="domain">IP地址对应的映射域名。</param>
      public HostElement(IPAddress ipAddress, string domain) => InitInstance(ipAddress.ToString(), domain, $"Server = {domain}");
      /// <summary>
      /// 构造函数，通过IPAddress表达形式的Host地址，映射域名和备注来创建一个HostElement实例。
      /// </summary>
      /// <param name="ipAddress">IPAddress表达形式的IPv4地址。</param>
      /// <param name="domain">IP地址对应的映射域名。</param>
      /// <param name="remarks">该Host条目的备注信息。</param>
      public HostElement(IPAddress ipAddress, string domain, string remarks) => InitInstance(ipAddress.ToString(), domain, remarks);
      /// <summary>
      /// 初始化实例，通过构造函数访问，该方法用于给属性赋值。
      /// </summary>
      /// <param name="hostItemString">字符串表达形式的Host条目，比如说：127.0.0.1 localhost #Loopback_Address。</param>
      private void InitInstance(string hostItemString)
      {
         string[] hostCell = hostItemString.Split(new char[] { '\x20' }, StringSplitOptions.RemoveEmptyEntries);
         if (hostCell.Length > 3)
         {
            for (int i = 2; i < hostCell.Length; i++) hostCell[2] += $"\x20{hostCell[i]}";
         }
         InitInstance(hostCell[0], hostCell[1], hostCell.Length > 2 ? hostCell[2] : $"Server = {Domain}");
      }
      /// <summary>
      /// 初始化实例，通过构造函数访问，该方法用于给属性赋值。
      /// </summary>
      /// <param name="ipAddress">字符串表达形式的IPv4地址。</param>
      /// <param name="domain">IP地址对应的映射域名。</param>
      /// <param name="remarks">该Host条目的备注信息。</param>
      private void InitInstance(string ipAddress, string domain, string remarks)
      {
         IpAddress = new IPAddress(IpAddressToLong(ipAddress));
         Domain = domain;
         Remarks = remarks;
      }
      /// <summary>
      /// 将字符串表达形式的IPv4地址转换为Int64数据类型。
      /// </summary>
      /// <param name="ipAddress">字符串表达形式的IPv4地址。</param>
      /// <returns></returns>
      private long IpAddressToLong(string ipAddress)
      {
         char[] separator = new char[] { '.' };
         string[] items = ipAddress.Split(separator);
         return long.Parse(items[3]) << IP_BIT_DISPL_01 | long.Parse(items[2]) << IP_BIT_DISPL_02 | long.Parse(items[1]) << IP_BIT_DISPL_03 | long.Parse(items[0]);
      }
      /// <summary>
      /// 获取或设置当前实例的Host IPv4地址。
      /// </summary>
      public IPAddress IpAddress { get; set; }
      /// <summary>
      /// 获取或设置当前实例的映射域名。
      /// </summary>
      public string Domain { get; set; }
      /// <summary>
      /// 获取或设置当前实例的条目备注。
      /// </summary>
      public string Remarks { get => _remarks; set => _remarks = $"#{value}"; }
      /// <summary>
      /// 通过一个指定的域名或者主机地址获取该域名或者主机地址映射的IP地址，这个地址包含了IPv4和IPv6地址。
      /// </summary>
      /// <param name="domain">指定的主机地址或者域名。</param>
      /// <returns>该操作将会返回一个IPAddress实例数组，这个数组包含了参数domain表示的域名所对应或者映射的IPv4或者IPv6地址。</returns>
      public static IPAddress[] GetHostAddresses(string domain) => Dns.GetHostAddresses(domain);
      /// <summary>
      /// 获取当前实例的字符串表达形式，但是在这个重载方法中，该操作返回的是当前实例的字符串表达形式的Host条目，比如说：127.0.0.1 localhost #Loopback_Address。
      /// </summary>
      /// <returns>该操作返回的是一个字符串表达形式的Host条目。</returns>
      public override string ToString() => $"{IpAddress.ToString()} {Domain} {Remarks}";
   }
}
