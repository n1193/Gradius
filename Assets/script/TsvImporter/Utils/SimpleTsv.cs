using System.Collections.Generic;
using UnityEngine;

public static class SimpleTsv
{
    public class Options
    {
        public string CommentPrefix = "#";   // 先頭セル/行頭がこれならコメント
        public string HeaderMarker = "##";  // 先頭セルがこれならヘッダー
    }

    public static List<Dictionary<string, string>> Parse(TextAsset ta, Options opt = null)
    {
        if (ta == null) throw new System.ArgumentNullException(nameof(ta));
        opt ??= new Options();

        var text = ta.text.Replace("\r\n", "\n").Replace("\r", "\n");
        if (text.Length > 0 && text[0] == '\uFEFF') text = text.Substring(1);

        var rows = new List<Dictionary<string, string>>();
        string[] headers = null;

        var lines = text.Split('\n');
        int startCol = 0;

        for (int ln = 0; ln < lines.Length; ln++)
        {
            var raw = lines[ln].TrimEnd();
            if (string.IsNullOrWhiteSpace(raw)) continue;

            var cells = raw.Split('\t');

            // 1) ヘッダー（どの列に"##"があるか検出）
            //    例のシートだと cells[0]=="##" なので startCol=1
            int markerIdx = -1;
            for (int i = 0; i < cells.Length; i++)
            {
                if (cells[i].Trim() == opt.HeaderMarker) { markerIdx = i; break; }
            }
            if (markerIdx >= 0)
            {
                startCol = markerIdx + 1;
                if (startCol >= cells.Length)
                {
                    Debug.LogWarning($"[TSV] ヘッダー行に項目がない line {ln + 1}");
                    headers = null;
                }
                else
                {
                    headers = Sub(cells, startCol);
                    for (int i = 0; i < headers.Length; i++) headers[i] = headers[i].Trim();
                    Debug.Log($"[TSV] headers: " + string.Join(" | ", System.Array.ConvertAll(headers, h => $"[{h}]")));
                }
                continue;
            }

            // 2) コメント判定（A列に # を置く運用想定）
            if (raw.StartsWith(opt.CommentPrefix) ||
                (cells.Length > 0 && cells[0].Trim() == opt.CommentPrefix))
                continue;

            // 3) ヘッダー未設定なら従来型（行そのものがヘッダー）。開始列は0に。
            if (headers == null)
            {
                startCol = 0;
                headers = (string[])cells.Clone();
                for (int i = 0; i < headers.Length; i++) headers[i] = headers[i].Trim();
                Debug.Log($"[TSV] headers(auto): " + string.Join(" | ", System.Array.ConvertAll(headers, h => $"[{h}]")));
                continue;
            }

            // 4) データ行：ヘッダーと同じ startCol を適用して整列
            var dcells = startCol > 0 && cells.Length > startCol ? Sub(cells, startCol) : cells;

            if (dcells.Length != headers.Length)
                Debug.LogWarning($"[TSV] 列数不一致 line {ln + 1}: expected {headers.Length}, got {dcells.Length}");

            var row = new Dictionary<string, string>(headers.Length);
            for (int i = 0; i < headers.Length; i++)
            {
                string v = (i < dcells.Length) ? dcells[i] : "";
                v = v.Replace("\\t", "\t").Replace("\\n", "\n");
                // 必要なら不可視空白/Unicodeマイナスの正規化もここで（Sanitize）
                row[headers[i]] = v.Trim();
            }
            rows.Add(row);
        }
        return rows;
    }

    static T[] Sub<T>(T[] a, int start)
    {
        var len = a.Length - start;
        var r = new T[len];
        System.Array.Copy(a, start, r, 0, len);
        return r;
    }
    public readonly struct Row
    {
        private readonly Dictionary<string,string> d;
        public Row(Dictionary<string,string> d) { this.d = d; }

        // 可視化用
        public override string ToString()
            => string.Join(" | ", System.Linq.Enumerable.Select(d, kv => $"{kv.Key}=[{Sanitize(kv.Value)}]"));

        public string Get(string key, string def = "")
            => d != null && d.TryGetValue(key, out var v) ? Sanitize(v) : def;

        public int GetInt(string key, int def = 0)
            => int.TryParse(Get(key,""), System.Globalization.NumberStyles.Integer,
               System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : def;

        public float GetFloat(string key, float def = 0f)
            => float.TryParse(Get(key,""), System.Globalization.NumberStyles.Float,
               System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : def;

        public bool GetBool(string key, bool def = false)
        {
            var s = Get(key,"");
            if (bool.TryParse(s, out var b)) return b;
            if (s == "1") return true; if (s == "0") return false;
            if (string.Equals(s,"TRUE",System.StringComparison.OrdinalIgnoreCase)) return true;
            if (string.Equals(s,"FALSE",System.StringComparison.OrdinalIgnoreCase)) return false;
            return def;
        }

        public TEnum GetEnum<TEnum>(string key, TEnum def) where TEnum : struct
        {
            var s = Get(key,"");
            return string.IsNullOrEmpty(s) || !System.Enum.TryParse<TEnum>(s, true, out var v) ? def : v;
        }
    }

    public static IEnumerable<Row> Wrap(IEnumerable<Dictionary<string,string>> src)
    {
        foreach (var d in src) yield return new Row(d);
    }

    // 文字列サニタイズ（不可視空白やUnicodeマイナスを除去/正規化）
    public static string Sanitize(string s)
    {
        if (s == null) return "";
        s = s.Replace("\u00A0", " ").Replace("\u2007"," ").Replace("\u202F"," ");
        s = s.Replace("\u2212", "-"); // Unicodeマイナス → ハイフン
        return s.Trim();
    }
}
