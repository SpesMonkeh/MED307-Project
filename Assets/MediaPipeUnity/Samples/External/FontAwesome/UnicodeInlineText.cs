// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine.UI;
using System.Text.RegularExpressions;

public class UnicodeInlineText : Text
{
  bool disableDirty;
  readonly Regex regexp = new(@"\\u(?<Value>[a-zA-Z0-9]+)");

  protected override void OnPopulateMesh(VertexHelper vh)
  {
    var cache = text;
    disableDirty = true;
    text = Decode(text);
    base.OnPopulateMesh(vh);
    text = cache;
    disableDirty = false;
  }

  string Decode(string value)
    => regexp.Replace(value, m => ((char)int.Parse(m.Groups["Value"].Value, System.Globalization.NumberStyles.HexNumber)).ToString());

  public override void SetLayoutDirty()
  {
    if (disableDirty)
      return;
    base.SetLayoutDirty();
  }

  public override void SetVerticesDirty()
  {
    if (disableDirty)
      return;
    base.SetVerticesDirty();
  }

  public override void SetMaterialDirty()
  {
    if (disableDirty)
      return;
    base.SetMaterialDirty();
  }
}
