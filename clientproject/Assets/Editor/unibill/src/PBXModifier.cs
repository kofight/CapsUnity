using System;
using System.IO;
using System.Collections.Generic;

namespace Unibill.Editor {
    public class PBXModifier {

        const string mod1 = "    08B24F76137BFDFA00FBA308 /* Storekit.framework in Frameworks */ = {isa = PBXBuildFile; fileRef = 08B24F75137BFDFA00FBA307 /* iAd.framework */; settings = {ATTRIBUTES = (Weak, ); }; };";
        const string mod2 = "    08B24F75137BFDFA00FBA307 /* Storekit.framework */ = {isa = PBXFileReference; lastKnownFileType = wrapper.framework; name = Storekit.framework; path = System/Library/Frameworks/Storekit.framework; sourceTree = SDKROOT; };";
        const string mod3 = "    08B24F76137BFDFA00FBA308 /* Storekit.framework in Frameworks */,";
        const string mod4 = "    08B24F75137BFDFA00FBA307 /* Storekit.framework */,";

        public PBXModifier () {
        }

        public string[] applyTo(string file) {
            List<string> lines = new List<string>(File.ReadAllLines (file));

            if (lines.Count < 10) {
                return lines.ToArray();
            }

            if (contains (lines, "Storekit.framework")) {
                return lines.ToArray();
            }

            var modIndex = indexOf (lines, "/* Begin PBXBuildFile section */");
            if (modIndex == -1) {
                return lines.ToArray();
            }

            lines.Insert (modIndex + 1, mod2);
            lines.Insert (modIndex + 1, mod1);

            modIndex = indexOf (lines, "isa = PBXFrameworksBuildPhase");
            if (modIndex == -1) {
                return lines.ToArray();
            }

            lines.Insert (modIndex + 3, mod3);

            modIndex = indexOf (lines, "/* CustomTemplate */ = {");
            if (modIndex == -1) {
                return lines.ToArray();
            }

            lines.Insert (modIndex + 3, mod4);

            return lines.ToArray ();
        }

        private static int indexOf(List<string> lines, string value) {
            for (int t = 0; t < lines.Count; t++) {
                if (lines[t].Contains(value)) {
                    return t;
                }
            }

            return -1;
        }

        private static bool contains(List<string> lines, string value) {
            foreach (var s in lines) {
                if (s.Contains(value)) {
                    return true;
                }
            }

            return false;
        }
    }
}

