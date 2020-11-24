using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace xml_normalization
{
    class Program
    {
        static void Main(string[] args)
        {
            // 引数処理
            Args setting = analysts(args);

            // xml解析して出力用string生成
            var xDoc = XDocument.Load(setting.source);
            var ans = xDoc.Declaration.ToString() + "\n";
            ans += dump(xDoc.Root, 0);
            Console.WriteLine(ans);

            // 出力
            using (var sw = new StreamWriter(setting.dest, false, Encoding.UTF8))
            {
                sw.Write(ans);
            }
        }

        private static Args analysts(string[] args)
        {
            var setting = new Args();

            if (args.Length != 2)
            {
                throw new ArgumentException();
            }

            // 引数判定
            if (args[0] == "-i")
            {
                setting.source = args[1];
                setting.dest = args[1];

                // 存在判定
                if (!File.Exists(setting.source))
                {
                    throw new FileNotFoundException();
                }
            }
            else
            {
                setting.source = args[0];
                setting.dest = args[1];

                // 存在判定
                if (!File.Exists(setting.source)
                    || File.Exists(setting.dest) )
                {
                    throw new FileNotFoundException();
                }
            }

            return setting;
        }

        static string dump(XElement element, int depth)
        {
            var tabs = new string('\t', depth);

            // 属性も要素も子ノードも無ければ即閉じる
            if (!element.HasAttributes
                && element.IsEmpty
                && !element.HasElements)
            {
                return $"{tabs}<{element.Name}/>\n";
            }

            string combined;
            // 属性が有ったら2tab足して次の行に追加
            if (element.HasAttributes)
            {
                combined = $"{tabs}<{element.Name}\n";
                var tabs_ = tabs + "\t\t";
                foreach (var attribute in element.Attributes())
                {
                    combined += $"{tabs_}{attribute.Name}={attribute.Value}\n";
                }
                combined += $"{tabs}>\n";
            }
            else
            {
                combined = $"{tabs}<{element.Name}>\n";
            }
            // 要素・子ノードが有ったら1tab足して次の行に追加
            if (!element.HasElements
                && !element.IsEmpty)
            {
                combined += $"{tabs}\t{element.Value}\n";
            }
            foreach (var node in element.Elements())
            {
                combined += dump(node, depth + 1);
            }
            combined += $"{tabs}</{element.Name}>\n";

            return combined;
        }

        class Args
        {
            public string source, dest;
        }
    }
}
