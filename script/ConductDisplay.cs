using System.Collections.Generic;
using System.Text;
using System;

namespace Alwinfy.Conducts {

    public class ConductDisplay
    {
        public static string MarkUpConducts(bool dead)
        {
            return MarkUpConducts(ConductLoader.System.ConductsByName, dead);
        }
        public static string MarkUpConducts(Dictionary<string, Conduct> conducts, bool dead)
        {
            var desirables = FilterHiddenConducts(conducts);
            desirables.Sort(CompareConducts);
            var ret = new StringBuilder();
            Conduct lastConduct = null;
            foreach (var conduct in desirables) {
                if (conduct.Description == "*nodisplay") {
                    continue;
                }
                if (lastConduct != null && lastConduct.Group != null && lastConduct.Group != conduct.Group) {
                    ret.Append('\n');
                }
                FormatConductTo(ref ret, conduct.Description ?? ("You follow{#|ed#} [" + conduct.Name + "] conduct."), dead);
                ret.Append('\n');
                lastConduct = conduct;
            }
            if (lastConduct == null) {
                ret.Append(dead ? "You adhered to no particular conducts.\n" : "You have not adhered to any conducts.\n");
            }
            UnityEngine.Debug.Log(ret.ToString());
            return ret.ToString();
        }

        public static void FormatConductTo(ref StringBuilder buffer, String fmt, bool dead) {
            int startIx = 0;
            int ix;
            while ((ix = fmt.IndexOf("{#", startIx)) != -1) {
                buffer.Append(fmt.Substring(startIx, ix - startIx));
                ix += 2;
                int delimiterIx = fmt.IndexOf("#}", ix);
                if (delimiterIx == -1) {
                    throw new Exception("Malformed format string - {# without closing #}: " + fmt);
                }
                int splitIx = fmt.IndexOf('|', ix);
                if (splitIx < delimiterIx && splitIx != -1) {
                    if (dead) {
                        buffer.Append(fmt.Substring(splitIx + 1, delimiterIx - splitIx - 1));
                    } else {
                        buffer.Append(fmt.Substring(ix, splitIx - ix));
                    }
                } else if (!dead) {
                    buffer.Append(fmt.Substring(ix, delimiterIx - ix));
                }
                startIx = delimiterIx + 2;
            }
            buffer.Append(fmt.Substring(startIx));
        }
        public static List<Conduct> FilterHiddenConducts(Dictionary<string, Conduct> conducts) {
            var ret = new List<Conduct>();
            foreach (var conduct in conducts.Values) {
                bool ok = true;
                foreach (var parent in conduct.HideIf) {
                    if (conducts.ContainsKey(parent)) {
                        ok = false;
                        break;
                    }
                }
                if (ok) {
                    ret.Add(conduct);
                }
            }
            return ret;
        }

        public static int CompareConducts(Conduct left, Conduct right) {
            var groupCompare = CompareNullAtEnd(left.Group, right.Group);
            return groupCompare == 0 ? left.Name.CompareTo(right.Name) : groupCompare;
        }
        public static int CompareNullAtEnd<T>(T left, T right) where T: IComparable<T> {
            if (left == null) {
                return right == null ? 0 : 1;
            } else {
                return right == null ? -1 : left.CompareTo(right);
            }
        }
    }

}
