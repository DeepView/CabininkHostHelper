using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
namespace CabininkHostHelper
{
   /// <summary>
   /// 用于表示一个Host文件，该实例可用于修改Windows操作系统的Host内容。
   /// </summary>
   public class HostFile
   {
      /// <summary>
      /// 构造函数，初始化当前的Host文件实例。
      /// </summary>
      public HostFile()
      {
         ReadHostsFromFile();
         InitInstance(FileContent);
      }
      /// <summary>
      /// 获取系统文件所在的目录。
      /// </summary>
      /// <returns>该操作返回的是系统文件所在的目录，一般而言，这个返回值是C:\Windows\system32，但是具体地址可能视Windows安装位置而改变。</returns>
      private string GetSystemDirectory() => Environment.SystemDirectory;
      /// <summary>
      /// 初始化实例，通过构造函数访问，该方法用于读取hostsContent所包含的当前Windows的所有Host条目，并存储于Hosts属性中。
      /// </summary>
      /// <param name="hostsContent">当前Windows的所有Host条目。</param>
      private void InitInstance(string hostsContent)
      {
         string[] hostCollection = hostsContent.Split(new char[] { '\n' });
         string[] hostItem;
         Hosts = new List<HostElement>();
         foreach (string item in hostCollection)
         {
            if (item.Length > 0 && item.Substring(0, 1) == "#") FileNotes += item;
            else
            {
               hostItem = item.Split(new char[] { '\x20' }, StringSplitOptions.RemoveEmptyEntries);
               if (hostItem.Length == 2) Add(new HostElement(hostItem[0], hostItem[1]));
               else if (hostItem.Length > 2)
               {
                  string remarks = string.Empty; ;
                  for (int i = 2; i < hostItem.Length; i++)
                  {
                     remarks += $"\x20{hostItem[i]}";
                  }
                  Add(new HostElement(hostItem[0], hostItem[1], remarks));
               }
#if DEBUG
               else Console.WriteLine("#This is a empty host item or invalid host item.");
#endif
            }
         }
      }
      /// <summary>
      /// 从Host文件读取内容到FileContent属性。
      /// </summary>
      private void ReadHostsFromFile()
      {
         using (StreamReader reader = new StreamReader(Path))
         {
            FileContent = reader.ReadToEnd();
         }
      }
      /// <summary>
      /// 获取或设置（private访问权限）当前实例的Host条目集合。
      /// </summary>
      public List<HostElement> Hosts { get; private set; }
      /// <summary>
      /// 获取或设置当前实例的Host文件集中注释。
      /// </summary>
      public string FileNotes { get; set; }
      /// <summary>
      /// 获取当前实例所包含的Hosts属性所包含的有效Host条目数量。
      /// </summary>
      public int Count => Hosts.Count;
      /// <summary>
      /// 获取或设置指定索引处的元素。
      /// </summary>
      /// <param name="index">指定的索引，可用于查找指定的Host条目。</param>
      /// <returns>该操作返回的是index参数所对应的那一个HostElement实例。</returns>
      public HostElement this[int index] { get => Hosts[index]; set => Hosts[index] = value; }
      /// <summary>
      /// 获取当前Windows的Host文件路径。
      /// </summary>
      public string Path => $@"{GetSystemDirectory()}\drivers\etc\hosts";
      /// <summary>
      /// 获取或设置当前实例的的Host文件内容，值得注意的是，这个内容并不是实时更新的，而是需要通过UpdateContent手动更新。
      /// </summary>
      public string FileContent { get; private set; }
      /// <summary>
      /// 将Hosts属性作为更新来源，并更新FileContent属性。
      /// </summary>
      public void UpdateContent()
      {
         if (FileContent != string.Empty) FileContent = string.Empty;
         if (!string.IsNullOrEmpty(FileNotes)) FileContent += $"{FileNotes}\n";
         foreach (HostElement item in Hosts)
         {
            FileContent += $"{item.ToString()}\n";
         }
      }
      /// <summary>
      /// 将FileContent作为更新来源，从而更新Host文件，但是这个操作可能会覆盖以前的Host设置，不过一般而言，这个操作更新的内容会包含以前的Host设置。
      /// </summary>
      public void UpdateFile() => UpdateFile(false);
      /// <summary>
      /// 将FileContent作为更新来源，从而更新Host文件，但是这个操作提供了是否已追加的方式将Host更新到文件中。
      /// </summary>
      /// <param name="isAppendToEnd">决定是否以追加的形式更新Host文件，如果这个参数值为true，则表示将当前实例的Host设置以追加的形式更新到Windows的Host文件中，否则会覆盖以前的Host设置。</param>
      public void UpdateFile(bool isAppendToEnd)
      {
         File.SetAttributes(Path, File.GetAttributes(Path) & (~FileAttributes.ReadOnly));
         FileStream fStream = new FileStream(Path, isAppendToEnd ? FileMode.Append : FileMode.Open);
         StreamWriter writer = new StreamWriter(fStream, Encoding.UTF8);
         UpdateContent();
         writer.Write(FileContent);
         if (writer != null) writer.Close();
         if (fStream != null) fStream.Close();
         File.SetAttributes(Path, File.GetAttributes(Path) | FileAttributes.ReadOnly);
      }
      /// <summary>
      /// 将指定的HostElement实例添加到Hosts中。
      /// </summary>
      /// <param name="value">指定的HostElement实例。</param>
      public void Add(HostElement value) => Hosts.Add(value);
      /// <summary>
      /// 从Hosts中移除所有的HostElement实例。
      /// </summary>
      public void Clear() => Hosts.Clear();
      /// <summary>
      /// 确定Hosts是否包含特定值。
      /// </summary>
      /// <param name="value">要在Hosts中定位的对象。</param>
      /// <returns>该操作如果在返回一个true值，则表示该操作从Hosts属性中找到了指定对象。</returns>
      public bool Contains(HostElement value) => Hosts.Contains(value);
      /// <summary>
      /// 确定Hosts中特定项的索引。
      /// </summary>
      /// <param name="value">要在Hosts中定位的对象。</param>
      /// <returns>如果指定对象在Hosts中找到，则会返回这个对象的索引，否则将会返回-1。</returns>
      public int IndexOf(HostElement value) => Hosts.IndexOf(value);
      /// <summary>
      /// 在Hosts中的指定索引处插入一个HostElement实例。
      /// </summary>
      /// <param name="index">指定需要插入新实例的索引。</param>
      /// <param name="value">插入到Hosts属性的HostElement实例。</param>
      public void Insert(int index, HostElement value) => Hosts.Insert(index, value);
      /// <summary>
      /// 从Hosts属性中移除特定对象的第一个匹配项。
      /// </summary>
      /// <param name="value">需要被移除与之相匹配的HostElement对象。</param>
      public void Remove(HostElement value) => Hosts.Remove(value);
      /// <summary>
      /// 从Hosts移除位于指定索引处的HostElement实例。
      /// </summary>
      /// <param name="index">需要被移除的HostElement所对应的索引。</param>
      public void RemoveAt(int index) => Hosts.RemoveAt(index);
      /// <summary>
      /// 从特定的ICollection索引处开始，将Array的元素复制到一个Array中。(继承自ICollection)
      /// </summary>
      /// <param name="hosts">一个HostElement数组，它是从ICollection复制的元素的目标，该数组必须具有从零开始的索引。</param>
      /// <param name="index">array中要从其开始复制的从零开始的索引。</param>
      public void CopyTo(HostElement[] hosts, int index) => Hosts.CopyTo(hosts, index);
      /// <summary>
      /// 返回循环访问集合的枚举器。
      /// </summary>
      /// <returns>一个可用于循环访问集合的IEnumerator对象。</returns>
      public IEnumerator GetEnumerator() => Hosts.GetEnumerator();
   }
}
